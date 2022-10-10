using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using ReactiveUI;
using Titanium.Web.Proxy.Models;
using Yuyuyui.AccountTransfer;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class TransferViewModel : ViewModelBase
{
    private ExplicitProxyEndPoint? endpoint;
    private WeakReference<MainWindowViewModel?> mainWindowVM = new(null);

    public TransferViewModel(MainWindowViewModel mainWindowViewModel)
    {
        mainWindowVM.SetTarget(mainWindowViewModel);

        buttonContent = "";
        TaskCompleteStatusList = new ObservableCollection<TaskCompleteStatus>();

        ResetTaskCompleteStatus(TaskCompleteStatusList);
    }

    public TransferViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        buttonContent = "";
        TaskCompleteStatusList = new ObservableCollection<TaskCompleteStatus>();

        ResetTaskCompleteStatus(TaskCompleteStatusList);
    }

    public void SetServerStatus(MainWindowViewModel.ServerStatus status)
    {
        ButtonContent = GetButtonContent(status);
    }

    public static string GetButtonContent(MainWindowViewModel.ServerStatus status)
    {
        return status switch
        {
            MainWindowViewModel.ServerStatus.Stopped => Localization.Resources.AT_BUTTON_START,
            MainWindowViewModel.ServerStatus.Transfer => Localization.Resources.AT_BUTTON_STOP,
            _ => ""
        };
    }

    private string buttonContent;

    public string ButtonContent
    {
        get => buttonContent;
        set => this.RaiseAndSetIfChanged(ref buttonContent, value);
    }

    public class TaskCompleteStatus : ReactiveObject
    {
        public TransferProgress.TaskType TaskType { get; set; }

        private string taskName = "";
        public string TaskName
        {
            get => taskName; 
            set => this.RaiseAndSetIfChanged(ref taskName, value);
        }

        private bool isCompleted = false;

        public bool IsCompleted
        {
            get => isCompleted;
            set => this.RaiseAndSetIfChanged(ref isCompleted, value);
        }
    }

    public ObservableCollection<TaskCompleteStatus> TaskCompleteStatusList { get; }

    public void ButtonPress()
    {
        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        switch (mainWindowViewModel!.Status)
        {
            case MainWindowViewModel.ServerStatus.Stopped:
                StartTransfer();
                return;
            case MainWindowViewModel.ServerStatus.Transfer:
                StopTransfer();
                return;
            default:
                return;
        }
    }

    public static void ResetTaskCompleteStatus(ObservableCollection<TaskCompleteStatus> list)
    {
        list.Clear();
        foreach (var task in TransferProgress.TaskName)
        {
            list.Add(new()
            {
                TaskType = task.Key,
                TaskName = task.Value,
                IsCompleted = false
            });
        }
    }

    private void StartTransfer()
    {
        ResetTaskCompleteStatus(TaskCompleteStatusList);
        
        TransferProgress.RegisterTaskCompleteCallback((task, progress) =>
        {
            TaskCompleteStatusList.First(t => t.TaskType == task).IsCompleted = true;
        });
        
        TransferProgress.RegisterAllTaskCompleteCallback(() =>
        {
            StopTransfer();

            Task.Run(() =>
            {
                Thread.Sleep(5000);
                Dispatcher.UIThread.Post(() => { ResetTaskCompleteStatus(TaskCompleteStatusList); });
            });
        });

        endpoint = Proxy<AccountTransferProxyCallbacks>.Start();

        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        mainWindowViewModel!.Status = MainWindowViewModel.ServerStatus.Transfer;

        Utils.LogTrace(Localization.Resources.LOG_AT_START);
    }

    public void StopTransfer()
    {
        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        
        if (mainWindowViewModel!.Status != MainWindowViewModel.ServerStatus.Transfer) return;

        Proxy<AccountTransferProxyCallbacks>.Stop();

        mainWindowViewModel!.Status = MainWindowViewModel.ServerStatus.Stopped;

        Utils.LogTrace(Localization.Resources.LOG_AT_STOP);
    }

    public string AT_NOTE => Localization.Resources.AT_NOTE;
    public string AT_NOTE_ONE_ACCOUNT => Localization.Resources.AT_NOTE_ONE_ACCOUNT;
    public string AT_NOTE_RESTART => Localization.Resources.AT_NOTE_RESTART;
    public string AT_NOTE_MORE_INFO => Localization.Resources.AT_NOTE_MORE_INFO;
}