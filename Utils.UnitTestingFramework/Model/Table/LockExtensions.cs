namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Table
{
    using System.Threading;

    public static class LockExtensions
    {
        public static ReadLockScope Read(this ReaderWriterLockSlim @lock)
            => new ReadLockScope(@lock);

        public static WriteLockScope Write(this ReaderWriterLockSlim @lock)
            => new WriteLockScope(@lock);
    }
}
