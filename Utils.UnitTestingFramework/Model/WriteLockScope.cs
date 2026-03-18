using System;
using System.Threading;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    public readonly struct WriteLockScope : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;

        public WriteLockScope(ReaderWriterLockSlim @lock)
        {
            _lock = @lock;
            _lock.EnterWriteLock();
        }

        public void Dispose()
        {
            _lock.ExitWriteLock();
        }
    }
}
