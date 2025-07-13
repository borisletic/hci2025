using EventManager.Services;
using EventManager.Views.Pages;
using System.Windows;
using System.Windows.Input;

namespace EventManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialize Navigation Service
            NavigationService.Instance.MainFrame = MainFrame;

            // Navigate to Home Page
            NavigationService.Instance.NavigateTo("Home");

            // Add keyboard handler for demo mode
            this.KeyDown += MainWindow_KeyDown;
            this.Focusable = true;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            // Stop demo on any key press (as per requirement)
            var demoService = DemoService.Instance;
            if (demoService.IsDemoRunning)
            {
                demoService.StopDemo();

                // Navigate back to home
                NavigationService.Instance.NavigateTo("Home");

                // Show brief message
                MessageBox.Show("Demo stopped by keyboard input.", "Demo Mode",
                              MessageBoxButton.OK, MessageBoxImage.Information);

                e.Handled = true;
            }
        }

        // Ensure window can receive keyboard focus
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            this.Focus();
        }
    }
}