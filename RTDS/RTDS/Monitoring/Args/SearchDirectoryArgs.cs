using System;
using RTDS.DTO;
using RTDS.Monitoring.Monitors;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public SearchDirectoryArgs(string path, string fileName, IMonitor relatedMonitor)
        {
            Path = path;
            FileName = fileName;
            RelatedMonitor = relatedMonitor;
        }

        public SearchDirectoryArgs(string path, string fileName)
        {
            Path = path;
            FileName = fileName;
        }

        public string Path { get; }

        public string FileName { get; }

        public IMonitor RelatedMonitor { get; }
    }
}