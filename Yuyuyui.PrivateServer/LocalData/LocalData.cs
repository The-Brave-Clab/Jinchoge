using System;
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

        public static async Task Update()
        {
            string url = $"{URL}/jp"; // language option here

            Utils.Log("Updating required local resources...");

            // Get remote data
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, new Uri(url));

            HttpResponseMessage response = await PrivateServer.HttpClient.SendAsync(requestMessage);
            string responseStr = await response.Content.ReadAsStringAsync();

            LocalDataResult localDataResult = JsonConvert.DeserializeObject<LocalDataResult>(responseStr)!;

            // Get local data
            var dataFolder = Utils.EnsureDirectory(PrivateServer.LOCAL_DATA_FOLDER);
            var localDataVersionFile = Path.Combine(dataFolder, PrivateServer.LOCAL_DATA_VERSION_FILE);

            IEnumerable<LocalDataDownload> needUpdate;

            if (File.Exists(localDataVersionFile))
            {
                // If we already have a local version file,
                // compare it with the remote result and 
                // update the ones with different etag

                string versionContent = File.ReadAllText(localDataVersionFile);
                LocalDataVersionList versionList = JsonConvert.DeserializeObject<LocalDataVersionList>(versionContent)!;

                needUpdate =
                    from version in versionList.results
                    from download in localDataResult.results
                    where version.key == download.key && version.etag != download.etag
                    select download;
            }
            else
            {
                // If we don't have a local version file,
                // we need to download all
                needUpdate = localDataResult.results;
            }

            int count = 0;
            foreach (var data in needUpdate)
            {
                // Download each file
                var filename = Path.Combine(PrivateServer.LOCAL_DATA_FOLDER,
                    data.key.Replace('/', Path.DirectorySeparatorChar));

                Utils.EnsureDirectory(Path.GetDirectoryName(filename)!);

                var fileUrl = data.url;

                Utils.Log($"Downloading {Path.GetFileName(filename)}...");

                using HttpResponseMessage downloadResponse = await PrivateServer.HttpClient.GetAsync(fileUrl);
                using Stream streamToReadFrom = await downloadResponse.Content.ReadAsStreamAsync();
                using FileStream fs = new FileStream(filename, FileMode.Create);
                await streamToReadFrom.CopyToAsync(fs);
                ++count;
            }

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
                    Path.Combine(PrivateServer.LOCAL_DATA_FOLDER, PrivateServer.LOCAL_DATA_VERSION_FILE),
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