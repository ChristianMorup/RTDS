using System;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public SearchDirectoryArgs(string path, string name, IMonitor relatedMonitor)
        {
            Path = path;
            Name = name;
            RelatedMonitor = relatedMonitor;
        }

        public string Path { get; }

        public string Name { get; }

        public IMonitor RelatedMonitor { get; }
    }
}