using System;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public SearchDirectoryArgs(string path, string name, MonitorInfo relatedMonitorInfo)
        {
            Path = path;
            Name = name;
            RelatedMonitorInfo = relatedMonitorInfo;
        }

        public SearchDirectoryArgs(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public string Path { get; }

        public string Name { get; }

        public MonitorInfo RelatedMonitorInfo { get; }
    }
}