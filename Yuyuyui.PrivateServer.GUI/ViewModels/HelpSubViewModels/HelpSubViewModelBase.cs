using System;
using System.Reflection;
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
        ConnectProxyButtonText = HelpViewModel.ButtonInfo[typeof(ConnectToProxyView)];
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
    
    private string connectProxyButtonText = "";

    public string ConnectProxyButtonText
    {
        get => connectProxyButtonText;
        set => this.RaiseAndSetIfChanged(ref connectProxyButtonText, value);
    }

    public void ConnectProxyButtonCommand()
    {
        HelpViewModel!.PushPage(typeof(ConnectToProxyView));
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

    protected static Uri MarkdownDocuments(string subFolder, int part)
    {
        return new Uri($"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Assets/Texts/{Localization.Resources.LAN_CODE}/Helps/{subFolder}/p{part}.md");
    }
}