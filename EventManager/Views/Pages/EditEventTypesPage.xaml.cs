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

        private void ChooseIcon_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Icon selection would be implemented here", "Icon Selection");
        }
    }
}