namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// Table model.
    /// </summary>
    /// <seealso cref="ITableModel" />
    public class TableModel : ITableModel
    {
        private readonly Dictionary<string, int> keyToRowIndex;
        private readonly Dictionary<int, int> columnIndexesToPids;
        private readonly Dictionary<int, IList<IParameterModel>> columnPidToColumnData;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModel"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        internal TableModel(int tableId)
        {
            TableId = tableId;

            columnPidToColumnData = new Dictionary<int, IList<IParameterModel>>();
            columnIndexesToPids = new Dictionary<int, int>();
            keyToRowIndex = new Dictionary<string, int>();
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
        /// Gets or sets the index of the key column.
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
        public int ColumnCount => columnPidToColumnData.Count;

        /// <summary>
        /// Gets the row count.
        /// </summary>
        public int RowCount => KeyToRowIndex.Count;

        internal IDictionary<int, IList<IParameterModel>> ColumnsMapper
        {
            get { return columnPidToColumnData; }
        }

        /// <summary>
        /// Gets a value indicating whether a key column exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a key column exists; otherwise, <c>false</c>.
        /// </value>
        private bool KeyColumnExists { get; set; }

        /// <summary>
        /// Retrieves the column data of the column with the specified pid.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns>The column data or <see langword="null"/> if there is no column with the specified ID.</returns>
        public object[] GetColumn(int pid)
        {
            if (!ColumnsMapper.TryGetValue(pid, out IList<IParameterModel> columnEntries))
            {
                throw new ArgumentException($"No column with ID {pid} exists.");
            }

            var columnValues = new object[columnEntries.Count];

            for (int i = 0; i < columnEntries.Count; i++)
            {
                columnValues[i] = columnEntries[i].Value;
            }

            return columnValues;
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
            if (!keyToRowIndex.ContainsValue(rowIndex))
            {
                throw new ArgumentException($"No row with index {rowIndex} exists.");
            }

            return ColumnsMapper.Select(x => x.Value[rowIndex].Value).ToArray();
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

            // QActionTableRow constructor arguments: (int index, int columnCount, object[] oRow)
            return (TRow)Activator.CreateInstance(typeof(TRow), index, row.Length, row) ?? throw new InvalidOperationException($"Unable to create a {typeof(TRow).Name} of row {index}");
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
            int columnsNumber = ColumnsMapper.Count;

            ColumnIndexesToPids.TryGetValue(idx, out int columnPid);

            if (!ColumnsMapper.ContainsKey(columnPid))
            {
                throw new ArgumentException("Invalid pid.");
            }

            for (int i = 0; i < keys.Length; i++)
            {
                if (KeyToRowIndex.ContainsKey(keys[i]))
                {
                    int rowIndex = KeyToRowIndex[keys[i]];

                    var parameterModel = new ParameterModel(values[i], timeInfo);
                    
                    columnPidToColumnData[columnPid][rowIndex] = parameterModel;

                    continue;
                }

                var row = new object[columnsNumber];

                row[PrimaryKeyColumnIdx] = keys[i];

                row[idx] = values[i];

                SetRow(row);
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
            int columnsNumber = ColumnsMapper.Count;

            int[] changes = new int[columnsNumber];

            for (int columnIdx = 0; columnIdx < columnsNumber; columnIdx++)
            {
                object data = null;

                if (columnIdx < rowData.Length)
                {
                    data = rowData[columnIdx];
                }

                changes[columnIdx] = data is null ? 0 : 1;

                var parameterModel = new ParameterModel(data, timestamp);

                ColumnsMapper[ColumnIndexesToPids[columnIdx]].Add(parameterModel);
            }

            var key = Convert.ToString(rowData[PrimaryKeyColumnIdx]);

            keyToRowIndex[key] = ColumnsMapper[ColumnIndexesToPids[PrimaryKeyColumnIdx]].Count - 1;

            return changes;
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
            int columnsNumber = ColumnsMapper.Count;

            int[] changes = new int[rowData.Length];

            for (int columnIdx = 0; columnIdx < rowData.Length; columnIdx++)
            {
                if (columnIdx == columnsNumber)
                {
                    break;
                }

                if (columnIdx == PrimaryKeyColumnIdx)
                {
                    continue;
                }

                object data = rowData[columnIdx];

                changes[columnIdx] = data == ColumnsMapper[ColumnIndexesToPids[columnIdx]][rowIndex].Value ? 2 : 1;

                var parameterModel = new ParameterModel(data, timestamp);

                columnPidToColumnData[ColumnIndexesToPids[columnIdx]][rowIndex] = parameterModel;
            }

            changes[PrimaryKeyColumnIdx] = 0;

            return changes;
        }

        /// <summary>
        /// Removes the row with the specified index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        public void RemoveRow(int rowIndex)
        {
            int columnsNumber = ColumnsMapper.Count;

            int remainingRows = KeyToRowIndex.Select(x => x.Key != null).Count();

            int[] pids = ColumnsMapper.Keys.ToArray();

            int pid = ColumnIndexesToPids[PrimaryKeyColumnIdx];

            for (int index = 0; index < columnsNumber; index++)
            {
                columnPidToColumnData[pids[index]][rowIndex] = ColumnsMapper[pids[index]][remainingRows - 1];

                columnPidToColumnData[pids[index]].RemoveAt(remainingRows - 1);
            }

            var primaryKey = KeyToRowIndex.FirstOrDefault(x => x.Value == rowIndex).Key;

            keyToRowIndex.Remove(primaryKey);

            remainingRows = KeyToRowIndex.Select(x => x.Key != null).Count();

            if (rowIndex < remainingRows)
            {
                string newKey = (string)ColumnsMapper[pid][rowIndex].Value;

                keyToRowIndex[newKey] = rowIndex;
            }
        }

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns>a array of rows.</returns>
        public IDictionary<string, object[]> GetAllRows()
        {
            var allRows = new Dictionary<string, object[]>();

            for (int i = 0; i < RowCount; i++)
            {
                var row = GetRow(i);
                allRows[Convert.ToString(row[PrimaryKeyColumnIdx])] = row;
            }

            return allRows;
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
            columnIndexesToPids[idx] = columnPid;

            columnPidToColumnData[columnPid] = new List<IParameterModel>();

            if (!isKey)
            {
                return;
            }

            if (KeyColumnExists)
            {
                throw new InvalidOperationException($"Column with pid '{ColumnIndexesToPids[PrimaryKeyColumnIdx]}' is already the primary key column.");
            }

            KeyColumnExists = true;
            PrimaryKeyColumnIdx = idx;
        }
    }
}