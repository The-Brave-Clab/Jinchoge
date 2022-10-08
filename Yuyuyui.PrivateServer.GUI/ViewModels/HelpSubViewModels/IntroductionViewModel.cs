using ReactiveUI;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class IntroductionViewModel : HelpSubViewModelBase
{
    public IntroductionViewModel()
    {
        PrivateServerButtonText = HelpViewModel.ButtonInfo[typeof(PrivateServerView)];
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
}