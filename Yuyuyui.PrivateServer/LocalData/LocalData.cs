﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yuyuyui.PrivateServer
{
    public static class LocalData
    {
        public const string URL = "https://tqdc60uiqc.execute-api.ap-northeast-1.amazonaws.com/test/master_data";

        public static async Task Update(Action<string, float>? singleFileProgress = null, Action<int, int>? totalProgress = null)
        {
            string url = $"{URL}/jp"; // language option here

            Utils.Log("Updating required local resources...");

            // Get remote data
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            HttpResponseMessage response = await PrivateServer.HttpClient.SendAsync(requestMessage);
            string responseStr = await response.Content.ReadAsStringAsync();

            LocalDataResult localDataResult = JsonConvert.DeserializeObject<LocalDataResult>(responseStr)!;

            // Get local data
            var dataFolder = Utils.EnsureDirectory(
                Path.Combine(PrivateServer.BASE_DIR, PrivateServer.LOCAL_DATA_FOLDER));
            var localDataVersionFile = Path.Combine(dataFolder, PrivateServer.LOCAL_DATA_VERSION_FILE);

            List<LocalDataDownload> needUpdate;

            if (File.Exists(localDataVersionFile))
            {
                // If we already have a local version file,
                // compare it with the remote result and 
                // update the ones with different etag

                string versionContent = File.ReadAllText(localDataVersionFile);
                LocalDataVersionList versionList = JsonConvert.DeserializeObject<LocalDataVersionList>(versionContent)!;

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
                var filePath = Path.Combine(
                    PrivateServer.BASE_DIR, 
                    PrivateServer.LOCAL_DATA_FOLDER,
                    data.key.Replace('/', Path.DirectorySeparatorChar));

                Utils.EnsureDirectory(Path.GetDirectoryName(filePath)!);

                var fileUrl = data.url;
                var fileName = Path.GetFileName(filePath);

                Utils.Log($"Downloading {fileName}...");

                using FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                await PrivateServer.HttpClient.DownloadAsync(fileUrl, fs, new Progress<float>(
                    progress =>
                    {
                        singleFileProgress?.Invoke(fileName, progress);
                    }));
                ++count;
            }

            totalProgress?.Invoke(needUpdate.Count, needUpdate.Count);

            if (count == 0)
            {
                Utils.Log("All data files are up to date.");
            }
            else
            {
                Utils.Log($"Updated {count} file(s).");

                // Save the new version list to local storage
                LocalDataVersionList newVersionList = new LocalDataVersionList
                {
                    results = localDataResult.results.Select(
                        r => new LocalDataVersion
                        {
                            etag = r.etag,
                            key = r.key
                        }).ToList()
                };

                File.WriteAllText(
                    Path.Combine(PrivateServer.BASE_DIR,
                        PrivateServer.LOCAL_DATA_FOLDER,
                        PrivateServer.LOCAL_DATA_VERSION_FILE),
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
            public IList<LocalDataVersion> results { get; set; } = new List<LocalDataVersion>();
        }

        private class LocalDataVersion
        {
            public string key { get; set; } = "";
            public string etag { get; set; } = "";
        }
    }
}