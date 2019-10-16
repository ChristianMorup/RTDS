using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;

namespace RTDS.Monitoring
{
    //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/czefa0ke(v=vs.71)?redirectedfrom=MSDN

    internal class MonitorController
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<IMonitor, ProjectionInfo> _monitorByQueueMap;
        private readonly IMonitor _folderMonitor;
        private readonly IMonitorFactory _monitorFactory;
        private readonly IProjectionFactory _projectionFactory;
        private readonly IFileMover _fileMover;

        public MonitorController(IMonitorFactory monitorFactory,
            IProjectionFactory projectionFactory, IFileMover fileMover)
        {
            _monitorByQueueMap = new Dictionary<IMonitor, ProjectionInfo>();
            _monitorFactory = monitorFactory;
            _folderMonitor = _monitorFactory.CreateFolderMonitor();
            _projectionFactory = projectionFactory;
            _fileMover = fileMover;
            _folderMonitor.Created += HandleNewFolder;
        }

        public void StartMonitoring(string path)
        {
            _folderMonitor.StartMonitoringAsync(path);
        }

        private void HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            var newFileMonitor = _monitorFactory.CreateFileMonitor();
            var projectionInfo = _projectionFactory.CreateProjectionInfo(args.Path);

            _monitorByQueueMap.Add(newFileMonitor, projectionInfo);

            newFileMonitor.Created += OnNewFileDetected;
            newFileMonitor.Finished += OnMonitorFinished;

            Logger.Info(CultureInfo.CurrentCulture, "New folder detected: {0}", args.Name);

            newFileMonitor.StartMonitoringAsync(args.Path);
        }

        private void OnNewFileDetected(object sender, SearchDirectoryArgs args)
        {
            Logger.Info(CultureInfo.CurrentCulture, "New file detected: {0}", args.Name);

            //TODO Move file, Create DTO and post in queue
        }

        private Task HandleNewFile(string path, IMonitor relatedMonitor)
        {
            Task task = new Task(async () =>
            {
                ProjectionInfo info;
                if (_monitorByQueueMap.TryGetValue(relatedMonitor, out info))
                {
                    var destPath = await _fileMover.MoveFileAsync(info.TemporalStoragePath, info.PermanentStoragePath);
                }
            });

            task.Start();

            return task;
        }

        private void OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            _monitorByQueueMap.Remove(args.Monitor);
            Logger.Info(CultureInfo.CurrentCulture, "Stops monitoring: {0}", args.Monitor.MonitoredPath);
        }
    }

    internal class FileMover : IFileMover
    {
        public async Task<string> MoveFileAsync(string sourceFile, string destinationFile)
        {
            return await MoveFileAsyncImpl(sourceFile, destinationFile);
        }

        private async Task<string> MoveFileAsyncImpl(string sourceFile, string destinationFile)
        {
            using (FileStream sourceStream = File.Open(sourceFile, FileMode.Open))
            {
                using (FileStream destinationStream = File.Create(destinationFile))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                    return destinationFile;
                }
            }
        }
    }

    internal interface IFileMover
    {
        Task<string> MoveFileAsync(string sourceFile, string destinationFile);
    }


    internal class ProjectionInfoFactory : IProjectionFactory
    {
        public ProjectionInfo CreateProjectionInfo(string tempStoragePath)
        {
            string permanentStoragePath = "some path"; //TODO Read this from configuration file. 

            return new ProjectionInfo(tempStoragePath, permanentStoragePath, new BlockingCollection<string>(),
                new BlockingCollection<string>());
        }
    }

    internal interface IProjectionFactory
    {
        ProjectionInfo CreateProjectionInfo(string tempStoragePath);
    }


    internal class ProjectionInfo
    {
        public ProjectionInfo(string temporalStoragePath, string permanentStoragePath,
            BlockingCollection<string> filesToBeTransferred, BlockingCollection<string> filesToBeConverted)
        {
            TemporalStoragePath = temporalStoragePath;
            PermanentStoragePath = permanentStoragePath;
            FilesToBeTransferred = filesToBeTransferred;
            FilesToBeConverted = filesToBeConverted;
        }

        public string TemporalStoragePath { get; set; }
        public string PermanentStoragePath { get; set; }
        public BlockingCollection<string> FilesToBeTransferred { get; set; }
        public BlockingCollection<string> FilesToBeConverted { get; set; }
    }
}