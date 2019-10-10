using System;
using System.Threading.Tasks;
using System.Timers;

namespace RTDS.Monitoring.Wrapper
{
    internal interface ITimerWrapper : IDisposable
    {
        event ElapsedEventHandler Elapsed;

        double Interval { get; set; }

        bool Enabled { get; set; }

        Task Reset();
    }
}