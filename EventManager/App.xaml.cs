using System.Windows;

namespace EventManager
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize services
            _ = Services.DataService.Instance;
        }
    }
}