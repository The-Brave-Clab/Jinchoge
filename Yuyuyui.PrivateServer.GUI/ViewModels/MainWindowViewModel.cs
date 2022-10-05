using System;
using System.Threading.Tasks;
using System.Threading;
using Yuyuyui.PrivateServer.GUI.Views;
using System.ComponentModel;
using Avalonia.Threading;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WeakReference<MainWindow?> window = new (null);

        public void SetWindow(MainWindow window)
        {
            this.window.SetTarget(window);
        }

        public bool ServerStarted { get; private set; } = false;

        public void StartPrivateServer()
        {
            window.TryGetTarget(out var mainWindow);
            var toolbarVM = (MainWindowBottomToolbarViewModel) mainWindow!.BottomToolBar.DataContext!;
            toolbarVM.ClearProgressBar();

            toolbarVM.IsProgressIndeterminate = true;
            toolbarVM.ProgressBarText = "";
            toolbarVM.IsProgressBarVisible = true;

            string currentFileName = "";
            int currentCount = 0;
            int totalCount = 0;
            bool canSet = true;
            void SetToolbarText()
            {
                toolbarVM.ProgressBarText = string.IsNullOrEmpty(currentFileName) && totalCount == 0 ? 
                    "" : 
                    $"{currentFileName} ({currentCount}/{totalCount})";
            }

            LocalData.Update(
                    (fileName, progress) =>
                    {
                        if (!canSet) return;
                        toolbarVM.ShowProgressText = true;
                        toolbarVM.IsProgressIndeterminate = false;
                        toolbarVM.ToolbarProgress = progress * 100;
                        currentFileName = fileName;
                        SetToolbarText();
                    },
                    (current, total) =>
                    {
                        if (!canSet) return;
                        toolbarVM.IsProgressIndeterminate = false;
                        if (total > 0)
                        {
                            toolbarVM.ShowProgressText = true;
                            currentCount = current;
                            totalCount = total;
                        }

                        SetToolbarText();
                    }
                )
                .ContinueWith(t => Console.WriteLine(t.Exception), TaskContinuationOptions.OnlyOnFaulted)
                .ContinueWith(_ =>
                {
                    Task.Run(() =>
                    {
                        toolbarVM.ToolbarProgress = totalCount == 0 ? 0 : 100;
                        canSet = false;
                        Thread.Sleep(2000);
                        toolbarVM.ClearProgressBar();
                    });

                    var endpoint = Proxy<PrivateServerProxyCallbacks>.Start();

                    ServerStarted = true;

                    Utils.LogTrace("Private Server Started!");
                });
        }

        public void StopPrivateServer()
        {
            if (!ServerStarted) return;

            Proxy<PrivateServerProxyCallbacks>.Stop();
        }
    }
}
