using System.Threading.Tasks;
using System.Timers;

namespace RTDS.Monitoring.Wrapper
{
    internal class TimerWrapper : ITimerWrapper
    {
        private readonly Timer _timer;
        public event ElapsedEventHandler Elapsed;

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

        public async Task Reset()
        {
            _timer.Stop();
            _timer.Start();
        }

        public TimerWrapper(Timer timer)
        {
            _timer = timer;
            _timer.Elapsed += OnElapsed;
        }

        private void OnElapsed(object source, ElapsedEventArgs e) => Elapsed?.Invoke(source, e);

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}