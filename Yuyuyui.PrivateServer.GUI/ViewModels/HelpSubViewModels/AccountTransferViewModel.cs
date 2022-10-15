using System;
using System.Collections.ObjectModel;
using ReactiveUI;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class AccountTransferViewModel : HelpSubViewModelBase
{
    public AccountTransferViewModel()
    {
        StartButtonContent = TransferViewModel.GetButtonContent(MainWindowViewModel.ServerStatus.Stopped);
        StopButtonContent = TransferViewModel.GetButtonContent(MainWindowViewModel.ServerStatus.Transfer);
        
        TransferViewModel.ResetTaskCompleteStatus(TaskList);
        TransferViewModel.ResetTaskCompleteStatus(TaskListSomeCompleted);
        TaskListSomeCompleted[0].IsCompleted = true;
        TaskListSomeCompleted[1].IsCompleted = true;
        TaskListSomeCompleted[3].IsCompleted = true;
        TaskListSomeCompleted[4].IsCompleted = true;
        TaskListSomeCompleted[5].IsCompleted = true;
        TaskListSomeCompleted[10].IsCompleted = true;
    }

    private string startButtonContent = "";

    public string StartButtonContent
    {
        get => startButtonContent;
        set => this.RaiseAndSetIfChanged(ref startButtonContent, value);
    }

    private string stopButtonContent = "";

    public string StopButtonContent
    {
        get => stopButtonContent;
        set => this.RaiseAndSetIfChanged(ref stopButtonContent, value);
    }

    public ObservableCollection<TransferViewModel.TaskCompleteStatus> TaskList { get; } = new();
    public ObservableCollection<TransferViewModel.TaskCompleteStatus> TaskListSomeCompleted { get; } = new();
    
    public Uri MdP1 => MarkdownDocuments("account-transfer", 1);
    public Uri MdP2 => MarkdownDocuments("account-transfer", 2);
    public Uri MdP3 => MarkdownDocuments("account-transfer", 3);
    public Uri MdP4 => MarkdownDocuments("account-transfer", 4);
}