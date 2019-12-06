namespace RTDS.Configuration.Data
{
    internal class RTDSConfiguration
    {
        public RTDSPaths Paths { get; set; }

        public RTDSMonitorSettings MonitorSettings { get; set; }

        public ESAPISettings ESAPISettings { get; set; }
    }
}