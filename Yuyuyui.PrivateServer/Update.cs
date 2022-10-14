using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer;

public static class Update
{

    private const string BASE_URL = "https://y3ps-publish.s3.ap-northeast-1.amazonaws.com";

    private static VersionInfo latestVersionInfo;

    private static LocalVersionInfo localVersionInfo;

    public static bool IsLocalBuild => localVersionInfo.is_local_build;
    public static BuildInfo LocalVersion => localVersionInfo.version_info;

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
        if (!latestVersionInfo.version_info.ContainsKey(localVersionInfo.version_info.branch)) return false;

        newVersionInfo = latestVersionInfo.version_info[localVersionInfo.version_info.branch];

        return localVersionInfo.version_info != newVersionInfo;
    }

    private class LocalVersionInfo
    {
        public bool is_local_build { get; set; } = true;
        public BuildInfo version_info { get; set; } = new();
    }

    public class VersionInfo
    {
        public IDictionary<string, BuildInfo> version_info = new Dictionary<string, BuildInfo>();
    }

    public class BuildInfo : IEquatable<BuildInfo>
    {
        public string created_at { get; set; } = "";
        public string commit_sha { get; set; } = "";
        public string ci_run { get; set; } = "";
        public string branch { get; set; } = "";

        public bool Equals(BuildInfo? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return created_at == other.created_at &&
                   commit_sha == other.commit_sha &&
                   ci_run == other.ci_run &&
                   branch == other.branch;
        }
    }

}