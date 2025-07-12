using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EventTypesPage : Page
    {
        private readonly DataService _dataService;

        public EventTypesPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadEventTypes();
        }

        private void LoadEventTypes()
        {
            // Load event types data - implementation depends on your XAML structure
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}