using System;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public SearchDirectoryArgs(string path, string name)
        {
            Path = path;
            Name = name;
        }

        public string Path { get; }

        public string Name { get; }
    }
}