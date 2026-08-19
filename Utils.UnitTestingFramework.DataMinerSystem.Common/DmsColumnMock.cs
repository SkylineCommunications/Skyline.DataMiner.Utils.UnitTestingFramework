namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
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
        private readonly IDmsTable table;
        private readonly Dictionary<string, EventHandler<CellChangedEventArgs>> columnMonitors =
            new Dictionary<string, EventHandler<CellChangedEventArgs>>();
        private readonly Dictionary<string, EventHandler<CellChangedEventArgs>> cellMonitors =
            new Dictionary<string, EventHandler<CellChangedEventArgs>>();

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
            this.table = table;

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

            SetupValueMonitors();
        }

        private void SetupValueMonitors()
        {
            Setup(c => c.StartValueMonitor(It.IsAny<string>(), It.IsAny<Action<ColumnValueChange<T>>>(), It.IsAny<bool>()))
                .Callback((string sourceId, Action<ColumnValueChange<T>> action, bool _) => StartColumnMonitor(sourceId, action));

            Setup(c => c.StartValueMonitor(It.IsAny<string>(), It.IsAny<Action<ColumnValueChange<T>>>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, Action<ColumnValueChange<T>> action, TimeSpan _, bool __) => StartColumnMonitor(sourceId, action));

            Setup(c => c.StartValueMonitor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<CellValueChange<T>>>(), It.IsAny<bool>()))
                .Callback((string sourceId, string primaryKey, Action<CellValueChange<T>> action, bool _) => StartCellMonitor(sourceId, primaryKey, action));

            Setup(c => c.StartValueMonitor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Action<CellValueChange<T>>>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, string primaryKey, Action<CellValueChange<T>> action, TimeSpan _, bool __) => StartCellMonitor(sourceId, primaryKey, action));

            Setup(c => c.StopValueMonitor(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string sourceId, bool _) => StopColumnMonitor(sourceId));

            Setup(c => c.StopValueMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, TimeSpan _, bool __) => StopColumnMonitor(sourceId));

            Setup(c => c.StopValueMonitor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string sourceId, string primaryKey, bool _) => StopCellMonitor(sourceId, primaryKey));

            Setup(c => c.StopValueMonitor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, string primaryKey, TimeSpan _, bool __) => StopCellMonitor(sourceId, primaryKey));
        }

        private void StartColumnMonitor(string sourceId, Action<ColumnValueChange<T>> action)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            StopColumnMonitor(sourceId);

            void Handler(object sender, CellChangedEventArgs e)
            {
                if (e.ParameterDefinition.Pid != columnPid)
                {
                    return;
                }

                var updates = new Dictionary<string, T>
                {
                    [e.PrimaryKey] = ValueConverter.Convert<T>(e.NewValue),
                };

                action(new ColumnValueChange<T>(CreateColumnSelector(), sourceId, null, updates));
            }

            columnMonitors[sourceId] = Handler;
            tableModel.CellChanged += Handler;
        }

        private void StopColumnMonitor(string sourceId)
        {
            if (sourceId != null && columnMonitors.TryGetValue(sourceId, out var handler))
            {
                tableModel.CellChanged -= handler;
                columnMonitors.Remove(sourceId);
            }
        }

        private void StartCellMonitor(string sourceId, string primaryKey, Action<CellValueChange<T>> action)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var monitorKey = CreateCellMonitorKey(sourceId, primaryKey);
            StopCellMonitor(sourceId, primaryKey);

            void Handler(object sender, CellChangedEventArgs e)
            {
                if (e.ParameterDefinition.Pid != columnPid || e.PrimaryKey != primaryKey)
                {
                    return;
                }

                var value = ValueConverter.Convert<T>(e.NewValue);

                action(new CellValueChange<T>(CreateCellSelector(primaryKey), value, sourceId, null));
            }

            cellMonitors[monitorKey] = Handler;
            tableModel.CellChanged += Handler;
        }

        private void StopCellMonitor(string sourceId, string primaryKey)
        {
            if (sourceId == null || primaryKey == null)
            {
                return;
            }

            var monitorKey = CreateCellMonitorKey(sourceId, primaryKey);
            if (cellMonitors.TryGetValue(monitorKey, out var handler))
            {
                tableModel.CellChanged -= handler;
                cellMonitors.Remove(monitorKey);
            }
        }

        private static string CreateCellMonitorKey(string sourceId, string primaryKey)
        {
            return $"{sourceId}|{primaryKey}";
        }

        private Column CreateColumnSelector()
        {
            var element = table?.Element;
            return new Column(element?.AgentId ?? 0, element?.Id ?? 0, tableModel.TableId, columnPid);
        }

        private Cell CreateCellSelector(string primaryKey)
        {
            var element = table?.Element;
            return new Cell(element?.AgentId ?? 0, element?.Id ?? 0, tableModel.TableId, columnPid, primaryKey);
        }
    }
}
