namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Standalone;

    /// <summary>
    /// Table model.
    /// </summary>
    /// <seealso cref="ITableModel" />
    public class TableModel : ITableModel
    {
        private readonly Dictionary<string, int> keyToRowIndex = new Dictionary<string, int>();
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        // List type to allow keep track of row indexes.
        private readonly List<IParameterModel[]> rows = new List<IParameterModel[]>();

        private int suspendNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModel"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="tableSchema"></param>
        internal TableModel(int tableId, TableSchema tableSchema)
        {
            TableId = tableId;
            Schema = tableSchema ?? throw new ArgumentNullException(nameof(tableSchema));
        }

        /// <inheritdoc/>
        public event EventHandler<CellChangedEventArgs> CellChanged;

        /// <inheritdoc/>
        public event EventHandler<RowChangedEventArgs> RowChanged;

        /// <inheritdoc/>
        public event EventHandler<TableChangedEventArgs> TableChanged;

        /// <inheritdoc/>
        public int TableId { get; }

        /// <inheritdoc/>
        public TableSchema Schema { get; }

        /// <inheritdoc/>
        public int RowCount
        {
            get
            {
                using (@lock.Read())
                {
                    return rows.Count;
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/>.</exception>
        public bool RowExists(string primaryKey)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            using (@lock.Read())
            {
                return keyToRowIndex.ContainsKey(primaryKey);
            }
        }

        /// <inheritdoc/>
        public int GetRowIndex(string primaryKey)
        {
            using (@lock.Read())
            {
                if (!keyToRowIndex.TryGetValue(primaryKey, out int rowIndex))
                {
                    // Return -1 if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return -1;
                }

                return rowIndex;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="rowIndex"/> is negative.</exception>
        public string GetRowPrimaryKey(int rowIndex)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, $"'{nameof(rowIndex)}' cannot be negative");
            }

            using (@lock.Read())
            {
                if (rowIndex >= rows.Count)
                {
                    // Return empty string if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return string.Empty;
                }

                var primaryKey = Convert.ToString(rows[rowIndex][Schema.PrimaryKeyColumn.Idx].Value);

                return primaryKey;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/> or whitespace.</exception>
        public void SetCell(string primaryKey, int columnPid, object value, DateTime? timestamp = null)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            var columnDefinition = Schema.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException(nameof(columnPid), $"A column with PID '{columnPid}' does not exist.");

            using (var eventDispatcher = GetEventDispatcher()) // Event Dispatcher needs to be disposed after the lock is released
            using (@lock.Write())
            {
                columnDefinition.Validate(value);

                var row = GetRow(primaryKey) ?? throw new ArgumentException($"No row with primary key '{primaryKey}' exists.", nameof(primaryKey));

                var cell = row[columnDefinition.Idx];

                var oldCellValue = cell.Value;
                var oldCellTimestamp = cell.Timestamp;

                bool cellChanged = cell.Update(value, timestamp);
                if (cellChanged)
                {
                    eventDispatcher.Enqueue(() => RaiseCellChanged(primaryKey, columnDefinition, oldCellValue, cell.Value, oldCellTimestamp, cell.Timestamp));
                    eventDispatcher.Enqueue(() => RaiseRowChanged(primaryKey, RowChangeType.Updated));
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/>.</exception>
        public IParameterValue[] GetRow(string primaryKey)
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            using(@lock.Read())
            {
                if (!keyToRowIndex.TryGetValue(primaryKey, out int rowIndex))
                {
                    // Return null if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return null;
                }

                return rows[rowIndex];
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">No column with the specified PID exists.</exception>"
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/> or whitespace.</exception>
        public IParameterValue GetCell(string primaryKey, int columnPid)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            var column = Schema.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException(nameof(columnPid), $"A column with PID '{columnPid}' does not exist.");
            
            using (@lock.Read())
            {
                if (!keyToRowIndex.TryGetValue(primaryKey, out int rowIndex))
                {
                    // Return null if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return null;
                }

                var cell = rows[rowIndex][column.Idx];

                return cell;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> is <see langword="null"/>.</exception>
        public void SetRow(object[] row, DateTime? timestamp = null)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            SetRows(new[] { row }, timestamp);
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="rows"/> is or contains <see langword="null"/>.</exception>
        public void SetRows(IEnumerable<object[]> rows, DateTime? timestamp = null)
        {
            if (rows is null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (rows.Any(row => row is null))
            {
                throw new ArgumentException("Row cannot be null.", nameof(rows));
            }

            using (var eventDispatcher = GetEventDispatcher()) // Event Dispatcher needs to be disposed after the lock is released
            using (@lock.Write())
            {
                foreach (var row in rows)
                {
                    if (row.Length != Schema.ColumnDefinitions.Count)
                    {
                        throw new ArgumentException($"Each row must contain exactly {Schema.ColumnDefinitions.Count} values, one for each column.", nameof(rows));
                    }

                    string primaryKey = Convert.ToString(row[Schema.PrimaryKeyColumn.Idx]);

                    var existingRow = GetRow(primaryKey);

                    if (existingRow == null)
                    {
                        AddNewRow(row, timestamp);
                        eventDispatcher.Enqueue(() => RaiseRowChanged(primaryKey, RowChangeType.Added));
                    }
                    else
                    {
                        UpdateExistingRow(row, timestamp, existingRow, eventDispatcher);
                    }
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="primaryKeys"/> is <see langword="null"/>.</exception>
        public void RemoveRows(params string[] primaryKeys)
        {
            if (primaryKeys == null)
            {
                throw new ArgumentNullException(nameof(primaryKeys));
            }

            using (var eventDispatcher = GetEventDispatcher()) // Event Dispatcher needs to be disposed after the lock is released
            using (@lock.Write())
            {
                foreach (var primaryKey in primaryKeys)
                {
                    RemoveRow(primaryKey, eventDispatcher);
                }
            }
        }

        /// <inheritdoc/>
        public void RemoveAllRows()
        {
            using (var eventDispatcher = GetEventDispatcher())
            using (@lock.Write())
            {
                foreach (var row in rows)
                {
                    string primaryKey = Convert.ToString(row[Schema.PrimaryKeyColumn.Idx].Value);

                    eventDispatcher.Enqueue(() => RaiseRowChanged(primaryKey, RowChangeType.Deleted));
                }

                rows.Clear();
                keyToRowIndex.Clear();
            }
        }

        /// <inheritdoc/>
        public ReadOnlyDictionary<string, IParameterValue[]> GetAllRows()
        {
            using (@lock.Read())
            {
                var dict = new Dictionary<string, IParameterValue[]>(rows.Count);

                foreach (var row in rows)
                {
                    dict[(string)row[Schema.PrimaryKeyColumn.Idx].Value] = row;
                }

                return new ReadOnlyDictionary<string, IParameterValue[]>(dict);
            }
        }

        /// <inheritdoc/>
        public IDisposable SuspendNotifications()
        {
            Interlocked.Increment(ref suspendNotifications);
            return new NotificationScope(this);
        }

        private void RemoveRow(string primaryKey, EventDispatchScope eventDispatchScope)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            using (@lock.Write())
            {
                if (!RowExists(primaryKey))
                {
                    return;
                }

                int rowIndex = GetRowIndex(primaryKey);

                rows.RemoveAt(rowIndex);
                keyToRowIndex.Remove(primaryKey);

                // Update indices for all rows after the removed row
                var keysToUpdate = keyToRowIndex.Where(kvp => kvp.Value > rowIndex).ToList();
                foreach (var kvp in keysToUpdate)
                {
                    keyToRowIndex[kvp.Key] = kvp.Value - 1;
                }

                eventDispatchScope.Enqueue(() => RaiseRowChanged(primaryKey, RowChangeType.Deleted));
            }
        }

        private void UpdateExistingRow(object[] rowData, DateTime? timestamp, IParameterValue[] existingRow, EventDispatchScope eventDispatchScope)
        {
            string primaryKey = Convert.ToString(existingRow[Schema.PrimaryKeyColumn.Idx].Value);

            bool oneOrMoreCellsChanged = false;
            foreach (var columnDefinition in Schema.ColumnDefinitions)
            {
                columnDefinition.Validate(rowData[columnDefinition.Idx]);

                var cell = existingRow[columnDefinition.Idx];

                var oldValue = cell.Value;
                var oldTimestamp = cell.Timestamp;
                var newValue = rowData[columnDefinition.Idx];

                bool changed = cell.Update(newValue, timestamp);

                if (changed)
                {
                    eventDispatchScope.Enqueue(() => RaiseCellChanged(primaryKey, columnDefinition, oldValue, newValue, oldTimestamp, cell.Timestamp));
                    oneOrMoreCellsChanged = true;
                }
            }

            if (oneOrMoreCellsChanged)
            {
                // If at least one cell has changed, we also need to raise a RowChanged event.
                eventDispatchScope.Enqueue(() => RaiseRowChanged(primaryKey, RowChangeType.Updated));
            }
        }

        private void AddNewRow(object[] rowData, DateTime? timestamp)
        {
            string primaryKey = Convert.ToString(rowData[Schema.PrimaryKeyColumn.Idx]);

            var rowToAdd = new IParameterModel[Schema.ColumnDefinitions.Count];

            foreach (var columnDefinition in Schema.ColumnDefinitions)
            {
                var valueToAdd = rowData[columnDefinition.Idx];

                columnDefinition.Validate(valueToAdd);       

                rowToAdd[columnDefinition.Idx] = new ParameterModel(columnDefinition, valueToAdd, timestamp);
            }

            rows.Add(rowToAdd);
            keyToRowIndex[primaryKey] = rows.Count - 1;
        }

        private EventDispatchScope GetEventDispatcher()
        {
            return new EventDispatchScope(this);
        }

        private void RaiseCellChanged(string primaryKey, ColumnDefinition column, object oldValue, object newValue, DateTime oldTimestamp, DateTime newTimestamp)
        {
            if (suspendNotifications > 0) return;
            CellChanged?.Invoke(this, new CellChangedEventArgs(primaryKey, column, oldValue, newValue, oldTimestamp, newTimestamp));
        }

        private void RaiseRowChanged(string primaryKey, RowChangeType changeType)
        {
            if (suspendNotifications > 0) return;
            RowChanged?.Invoke(this, new RowChangedEventArgs(primaryKey, changeType));
        }

        private void RaiseTableChanged()
        {
            if (suspendNotifications > 0) return;
            TableChanged?.Invoke(this, new TableChangedEventArgs());
        }

        private sealed class NotificationScope : IDisposable
        {
            private readonly TableModel _table;

            public NotificationScope(TableModel table)
            {
                _table = table;
            }

            public void Dispose()
            {
                Interlocked.Decrement(ref _table.suspendNotifications);
            }
        }

        private sealed class EventDispatchScope : IDisposable
        {
            private readonly List<Action> eventsToRaise = new List<Action>();
            private readonly TableModel tableModel;

            private bool tableChangedEventEnqueued;
            private bool _disposed;

            public EventDispatchScope(TableModel tableModel)
            {
                this.tableModel = tableModel ?? throw new ArgumentNullException(nameof(tableModel));
            }

            public void Enqueue(Action action)
            {
                if (_disposed)
                    throw new ObjectDisposedException(nameof(EventDispatchScope));

                if (action == null)
                {
                    return;
                }

                eventsToRaise.Add(action);

                if (!tableChangedEventEnqueued)
                {
                    eventsToRaise.Add(() => tableModel.RaiseTableChanged());
                    tableChangedEventEnqueued = true;
                }
            }

            public void Dispose()
            {
                if (_disposed)
                    return;

                _disposed = true;

                foreach (var eventToRaise in eventsToRaise)
                {
                    eventToRaise();
                }
            }
        }
    }
}