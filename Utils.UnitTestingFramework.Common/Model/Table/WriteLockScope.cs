namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Threading;

    internal readonly struct WriteLockScope : IDisposable
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
