using EventManager.Models;
using EventManager.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EventManager.Views.Pages
{
    public partial class EditTagsPage : Page
    {
        private readonly DataService _dataService;
        private Tag? _currentTag;
        private string _selectedColor = "#0066CC";

        public EditTagsPage()
        {
            InitializeComponent();
            _dataService = DataService.Instance;
            Loaded += EditTagsPage_Loaded;
        }

        private void EditTagsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is Tag tagToEdit)
            {
                _currentTag = tagToEdit;
                LoadTagData();
            }
        }

        private void LoadTagData()
        {
            if (_currentTag == null) return;

            TagIdTextBox.Text = _currentTag.TagId;
            TagNameTextBox.Text = _currentTag.Name;
            DescriptionTextBox.Text = _currentTag.Description;
            _selectedColor = _currentTag.Color;
            ColorPreview.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_selectedColor));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.GoBack();
        }

        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateTag())
            {
                if (_currentTag != null)
                {
                    _currentTag.Name = TagNameTextBox.Text.Trim();
                    _currentTag.Description = DescriptionTextBox.Text.Trim();
                    _currentTag.Color = _selectedColor;

                    _dataService.UpdateTag(_currentTag);
                    MessageBox.Show("Tag updated successfully!", "Success");
                    Services.NavigationService.Instance.NavigateTo("Tags");
                }
            }
        }

        private bool ValidateTag()
        {
            if (string.IsNullOrWhiteSpace(TagNameTextBox.Text))
            {
                MessageBox.Show("Please fill in Tag Name.", "Validation Error");
                return false;
            }
            return true;
        }

        private void CancelEdit_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Tags");
        }

        private void ChooseColor_Click(object sender, RoutedEventArgs e)
        {
            // Simple color selection - cycle through predefined colors
            var colors = new[] { "#0066CC", "#28A745", "#DC3545", "#FFD700", "#9B59B6", "#FF6B6B" };
            var currentIndex = System.Array.IndexOf(colors, _selectedColor);
            _selectedColor = colors[(currentIndex + 1) % colors.Length];
            ColorPreview.Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(_selectedColor));
        }
    }
}