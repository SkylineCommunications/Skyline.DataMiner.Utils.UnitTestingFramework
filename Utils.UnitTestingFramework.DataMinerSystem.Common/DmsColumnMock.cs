namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Mock of an <see cref="IDmsColumn{T}"/> that is backed by an <see cref="ITableModel"/>,
    /// so that cell values that are set are stored and can be retrieved again.
    /// </summary>
    /// <typeparam name="T">The type of the column value.</typeparam>
    internal class DmsColumnMock<T> : Mock<IDmsColumn<T>>
    {
        private readonly ITableModel tableModel;
        private readonly int columnPid;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsColumnMock{T}"/> class.
        /// </summary>
        /// <param name="tableModel">The table model that holds the cell values.</param>
        /// <param name="columnPid">The parameter ID of the column.</param>
        /// <param name="table">The table this column belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        public DmsColumnMock(ITableModel tableModel, int columnPid, IDmsTable table)
        {
            this.tableModel = tableModel ?? throw new ArgumentNullException(nameof(tableModel));
            this.columnPid = columnPid;

            Setup(c => c.Id).Returns(columnPid);
            Setup(c => c.Table).Returns(table);

#pragma warning disable CS0618 // Type or member is obsolete - the obsolete overload is set up to remain usable by callers.
            Setup(c => c.GetValue(It.IsAny<string>()))
                .Returns((string key) => ValueConverter.Convert<T>(this.tableModel.GetCell(key, this.columnPid)));
#pragma warning restore CS0618

            Setup(c => c.GetValue(It.IsAny<string>(), It.IsAny<KeyType>()))
                .Returns((string key, KeyType _) => ValueConverter.Convert<T>(this.tableModel.GetCell(key, this.columnPid)));

            Setup(c => c.SetValue(It.IsAny<string>(), It.IsAny<T>()))
                .Callback((string key, T value) => this.tableModel.SetCell(key, this.columnPid, value));

            Setup(c => c.SetValue(It.IsAny<string>(), It.IsAny<KeyType>(), It.IsAny<T>()))
                .Callback((string key, KeyType _, T value) => this.tableModel.SetCell(key, this.columnPid, value));

            Setup(c => c.SetValue(It.IsAny<string>(), It.IsAny<KeyType>(), It.IsAny<T>(), It.IsAny<TimeSpan>(), It.IsAny<Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges>()))
                .Callback((string key, KeyType _, T value, TimeSpan __, Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges ___) => this.tableModel.SetCell(key, this.columnPid, value));
        }
    }
}
