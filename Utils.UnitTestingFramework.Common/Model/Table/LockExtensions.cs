namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System.Threading;

    internal static class LockExtensions
    {
        public static ReadLockScope Read(this ReaderWriterLockSlim @lock)
            => new ReadLockScope(@lock);

        public static WriteLockScope Write(this ReaderWriterLockSlim @lock)
            => new WriteLockScope(@lock);
    }
}
