using EventManager.Models;
using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EditEventTypesPage : Page
    {
        private readonly DataService _dataService;
        private EventType? _currentEventType;
        private string _selectedIcon = "⚙️";

        public EditEventTypesPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            Loaded += EditEventTypesPage_Loaded;
        }

        private void EditEventTypesPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is EventType eventTypeToEdit)
            {
                _currentEventType = eventTypeToEdit;
                LoadEventTypeData();
            }
        }

        private void LoadEventTypeData()
        {
            if (_currentEventType == null) return;

            EventTypeIdTextBox.Text = _currentEventType.EventTypeId;
            NameTextBox.Text = _currentEventType.Name;
            DescriptionTextBox.Text = _currentEventType.Description;

            // Load current icon or use default
            _selectedIcon = string.IsNullOrEmpty(_currentEventType.IconPath) ? "⚙️" : _currentEventType.IconPath;
            UpdateIconPreview();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        private void SaveEventType_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateEventType())
            {
                if (_currentEventType != null)
                {
                    _currentEventType.Name = NameTextBox.Text.Trim();
                    _currentEventType.Description = DescriptionTextBox.Text.Trim();
                    _currentEventType.IconPath = _selectedIcon; // Save selected icon

                    _dataService.UpdateEventType(_currentEventType);
                    MessageBox.Show("Event Type updated successfully!", "Success");
                    Services.NavigationService.Instance.NavigateTo("EventTypes");
                }
            }
        }

        private bool ValidateEventType()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Please fill in Name.", "Validation Error");
                return false;
            }
            return true;
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("EventTypes");
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
    }
}