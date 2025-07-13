using EventManager.Models;
using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EventInfoPage : Page
    {
        private Event? _currentEvent;

        public EventInfoPage()
        {
            InitializeComponent();
            Loaded += EventInfoPage_Loaded;
        }

        private void EventInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event eventToView)
            {
                _currentEvent = eventToView;
                LoadEventInfo();
            }
        }

        private void LoadEventInfo()
        {
            if (_currentEvent != null)
            {
                DataContext = _currentEvent;
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