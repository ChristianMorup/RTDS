using System.Timers;

namespace RTDS.CBCTDataProvider.Monitoring.Wrappers
{
    internal class TimerWrapper : ITimerWrapper
    {
        public event ElapsedEventHandler Elapsed;
        private readonly Timer _timer;

        public TimerWrapper(Timer timer)
        {
            _timer = timer;
            _timer.Elapsed += OnElapsed;
        }

        public double Interval
        {
            get => _timer.Interval;
            set => _timer.Interval = value;
        }

        public bool Enabled
        {
            get => _timer.Enabled;
            set => _timer.Enabled = value;
        }

        public void Reset()
        {
            _timer.Stop();
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private void OnElapsed(object source, ElapsedEventArgs args) => Elapsed?.Invoke(source, args);
    }
}