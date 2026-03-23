namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Threading;

    internal readonly struct ReadLockScope : IDisposable
    {
        private readonly ReaderWriterLockSlim _lock;

        public ReadLockScope(ReaderWriterLockSlim @lock)
        {
            _lock = @lock ?? throw new ArgumentNullException(nameof(@lock));
            _lock.EnterReadLock();
        }

        public void Dispose()
        {
            _lock.ExitReadLock();
        }
    }
}
