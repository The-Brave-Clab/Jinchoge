using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Yuyuyui.PrivateServer.Localization;

namespace Yuyuyui.PrivateServer.Desktop;

public class AWSMasterDataProvider : IMasterDataProvider
{
    public AWSMasterDataProvider(Action<string, float>? singleFileProgress = null,
        Action<int, int>? totalProgress = null)
    {
        this.singleFileProgress = singleFileProgress;
        this.totalProgress = totalProgress;
    }

    private readonly Action<string, float>? singleFileProgress = null;
    private readonly Action<int, int>? totalProgress = null;
    private const string URL = "https://tqdc60uiqc.execute-api.ap-northeast-1.amazonaws.com/test/master_data";

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version!.ToString();
    }

    private string ResourceDir => Path.Combine(FileSystemData.BASE_DIR, FileSystemData.LOCAL_DATA_FOLDER);
    public string Directory => Path.Combine(ResourceDir, "master_data");

    public async Task Initialize()
    {
        string url = $"{URL}/jp"; // language option here

        Utils.Log(Resources.LOG_PS_LOCAL_DATA_UPDATING);

        // Get remote data
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

        HttpResponseMessage response = await PrivateServerDesktop.HttpClient.SendAsync(requestMessage);
        string responseStr = await response.Content.ReadAsStringAsync();

        LocalDataResult localDataResult = JsonConvert.DeserializeObject<LocalDataResult>(responseStr)!;

        // Get local data
        var dataFolder = Utils.EnsureDirectory(ResourceDir);
        var localDataVersionFile = Path.Combine(dataFolder, FileSystemData.LOCAL_DATA_VERSION_FILE);

        List<LocalDataDownload> needUpdate;

        bool versionMatch = false;
        bool localDataVersionFileExists = File.Exists(localDataVersionFile);
        LocalDataVersionList versionList = new();
        if (localDataVersionFileExists)
        {
            string versionContent = await File.ReadAllTextAsync(localDataVersionFile);
            versionList = JsonConvert.DeserializeObject<LocalDataVersionList>(versionContent)!;
            versionMatch = versionList.app_version == GetAssemblyVersion();
        }

        if (versionMatch && localDataVersionFileExists)
        {
            // If we already have a local version file,
            // compare it with the remote result and 
            // update the ones with different etag

            IEnumerable<LocalDataDownload> needUpdateEnumerable =
                from version in versionList.results
                from download in localDataResult.results
                where version.key == download.key && version.etag != download.etag
                select download;
            needUpdate = needUpdateEnumerable.ToList();
        }
        else
        {
            // If we don't have a local version file,
            // we need to download all
            needUpdate = localDataResult.results.ToList();
        }

        int count = 0;
        foreach (var data in needUpdate)
        {
            totalProgress?.Invoke(count, needUpdate.Count);
            // Download each file
            var filePath = Path.Combine(ResourceDir, data.key.Replace('/', Path.DirectorySeparatorChar));

            Utils.EnsureDirectory(Path.GetDirectoryName(filePath)!);

            var fileUrl = data.url;
            var fileName = Path.GetFileName(filePath);

            Utils.Log(string.Format(Resources.LOG_PS_LOCAL_DATA_DOWNLOADING, fileName));

            await using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await PrivateServerDesktop.HttpClient.DownloadAsync(fileUrl, fs,
                new Progress<float>(progress => { singleFileProgress?.Invoke(fileName, progress); }));
            ++count;
        }

        totalProgress?.Invoke(needUpdate.Count, needUpdate.Count);

        if (count == 0)
        {
            Utils.Log(Resources.LOG_PS_LOCAL_DATA_UP_TO_DATE);
        }
        else
        {
            Utils.Log(string.Format(Resources.LOG_PS_LOCAL_DATA_UPDATED, count));

            // Save the new version list to local storage
            LocalDataVersionList newVersionList = new LocalDataVersionList
            {
                app_version = GetAssemblyVersion(),
                results = localDataResult.results.Select(r => new LocalDataVersion
                {
                    etag = r.etag,
                    key = r.key
                }).ToList()
            };

            await File.WriteAllTextAsync(
                Path.Combine(ResourceDir, FileSystemData.LOCAL_DATA_VERSION_FILE),
                JsonConvert.SerializeObject(newVersionList));
        }
    }

    private class LocalDataResult
    {
        public IList<LocalDataDownload> results { get; set; } = new List<LocalDataDownload>();
    }

    private class LocalDataDownload
    {
        public string key { get; set; } = "";
        public string etag { get; set; } = "";
        public long size { get; set; }
        public string url { get; set; } = "";
    }

    private class LocalDataVersionList
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        public string app_version { get; set; } = "";

        public IList<LocalDataVersion> results { get; set; } = new List<LocalDataVersion>();
    }

    private class LocalDataVersion
    {
        public string key { get; set; } = "";
        public string etag { get; set; } = "";
    }
}