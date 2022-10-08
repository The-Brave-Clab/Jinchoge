using ReactiveUI;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class PrivateServerViewModel : HelpSubViewModelBase
{
    public PrivateServerViewModel()
    {
        ButtonStartContent = MainWindowViewModel.GetButtonContent(MainWindowViewModel.ServerStatus.Stopped);
        ButtonStartDescription = MainWindowViewModel.GetButtonDescription(MainWindowViewModel.ServerStatus.Stopped);
        ButtonUpdateContent = MainWindowViewModel.GetButtonContent(MainWindowViewModel.ServerStatus.Updating);
        ButtonUpdateDescription = MainWindowViewModel.GetButtonDescription(MainWindowViewModel.ServerStatus.Updating);
        ButtonTransferContent = MainWindowViewModel.GetButtonContent(MainWindowViewModel.ServerStatus.Transfer);
        ButtonTransferDescription = MainWindowViewModel.GetButtonDescription(MainWindowViewModel.ServerStatus.Transfer);
    }
    
    private string buttonStartContent = "";

    public string ButtonStartContent
    {
        get => buttonStartContent;
        set => this.RaiseAndSetIfChanged(ref buttonStartContent, value);
    }

    private string buttonStartDescription = "";

    public string ButtonStartDescription
    {
        get => buttonStartDescription;
        set => this.RaiseAndSetIfChanged(ref buttonStartDescription, value);
    }
    
    private string buttonUpdateContent = "";

    public string ButtonUpdateContent
    {
        get => buttonUpdateContent;
        set => this.RaiseAndSetIfChanged(ref buttonUpdateContent, value);
    }

    private string buttonUpdateDescription = "";

    public string ButtonUpdateDescription
    {
        get => buttonUpdateDescription;
        set => this.RaiseAndSetIfChanged(ref buttonUpdateDescription, value);
    }
    
    private string buttonTransferContent = "";

    public string ButtonTransferContent
    {
        get => buttonTransferContent;
        set => this.RaiseAndSetIfChanged(ref buttonTransferContent, value);
    }

    private string buttonTransferDescription = "";

    public string ButtonTransferDescription
    {
        get => buttonTransferDescription;
        set => this.RaiseAndSetIfChanged(ref buttonTransferDescription, value);
    }
}