using EventManager.Services;
using EventManager.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class HomePage : Page
    {
        private HomePageViewModel _viewModel;

        public HomePage()
        {
            InitializeComponent();
            _viewModel = new HomePageViewModel();
            DataContext = _viewModel;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            // Refresh the main page by reloading data and updating stats
            RefreshHomePage();
        }

        private void RefreshHomePage()
        {
            // Create new ViewModel instance to refresh data
            _viewModel = new HomePageViewModel();
            DataContext = _viewModel;
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