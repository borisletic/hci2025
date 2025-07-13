using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class DemoPage : Page
    {
        private readonly DemoService _demoService;
        private readonly NavigationService _navigationService;

        public DemoPage()
        {
            InitializeComponent();
            _demoService = DemoService.Instance;
            _navigationService = Services.NavigationService.Instance; // Fix: Properly instantiate
            _demoService.DemoActionOccurred += OnDemoActionOccurred;
        }

        private void OnDemoActionOccurred(object? sender, string action)
        {
            // Update demo status display
            Dispatcher.Invoke(() =>
            {
                if (DemoStatusText != null)
                {
                    DemoStatusText.Text = action;
                }
            });
        }

        private async void StartDemoButton_Click(object sender, RoutedEventArgs e)
        {
            StartDemoButton.IsEnabled = false;
            StopDemoButton.IsEnabled = true;

            if (DemoStatusText != null)
            {
                DemoStatusText.Text = "Demo is starting...";
            }

            await _demoService.StartDemoAsync();

            // Re-enable buttons when demo finishes
            StartDemoButton.IsEnabled = true;
            StopDemoButton.IsEnabled = false;

            if (DemoStatusText != null)
            {
                DemoStatusText.Text = "Demo finished. Ready to start again.";
            }
        }

        private void StopDemoButton_Click(object sender, RoutedEventArgs e)
        {
            _demoService.StopDemo();

            StartDemoButton.IsEnabled = true;
            StopDemoButton.IsEnabled = false;

            if (DemoStatusText != null)
            {
                DemoStatusText.Text = "Demo stopped by user.";
            }

            // Navigate back to Home
            _navigationService.NavigateTo("Home");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Stop demo if running
            if (_demoService.IsDemoRunning)
            {
                _demoService.StopDemo();
            }

            // Navigate back to Home page
            _navigationService.NavigateTo("Home");
        }
    }
}