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
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(language);
            if (!Equals(cultureInfo, CultureInfo.InvariantCulture))
            {
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
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
