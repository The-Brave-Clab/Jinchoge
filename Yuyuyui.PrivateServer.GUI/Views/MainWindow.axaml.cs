using Avalonia.Controls;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Yuyuyui.PrivateServer.GUI.ViewModels;
using Yuyuyui.PrivateServer.GUI.Controls;

namespace Yuyuyui.PrivateServer.GUI.Views
{
    public partial class MainWindow : Window
    {
        private ConsolePageViewModel consolePageVM;

        public MainWindow()
        {
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

            consolePageVM = new ConsolePageViewModel();
            ConsolePage.DataContext = consolePageVM;

            var logTextDefaultBrush = LogText.Foreground;

            Utils.SetLogCallbacks(
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Green;
                        string content = o.ToString() ?? "";
                        bottomToolbarVM.ToolbarText = content;
                        consolePageVM.Logs.Add(new()
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
                        consolePageVM.Logs.Add(new()
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
                        consolePageVM.Logs.Add(new()
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
                        consolePageVM.Logs.Add(new()
                        {
                            LogType = LogEntryControl.LogType.Error,
                            LogContent = content
                        });
                    });
                }
            );
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
    }
}
