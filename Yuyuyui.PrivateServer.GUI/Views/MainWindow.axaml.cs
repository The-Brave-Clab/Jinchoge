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
        private StatusPage statusPage;
        private SettingsPage settingsPage;
        private TutorialPage tutorialPage;
        private AboutPage aboutPage;

        private MainWindowViewModel mainWindowVM;

        public MainWindow()
        {
            mainWindowVM = new MainWindowViewModel();
            mainWindowVM.SetWindow(this);
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

            tutorialPage = new TutorialPage
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

            Utils.SetLogCallbacks(
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Green;
                        string content = o.ToString() ?? "";
                        bottomToolbarVM.ToolbarText = content;
                        mainWindowVM.consolePageVM.Logs.Add(new()
                        {
                            LogType = LogEntryControl.LogType.Trace,
                            LogContent = content
                        });
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = logTextDefaultBrush;
                        string content = o.ToString() ?? "";
                        bottomToolbarVM.ToolbarText = content;
                        mainWindowVM.consolePageVM.Logs.Add(new()
                        {
                            LogType = LogEntryControl.LogType.Log,
                            LogContent = content
                        });
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Yellow;
                        string content = o.ToString() ?? "";
                        bottomToolbarVM.ToolbarText = content;
                        mainWindowVM.consolePageVM.Logs.Add(new()
                        {
                            LogType = LogEntryControl.LogType.Warning,
                            LogContent = content
                        });
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Red;
                        string content = o.ToString() ?? "";
                        bottomToolbarVM.ToolbarText = content;
                        mainWindowVM.consolePageVM.Logs.Add(new()
                        {
                            LogType = LogEntryControl.LogType.Error,
                            LogContent = content
                        });
                    });
                }
            );
            
            Utils.LogTrace("Initialized GUI.");
        }

        private void WindowOnClosing(object? sender, CancelEventArgs e)
        {
            ((MainWindowViewModel) DataContext!).StopPrivateServer();
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
                    nameof(StatusButton) => statusPage,
                    nameof(SettingsButton) => settingsPage,
                    nameof(TutorialButton) => tutorialPage,
                    nameof(AboutButton) => aboutPage,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
