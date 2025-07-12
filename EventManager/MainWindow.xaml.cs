using EventManager.Services;
using EventManager.Views.Pages;
using System.Windows;

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
        }
    }
}