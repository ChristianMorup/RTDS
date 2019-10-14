﻿using System;
using System.IO;

namespace RTDS.Monitoring.Wrapper
{
    internal interface IFileSystemWatcherWrapper : IDisposable
    {
        string Path { get; set; }

        event FileSystemEventHandler Created;

        bool EnableRaisingEvents { get; set; }

        string Filter { get; set; }

        NotifyFilters NotifyFilters { get; set; }
    }
}