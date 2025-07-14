using EventManager.Services;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace EventManager.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;

        public HomePageViewModel()
        {
            _dataService = DataService.Instance;
            LoadStats();
        }

        public int TotalEvents { get; private set; }
        public int EventsOnMap { get; private set; }

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
    }
}