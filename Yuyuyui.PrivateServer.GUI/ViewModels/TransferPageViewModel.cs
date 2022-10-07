using System;
using Avalonia.Controls;
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
    }

    public TransferPageViewModel()
    {
        if (!Design.IsDesignMode)
            throw new NotImplementedException();

        buttonContent = "";
    }

    public void SetServerStatus(MainWindowViewModel.ServerStatus status)
    {
        ButtonContent = status switch
        {
            MainWindowViewModel.ServerStatus.Stopped => "Start Account Transfer",
            MainWindowViewModel.ServerStatus.Transfer => "Stop Account Transfer",
            _ => ""
        };
        
        
    }

    private string buttonContent;

    public string ButtonContent
    {
        get => buttonContent;
        set => this.RaiseAndSetIfChanged(ref buttonContent, value);
    }

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

    private void StartTransfer()
    {
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