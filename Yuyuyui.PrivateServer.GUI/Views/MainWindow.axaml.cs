using Avalonia.Controls;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Yuyuyui.PrivateServer.GUI.ViewModels;
using Yuyuyui.PrivateServer.GUI.Controls;

namespace Yuyuyui.PrivateServer.GUI.Views
{
    public partial class MainWindow : Window
    {
        private LogView logView;
        private StatusView statusView;
        private SettingsView settingsView;
        private HelpView helpView;
        private AboutView aboutView;

        private MainWindowViewModel mainWindowVM;

        public MainWindow()
        {
            mainWindowVM = new MainWindowViewModel(this);
            DataContext = mainWindowVM;
            
            InitializeComponent();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                TransparencyLevelHint = WindowTransparencyLevel.Mica;
                ExtendClientAreaToDecorationsHint = true;
                Background = Brushes.Transparent;
            }
            else
            {
                TransparencyLevelHint = WindowTransparencyLevel.None;
                ExtendClientAreaToDecorationsHint = false;
                Background = Brushes.Gray;
            }

            var toolbarVM = new ToolbarViewModel
            {
                ToolbarText = "",
                IsProgressIndeterminate = false,
                ShowProgressText = false,
                ToolbarProgress = 0,
                ProgressBarText = ""
            };
            BottomToolBar.DataContext = toolbarVM;

            logView = new LogView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.logVM
            };

            statusView = new StatusView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.statusVM
            };

            settingsView = new SettingsView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.settingsVM
            };

            helpView = new HelpView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.helpVM
            };

            aboutView = new AboutView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.aboutVM
            };


            MainPageContentControl.Content = logView;

            var logTextDefaultBrush = LogText.Foreground;
            
            Utils.SetLogCallback(
                (o, t) =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        string content = o.ToString() ?? "";
                        LogText.Foreground = t switch
                        {
                            Utils.LogType.Trace => Brushes.Green,
                            Utils.LogType.Info => logTextDefaultBrush,
                            Utils.LogType.Warning => Brushes.Yellow,
                            Utils.LogType.Error => Brushes.Red,
                            _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
                        };
                        toolbarVM.ToolbarText = content;
                        mainWindowVM.logVM.Logs.Add(new LogEntry
                        {
                            LogType = t,
                            LogContent = content
                        });
                    });
                }
            );
            
            Utils.LogTrace(Localization.Resources.LOG_GUI_INITIALIZED);

            if (!Design.IsDesignMode)
            {
                mainWindowVM.UpdateLocalData();
            }
        }

        private void WindowOnClosing(object? sender, CancelEventArgs e)
        {
            mainWindowVM.StopPrivateServer();
        }

        private void OnPointerEnterNavigation(object? sender, PointerEventArgs e)
        {
            Task.Run(() =>
            {
                float time = 0;
                while (time < 0.5f)
                {
                    Thread.Sleep(100);
                    time += 0.1f;
                    if (!NavigationPanel.IsPointerOver) return;
                }

                Dispatcher.UIThread.Post(() => MainSplitView.IsPaneOpen = true);
            });
        }

        private void OnPointerLeaveNavigation(object? sender, PointerEventArgs e)
        {
            MainSplitView.IsPaneOpen = false;
        }

        private void OnNavigationButtonChecked(object? sender, RoutedEventArgs e)
        {
            NavigationButton button = (sender as NavigationButton)!;

            if (MainPageContentControl != null)
            {
                MainPageContentControl.Content = button.Name switch
                {
                    nameof(LogButton) => logView,
                    nameof(StatusButton) => statusView,
                    nameof(SettingsButton) => settingsView,
                    nameof(HelpButton) => helpView,
                    nameof(AboutButton) => aboutView,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            mainWindowVM.ProjectName = "JINCHŌGE";
            mainWindowVM.ProjectDescription = Localization.Resources.PROJ_DESC_JINCHOGE;

            if (button.Name == nameof(SettingsButton))
            {
                mainWindowVM.settingsVM.Refresh();
            }
        }
    }
}
