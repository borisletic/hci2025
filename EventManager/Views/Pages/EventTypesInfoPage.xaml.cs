using EventManager.Models;
using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EventTypesInfoPage : Page
    {
        private EventType? _currentEventType;

        public EventTypesInfoPage()
        {
            InitializeComponent();
            Loaded += EventTypesInfoPage_Loaded;
        }

        private void EventTypesInfoPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EventType eventTypeToView)
            {
                _currentEventType = eventTypeToView;
                LoadEventTypeInfo();
            }
        }

        private void LoadEventTypeInfo()
        {
            if (_currentEventType != null)
            {
                DataContext = _currentEventType;
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