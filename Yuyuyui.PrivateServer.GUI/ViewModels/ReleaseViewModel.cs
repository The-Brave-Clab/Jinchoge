using System;
using System.IO;
using System.Linq;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class ReleaseViewModel : ViewModelBase
{
    public string ReleaseNotesMd => ServerResources.ReadAllTextFromAssemblyResources(
            $"documents.{Resources.LAN_CODE}.{ServerResources.RELEASE_NOTES_PATH}.md");
}