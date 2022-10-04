using Avalonia.Controls;
using System;
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

            object logLock = new();
            Utils.SetLogCallbacks(
                o =>
                {
                    lock (logLock)
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                },
                o =>
                {
                    lock (logLock)
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                },
                o =>
                {
                    lock (logLock)
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                },
                o =>
                {
                    lock (logLock)
                        bottomToolbarVM.ToolbarText = o.ToString() ?? "";
                }
            );
        }
    }
}
