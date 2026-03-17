namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;

    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// Table model.
    /// </summary>
    /// <seealso cref="ITableModel" />
    public class TableModel : ITableModel
    {
        private readonly Dictionary<string, int> keyToRowIndex;
        private readonly Dictionary<int, int> columnIndexesToPids;

        private readonly ReaderWriterLockSlim _lock;
        private readonly Dictionary<string, Column> _columns;
        private readonly List<Dictionary<string, IParameterModel>> _rows;
        private int _suspendNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModel"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        internal TableModel(int tableId) : this(tableId, Enumerable.Empty<Column>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModel"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The table columns.</param>
        internal TableModel(int tableId, IEnumerable<Column> columns)
        {
            keyToRowIndex = new Dictionary<string, int>();
            columnIndexesToPids = new Dictionary<int, int>();

            TableId = tableId;
            _columns = columns.ToDictionary(c => c.Name);

            foreach (var column in _columns.Values)
            {
                columnIndexesToPids[column.Idx] = column.Pid;
            }

            _rows = new List<Dictionary<string, IParameterModel>>();
            _lock = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Gets the column index to pid mapping.
        /// </summary>
        /// <value>
        /// The column index to pid mapping.
        /// </value>
        public IReadOnlyDictionary<int, int> ColumnIndexesToPids
        {
            get { return new ReadOnlyDictionary<int, int>(columnIndexesToPids); }
        }

        /// <summary>
        /// Gets the primary key to row index dictionary.
        /// </summary>
        /// <value>
        /// The primary key to row index dictionary.
        /// </value>
        public IReadOnlyDictionary<string, int> KeyToRowIndex
        {
            get { return new ReadOnlyDictionary<string, int>(keyToRowIndex); }
        }

        /// <summary>
        /// Gets the table identifier.
        /// </summary>
        /// <value>
        /// The table identifier.
        /// </value>
        public int TableId { get; }

        /// <summary>
        /// Gets the index of the key column.
        /// </summary>
        /// <value>
        /// The index of the key column.
        /// </value>
        public int PrimaryKeyColumnIdx { get; private set; }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        public int ColumnCount => _columns.Count;

        /// <summary>
        /// Gets the row count.
        /// </summary>
        public int RowCount => _rows.Count;

        /// <summary>
        /// Gets a value indicating whether a key column exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a key column exists; otherwise, <c>false</c>.
        /// </value>
        private bool KeyColumnExists { get; set; }

        public void SetCell(int rowIndex, string columnName, object value, DateTime? timestamp = null)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, $"'{nameof(rowIndex)}' cannot be negative");
            }

            _lock.EnterWriteLock();
            try
            {
                var column = _columns[columnName];
                column.Validate(value);

                if (rowIndex >= _rows.Count)
                {
                    throw new InvalidOperationException($"No row with index {rowIndex} exists. Rows count: '{_rows.Count}'");
                }

                var row = _rows[rowIndex];
                if (!row.TryGetValue(columnName, out var oldValue))
                {
                    throw new InvalidOperationException($"No column with name '{columnName}' exists in row {rowIndex}.");
                }

                if (Equals(oldValue.Value, value) && Equals(oldValue.Timestamp, timestamp))
                {
                    return;
                }

                row[columnName] = new ParameterModel(value, timestamp);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Retrieves the column data of the column with the specified pid.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns>The column data or <see langword="null"/> if there is no column with the specified ID.</returns>
        public object[] GetColumn(int pid)
        {
            var column = FindColumnByPid(pid);

            _lock.EnterReadLock();
            try
            {
                return _rows.Select(row => row[column.Name].Value).ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Rows the row data of the row with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">No row with key the specified key exists.</exception>
        public object[] GetRow(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (!KeyToRowIndex.TryGetValue(key, out int rowIndex))
            {
                throw new ArgumentException($"No row with key '{key}' exists.");
            }

            return GetRow(rowIndex);
        }

        /// <summary>
        /// Retrieves the row data of the row with the specified row index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="ArgumentException">No row with the specified index exists.</exception>
        public object[] GetRow(int rowIndex)
        {
            _lock.EnterReadLock();
            try
            {
                if (rowIndex < 0 || rowIndex >= _rows.Count)
                {
                    throw new ArgumentException($"No row with index {rowIndex} exists.");
                }

                var row = _rows[rowIndex];
                return OrderedColumns().Select(c => row[c.Name].Value).ToArray();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Retrieves the row with the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The row.</returns>
        public TRow GetRow<TRow>(string key) where TRow : QActionTableRow
        {
            return GetRow<TRow>(KeyToRowIndex[key]);
        }

        /// <summary>
        /// Retrieves the row at the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>The row.</returns>
        public TRow GetRow<TRow>(int index) where TRow : QActionTableRow
        {
            var row = GetRow(index);

            var rowAsConstructorArgument = new object[] { row };

            return (TRow)Activator.CreateInstance(typeof(TRow), rowAsConstructorArgument) ?? throw new InvalidOperationException($"Unable to create a {typeof(TRow).Name} of row {index}");
        }

        /// <summary>
        /// Sets the column with the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="values">The values.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <exception cref="ArgumentException">Invalid pid.</exception>
        public void SetColumn(int idx, string[] keys, object[] values, DateTime? timeInfo = null)
        {
            var column = FindColumnByIdx(idx);

            _lock.EnterWriteLock();
            try
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    var key = keys[i];
                    var value = values[i];

                    if (keyToRowIndex.TryGetValue(key, out int rowIndex))
                    {
                        var parameterModel = _rows[rowIndex][column.Name];
                        parameterModel.Update(value, timeInfo);
                        continue;
                    }

                    var newRow = CreateEmptyRow(timeInfo);
                    var pkColumn = FindColumnByIdx(PrimaryKeyColumnIdx);

                    newRow[pkColumn.Name] = new ParameterModel(key, timeInfo);
                    newRow[column.Name] = new ParameterModel(value, timeInfo);

                    _rows.Add(newRow);
                    keyToRowIndex[key] = _rows.Count - 1;
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sets the specified row.
        /// </summary>
        /// <param name="rowData">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The changes.</returns>
        public object SetRow(object[] rowData, DateTime? timestamp = null)
        {
            _lock.EnterWriteLock();
            try
            {
                var columns = OrderedColumns();
                int columnsNumber = columns.Count;

                int[] changes = new int[columnsNumber];
                var row = new Dictionary<string, IParameterModel>(columnsNumber);

                for (int columnIdx = 0; columnIdx < columnsNumber; columnIdx++)
                {
                    object data = columnIdx < rowData.Length ? rowData[columnIdx] : null;
                    changes[columnIdx] = data is null ? 0 : 1;
                    row[columns[columnIdx].Name] = new ParameterModel(data, timestamp);
                }

                var key = Convert.ToString(rowData[PrimaryKeyColumnIdx]);
                _rows.Add(row);
                keyToRowIndex[key] = _rows.Count - 1;

                return changes;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Sets the existing row.
        /// </summary>
        /// <param name="rowData">The row data.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The changes.</returns>
        public object SetExistingRow(object[] rowData, int rowIndex, DateTime? timestamp = null)
        {
            _lock.EnterWriteLock();
            try
            {
                var columns = OrderedColumns();
                int[] changes = new int[rowData.Length];

                for (int columnIdx = 0; columnIdx < rowData.Length && columnIdx < columns.Count; columnIdx++)
                {
                    if (columnIdx == PrimaryKeyColumnIdx)
                    {
                        changes[columnIdx] = 0;
                        continue;
                    }

                    object data = rowData[columnIdx];
                    var column = columns[columnIdx];
                    var parameterModel = _rows[rowIndex][column.Name];

                    changes[columnIdx] = Equals(data, parameterModel.Value) ? 2 : 1;
                    parameterModel.Update(data, timestamp);
                }

                return changes;
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Removes the row with the specified index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        public void RemoveRow(int rowIndex)
        {
            _lock.EnterWriteLock();
            try
            {
                if (rowIndex < 0 || rowIndex >= _rows.Count)
                {
                    throw new ArgumentException($"No row with index {rowIndex} exists.");
                }

                int lastIndex = _rows.Count - 1;
                var pkColumn = FindColumnByIdx(PrimaryKeyColumnIdx);

                var primaryKey = (string)_rows[rowIndex][pkColumn.Name].Value;
                keyToRowIndex.Remove(primaryKey);

                if (rowIndex < lastIndex)
                {
                    var lastRow = _rows[lastIndex];
                    _rows[rowIndex] = lastRow;

                    var swappedPrimaryKey = (string)lastRow[pkColumn.Name].Value;
                    keyToRowIndex[swappedPrimaryKey] = rowIndex;
                }

                _rows.RemoveAt(lastIndex);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Gets all rows.
        /// </summary>
        public IDictionary<string, object[]> GetAllRows()
        {
            _lock.EnterReadLock();
            try
            {
                var pkColumn = FindColumnByIdx(PrimaryKeyColumnIdx);
                var result = new Dictionary<string, object[]>(_rows.Count);

                for (int i = 0; i < _rows.Count; i++)
                {
                    var row = _rows[i];
                    var rowValues = OrderedColumns().Select(c => row[c.Name].Value).ToArray();
                    result[Convert.ToString(row[pkColumn.Name].Value)] = rowValues;
                }

                return result;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Adds a column with the specified metadata.
        /// </summary>
        internal void AddColumn(Column column, bool isKey = false)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            _columns[column.Name] = column;
            columnIndexesToPids[column.Idx] = column.Pid;

            if (isKey)
            {
                if (KeyColumnExists)
                {
                    throw new InvalidOperationException($"Column with pid '{ColumnIndexesToPids[PrimaryKeyColumnIdx]}' is already the primary key column.");
                }

                KeyColumnExists = true;
                PrimaryKeyColumnIdx = column.Idx;
            }
        }

        /// <summary>
        /// Adds a column with the specified column ID and index.
        /// </summary>
        /// <param name="columnPid">The column pid.</param>
        /// <param name="idx">The index.</param>
        /// <param name="isKey">if set to <c>true</c> [is key].</param>
        /// <exception cref="InvalidOperationException">Another column has already been added as primary key column.</exception>
        internal void AddColumn(int columnPid, int idx, bool isKey = false)
        {
            throw new NotSupportedException("AddColumn(int, int, bool) is no longer supported. Provide full column metadata via AddColumn(Column, bool).");
        }

        private Column FindColumnByPid(int pid)
        {
            return _columns.Values.FirstOrDefault(c => c.Pid == pid) ?? throw new ArgumentException($"No column with ID {pid} exists.");
        }

        private Column FindColumnByIdx(int idx)
        {
            return _columns.Values.FirstOrDefault(c => c.Idx == idx) ?? throw new ArgumentException($"Invalid idx '{idx}'.");
        }

        private Dictionary<string, IParameterModel> CreateEmptyRow(DateTime? timestamp)
        {
            var row = new Dictionary<string, IParameterModel>(_columns.Count);
            foreach (var column in _columns.Values)
            {
                row[column.Name] = new ParameterModel(null, timestamp);
            }

            return row;
        }

        private List<Column> OrderedColumns()
        {
            return _columns.Values.OrderBy(c => c.Idx).ToList();
        }
    }
}