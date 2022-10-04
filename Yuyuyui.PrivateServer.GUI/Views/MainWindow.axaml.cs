using Avalonia.Controls;
using System;
using System.ComponentModel;
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
    }
}
