using EventManager.Services;
using EventManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class HomePage : Page
    {
        public HomePage()
        {
            InitializeComponent();
            DataContext = new HomePageViewModel();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // For now, just show a message. In full implementation, 
            // this would show a slide-out menu
            MessageBox.Show("Menu functionality would go here", "Menu",
                          MessageBoxButton.OK, MessageBoxImage.Information);
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