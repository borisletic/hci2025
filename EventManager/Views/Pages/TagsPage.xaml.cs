using EventManager.Models;
using EventManager.Services;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EventManager.Views.Pages
{
    public partial class TagsPage : Page
    {
        private readonly DataService _dataService;
        private string _selectedColor = "#0066CC";

        public TagsPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            LoadTags();
        }

        private void LoadTags()
        {
            var tags = _dataService.GetAllTags();
            TagsListView.ItemsSource = tags;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        // Info button click handler
        private void InfoTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Tag tagToView)
            {
                Services.NavigationService.Instance.NavigateTo("TagsInfo", tagToView);
            }
        }

        // Edit button click handler
        private void EditTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Tag tagToEdit)
            {
                MessageBox.Show($"Edit Tag: {tagToEdit.Name}", "Edit",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // Delete button click handler
        private void DeleteTag_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Tag tagToDelete)
            {
                var result = MessageBox.Show($"Are you sure you want to delete '{tagToDelete.Name}'?",
                                           "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _dataService.DeleteTag(tagToDelete.TagId);
                    LoadTags(); // Refresh the list
                }
            }
        }

        // Add new tag
        private void AddTag_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateNewTag())
            {
                var newTag = new Tag
                {
                    TagId = TagIdTextBox.Text.Trim(),
                    Name = TagNameTextBox.Text.Trim(),
                    Description = DescriptionTextBox.Text.Trim(),
                    Color = _selectedColor
                };

                _dataService.AddTag(newTag);
                ClearForm();
                LoadTags();
                MessageBox.Show("Tag added successfully!", "Success");
            }
        }

        private bool ValidateNewTag()
        {
            if (string.IsNullOrWhiteSpace(TagIdTextBox.Text) ||
                string.IsNullOrWhiteSpace(TagNameTextBox.Text))
            {
                MessageBox.Show("Please fill in Tag ID and Name.", "Validation Error");
                return false;
            }

            // Check if ID already exists
            if (_dataService.GetAllTags().Any(t => t.TagId == TagIdTextBox.Text.Trim()))
            {
                MessageBox.Show("Tag ID already exists!", "Validation Error");
                return false;
            }

            return true;
        }

        private void ClearForm()
        {
            TagIdTextBox.Text = "";
            TagNameTextBox.Text = "";
            DescriptionTextBox.Text = "";
            _selectedColor = "#0066CC";
            ColorPreview.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_selectedColor));
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ChooseColor_Click(object sender, RoutedEventArgs e)
        {
            // Simple color selection - cycle through predefined colors
            var colors = new[] { "#0066CC", "#28A745", "#DC3545", "#FFD700", "#9B59B6", "#FF6B6B" };
            var currentIndex = System.Array.IndexOf(colors, _selectedColor);
            _selectedColor = colors[(currentIndex + 1) % colors.Length];
            ColorPreview.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_selectedColor));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}