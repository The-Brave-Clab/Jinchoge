using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using ReactiveUI;
using Titanium.Web.Proxy.Models;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WeakReference<MainWindow?> window = new(null);

        internal LogViewModel logVM;
        internal TransferViewModel transferVM;
        internal StatusViewModel statusVM;
        internal SettingsViewModel settingsVM;
        internal HelpViewModel helpVM;
        internal AboutViewModel aboutVM;

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

            logVM = new LogViewModel();
            transferVM = new TransferViewModel(this);
            statusVM = new StatusViewModel();
            settingsVM = new SettingsViewModel(this);
            helpVM = new HelpViewModel();
            aboutVM = new AboutViewModel();
        }

        public MainWindowViewModel()
        {
            if (!Design.IsDesignMode)
                throw new NotImplementedException();
            
            endpoint = null;
            buttonContent = "";
            buttonDescription = "";
            Status = ServerStatus.Stopped;

            logVM = new LogViewModel();
            transferVM = new TransferViewModel(this);
            statusVM = new StatusViewModel();
            settingsVM = new SettingsViewModel(this);
            helpVM = new HelpViewModel();
            aboutVM = new AboutViewModel();
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
                ButtonContent = GetButtonContent(status);
                ButtonDescription = GetButtonDescription(status);

                statusVM?.SetServerStatus(value);
                transferVM?.SetServerStatus(value);
                
                window.TryGetTarget(out var mainWindow);
                if (!IsTransferPageEnabled && (bool)mainWindow!.TransferButton.IsChecked!)
                {
                    // we are on transfer page when transfer is disabled, fall back to log
                    mainWindow.LogButton.IsChecked = true;
                }
            }
        }

        public static string GetButtonContent(ServerStatus status)
        {
            return status switch
            {
                ServerStatus.Updating => Localization.Resources.PS_BUTTON_UPDATE,
                ServerStatus.Stopped => Localization.Resources.PS_BUTTON_START,
                ServerStatus.Started => Localization.Resources.PS_BUTTON_STOP,
                ServerStatus.Transfer => Localization.Resources.PS_BUTTON_TRANSFER,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public static string GetButtonDescription(ServerStatus status)
        {
            return status switch
            {
                ServerStatus.Updating => Localization.Resources.PS_BUTTON_UPDATE_DESC,
                ServerStatus.Stopped => Localization.Resources.PS_BUTTON_START_DESC,
                ServerStatus.Started => Localization.Resources.PS_BUTTON_STOP_DESC,
                ServerStatus.Transfer => Localization.Resources.PS_BUTTON_TRANSFER_DESC,
                _ => throw new ArgumentOutOfRangeException()
            };
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
                    break;
                case ServerStatus.Started:
                    StopPrivateServer();
                    break;
                default:
                    return;
            }
            
            settingsVM.Refresh();
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
        
        private string projectDescription = "";
        public string ProjectDescription
        {
            get => projectDescription;
            set => this.RaiseAndSetIfChanged(ref projectDescription, value);
        }

        private string projectName = "";
        public string ProjectName
        {
            get => projectName;
            set => this.RaiseAndSetIfChanged(ref projectName, value);
        }

        public string NAV_LOG => Localization.Resources.NAV_BUTTON_LOG;
        public string NAV_TRANSFER => Localization.Resources.NAV_BUTTON_TRANSFER;
        public string NAV_STATUS => Localization.Resources.NAV_BUTTON_STATUS;
        public string NAV_SETTINGS => Localization.Resources.NAV_BUTTON_SETTINGS;
        public string NAV_HELP => Localization.Resources.NAV_BUTTON_HELP;
        public string NAV_ABOUT => Localization.Resources.NAV_BUTTON_ABOUT;
        public string WINDOW_TITLE => "Jinchōge";

        public string VERSION_NUMBER => Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;

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
            var toolbarVM = (ToolbarViewModel)mainWindow!.BottomToolBar.DataContext!;
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

            Utils.LogTrace(Localization.Resources.LOG_PS_START);
        }

        public void StopPrivateServer()
        {
            if (Status != ServerStatus.Started) return;

            Proxy<PrivateServerProxyCallbacks>.Stop();

            Status = ServerStatus.Stopped;

            Utils.LogTrace(Localization.Resources.LOG_PS_STOP);
        }
    }
}