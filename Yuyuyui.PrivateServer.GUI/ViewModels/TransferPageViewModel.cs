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

public class TransferPageViewModel : ViewModelBase
{
    private ExplicitProxyEndPoint? endpoint;
    private WeakReference<MainWindowViewModel?> mainWindowVM = new(null);

    public TransferPageViewModel(MainWindowViewModel mainWindowViewModel)
    {
        mainWindowVM.SetTarget(mainWindowViewModel);

        buttonContent = "";
        TaskCompleteStatusList = new ObservableCollection<TaskCompleteStatus>();

        ResetTaskCompleteStatus();
    }

    public TransferPageViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        buttonContent = "";
        TaskCompleteStatusList = new ObservableCollection<TaskCompleteStatus>();

        ResetTaskCompleteStatus();
    }

    public void SetServerStatus(MainWindowViewModel.ServerStatus status)
    {
        ButtonContent = status switch
        {
            MainWindowViewModel.ServerStatus.Stopped => "Start\nAccount\nTransfer",
            MainWindowViewModel.ServerStatus.Transfer => "Stop\nAccount\nTransfer",
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

    private void ResetTaskCompleteStatus()
    {
        TaskCompleteStatusList.Clear();
        foreach (var task in TransferProgress.TaskName)
        {
            TaskCompleteStatusList.Add(new()
            {
                TaskType = task.Key,
                TaskName = task.Value,
                IsCompleted = false
            });
        }
    }

    private void StartTransfer()
    {
        ResetTaskCompleteStatus();
        
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
                Dispatcher.UIThread.Post(ResetTaskCompleteStatus);
            });
        });

        endpoint = Proxy<AccountTransferProxyCallbacks>.Start();

        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        mainWindowViewModel!.Status = MainWindowViewModel.ServerStatus.Transfer;

        Utils.LogTrace("Account Transfer Started!");
    }

    public void StopTransfer()
    {
        mainWindowVM.TryGetTarget(out var mainWindowViewModel);
        
        if (mainWindowViewModel!.Status != MainWindowViewModel.ServerStatus.Transfer) return;

        Proxy<AccountTransferProxyCallbacks>.Stop();

        mainWindowViewModel!.Status = MainWindowViewModel.ServerStatus.Stopped;

        Utils.LogTrace("Account Transfer Stopped!");
    }
}