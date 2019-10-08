using System;
using System.IO;

namespace RTDS.Monitoring
{
    public interface IFileSystemWatcherWrapper : IDisposable
    {
        string Path { get; set; }

        event FileSystemEventHandler Created;

        bool EnableRaisingEvents { get; set; }

        string Filter { get; set; }

        NotifyFilters NotifyFilters { get; set; }
    }
}