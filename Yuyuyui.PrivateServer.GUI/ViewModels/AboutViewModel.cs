using System;
using System.Reflection;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public Uri AboutMd =>
        new ($"avares://{Assembly.GetExecutingAssembly().GetName().Name}/Assets/Texts/{Localization.Resources.LAN_CODE}/about.md");
}