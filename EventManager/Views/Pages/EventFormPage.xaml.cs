using EventManager.Models;
using EventManager.Services;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class EventFormPage : Page
    {
        private readonly DataService _dataService;
        private Event? _editingEvent;
        private bool _isEditMode;

        public EventFormPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadComboBoxData();
            Loaded += EventFormPage_Loaded;
        }

        private void EventFormPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Event eventToEdit)
            {
                _editingEvent = eventToEdit;
                _isEditMode = true;
                PageTitle.Text = "Edit Event 1/2";
                LoadEventData();
            }
            else
            {
                _isEditMode = false;
                PageTitle.Text = "Add New Event 1/2";
                GenerateNewEventId();
            }
        }

        private void LoadComboBoxData()
        {
            var eventTypes = _dataService.GetAllEventTypes();
            EventTypeComboBox.ItemsSource = eventTypes;
            EventTypeComboBox.DisplayMemberPath = "Name";
            EventTypeComboBox.SelectedValuePath = "EventTypeId";

            AttendanceComboBox.ItemsSource = System.Enum.GetValues(typeof(Attendance))
                .Cast<Attendance>()
                .Select(a => new { Value = a, Display = a.ToDisplayString() });
            AttendanceComboBox.DisplayMemberPath = "Display";
            AttendanceComboBox.SelectedValuePath = "Value";
        }

        private void GenerateNewEventId()
        {
            var existingIds = _dataService.GetAllEvents().Select(e => e.EventId).ToList();
            var counter = 1;
            string newId;

            do
            {
                newId = $"event-{counter:D3}";
                counter++;
            } while (existingIds.Contains(newId));

            EventIdTextBox.Text = newId;
        }

        private void LoadEventData()
        {
            if (_editingEvent == null) return;

            EventIdTextBox.Text = _editingEvent.EventId;
            EventNameTextBox.Text = _editingEvent.Name;
            EventDescriptionTextBox.Text = _editingEvent.Description;
            EventTypeComboBox.SelectedValue = _editingEvent.EventTypeId;
            AttendanceComboBox.SelectedValue = _editingEvent.Attendance;
            HumanitarianCheckBox.IsChecked = _editingEvent.IsHumanitarian;
            AveragePriceTextBox.Text = _editingEvent.AveragePrice.ToString();

            // Step 2 data
            CountryTextBox.Text = _editingEvent.Country;
            CityTextBox.Text = _editingEvent.City;
            DateTextBox.Text = _editingEvent.CurrentYearDate.ToString("MM/dd/yyyy");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateStep1())
            {
                Step1Panel.Visibility = Visibility.Collapsed;
                Step2Panel.Visibility = Visibility.Visible;
                PageTitle.Text = _isEditMode ? "Edit Event 2/2" : "Add New Event 2/2";
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            Step2Panel.Visibility = Visibility.Collapsed;
            Step1Panel.Visibility = Visibility.Visible;
            PageTitle.Text = _isEditMode ? "Edit Event 1/2" : "Add New Event 1/2";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateStep2())
            {
                SaveEvent();
                Services.NavigationService.Instance.NavigateTo("EventList");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }

        private void ChooseIconButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Icon selection would be implemented here", "Icon Selection",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AddPreviousDateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Previous date selection would be implemented here", "Add Date",
                          MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool ValidateStep1()
        {
            if (string.IsNullOrWhiteSpace(EventIdTextBox.Text) ||
                string.IsNullOrWhiteSpace(EventNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EventDescriptionTextBox.Text) ||
                EventTypeComboBox.SelectedValue == null ||
                AttendanceComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private bool ValidateStep2()
        {
            if (string.IsNullOrWhiteSpace(CountryTextBox.Text) ||
                string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                MessageBox.Show("Please fill in all required fields.", "Validation Error",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void SaveEvent()
        {
            var eventObj = _editingEvent ?? new Event();

            eventObj.EventId = EventIdTextBox.Text;
            eventObj.Name = EventNameTextBox.Text;
            eventObj.Description = EventDescriptionTextBox.Text;
            eventObj.EventTypeId = EventTypeComboBox.SelectedValue?.ToString() ?? "";
            eventObj.Attendance = (Attendance)(AttendanceComboBox.SelectedValue ?? Attendance.UpTo1000);
            eventObj.IsHumanitarian = HumanitarianCheckBox.IsChecked ?? false;

            if (decimal.TryParse(AveragePriceTextBox.Text, out decimal price))
                eventObj.AveragePrice = price;

            eventObj.Country = CountryTextBox.Text;
            eventObj.City = CityTextBox.Text;

            if (DateTime.TryParse(DateTextBox.Text, out DateTime date))
                eventObj.CurrentYearDate = date;

            if (_isEditMode)
            {
                _dataService.UpdateEvent(eventObj);
            }
            else
            {
                _dataService.AddEvent(eventObj);
            }
        }
    }
}