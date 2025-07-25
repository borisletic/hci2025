﻿using EventManager.Models;
using EventManager.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EventListPage : Page
    {
        private readonly DataService _dataService;
        private List<Event> _allEvents = new();
        private List<Event> _filteredEvents = new();

        public EventListPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadData();
        }

        private void LoadData()
        {
            _allEvents = _dataService.GetAllEvents();
            _filteredEvents = _allEvents.ToList();
            EventsListView.ItemsSource = _filteredEvents;

            // Load ComboBox data
            var eventTypes = _dataService.GetAllEventTypes();
            EventTypeComboBox.ItemsSource = eventTypes;
            EventTypeComboBox.DisplayMemberPath = "Name";
            EventTypeComboBox.SelectedValuePath = "EventTypeId";

            AttendanceComboBox.ItemsSource = System.Enum.GetValues(typeof(Attendance));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            ApplySearch();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSearch();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Search is triggered manually with Search button
        }

        private void FilterChanged(object sender, SelectionChangedEventArgs e)
        {
            // Filter is triggered manually with Search button
        }

        private void LiveFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyLiveFilter();
        }

        private void CancelFilterButton_Click(object sender, RoutedEventArgs e)
        {
            LiveFilterBox.Text = "";
            ApplyLiveFilter();
        }

        private void ApplySearch()
        {
            var filtered = _allEvents.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(NameSearchBox.Text))
            {
                filtered = filtered.Where(e => e.Name.ToLower().Contains(NameSearchBox.Text.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(DescriptionSearchBox.Text))
            {
                filtered = filtered.Where(e => e.Description.ToLower().Contains(DescriptionSearchBox.Text.ToLower()));
            }

            if (EventTypeComboBox.SelectedValue != null)
            {
                var selectedTypeId = EventTypeComboBox.SelectedValue.ToString();
                filtered = filtered.Where(e => e.EventTypeId == selectedTypeId);
            }

            if (AttendanceComboBox.SelectedValue != null)
            {
                var selectedAttendance = (Attendance)AttendanceComboBox.SelectedValue;
                filtered = filtered.Where(e => e.Attendance == selectedAttendance);
            }

            _filteredEvents = filtered.ToList();
            EventsListView.ItemsSource = _filteredEvents;
        }

        private void ApplyLiveFilter()
        {
            if (string.IsNullOrWhiteSpace(LiveFilterBox.Text))
            {
                EventsListView.ItemsSource = _filteredEvents;
                return;
            }

            var searchText = LiveFilterBox.Text.ToLower();
            var liveFiltered = _filteredEvents.Where(e =>
                e.Name.ToLower().Contains(searchText) ||
                e.Description.ToLower().Contains(searchText) ||
                e.City.ToLower().Contains(searchText) ||
                e.Country.ToLower().Contains(searchText)
            ).ToList();

            EventsListView.ItemsSource = liveFiltered;
        }

        private void ClearSearch()
        {
            NameSearchBox.Text = "";
            DescriptionSearchBox.Text = "";
            EventTypeComboBox.SelectedIndex = -1;
            AttendanceComboBox.SelectedIndex = -1;
            _filteredEvents = _allEvents.ToList();
            EventsListView.ItemsSource = _filteredEvents;
        }

        private void DeleteEvent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventToDelete)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{eventToDelete.Name}'?",
                                           "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dataService.DeleteEvent(eventToDelete.EventId);
                    LoadData();
                }
            }
        }

        private void EditEvent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventToEdit)
            {
                Services.NavigationService.Instance.NavigateTo("EditEvent", eventToEdit);
            }
        }

        private void InfoEvent_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Event eventToView)
            {
                Services.NavigationService.Instance.NavigateTo("EventInfo", eventToView);
            }
        }
    }
}