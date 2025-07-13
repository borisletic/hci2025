using EventManager.Services;
using EventManager.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EventManager.Views.Pages
{
    public partial class HomePage : Page
    {
        private HomePageViewModel _viewModel;
        private DispatcherTimer _timeTimer;

        public HomePage()
        {
            InitializeComponent();
            InitializeViewModel();
            InitializeTimeDisplay();
        }

        private void InitializeViewModel()
        {
            _viewModel = new HomePageViewModel();
            DataContext = _viewModel;
        }

        private void InitializeTimeDisplay()
        {
            // Set initial time immediately
            UpdateTimeDisplay();

            // Create timer for time updates
            _timeTimer = new DispatcherTimer();
            _timeTimer.Interval = TimeSpan.FromSeconds(1);
            _timeTimer.Tick += TimeTimer_Tick;
            _timeTimer.Start();
        }

        private void TimeTimer_Tick(object? sender, EventArgs e)
        {
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            if (TimeDisplay != null)
            {
                TimeDisplay.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshHomePage();
        }

        private void RefreshHomePage()
        {
            InitializeViewModel();
            UpdateTimeDisplay();
        }

        private void ViewAllEventsButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("EventList");
        }

        private void AddNewEventButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("AddEvent");
        }

        private void ViewWorldMapButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("WorldMap");
        }

        private void ManageTagsButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Tags");
        }

        private void EventTypesButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("EventTypes");
        }

        private void DemoModeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Demo");
        }
    }
}