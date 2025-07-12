using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class TagsPage : Page
    {
        private readonly DataService _dataService;

        public TagsPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadTags();
        }

        private void LoadTags()
        {
            // Load tags data - implementation depends on your XAML structure
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}