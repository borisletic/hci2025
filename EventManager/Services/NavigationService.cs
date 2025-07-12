using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using EventManager.Views.Pages;

namespace EventManager.Services
{
    public class NavigationService
    {
        private static NavigationService? _instance;
        private Frame? _mainFrame;

        public static NavigationService Instance => _instance ??= new NavigationService();

        public Frame? MainFrame
        {
            get => _mainFrame;
            set => _mainFrame = value;
        }

        public void NavigateTo(string pageType, object? parameter = null)
        {
            if (_mainFrame == null) return;

            Page? page = pageType switch
            {
                "Home" => new Views.Pages.HomePage(),
                "EventList" => new Views.Pages.EventListPage(),
                "AddEvent" => new Views.Pages.EventFormPage(),
                "EditEvent" => new Views.Pages.EventFormPage(),
                "EventInfo" => new Views.Pages.EventInfoPage(),
                "EventTypes" => new Views.Pages.EventTypesPage(),
                "Tags" => new Views.Pages.TagsPage(),
                "WorldMap" => new Views.Pages.WorldMapPage(),
                "Demo" => new Views.Pages.DemoPage(),
                _ => null
            };

            if (page != null)
            {
                page.DataContext = parameter;
                _mainFrame.Navigate(page);
            }
        }

        public void GoBack()
        {
            if (_mainFrame?.CanGoBack == true)
            {
                _mainFrame.GoBack();
            }
        }

        public void GoForward()
        {
            if (_mainFrame?.CanGoForward == true)
            {
                _mainFrame.GoForward();
            }
        }

        public bool CanGoBack => _mainFrame?.CanGoBack ?? false;
        public bool CanGoForward => _mainFrame?.CanGoForward ?? false;
    }
}