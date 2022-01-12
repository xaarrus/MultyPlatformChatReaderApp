using MultyPlatformChatReaderApp.Locators;
using System.Windows;

namespace MultyPlatformChatReaderApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            MainLocator.Init();

            base.OnStartup(e);
        }
    }
}
