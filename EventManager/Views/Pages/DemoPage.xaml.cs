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
            _navigationService = new NavigationService(); // Assuming you need to instantiate it  
            _demoService.DemoActionOccurred += OnDemoActionOccurred;
        }

        private void OnDemoActionOccurred(object sender, string action)
        {
            // Update demo status display  
            // This would be implemented in the XAML  
        }

        private async void StartDemoButton_Click(object sender, RoutedEventArgs e)
        {
            await _demoService.StartDemoAsync();
        }

        private void StopDemoButton_Click(object sender, RoutedEventArgs e)
        {
            _demoService.StopDemo();
            _navigationService.NavigateTo("Home");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_demoService.IsDemoRunning)
            {
                _demoService.StopDemo();
            }
            _navigationService.NavigateTo("Home");
        }
    }
}