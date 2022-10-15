using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Media;

namespace Yuyuyui.PrivateServer.GUI
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
        {
            Config.Load();

            string language = Config.Get().General.Language;
            CultureInfo cultureInfo = CultureInfo.GetCultureInfo(language);
            if (!Equals(cultureInfo, CultureInfo.InvariantCulture))
                Thread.CurrentThread.CurrentUICulture = cultureInfo;

            IReadOnlyList<FontFallback> fallbacks = Array.Empty<FontFallback>();

            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "zh")
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    fallbacks = new[]
                    {
                        new FontFallback { FontFamily = new FontFamily("Microsoft YaHei UI") },
                    };
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    fallbacks = new[]
                    {
                        new FontFallback { FontFamily = new FontFamily("PingFang SC") },
                    };
                }
                else
                {
                    fallbacks = new[]
                    {
                        new FontFallback { FontFamily = new FontFamily("Noto Sans CJK SC") },
                    };
                }
            }

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new FontManagerOptions
                {
                    FontFallbacks = fallbacks
                })
                .LogToTrace()
                .UseReactiveUI();
        }
    }
}