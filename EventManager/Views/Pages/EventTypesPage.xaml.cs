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
        private string _selectedIcon = "⚙️"; // Default icon

        public EventTypesPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadEventTypes();
        }

        private void LoadEventTypes()
        {
            var eventTypes = _dataService.GetAllEventTypes();

            // Convert file paths to emojis for display
            foreach (var eventType in eventTypes)
            {
                if (!string.IsNullOrEmpty(eventType.IconPath) &&
                    (eventType.IconPath.Contains("/") || eventType.IconPath.Contains("\\")))
                {
                    // Convert file path to emoji for display
                    eventType.IconPath = ConvertFilePathToEmoji(eventType.IconPath);
                }
            }

            EventTypesListView.ItemsSource = eventTypes;
        }

        private string ConvertFilePathToEmoji(string filePath)
        {
            // Convert old file paths to emojis based on the path content
            var path = filePath.ToLower();
            if (path.Contains("music")) return "🎵";
            if (path.Contains("film") || path.Contains("movie")) return "🎬";
            if (path.Contains("sports")) return "🏀";
            if (path.Contains("conference") || path.Contains("business")) return "💼";
            if (path.Contains("cultural") || path.Contains("culture")) return "🎭";
            if (path.Contains("art")) return "🎨";
            return "⚙️"; // default
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

        // Edit button click handler
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
                    IconPath = _selectedIcon // Use selected icon instead of default path
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
            _selectedIcon = "⚙️"; // Reset to default icon
            UpdateIconPreview();
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        // UPDATED: Implement icon cycling selection
        private void ChooseIcon_Click(object sender, RoutedEventArgs e)
        {
            // Predefined icons for event types
            var icons = new[] { "⚙️", "🎵", "🎬", "🏀", "💼", "🎭", "🎪", "🎨", "📚", "🌟" };
            var currentIndex = System.Array.IndexOf(icons, _selectedIcon);
            _selectedIcon = icons[(currentIndex + 1) % icons.Length];
            UpdateIconPreview();
        }

        private void UpdateIconPreview()
        {
            // Update the button text and preview
            if (ChooseIconButton != null)
            {
                ChooseIconButton.Content = $"{_selectedIcon} Choose icon";
            }
            if (IconPreview != null)
            {
                IconPreview.Text = _selectedIcon;
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}