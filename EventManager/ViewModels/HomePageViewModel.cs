using EventManager.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace EventManager.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private readonly DispatcherTimer _timer;
        private string _currentTime = string.Empty;

        public HomePageViewModel()
        {
            _dataService = DataService.Instance;

            // IMPORTANT: Set initial time immediately
            UpdateTime();

            // Setup timer for continuous updates
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            LoadStats();
        }

        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged();
                }
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
            try
            {
                var events = _dataService.GetAllEvents();
                TotalEvents = events.Count;
                EventsOnMap = events.Count(e => e.IsOnMap);

                OnPropertyChanged(nameof(TotalEvents));
                OnPropertyChanged(nameof(EventsOnMap));
            }
            catch (Exception ex)
            {
                // Handle any data loading errors gracefully
                TotalEvents = 0;
                EventsOnMap = 0;
                System.Diagnostics.Debug.WriteLine($"Error loading stats: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Cleanup timer when ViewModel is disposed
        public void Dispose()
        {
            _timer?.Stop();
        }
    }
}