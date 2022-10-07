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
        private ConsolePage consolePage;
        private TransferPage transferPage;
        private StatusPage statusPage;
        private SettingsPage settingsPage;
        private HelpPage helpPage;
        private AboutPage aboutPage;

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

            consolePage = new ConsolePage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.consolePageVM
            };

            transferPage = new TransferPage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.transferPageVM
            };

            statusPage = new StatusPage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                DataContext = mainWindowVM.statusPageVM
            };

            settingsPage = new SettingsPage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            helpPage = new HelpPage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };

            aboutPage = new AboutPage
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };


            MainPageContentControl.Content = consolePage;

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
                        mainWindowVM.consolePageVM.Logs.Add(new LogEntry
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
            mainWindowVM.transferPageVM.StopTransfer();
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
                    nameof(LogButton) => consolePage,
                    nameof(TransferButton) => transferPage,
                    nameof(StatusButton) => statusPage,
                    nameof(SettingsButton) => settingsPage,
                    nameof(HelpButton) => helpPage,
                    nameof(AboutButton) => aboutPage,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
