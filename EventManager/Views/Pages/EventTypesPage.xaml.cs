using EventManager.Models;
using EventManager.Services;
using System.Linq;
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
            var eventTypes = _dataService.GetAllEventTypes();
            EventTypesListView.ItemsSource = eventTypes;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        // Info button click handler
        private void InfoEventType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is EventType eventTypeToView)
            {
                Services.NavigationService.Instance.NavigateTo("EventTypesInfo", eventTypeToView);
            }
        }

        // Edit button click handler - UPDATED to navigate to edit page
        private void EditEventType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is EventType eventTypeToEdit)
            {
                Services.NavigationService.Instance.NavigateTo("EditEventTypes", eventTypeToEdit);
            }
        }

        // Delete button click handler
        private void DeleteEventType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is EventType eventTypeToDelete)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{eventTypeToDelete.Name}'?",
                                           "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dataService.DeleteEventType(eventTypeToDelete.EventTypeId);
                    LoadEventTypes(); // Refresh the list
                }
            }
        }

        // Add new event type
        private void AddEventType_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateNewEventType())
            {
                var newEventType = new EventType
                {
                    EventTypeId = EventTypeIdTextBox.Text.Trim(),
                    Name = NameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    IconPath = "/Assets/Icons/default.png"
                };

                _dataService.AddEventType(newEventType);
                ClearForm();
                LoadEventTypes();
                MessageBox.Show("Event Type added successfully!", "Success");
            }
        }

        private bool ValidateNewEventType()
        {
            if (string.IsNullOrWhiteSpace(EventTypeIdTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please fill in Event Type ID and Name.", "Validation Error");
                return false;
            }

            // Check if ID already exists
            if (_dataService.GetAllEventTypes().Any(et => et.EventTypeId == EventTypeIdTextBox.Text.Trim()))
            {
                MessageBox.Show("Event Type ID already exists!", "Validation Error");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            EventTypeIdTextBox.Text = "";
            NameTextBox.Text = "";
            DescriptionTextBox.Text = "";
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ChooseIcon_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Icon selection would be implemented here", "Icon Selection");
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}