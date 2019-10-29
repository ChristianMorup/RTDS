using System;
using RTDS.DTO;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public SearchDirectoryArgs(string path, string fileName, MonitorInfo relatedMonitorInfo)
        {
            Path = path;
            FileName = fileName;
            RelatedMonitorInfo = relatedMonitorInfo;
        }

        public SearchDirectoryArgs(string path, string fileName)
        {
            Path = path;
            FileName = fileName;
        }

        public string Path { get; }

        public string FileName { get; }

        public MonitorInfo RelatedMonitorInfo { get; }
    }
}