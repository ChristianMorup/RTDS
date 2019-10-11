using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTDS.Monitoring.Args;
using RTDS.Monitoring.Factory;
using RTDS.Monitoring.Wrapper;

namespace RTDS.Monitoring
{
    //https://docs.microsoft.com/en-us/previous-versions/dotnet/netframework-1.1/czefa0ke(v=vs.71)?redirectedfrom=MSDN
    //TODO Gennemgå alle access specifiers 
    internal class MonitorController
    {
        private List<IFileMonitor> _fileMonitors;

        private readonly IMonitor _folderMonitor;
        private readonly IMonitorFactory _factory;

        public MonitorController(IMonitor folderMonitor, IMonitorFactory factory)
        {
            _fileMonitors = new List<IFileMonitor>();
            _folderMonitor = folderMonitor;
            _factory = factory;
            _folderMonitor.Created += async (sender, args) => await HandleNewFolder(sender, args).ConfigureAwait(false);
        }

        public Task StartMonitoring(string path)
        {
            return _folderMonitor.StartMonitoringAsync(path);
        }

        private async Task HandleNewFolder(object sender, SearchDirectoryArgs args)
        {
            Console.WriteLine("A new folder has been created!");

            var newFileMonitor = _factory.CreateFileMonitor();

            _fileMonitors.Add(newFileMonitor);

            newFileMonitor.Created += async (s, a) => await OnNewFileDetected(s, a).ConfigureAwait(false);

            newFileMonitor.Finished += async (s, a) => await OnMonitorFinished(s, a).ConfigureAwait(false);

            //TODO Create queue and notify some listener that it has been created. 

            newFileMonitor.StartMonitoringAsync(args.Path);
        }

        private async Task OnNewFileDetected(object sender, EventArgs args)
        {
            Console.WriteLine("New file was detected!");
            //TODO Create DTO and post in queue
        }

        private async Task OnMonitorFinished(object sender, FileMonitorFinishedArgs args)
        {
            _fileMonitors.Remove(args.Monitor);
        }
    }
}
