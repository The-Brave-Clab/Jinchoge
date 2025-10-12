using System;
using System.Threading.Tasks;

namespace Yuyuyui.PrivateServer;

/// <summary>
/// Provides distributed locking mechanism for PlayerProfile operations to prevent concurrent modification.
/// </summary>
public interface IDistributedLockProvider
{
    /// <summary>
    /// Acquires a lock for a specific PlayerProfile. The lock is released when the returned IDisposable is disposed.
    /// </summary>
    /// <param name="playerCode">The player code to lock (e.g., "0000000001")</param>
    /// <returns>A disposable handle that releases the lock when disposed</returns>
    /// <exception cref="Exception">Thrown if lock cannot be acquired within timeout period</exception>
    Task<IAsyncDisposable> AcquirePlayerProfileLock(string playerCode);
}
