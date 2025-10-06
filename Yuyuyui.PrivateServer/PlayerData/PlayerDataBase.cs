using System;
using System.Threading;

namespace Yuyuyui.PrivateServer;

public abstract class PlayerDataBase
{
    protected abstract string DataType { get; }

    private readonly ReaderWriterLockSlim rwLock = new();

    public ReadLock Read() => new ReadLock(rwLock);
    public WriteLock Write() => new WriteLock(rwLock);
    public UpgradeableReadLock UpgradeableRead() => new UpgradeableReadLock(rwLock);

    public class ReadLock : IDisposable
    {
        public ReadLock(ReaderWriterLockSlim rwLock)
        {
            rw = rwLock;
            rw.EnterReadLock();
        }

        public void Dispose()
        {
            rw.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim rw;
    }

    public class WriteLock : IDisposable
    {
        public WriteLock(ReaderWriterLockSlim rwLock)
        {
            rw = rwLock;
            rw.EnterWriteLock();
        }

        public void Dispose()
        {
            rw.ExitWriteLock();
        }

        private readonly ReaderWriterLockSlim rw;
    }

    public class UpgradeableReadLock : IDisposable
    {
        public UpgradeableReadLock(ReaderWriterLockSlim rwLock)
        {
            rw = rwLock;
            rw.EnterUpgradeableReadLock();
        }

        public void Dispose()
        {
            rw.ExitUpgradeableReadLock();
        }

        private readonly ReaderWriterLockSlim rw;
    }
}