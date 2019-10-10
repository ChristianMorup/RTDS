using System;
using System.Timers;

namespace RTDS.Monitoring.Args
{
    internal class SearchDirectoryArgs : EventArgs
    {
        public string Path { get; }

        public SearchDirectoryArgs(string path)
        {
            Path = path;
        }
    }
}
