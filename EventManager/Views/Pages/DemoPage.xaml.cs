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
            _navigationService = Services.NavigationService.Instance;

            // Subscribe to demo events
            _demoService.DemoActionOccurred += OnDemoActionOccurred;

            // Initialize button states
            InitializeButtonStates();
        }

        private void InitializeButtonStates()
        {
            if (StartDemoButton != null && StopDemoButton != null)
            {
                if (_demoService.IsDemoRunning)
                {
                    StartDemoButton.IsEnabled = false;
                    StopDemoButton.IsEnabled = true;
                }
                else
                {
                    StartDemoButton.IsEnabled = true;
                    StopDemoButton.IsEnabled = false;
                }
            }
        }

        private void OnDemoActionOccurred(object? sender, string action)
        {
            // Update demo status display on UI thread
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
            try
            {
                StartDemoButton.IsEnabled = false;
                StopDemoButton.IsEnabled = true;

                if (DemoStatusText != null)
                {
                    DemoStatusText.Text = "Demo is starting...";
                }

                await _demoService.StartDemoAsync();

                // Re-enable buttons when demo finishes naturally
                StartDemoButton.IsEnabled = true;
                StopDemoButton.IsEnabled = false;

                if (DemoStatusText != null)
                {
                    DemoStatusText.Text = "Demo finished. Ready to start again.";
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error starting demo: {ex.Message}", "Demo Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);

                // Reset button states on error
                StartDemoButton.IsEnabled = true;
                StopDemoButton.IsEnabled = false;
            }
        }

        private void StopDemoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop the demo
                _demoService.StopDemo();

                // Update button states immediately
                StartDemoButton.IsEnabled = true;
                StopDemoButton.IsEnabled = false;

                if (DemoStatusText != null)
                {
                    DemoStatusText.Text = "Demo stopped by user.";
                }

                // Navigate back to Home
                _navigationService.NavigateTo("Home");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error stopping demo: {ex.Message}", "Demo Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Stop demo if running
                if (_demoService.IsDemoRunning)
                {
                    _demoService.StopDemo();
                }

                // Navigate back to Home page
                _navigationService.NavigateTo("Home");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Error navigating back: {ex.Message}", "Navigation Error",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Clean up event subscription when page is unloaded
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_demoService != null)
            {
                _demoService.DemoActionOccurred -= OnDemoActionOccurred;
            }
        }
    }
}