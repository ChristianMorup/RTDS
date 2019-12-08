using System;
using System.Timers;

namespace RTDS.CBCTDataProvider.Monitoring.Wrappers
{
    internal interface ITimerWrapper : IDisposable
    {
        event ElapsedEventHandler Elapsed;

        double Interval { get; set; }

        bool Enabled { get; set; }

        void Reset();
    }
}