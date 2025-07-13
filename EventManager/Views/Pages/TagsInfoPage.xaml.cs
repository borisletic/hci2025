using EventManager.Models;
using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class TagsInfoPage : Page
    {
        private Tag? _currentTag;

        public TagsInfoPage()
        {
            InitializeComponent();
            Loaded += TagsInfoPage_Loaded;
        }

        private void TagsInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Tag tagToView)
            {
                _currentTag = tagToView;
                LoadTagInfo();
            }
        }

        private void LoadTagInfo()
        {
            if (_currentTag != null)
            {
                DataContext = _currentTag;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}