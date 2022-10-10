using System;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public Uri AboutMd =>
        new ($"avares://YuyuyuiPrivateServerGUI/Assets/Texts/{Localization.Resources.LAN_CODE}/about.md");
}