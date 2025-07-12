using EventManager.Services;
using System.Windows;
using System.Windows.Controls;

namespace EventManager.Views.Pages
{
    public partial class WorldMapPage : Page
    {
        public WorldMapPage()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Services.NavigationService.Instance.NavigateTo("Home");
        }
    }
}