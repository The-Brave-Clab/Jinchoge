using System;
using System.IO;
using System.Linq;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer.GUI.ViewModels;

public class ReleaseViewModel : ViewModelBase
{
    public string ReleaseNotesMd
    {
        get
        {
            var expectedFile = $"documents.{Resources.LAN_CODE}.release-notes.md";
            var embeddedResource = ProxyUtils.AssemblyResources.First(r =>
                r.Contains(expectedFile, StringComparison.InvariantCultureIgnoreCase));
            using Stream stream = typeof(ProxyUtils).Assembly.GetManifestResourceStream(embeddedResource)!;
            using StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }   
    }
}