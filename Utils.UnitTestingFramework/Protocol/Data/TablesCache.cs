namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// Represents the table cache.
    /// </summary>
    public class TablesCache
    {
        private readonly Dictionary<int, ITableModel> tablesPerTablePid = new Dictionary<int, ITableModel>();

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        public void AddTable(ITableModel tableModel)
        {
            if (tableModel == null)
            {
                throw new ArgumentNullException(nameof(tableModel));
            }

            tablesPerTablePid[tableModel.TableId] = tableModel;
        }

        /// <summary>
        /// Gets the table model for the table with the specified parameter ID.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <returns>The table model for the table with the specified ID.</returns>
        /// <exception cref="ArgumentException">There is no table with ID " + tableId</exception>
        public ITableModel GetTable(int tablePid)
        {
            if (tablesPerTablePid.TryGetValue(tablePid, out var tableModel))
            {
                return tableModel;
            }

            throw new ArgumentException($"There is no table with ID '{tablePid}'");
        }

        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <param name="row">The row data.</param>
        /// <returns>The 1-based row number or 0 if the cache does not contain a table model for the specified table ID.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> is <see langword="null"/>.</exception>
        public int AddRow(int tablePid, object[] row, DateTime? timestamp = null)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var tableModel = GetTable(tablePid);

            int primaryKeyIndex = tableModel.PrimaryKeyColumnIdx;
            string primaryKey = (string)row[primaryKeyIndex];

            if (tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                return tableModel.KeyToRowIndex[primaryKey] + 1;
            }

            tableModel.SetRow(row, timestamp);

            int oneBasedRowNumber = tableModel.KeyToRowIndex[primaryKey] + 1;

            return oneBasedRowNumber;
        }

        /// <summary>
        /// Adds a row with the specified primary key.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>The 1-based row number or 0 if no table exists with the specified table ID.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/>.</exception>
        public int AddRow(int tablePid, string primaryKey)
        {
            var tableModel = GetTable(tablePid);

            if (tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                return tableModel.KeyToRowIndex[primaryKey] + 1;
            }

            int columnsNumber = tableModel.ColumnCount;
            object[] row = new object[columnsNumber];
            row[tableModel.PrimaryKeyColumnIdx] = primaryKey;
            tableModel.SetRow(row);

            int oneBasedRowNumber = tableModel.KeyToRowIndex[primaryKey] + 1;

            return oneBasedRowNumber;
        }

        /// <summary>
        /// Adds the specified row to the table with the specified ID.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="row">The row.</param>
        /// <returns>The primary key of the added row or <see langword="null"/> if the cache does not contain a table model for the specified table ID.</returns>
        public string AddRowReturnKey(int tablePid, object[] row)
        {
            if (row == null)
            {
                return AddRowReturnKey(tablePid);
            }

            var tableModel = GetTable(tablePid);

            tableModel.SetRow(row);

            return (string)row[tableModel.PrimaryKeyColumnIdx];
        }

        /// <summary>
        /// Adds a row with the specified primary key to the table with the specified ID.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <returns>The primary key of the added row or <see langword="null"/> if the cache does not contain a table model for the specified table ID.</returns>
        public string AddRowReturnKey(int tablePid, string primaryKey)
        {
            if (primaryKey == null)
            {
                return AddRowReturnKey(tablePid);
            }

            var tableModel = GetTable(tablePid);

            var row = new object[tableModel.ColumnIndexesToPids.Count];
            row[tableModel.PrimaryKeyColumnIdx] = primaryKey;

            return AddRowReturnKey(tablePid, row);
        }

        /// <summary>
        /// Adds a row to the specified table and returns the primary key. Only used for auto-increment key tables.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <returns>The primary key of the added row.</returns>
        public string AddRowReturnKey(int tablePid)
        {
            var tableModel = GetTable(tablePid);

            string[] keys = GetKeys(tablePid);
            string primaryKey = "1";

            if (keys.Length != 0)
            {
                primaryKey = (Array.ConvertAll(keys, Convert.ToInt32).Max() + 1).ToString();
            }

            int columnsNumber = tableModel.ColumnCount;

            object[] newRow = new object[columnsNumber];
            newRow[tableModel.PrimaryKeyColumnIdx] = primaryKey;

            tableModel.SetRow(newRow);

            return primaryKey;
        }

        /// <summary>
        /// Deletes the specified rows in the specified table.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="primaryKeys">The primary keys.</param>
        /// <returns>The number of remaining rows.</returns>
        public int DeleteRow(int tablePid, string[] primaryKeys)
        {
            int remainingRows = -1;

            var tableModel = GetTable(tablePid);

            if (!primaryKeys.Any())
            {
                return tableModel.KeyToRowIndex
                    .Select(x => !String.IsNullOrWhiteSpace(x.Key))
                    .Count();
            }

            foreach (string pk in primaryKeys)
            {
                int rowIndex = tableModel.KeyToRowIndex[pk];
                remainingRows = DeleteRow(tablePid, rowIndex);
            }

            return remainingRows;
        }

        /// <summary>
        /// Deletes the specified row from the specified table.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The number of remaining rows.</returns>
        public int DeleteRow(int tablePid, int rowIndex)
        {
            var tableModel = GetTable(tablePid);

            int existingRows = tableModel.KeyToRowIndex
                .Select(x => !String.IsNullOrWhiteSpace(x.Key))
                .Count();

            if (rowIndex > existingRows)
            {
                return 0;
            }

            tableModel.RemoveRow(rowIndex);

            return tableModel.KeyToRowIndex
                .Select(x => !String.IsNullOrWhiteSpace(x.Key))
                .Count();
        }

        /// <summary>
        /// Deletes the specified row from the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>The number of remaining rows.</returns>
        public int DeleteRow(int tableId, string primaryKey)
        {
            var tableModel = GetTable(tableId);

            if (!tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                return 0;
            }

            int rowIndex = tableModel.KeyToRowIndex[primaryKey];
            int remainingRows = DeleteRow(tableId, rowIndex);

            return remainingRows;
        }

        /// <summary>
        /// Removes all rows from the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <returns>0 or -1 if the table cache does not contain a table model for the specified table ID.</returns>
        public object ClearAllKeys(int tableId)
        {
            var tableModel = GetTable(tableId);

            var tableKeys = tableModel.KeyToRowIndex.Keys.ToList();

            foreach (var key in tableKeys)
            {
                DeleteRow(tableId, key);
            }

            return 0;
        }

        /// <summary>
        /// Determines whether the table with the specified ID contains a row with the specified primary key.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns><c>true</c> the table with the specified ID contains a row with the specified primary key; otherwise, <c>false</c>.</returns>
        public bool Exists(int tableId, string primaryKey)
        {
            var tableModel = GetTable(tableId);

            return tableModel.KeyToRowIndex.ContainsKey(primaryKey);
        }

        /// <summary>
        /// Gets the 1-based key position of the row with the specified primary key in the table with the specified ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>The 1-based row position or 0 if the table does not contain a row with the specified primary key.</returns>
        public int GetOneBasedRowIndex(int tableId, string primaryKey)
        {
            var tableModel = GetTable(tableId);

            return tableModel.KeyToRowIndex.ContainsKey(primaryKey)
                ? tableModel.KeyToRowIndex[primaryKey] + 1
                : 0;
        }

        /// <summary>
        /// Gets the specified row from the table with the specified table ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data.</returns>
        public object GetRow(int tableId, int rowIndex)
        {
            if (!tablesPerTablePid.ContainsKey(tableId))
            {
                return null;
            }

            return InternalGetRow(tableId, rowIndex);
        }

        /// <summary>
        /// Gets the specified row from the table with the specified table ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>The row data.</returns>
        public object GetRow(int tableId, string primaryKey)
        {
            var tableModel = GetTable(tableId);

            if (!tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                int columnsNumber = tableModel.ColumnCount;
                var row = new object[columnsNumber];

                return row;
            }

            int rowIndex = tableModel.KeyToRowIndex[primaryKey];

            return InternalGetRow(tableId, rowIndex);
        }

        /// <summary>
        /// Sets the specified row in the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="row">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="useClearAndLeave">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public object SetRow(int tableId, int rowIndex, object[] row, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            // TODO: issue 33 from project internship-2022
            var tableModel = GetTable(tableId);

            var rowToSet = row.ToArray(); // Make a copy to avoid modifying the original array

            if (useClearAndLeave)
            {
                rowToSet = ConvertProtocolClearAndleaveToActualValuesForRow(rowIndex, rowToSet, tableModel);
            }

            if (tableModel.ColumnCount < rowIndex + 1)
            {
                var changes = new int[rowToSet.Length];

                tableModel.SetRow(rowToSet, timestamp);

                return changes;
            }

            return tableModel.SetExistingRow(rowToSet, rowIndex, timestamp);
        }

        /// <summary>
        /// Sets the specified row in the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="row">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="useClearAndLeave">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public object SetRow(int tableId, string primaryKey, object[] row, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            var tableModel = GetTable(tableId);

            var rowToSet = row.ToArray(); // Make a copy to avoid modifying the original array

            if (useClearAndLeave)
            {
                rowToSet = ConvertProtocolClearAndleaveToActualValuesForRow(primaryKey, rowToSet, tableModel);
            }

            int rowIndex = tableModel.KeyToRowIndex[primaryKey];

            return tableModel.SetExistingRow(rowToSet, rowIndex, timestamp);
        }

        /// <summary>
        /// Sets the content of the table to the provided content.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="option">The save option.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c></returns>
        public object FillArray(int tableId, List<object[]> rows, NotifyProtocol.SaveOption option, DateTime? timeInfo = null)
        {
            var tableModel = GetTable(tableId);

            if (option == NotifyProtocol.SaveOption.Full)
            {
                ClearAllKeys(tableId);

                foreach (var rowData in rows)
                {
                    tableModel.SetRow(rowData, timeInfo);
                }
            }
            else
            {
                foreach (var rowData in rows)
                {
                    string pk = (string)rowData[tableModel.PrimaryKeyColumnIdx];

                    if (tableModel.KeyToRowIndex.ContainsKey(pk))
                    {
                        tableModel.SetExistingRow(rowData, tableModel.KeyToRowIndex[pk], timeInfo);
                    }
                    else
                    {
                        tableModel.SetRow(rowData, timeInfo);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Sets the content of the table to the provided content.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <param name="useClearAndLeave"></param>
        /// <returns><c>true</c></returns>
        public void FillArray(int tableId, object[][] columns, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            var tableModel = GetTable(tableId);

           var primaryKeyColumn = FindPrimaryKeyColumn(columns, tableModel);

            FillArrayNoDelete(tableId, columns, timeInfo, useClearAndLeave);

            var primaryKeysInTable = tableModel.KeyToRowIndex.Keys.ToList();

            var keysToDelete = primaryKeysInTable.Except(primaryKeyColumn).ToArray();

            DeleteRow(tableId, keysToDelete);
        }

        private static string[] FindPrimaryKeyColumn(object[][] columns, ITableModel tableModel)
        {
            var primaryKeyColumn = columns[tableModel.PrimaryKeyColumnIdx];

            var primaryKeyColumnOfStrings = primaryKeyColumn.OfType<string>().ToArray();

            if (primaryKeyColumn.Length != primaryKeyColumnOfStrings.Length)
            {
                throw new ArgumentException("The primary key column should only contain string values.");
            }

            return primaryKeyColumnOfStrings;
        }

        /// <summary>
        /// Adds the provided rows to the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <param name="useClearAndLeave"></param>
        /// <returns><c>true</c> or <see langword="null"/> if the table cache does not contain a model for that table with the specified ID.</returns>
        public void FillArrayNoDelete(int tableId, object[][] columns, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            var tableModel = GetTable(tableId);

            var primaryKeyColumn = FindPrimaryKeyColumn(columns, tableModel);

            for (int index = 0; index < columns.Length; index++)
            {
                var columnToSet = columns[index].ToArray(); // Make a copy to avoid modifying the original array

                if (useClearAndLeave)
                {
                    columnToSet = ConvertProtocolClearAndleaveToActualValuesForColumn(primaryKeyColumn, columnToSet, tableModel, index);
                }

                tableModel.SetColumn(index, primaryKeyColumn, columnToSet, timeInfo);
            }
        }

        /// <summary>
        /// Sets the specified cells of a column with the provided values.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <param name="columnPid">The column identifier.</param>
        /// <param name="primaryKeys">The primary keys.</param>
        /// <param name="columnValues">The values.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <param name="useClearAndLeave">A boolean indicating if values uses protocol.Leave and protocol.Clear.</param>
        /// <returns><c>true</c></returns>
        /// <exception cref="ArgumentException">There should be as many primary keys as values or instead only one value.</exception>
        public void FillArrayWithColumn(int tablePid, int columnPid, string[] primaryKeys, object[] columnValues, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            if (primaryKeys.Length != columnValues.Length && (primaryKeys.Length == columnValues.Length || columnValues.Length != 1))
            {
                throw new ArgumentException("There should be as many primary keys as values or instead only one value.");
            }

            var tableModel = GetTable(tablePid);

            int columnIndex = GetColumnIndex(tableModel, columnPid);

            var columnValuesToSet = columnValues.ToArray(); // Make a copy to avoid modifying the original array

            if (columnValuesToSet.Length == 1)
            {
                var newValues = new object[primaryKeys.Length];

                for (int i = 0; i < newValues.Length; i++)
                {
                    newValues[i] = columnValuesToSet[0];
                }

                columnValuesToSet = newValues;
            }

            if (useClearAndLeave)
            {
                columnValuesToSet = ConvertProtocolClearAndleaveToActualValuesForColumn(primaryKeys, columnValuesToSet, tableModel, columnIndex);
            }

            tableModel.SetColumn(columnIndex, primaryKeys, columnValuesToSet, timeInfo);
        }

        private static int GetColumnIndex(ITableModel tableModel, int columnPid)
        {
            var columnIdxToPid = tableModel.ColumnIndexesToPids.FirstOrDefault(x => x.Value == columnPid);

            if (columnIdxToPid.Equals(default(KeyValuePair<int, int>)))
            {
                throw new InvalidOperationException($"Column with ID '{columnPid}' does not exist in table with ID '{tableModel.TableId}'.");
            }

            int columnIndex = columnIdxToPid.Key;

            return columnIndex;
        }

        private static object[] ConvertProtocolClearAndleaveToActualValuesForRow(string primaryKey, object[] row, ITableModel tableModel)
        {
            object[] existingRow = null;

            if (tableModel.KeyToRowIndex.Keys.Contains(primaryKey))
            {
                existingRow = tableModel.GetRow(primaryKey);
            }

            return ConvertProtocolClearAndleaveToActualValuesForRow(row, existingRow);
        }

        private static object[] ConvertProtocolClearAndleaveToActualValuesForRow(object[] rowToConvert, object[] existingRow)
        {
            var convertedRow = new object[rowToConvert.Length];

            for (int i = 0; i < rowToConvert.Length; i++)
            {
                if (rowToConvert[i].IsProtocolClear())
                {
                    convertedRow[i] = null;
                }
                else if (rowToConvert[i].IsProtocolLeave() && existingRow != null)
                {
                    convertedRow[i] = existingRow[i];
                }
            }

            return convertedRow;
        }

        private static object[] ConvertProtocolClearAndleaveToActualValuesForRow(int rowIndex, object[] row, ITableModel tableModel)
        {
            object[] existingRow = null;

            if (tableModel.KeyToRowIndex.Values.Contains(rowIndex))
            {
                existingRow = tableModel.GetRow(rowIndex);
            }

            return ConvertProtocolClearAndleaveToActualValuesForRow(row, existingRow);
        }

        private static object[] ConvertProtocolClearAndleaveToActualValuesForColumn(string[] primaryKeys, object[] column, ITableModel tableModel, int columnIndex)
        {
            var convertedColumn = new object[column.Length];

            for (int i = 0; i < column.Length; i++)
            {
                if (column[i].IsProtocolClear())
                {
                    convertedColumn[i] = null;
                }
                else if (column[i].IsProtocolLeave() && tableModel.KeyToRowIndex.TryGetValue(primaryKeys[i], out int rowIndex))
                {
                    var existingRow = tableModel.GetRow(rowIndex);
                    var existingValue = existingRow[columnIndex];
                    convertedColumn[i] = existingValue;
                }
            }

            return convertedColumn;
        }

        /// <summary>
        /// Sets columns in a table based on the provided column info and values information.
        /// </summary>
        public void FillArrayWithColumns(int tablePid, string[] primaryKeys, IEnumerable<KeyValuePair<int, object[]>> columnPidsToValues, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            if (columnPidsToValues == null)
            {
                throw new ArgumentNullException(nameof(columnPidsToValues));
            }

            foreach (var columnPidToValues in columnPidsToValues)
            {
                FillArrayWithColumn(tablePid, columnPidToValues.Key, primaryKeys, columnPidToValues.Value, timestamp, useClearAndLeave);
            }
        }

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columnIndexes">The column indexes.</param>
        /// <returns>A jagged array where each entry represents a column.</returns>
        public object[][] GetTableColumns(int tableId, uint[] columnIndexes)
        {
            var tableModel = GetTable(tableId);

            object[][] columns = new object[columnIndexes.Length][];

            int index = 0;

            foreach (var columnIdx in columnIndexes)
            {
                if (tableModel.ColumnIndexesToPids.ContainsKey((int)columnIdx))
                {
                    int pid = tableModel.ColumnIndexesToPids[(int)columnIdx];
                    object[] column = tableModel.GetColumn(pid);
                    columns[index] = column;
                }
                else
                {
                    columns[index] = null;
                }

                index++;
            }

            return columns.ToArray();
        }

        /// <summary>
        /// Retrieves the specified column of the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columnPid">The column pid.</param>
        /// <returns>The column data.</returns>
        public object[] GetColumn(int tableId, int columnPid)
        {
            var tableModel = GetTable(tableId);

            return tableModel.GetColumn(columnPid);
        }

        /// <summary>
        /// Gets the number of rows present in the specified table.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <returns>The number of rows the table contains. If the table was not found, a value of -1 is returned.</returns>
        public int RowCount(int tablePid)
        {
            var tableModel = GetTable(tablePid);

            return tableModel.KeyToRowIndex.Count;
        }

        /// <summary>
        /// Gets the primary keys of the specified table.
        /// </summary>
        /// <param name="tablePid">The table identifier.</param>
        /// <returns>The primary keys of the rows present in the table.</returns>
        public string[] GetKeys(int tablePid)
        {
            var tableModel = GetTable(tablePid);

            return tableModel.KeyToRowIndex.Keys.ToArray();
        }

        /// <summary>
        /// Retrieves the value of a cell in the table specified by the 1-based row and column position.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <param name="oneBasedRowIndex">The 1-based position of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <returns>The value of the cell.</returns>
        public object GetParameterIndex(int tablePid, int oneBasedRowIndex, int oneBasedColumnIndex)
        {
            if (oneBasedRowIndex < 1)
            {
                throw new ArgumentException("Row index must be 1 or higher.", nameof(oneBasedRowIndex));
            }

            if (oneBasedColumnIndex < 1)
            {
                throw new ArgumentException("Column index must be 1 or higher.", nameof(oneBasedColumnIndex));
            }

            var tableModel = GetTable(tablePid);

            if (oneBasedColumnIndex > tableModel.ColumnCount)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedRowIndex > tableModel.RowCount)
            {
                throw new ArgumentException("Row index exceeds number of rows", nameof(oneBasedRowIndex));
            }

            var row = tableModel.GetRow(oneBasedRowIndex - 1);

            return row[oneBasedColumnIndex - 1];
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by its 1-based row and column position, with the specified value.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <param name="oneBasedRowIndex">The 1-based position of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public bool SetParameterIndex(int tablePid, int oneBasedRowIndex, int oneBasedColumnIndex, object value, DateTime? timeInfo = null)
        {
            if (oneBasedRowIndex < 1)
            {
                throw new ArgumentException("Row index must be 1 or higher.", nameof(oneBasedRowIndex));
            }

            if (oneBasedColumnIndex < 2)
            {
                throw new ArgumentException("Column index must be 2 or higher.", nameof(oneBasedColumnIndex));
            }

            var tableModel = GetTable(tablePid);

            if (oneBasedColumnIndex > tableModel.ColumnCount)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedRowIndex > tableModel.RowCount)
            {
                throw new ArgumentException("Row index exceeds number of rows", nameof(oneBasedRowIndex));
            }

            var row = tableModel.GetRow(oneBasedRowIndex - 1);
            row[oneBasedColumnIndex - 1] = value;

            tableModel.SetExistingRow(row, oneBasedRowIndex - 1, timeInfo);

            return true;
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by its 1-based row and column position, with the specified value.
        /// </summary>
        /// <param name="ids">The IDs of the table parameters.</param>
        /// <param name="iXs">The 1-based positions of the rows.</param>
        /// <param name="iYs">The 1-based positions of the columns.</param>
        /// <param name="values">The values to set.</param>
        /// <param name="timeInfos">Time stamps.</param>
        /// <returns>This method call can return an unsigned integer error code, e.g. when the size of the ids array does not
        /// match the size of the values array. Otherwise a uint[] is returned that has the same size as the ids array containing
        /// the HRESULT value.At each position, this array contains the result value as would be returned when performing a
        /// SetParameterIndex call on the individual cell.In case the value in the array is 262730 (0x0004024AL), this indicates
        /// the cell value changed.</returns>
        public object SetParametersIndex(int[] ids, int[] iXs, int[] iYs, object[] values, DateTime?[] timeInfos = null)
        {
            if (!(ids.Length == iXs.Length
                    && ids.Length == iYs.Length
                    && ids.Length == values.Length))
            {
                return 0x80040221L; // Invalid data.
            }

            if (timeInfos == null)
            {
                timeInfos = new DateTime?[5];
            }

            uint[] results = new uint[ids.Length];

            for (int i = 0; i < ids.Length; i++)
            {
                if (SetParameterIndex(ids[i], iXs[i], iYs[i], values[i], timeInfos[i]))
                {
                    results[i] = (uint)0x0004024AL; //// Parameter changed
                }
                else
                {
                    results[i] = (uint)0x800402A4L; //// Action not performed;
                }
            }

            return results;
        }

        /// <summary>
        /// Retrieves the value of a cell in the table specified by the primary key and 1-based column position.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <returns>The value of the cell.</returns>
        public object GetParameterIndexByKey(int tablePid, string primaryKey, int oneBasedColumnIndex)
        {
            if (!Exists(tablePid, primaryKey))
            {
               throw new ArgumentException($"The table with ID '{tablePid}' does not contain a row with primary key '{primaryKey}'", nameof(primaryKey));
            }

            if (oneBasedColumnIndex < 1)
            {
                throw new ArgumentException("Column index must be 1 or higher", nameof(oneBasedColumnIndex));
            }

            var tableModel = GetTable(tablePid);

            if (oneBasedColumnIndex > tableModel.ColumnCount)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            var row = tableModel.GetRow(primaryKey);

            return row[oneBasedColumnIndex - 1];
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public bool SetParameterIndexByKey(int tablePid, string primaryKey, int oneBasedColumnIndex, object value, DateTime? timeInfo = null)
        {
            if (!Exists(tablePid, primaryKey))
            {
                throw new ArgumentException($"The table with ID '{tablePid}' does not contain a row with primary key '{primaryKey}'", nameof(primaryKey));
            }

            if (oneBasedColumnIndex < 2)
            {
                throw new ArgumentException("Column index must be 2 or higher", nameof(oneBasedColumnIndex));
            }

            var tableModel = GetTable(tablePid);

            if (oneBasedColumnIndex > tableModel.ColumnCount)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            var row = tableModel.GetRow(primaryKey);
            row[oneBasedColumnIndex - 1] = value;

            int rowIndex = tableModel.KeyToRowIndex[primaryKey];

            tableModel.SetExistingRow(row, rowIndex, timeInfo);

            return true;
        }

        /// <summary>
        /// Sets the value of cells in tables, identified by their primary key and 1-based column position, with the specified values.
        /// </summary>
        /// <param name="ids">The IDs of the table parameters.</param>
        /// <param name="keys">The primary keys of the rows.</param>
        /// <param name="iYs">The 1-based positions of the columns.</param>
        /// <param name="values">The values to set.</param>
        /// <param name="timeInfos">Time stamps.</param>
        /// <returns>This method call can return an unsigned integer error code, e.g. when the size of the ids array does not match the
        /// size of the values array.Otherwise a uint[] is returned that has the same size as the ids array containing the HRESULT value.
        /// At each position, this array contains the result value as would be returned when performing a SetParameterIndexByKey call on
        /// the individual cell.In case the value in the array is 262730 (0x0004024AL), this indicates the cell value changed.</returns>
        public object SetParametersIndexByKey(int[] ids, string[] keys, int[] iYs, object[] values, DateTime?[] timeInfos = null)
        {
            if (!(ids.Length == keys.Length
                    && ids.Length == iYs.Length
                    && ids.Length == values.Length))
            {
                return 0x80040221L; // Invalid data
            }

            if (timeInfos == null)
            {
                timeInfos = new DateTime?[5];
            }

            uint[] results = new uint[ids.Length];

            for (int i = 0; i < ids.Length; i++)
            {
                if (SetParameterIndexByKey(ids[i], keys[i], iYs[i], values[i], timeInfos[i]))
                {
                    results[i] = (uint)0x0004024AL; // Parameter changed
                }
                else
                {
                    results[i] = (uint)0x800402A4L; // Action not performed
                }
            }

            return results;
        }

        /// <summary>
        /// Gets all the names of the columns in a table.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <param name="tableId">The ID of the table parameter.</param>
        /// <param name="names">The names of the columns.</param>
        /// <param name="pids">The pids of the columns.</param>
        public void GetColumnsNamesAndPids(IProtocolCache cache, int tableId, out string[] names, out int[] pids)
        {
            var tableModel = GetTable(tableId);

            int size = tableModel.ColumnCount;

            string[] columnsNames = new string[size];
            int[] columnsPids = new int[size];

            for (int i = 0; i < size; i++)
            {
                columnsPids[i] = tableModel.ColumnIndexesToPids[i];

                if (!cache.Parameters.TryGetParameterNameByPID(columnsPids[i], out columnsNames[i]))
                {
                    columnsNames[i] = null;
                }
            }

            pids = columnsPids;
            names = columnsNames;
        }

        private object InternalGetRow(int tablePid, int rowIndex)
        {
            var tableModel = GetTable(tablePid);

            int columnsNumber = tableModel.ColumnCount;

            var row = new object[columnsNumber];

            try
            {
                row = tableModel.GetRow(rowIndex);
            }
            catch (ArgumentException)
            {
                return row;
            }

            return row;
        }
    }
}