using ReactiveUI;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class HelpSubViewModelBase : ViewModelBase
{
    public HelpViewModel? HelpViewModel = null;

    public HelpSubViewModelBase()
    {
        IntroductionButtonText = HelpViewModel.ButtonInfo[typeof(IntroductionView)];
        PrivateServerButtonText = HelpViewModel.ButtonInfo[typeof(PrivateServerView)];
        AccountTransferButtonText = HelpViewModel.ButtonInfo[typeof(AccountTransferView)];
        ChooseIpButtonText = HelpViewModel.ButtonInfo[typeof(ChooseProperProxyIpView)];
        InstallCertIosButtonText = HelpViewModel.ButtonInfo[typeof(InstallCertificateIosView)];
        InstallCertAndroidButtonText = HelpViewModel.ButtonInfo[typeof(InstallCertificateAndroidView)];
    }
    
    private string introductionButtonText = "";

    public string IntroductionButtonText
    {
        get => introductionButtonText;
        set => this.RaiseAndSetIfChanged(ref introductionButtonText, value);
    }

    public void IntroductionButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(IntroductionView));
    }
    
    private string privateServerButtonText = "";

    public string PrivateServerButtonText
    {
        get => privateServerButtonText;
        set => this.RaiseAndSetIfChanged(ref privateServerButtonText, value);
    }

    public void PrivateServerButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(PrivateServerView));
    }
    
    private string accountTransferButtonText = "";

    public string AccountTransferButtonText
    {
        get => accountTransferButtonText;
        set => this.RaiseAndSetIfChanged(ref accountTransferButtonText, value);
    }

    public void AccountTransferButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(AccountTransferView));
    }
    
    private string chooseIpButtonText = "";

    public string ChooseIpButtonText
    {
        get => chooseIpButtonText;
        set => this.RaiseAndSetIfChanged(ref chooseIpButtonText, value);
    }

    public void ChooseIpButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(ChooseProperProxyIpView));
    }
    
    private string installCertIosButtonText = "";

    public string InstallCertIosButtonText
    {
        get => installCertIosButtonText;
        set => this.RaiseAndSetIfChanged(ref installCertIosButtonText, value);
    }

    public void InstallCertIosButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(InstallCertificateIosView));
    }
    
    private string installCertAndroidButtonText = "";

    public string InstallCertAndroidButtonText
    {
        get => installCertAndroidButtonText;
        set => this.RaiseAndSetIfChanged(ref installCertAndroidButtonText, value);
    }

    public void InstallCertAndroidButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(InstallCertificateAndroidView));
    }
}