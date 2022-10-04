using Avalonia.Controls;
using System;
using System.ComponentModel;
using Avalonia.Media;
using Avalonia.Threading;
using Yuyuyui.PrivateServer.GUI.ViewModels;

namespace Yuyuyui.PrivateServer.GUI.Views
{
    public partial class MainWindow : Window
    {
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

            var logTextDefaultBrush = LogText.Foreground;

            Utils.SetLogCallbacks(
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Green;
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = logTextDefaultBrush;
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Yellow;
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                    });
                },
                o =>
                {
                    Dispatcher.UIThread.Post(() =>
                    {
                        LogText.Foreground = Brushes.Green;
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
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
