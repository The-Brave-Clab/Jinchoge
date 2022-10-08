using ReactiveUI;
using Yuyuyui.PrivateServer.GUI.Views;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class PrivateServerViewModel : HelpSubViewModelBase
{
    public PrivateServerViewModel()
    {
        IntroductionButtonText = HelpViewModel.ButtonInfo[typeof(IntroductionView)];
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
}