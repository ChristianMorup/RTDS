using RTDS.Monitoring.Args;

namespace RTDS.Monitoring
{
    internal interface IFileMonitorListener
    {
        void OnNewFileDetected(object sender, SearchDirectoryArgs args);

        void OnMonitorFinished(object sender, FileMonitorFinishedArgs args);
    }
}