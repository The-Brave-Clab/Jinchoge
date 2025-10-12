using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace Yuyuyui.PrivateServer.Desktop;

public class FilesystemPlayerDataProvider<TPlayerData, TIdentifier> : IPlayerDataProvider<TPlayerData, TIdentifier>
    where TPlayerData : BasePlayerData<TPlayerData, TIdentifier>
    where TIdentifier : notnull
{
    public async Task<TPlayerData> Load(TIdentifier id)
    {
        await cacheSemaphore.WaitAsync();
        try
        {
            // Get directly from cache if possible
            if (cache.TryGetValue(id, out var load))
            {
                return load;
            }

            var sem = GetLockObj(id);
            await sem.WaitAsync();
            try
            {
                string file = GetFileName(id);
                string content = await File.ReadAllTextAsync(file, Encoding.UTF8);
                //return JsonConvert.DeserializeObject<T>(content)!;
                var deserializer = new DeserializerBuilder()
                    .IgnoreUnmatchedProperties()
                    .Build();
                TPlayerData result = deserializer.Deserialize<TPlayerData>(content);

                // Add to cache
                cache.Add(id, result);

                return result;
            }
            finally
            {
                sem.Release();
            }
        }
        finally
        {
            cacheSemaphore.Release();
        }
    }

    public async Task<IEnumerable<TPlayerData>> LoadMany(IEnumerable<TIdentifier> ids)
    {
        return await ids.Select(Load).WhenAll();
    }

    public async Task Save(TPlayerData entity, TIdentifier id)
    {
        var sem = GetLockObj(id);
        await sem.WaitAsync();

        try
        {
            string file = GetFileName(id);
            //File.WriteAllText(file, JsonConvert.SerializeObject(entity, Formatting.Indented));
            var serializer = new Serializer();
            await File.WriteAllTextAsync(file, serializer.Serialize(entity));

            // Update cache
            cache.TryAdd(id, entity);
        }
        finally
        {
            sem.Release();
        }
    }

    public async Task<bool> Exists(TIdentifier id)
    {
        var sem = GetLockObj(id);
        await sem.WaitAsync();
        try
        {
            return File.Exists(GetFileName(id));
        }
        finally
        {
            sem.Release();
        }
    }

    public async Task Delete(TIdentifier id)
    {
        await cacheSemaphore.WaitAsync();
        try
        {
            cache.Remove(id);
        }
        finally
        {
            cacheSemaphore.Release();
        }

        var sem = GetLockObj(id);
        await sem.WaitAsync();
        try
        {
            File.Delete(GetFileName(id));
        }
        finally
        {
            sem.Release();
        }
    }

    
    private static readonly Dictionary<TIdentifier, TPlayerData> cache = new();
    // ReSharper disable once StaticMemberInGenericType
    private static readonly SemaphoreSlim cacheSemaphore = new(1, 1);
    private static readonly Dictionary<TIdentifier, SemaphoreSlim> semaphores = new();

    private static SemaphoreSlim GetLockObj(TIdentifier identifier)
    {
        lock (semaphores)
        {
            if (semaphores.TryGetValue(identifier, out var obj))
                return obj;

            var lockObj = new SemaphoreSlim(1, 1);
            semaphores.Add(identifier, lockObj);
            return lockObj;
        }
    }

    private static string GetFileName(TIdentifier identifier)
    {
        return Path.Combine(EnsurePlayerDataFolder(typeof(TPlayerData).Name), $"{identifier}.yaml");
    }

    private static string EnsurePlayerDataFolder(string subFolder)
    {
        string dir = Path.Combine(FileSystemData.BASE_DIR, FileSystemData.PLAYER_DATA_FOLDER, subFolder);
        return Utils.EnsureDirectory(dir);
    }
}