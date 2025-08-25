using EventManager.Models;
using EventManager.Services;
using System;
using System.Collections.Generic;
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
        private string _selectedIcon = "🎪"; // Default event icon
        private List<DateTime> _previousDates = new List<DateTime>(); // Store previous dates

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

            // Load current icon or use default
            _selectedIcon = string.IsNullOrEmpty(_editingEvent.IconPath) ? "🎪" : _editingEvent.IconPath;
            UpdateIconPreview();

            // Load previous dates
            _previousDates = _editingEvent.PreviousDates?.ToList() ?? new List<DateTime>();
            LoadPreviousDates();
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

        // UPDATED: Implement icon cycling selection for events
        private void ChooseIconButton_Click(object sender, RoutedEventArgs e)
        {
            // Predefined icons for events
            var icons = new[] { "🎪", "🎵", "🎬", "🏀", "💼", "🎭", "🎨", "🌟", "🎆", "🎊" };
            var currentIndex = System.Array.IndexOf(icons, _selectedIcon);
            _selectedIcon = icons[(currentIndex + 1) % icons.Length];
            UpdateIconPreview();
        }

        private void UpdateIconPreview()
        {
            // Update the button text and preview
            if (ChooseIconButton != null)
            {
                ChooseIconButton.Content = $"{_selectedIcon} Choose icon...";
            }
            if (IconPreview != null)
            {
                IconPreview.Text = _selectedIcon;
            }
        }

        private void AddPreviousDateButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a simple date input dialog
            var dateInputWindow = new Window
            {
                Title = "Add Previous Date",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Enter previous date:",
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 14
            });

            var datePicker = new DatePicker
            {
                SelectedDate = DateTime.Now.AddYears(-1),
                Margin = new Thickness(0, 0, 0, 20)
            };
            stackPanel.Children.Add(datePicker);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };
            okButton.Click += (s, args) =>
            {
                dateInputWindow.DialogResult = true;
                dateInputWindow.Close();
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };
            cancelButton.Click += (s, args) =>
            {
                dateInputWindow.DialogResult = false;
                dateInputWindow.Close();
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(buttonPanel);

            dateInputWindow.Content = stackPanel;

            if (dateInputWindow.ShowDialog() == true && datePicker.SelectedDate.HasValue)
            {
                AddPreviousDateToDisplay(datePicker.SelectedDate.Value);
            }
        }

        private void CalendarButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a date picker dialog for current year date
            var dateInputWindow = new Window
            {
                Title = "Select Date This Year",
                Width = 300,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            stackPanel.Children.Add(new TextBlock
            {
                Text = "Select date for this year:",
                Margin = new Thickness(0, 0, 0, 10),
                FontSize = 14
            });

            var datePicker = new DatePicker
            {
                SelectedDate = DateTime.TryParse(DateTextBox.Text, out DateTime currentDate) ? currentDate : DateTime.Now,
                Margin = new Thickness(0, 0, 0, 20)
            };
            stackPanel.Children.Add(datePicker);

            var buttonPanel = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };
            okButton.Click += (s, args) =>
            {
                dateInputWindow.DialogResult = true;
                dateInputWindow.Close();
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5)
            };
            cancelButton.Click += (s, args) =>
            {
                dateInputWindow.DialogResult = false;
                dateInputWindow.Close();
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            stackPanel.Children.Add(buttonPanel);

            dateInputWindow.Content = stackPanel;

            if (dateInputWindow.ShowDialog() == true && datePicker.SelectedDate.HasValue)
            {
                DateTextBox.Text = datePicker.SelectedDate.Value.ToString("MM/dd/yyyy");
            }
        }

        private void LoadPreviousDates()
        {
            // Clear existing dates display (except the sample one)
            if (PreviousDatesPanel != null)
            {
                PreviousDatesPanel.Children.Clear();

                // Load all previous dates
                foreach (var date in _previousDates)
                {
                    CreateDateDisplay(date);
                }
            }
        }

        private void AddPreviousDateToDisplay(DateTime date)
        {
            // Add to our list
            if (!_previousDates.Contains(date))
            {
                _previousDates.Add(date);
                CreateDateDisplay(date);
            }
        }

        private void CreateDateDisplay(DateTime date)
        {
            if (PreviousDatesPanel == null) return;

            var datePanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(10, 5, 0, 0)
            };

            var dateBlock = new TextBlock
            {
                Text = $"📅 {date:yyyy-MM-dd}",
                FontSize = 14,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };

            var removeButton = new Button
            {
                Content = "❌",
                Width = 25,
                Height = 25,
                Background = System.Windows.Media.Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 12
            };

            removeButton.Click += (s, e) =>
            {
                _previousDates.Remove(date);
                PreviousDatesPanel.Children.Remove(datePanel);
            };

            datePanel.Children.Add(dateBlock);
            datePanel.Children.Add(removeButton);
            PreviousDatesPanel.Children.Add(datePanel);
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
            eventObj.IconPath = _selectedIcon; // Save selected icon

            if (decimal.TryParse(AveragePriceTextBox.Text, out decimal price))
                eventObj.AveragePrice = price;

            eventObj.Country = CountryTextBox.Text;
            eventObj.City = CityTextBox.Text;

            if (DateTime.TryParse(DateTextBox.Text, out DateTime date))
                eventObj.CurrentYearDate = date;

            // Save previous dates
            eventObj.PreviousDates = _previousDates.ToList();

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