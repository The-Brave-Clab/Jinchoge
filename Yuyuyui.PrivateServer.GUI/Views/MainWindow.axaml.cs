using Avalonia.Controls;
using System;
using System.ComponentModel;
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
        private TransferView transferView;
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

            var bottomToolbarVM = new MainWindowBottomToolbarViewModel
            {
                ToolbarText = "",
                IsProgressIndeterminate = false,
                ShowProgressText = false,
                ToolbarProgress = 0,
                ProgressBarText = ""
            };
            BottomToolBar.DataContext = bottomToolbarVM;

            logView = new LogView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.logVM
            };

            transferView = new TransferView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.transferVM
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
                VerticalAlignment = VerticalAlignment.Stretch
            };

            helpView = new HelpView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            aboutView = new AboutView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
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
                        bottomToolbarVM.ToolbarText = content;
                        mainWindowVM.logVM.Logs.Add(new LogEntry
                        {
                            LogType = t,
                            LogContent = content
                        });
                    });
                }
            );
            
            Utils.LogTrace("Initialized GUI.");

            if (!Design.IsDesignMode)
            {
                mainWindowVM.UpdateLocalData();
            }
        }

        private void WindowOnClosing(object? sender, CancelEventArgs e)
        {
            mainWindowVM.StopPrivateServer();
            mainWindowVM.transferVM.StopTransfer();
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
                    nameof(TransferButton) => transferView,
                    nameof(StatusButton) => statusView,
                    nameof(SettingsButton) => settingsView,
                    nameof(HelpButton) => helpView,
                    nameof(AboutButton) => aboutView,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
