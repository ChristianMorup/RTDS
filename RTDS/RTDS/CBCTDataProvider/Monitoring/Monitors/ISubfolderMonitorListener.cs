using RTDS.CBCTDataProvider.Monitoring.Args;

namespace RTDS.CBCTDataProvider.Monitoring.Monitors
{
    internal interface ISubfolderMonitorListener
    {
        void OnNewFileDetected(object sender, SearchDirectoryArgs args);

        void OnMonitorFinished(object sender, FileMonitorFinishedArgs args);
    }
}