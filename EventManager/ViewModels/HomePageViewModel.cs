using EventManager.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace EventManager.ViewModels
{
    public partial class HomePageViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private readonly DispatcherTimer _timer;
        private string _currentTime = string.Empty;

        public HomePageViewModel()
        {
            _dataService = DataService.Instance;

            // Setup timer for current time display
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateTime();
            LoadStats();
        }

        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                _currentTime = value;
                OnPropertyChanged();
            }
        }

        public int TotalEvents { get; private set; }
        public int EventsOnMap { get; private set; }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        private void LoadStats()
        {
            var events = _dataService.GetAllEvents();
            TotalEvents = events.Count;
            EventsOnMap = events.Count(e => e.IsOnMap);

            OnPropertyChanged(nameof(TotalEvents));
            OnPropertyChanged(nameof(EventsOnMap));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
