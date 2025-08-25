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

            // Load current icon or use default - handle file paths vs emojis
            if (!string.IsNullOrEmpty(_currentEventType.IconPath))
            {
                // Check if it's a file path or an emoji
                if (_currentEventType.IconPath.Contains("/") || _currentEventType.IconPath.Contains("\\"))
                {
                    // It's a file path, convert to appropriate emoji based on type
                    _selectedIcon = ConvertFilePathToEmoji(_currentEventType.IconPath);
                }
                else
                {
                    // It's already an emoji
                    _selectedIcon = _currentEventType.IconPath;
                }
            }
            else
            {
                _selectedIcon = "⚙️";
            }

            UpdateIconPreview();
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