using System.Timers;
using Timer = System.Timers.Timer;

namespace WPF_NhaMayCaoSu.Service.Services
{
    public class SharedTimerService
    {
        private static readonly Lazy<SharedTimerService> _instance = new(() => new SharedTimerService());
        public static SharedTimerService Instance => _instance.Value;

        private Timer _timer;
        private int _remainingSeconds;

        public event EventHandler<int> TimerTicked;   // Event for notifying windows
        public event EventHandler TimerEnded;         // Event for timer end

        public bool IsCountingDown { get; private set; }

        private SharedTimerService()
        {
            _timer = new Timer(1000); // 1 second interval
            _timer.Elapsed += OnTimerTick;
        }

        public void StartCountdown(int durationInSeconds)
        {
            _remainingSeconds = durationInSeconds;
            IsCountingDown = true;
            _timer.Start();
        }

        public void StopCountdown()
        {
            _timer.Stop();
            IsCountingDown = false;
        }

        private void OnTimerTick(object sender, ElapsedEventArgs e)
        {
            _remainingSeconds--;

            TimerTicked?.Invoke(this, _remainingSeconds);

            if (_remainingSeconds <= 0)
            {
                _timer.Stop();
                IsCountingDown = false;
                TimerEnded?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
