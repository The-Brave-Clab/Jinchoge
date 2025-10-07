using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Yuyuyui.PrivateServer;

public static class ServerResources
{
    public const string RELEASE_NOTES_PATH = "release-notes";

    private static readonly Assembly assembly = typeof(ServerResources).Assembly;

    public static readonly string[] EmbeddedResources = assembly.GetManifestResourceNames();
    public static string ReadAllTextFromAssemblyResources(string resourceName)
    {
        var embeddedResource = EmbeddedResources.First(r =>
            r.Contains(resourceName, StringComparison.InvariantCultureIgnoreCase));
        using Stream stream = assembly.GetManifestResourceStream(embeddedResource)!;
        using StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}