using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using RTDS.CBCTDataProvider.Monitoring.Args;
using RTDS.Configuration;
using RTDS.CTDataProvider.Callbacks;
using RTDS.DTO;
using RTDS.ExceptionHandling;

namespace RTDS
{
    internal class DataFlowSynchronizer : ICTScanRetrievedCallback, ICorrectedCTScanRetrievedCallback
    {
        private readonly ConcurrentQueue<PermStorageFolderStructure> _createdFolderStructures;
        private ConcurrentQueue<CTScanInfo> _ctScanInfos;
        private readonly object _lock = new object();

        public DataFlowSynchronizer()
        {
            _createdFolderStructures = new ConcurrentQueue<PermStorageFolderStructure>();
            _ctScanInfos = new ConcurrentQueue<CTScanInfo>();
        }

        public void OnFolderCreated(object sender, PermFolderCreatedArgs args)
        {
            TaskWatcher.AddTask(Task.Run(() =>
            {
                _createdFolderStructures.Enqueue(args.PermStorageFolderStructure);
                SynchronizeDataFlows();
            }));
        }

        public void OnCTScanRetrieved(CTScanInfo ctScanInfo)
        {
            _ctScanInfos.Enqueue(ctScanInfo);
        }

        private void SynchronizeDataFlows()
        {
            lock (_lock)
            {
                if (!_createdFolderStructures.IsEmpty && !_ctScanInfos.IsEmpty)
                {
                    CTScanInfo ctScanInfo;
                    while (_ctScanInfos.TryDequeue(out ctScanInfo)) ;
                    PermStorageFolderStructure folderStructure;
                    while (_createdFolderStructures.TryDequeue(out folderStructure));

                    StoreDicomFilesInPermStorage(ctScanInfo, folderStructure.CtPath);
                }
            }
        }

        private void StoreDicomFilesInPermStorage(CTScanInfo ctInfo, string permStorage)
        {
            foreach (var file in ctInfo.DcmFiles)
            {
                Directory.Move(file, permStorage);
            }
        }

        public void OnCorrectedCTScanRetrieved(CTScanInfo info, string id)
        {
            _ctScanInfos = new ConcurrentQueue<CTScanInfo>();  //Equal to clearing the queue. 

            var baseDirectory = Directory.GetDirectories(ConfigurationManager.GetConfigurationPaths().BaseTargetPath, "*" + id + "*")[0];
            var ctDirectory = Directory.GetDirectories(baseDirectory, "*" + "CT" + "*")[0];

            DirectoryInfo directoryInfo = new DirectoryInfo(ctDirectory);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }

            StoreDicomFilesInPermStorage(info, ctDirectory);
        }
    }
}