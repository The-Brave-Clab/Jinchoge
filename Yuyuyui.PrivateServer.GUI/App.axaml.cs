using System.Globalization;
using System.Threading;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            Config.Load();

            AvaloniaXamlLoader.Load(this);

            string language = Config.Get().General.Language;
            if (!string.IsNullOrEmpty(language))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(language);
            }
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
