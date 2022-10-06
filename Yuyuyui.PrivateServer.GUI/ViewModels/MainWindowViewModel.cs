using System;
using System.Threading.Tasks;
using System.Threading;
using Yuyuyui.PrivateServer.GUI.Views;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using Titanium.Web.Proxy.Models;
using Yuyuyui.PrivateServer.GUI.Controls;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WeakReference<MainWindow?> window = new(null);

        public void SetWindow(MainWindow window)
        {
            this.window.SetTarget(window);
        }

        internal ConsolePageViewModel consolePageVM;
        internal StatusPageViewModel statusPageVM;

        public enum ServerStatus
        {
            Stopped,
            Starting,
            Started
        }

        public MainWindowViewModel()
        {
            endpoint = null;
            buttonContent = "";
            buttonDescription = "";
            Status = ServerStatus.Stopped;

            consolePageVM = new ConsolePageViewModel();
            statusPageVM = new StatusPageViewModel();
        }

        private ServerStatus status = ServerStatus.Stopped;
        private ExplicitProxyEndPoint? endpoint;

        private ServerStatus Status
        {
            get => status;
            set
            {
                status = value;

                IsStopped = status == ServerStatus.Stopped;
                IsStarted = status == ServerStatus.Started;
                IsLoading = status == ServerStatus.Starting;
                ButtonContent = status switch
                {
                    ServerStatus.Stopped => "START",
                    ServerStatus.Starting => "STARTING",
                    ServerStatus.Started => "STOP",
                    _ => throw new ArgumentOutOfRangeException()
                };
                ButtonDescription = status switch
                {
                    ServerStatus.Stopped => "Start the Private Server",
                    ServerStatus.Starting => "Starting, Please Wait...",
                    ServerStatus.Started => $"Listening at Port {endpoint!.Port}",
                    _ => throw new ArgumentOutOfRangeException()
                };

                statusPageVM?.SetServerStatus(value);
            }
        }

        private string buttonContent;

        public string ButtonContent
        {
            get => buttonContent;
            set => this.RaiseAndSetIfChanged(ref buttonContent, value);
        }

        private string buttonDescription;

        public string ButtonDescription
        {
            get => buttonDescription;
            set => this.RaiseAndSetIfChanged(ref buttonDescription, value);
        }

        public void ButtonPress()
        {
            switch (Status)
            {
                case ServerStatus.Stopped:
                    StartPrivateServer();
                    return;
                case ServerStatus.Starting:
                    return;
                case ServerStatus.Started:
                    StopPrivateServer();
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool isStopped;

        public bool IsStopped
        {
            get => isStopped;
            set => this.RaiseAndSetIfChanged(ref isStopped, value);
        }

        private bool isStarted;
        public bool IsStarted
        {
            get => isStarted;
            set => this.RaiseAndSetIfChanged(ref isStarted, value);
        }

        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set => this.RaiseAndSetIfChanged(ref isLoading, value);
        }

        public TextAlignment TitleAlignment => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TextAlignment.Left
            : TextAlignment.Center;

        public Thickness TitleMargin => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new Thickness(10, 5, 0, 0)
            : new Thickness(0, 5, 0, 0);

        public void StartPrivateServer()
        {
            Status = ServerStatus.Starting;

            window.TryGetTarget(out var mainWindow);
            var toolbarVM = (MainWindowBottomToolbarViewModel)mainWindow!.BottomToolBar.DataContext!;
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
                toolbarVM.ProgressBarText = string.IsNullOrEmpty(currentFileName) && totalCount == 0
                    ? ""
                    : $"{currentFileName} ({currentCount}/{totalCount})";
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

                    endpoint = Proxy<PrivateServerProxyCallbacks>.Start();

                    Status = ServerStatus.Started;

                    Utils.LogTrace("Private Server Started!");
                });
        }

        public void StopPrivateServer()
        {
            if (Status == ServerStatus.Stopped) return;

            if (Status == ServerStatus.Started)
            {
                Proxy<PrivateServerProxyCallbacks>.Stop();
            }

            Status = ServerStatus.Stopped;

            Utils.LogTrace("Private Server Stopped!");
        }
    }
}