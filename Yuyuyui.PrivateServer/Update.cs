using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer;

public static class Update
{

    public const string BASE_URL = "https://dblk8ymnxb1f2.cloudfront.net";

    private static VersionInfo latestVersionInfo;

    private static LocalVersionInfo localVersionInfo;

    public static LocalVersionInfo LocalVersion => localVersionInfo;

    static Update()
    {
        latestVersionInfo = new();

        var assembly = typeof(Update).Assembly;
        var jsonName = "Yuyuyui.PrivateServer.Resources.version.json";
        using Stream stream = assembly.GetManifestResourceStream(jsonName)!;
        using StreamReader reader = new StreamReader(stream);
        localVersionInfo = (JsonSerializer.Create().Deserialize(reader, typeof(LocalVersionInfo)) as LocalVersionInfo)!;
    }

    public static async Task Check()
    {
        if (localVersionInfo.is_local_build) return;

        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri($"{BASE_URL}/version.json"));
        requestMessage.Headers.Connection.Add("Keep-Alive");
        requestMessage.Headers.AcceptEncoding.TryParseAdd("gzip");
        using var httpClient = new HttpClient();
        
        HttpResponseMessage response = await httpClient.SendAsync(requestMessage);
        using var stream = await response.Content.ReadAsStreamAsync();
        using StreamReader reader = new StreamReader(stream);
        latestVersionInfo = (JsonSerializer.Create().Deserialize(reader, typeof(VersionInfo)) as VersionInfo)!;
    }

    public static bool TryGetNewerVersion(out BuildInfo newVersionInfo)
    {
        newVersionInfo = new();

        if (localVersionInfo.is_local_build) return false;
        if (latestVersionInfo.version_info.Count == 0) return false;
        if (!latestVersionInfo.version_info.ContainsKey(Config.Get().General.UpdateBranch)) return false;

        newVersionInfo = latestVersionInfo.version_info[Config.Get().General.UpdateBranch];

        return newVersionInfo.IsNewerThan(localVersionInfo.version_info);
    }

    public class LocalVersionInfo
    {
        public bool is_local_build { get; set; } = true;
        public string framework { get; set; } = "";
        public string runtime_id { get; set; } = "";
        public BuildInfo version_info { get; set; } = new();
    }

    public class VersionInfo
    {
        public IDictionary<string, BuildInfo> version_info = new Dictionary<string, BuildInfo>();
    }

    public class BuildInfo
    {
        public string created_at { get; set; } = "";
        public string commit_sha { get; set; } = "";
        public int ci_run { get; set; } = 0;
        public string branch { get; set; } = "";
        public IDictionary<string, string>? artifacts { get; set; } = null;

        private DateTime CreatedAt => DateTime.ParseExact(created_at, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);

        public bool IsNewerThan(BuildInfo other)
        {
            return CreatedAt > other.CreatedAt &&
                   commit_sha != other.commit_sha &&
                   ci_run > other.ci_run;
        }
    }

}