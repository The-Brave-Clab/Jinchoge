using System;
using System.Threading.Tasks;
using System.Threading;
using Yuyuyui.PrivateServer.GUI.Views;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
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

        internal ConsolePageViewModel consolePageVM;
        internal TransferPageViewModel transferPageVM;
        internal StatusPageViewModel statusPageVM;

        public enum ServerStatus
        {
            Updating,
            Stopped,
            Started,
            Transfer
        }

        public MainWindowViewModel(MainWindow window)
        {
            this.window.SetTarget(window);
            
            endpoint = null;
            buttonContent = "";
            buttonDescription = "";
            Status = ServerStatus.Stopped;

            consolePageVM = new ConsolePageViewModel();
            transferPageVM = new TransferPageViewModel(this);
            statusPageVM = new StatusPageViewModel();
        }

        public MainWindowViewModel()
        {
            if (!Design.IsDesignMode)
                throw new NotImplementedException();
            
            endpoint = null;
            buttonContent = "";
            buttonDescription = "";
            Status = ServerStatus.Stopped;

            consolePageVM = new ConsolePageViewModel();
            transferPageVM = new TransferPageViewModel(this);
            statusPageVM = new StatusPageViewModel();
        }

        private ServerStatus status = ServerStatus.Stopped;
        private ExplicitProxyEndPoint? endpoint;

        public ServerStatus Status
        {
            get => status;
            set
            {
                status = value;

                IsStopped = status == ServerStatus.Stopped;
                IsStarted = status == ServerStatus.Started;
                CanStart = (status != ServerStatus.Transfer && status != ServerStatus.Updating);
                IsTransferPageEnabled = (status != ServerStatus.Started && status != ServerStatus.Updating);
                ButtonContent = status switch
                {
                    ServerStatus.Updating => "UPDATING",
                    ServerStatus.Stopped => "START",
                    ServerStatus.Started => "STOP",
                    ServerStatus.Transfer => "TRANSFER",
                    _ => throw new ArgumentOutOfRangeException()
                };
                ButtonDescription = status switch
                {
                    ServerStatus.Updating => "Updating Required Files...",
                    ServerStatus.Stopped => "Start the Private Server",
                    ServerStatus.Started => $"Listening at Port {endpoint!.Port}",
                    ServerStatus.Transfer => "Transferring Account...",
                    _ => throw new ArgumentOutOfRangeException()
                };

                statusPageVM?.SetServerStatus(value);
                transferPageVM?.SetServerStatus(value);
                
                window.TryGetTarget(out var mainWindow);
                if (!IsTransferPageEnabled && (bool)mainWindow!.TransferButton.IsChecked!)
                {
                    // we are on transfer page when transfer is disabled, fall back to log
                    mainWindow.LogButton.IsChecked = true;
                }
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
                case ServerStatus.Started:
                    StopPrivateServer();
                    return;
                default:
                    return;
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

        private bool canStart;
        public bool CanStart
        {
            get => canStart;
            set => this.RaiseAndSetIfChanged(ref canStart, value);
        }

        private bool isTransferPageEnabled;
        public bool IsTransferPageEnabled
        {
            get => isTransferPageEnabled;
            set => this.RaiseAndSetIfChanged(ref isTransferPageEnabled, value);
        }

        public TextAlignment TitleAlignment => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TextAlignment.Left
            : TextAlignment.Center;

        public Thickness TitleMargin => RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? new Thickness(10, 5, 0, 0)
            : new Thickness(0, 5, 0, 0);

        public void UpdateLocalData()
        {
            Status = ServerStatus.Updating;

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

                    Status = ServerStatus.Stopped;
                });
        }

        private void StartPrivateServer()
        {
            endpoint = Proxy<PrivateServerProxyCallbacks>.Start();

            Status = ServerStatus.Started;

            Utils.LogTrace("Private Server Started!");
        }

        public void StopPrivateServer()
        {
            if (Status != ServerStatus.Started) return;

            Proxy<PrivateServerProxyCallbacks>.Stop();

            Status = ServerStatus.Stopped;

            Utils.LogTrace("Private Server Stopped!");
        }
    }
}