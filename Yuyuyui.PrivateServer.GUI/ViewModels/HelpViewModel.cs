using System;
using System.Collections.ObjectModel;
using Avalonia.Collections;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class HelpViewModel : ViewModelBase
{
    public ObservableCollection<HelpButtonDataTemplate> Buttons { get; }

    public HelpViewModel()
    {
        Buttons = new ObservableCollection<HelpButtonDataTemplate>
        {
            new() { ButtonName = "GeneralInfo",     ButtonText = "What is this project?" },
            new() { ButtonName = "PrivateServer",   ButtonText = "How to Use the Private Server?" },
            new() { ButtonName = "AccountTransfer", ButtonText = "How to Use the Account Transfer Tool?" },
        };
    }

    public class HelpButtonDataTemplate
    {
        public string ButtonName { get; set; } = "";
        public string ButtonText { get; set; } = "";

        public void HelpNavigationButtonClick()
        {
            Utils.LogWarning(ButtonName);
        }
    }
}