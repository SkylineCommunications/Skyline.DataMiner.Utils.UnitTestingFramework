namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// Represents the table cache.
    /// </summary>
    public class TablesCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TablesCache"/> class.
        /// </summary>
        public TablesCache()
        {
            TablesCacheDict = new Dictionary<int, ITableModel>();
        }

        /// <summary>
        /// Dictionary containing the table ID to table model map.
        /// </summary>
        protected Dictionary<int, ITableModel> TablesCacheDict { get; }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        public void AddModel(ITableModel tableModel)
        {
            if (tableModel == null)
            {
                throw new ArgumentNullException(nameof(tableModel));
            }

            TablesCacheDict[tableModel.TableId] = tableModel;
        }

        /// <summary>
        /// Gets the table model for the table with the specified parameter ID.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <returns>The table model for the table with the specified ID.</returns>
        /// <exception cref="ArgumentException">There is no table with ID " + tableId</exception>
        public ITableModelReader GetTableModel(int tablePid)
        {
            if (TablesCacheDict.TryGetValue(tablePid, out ITableModel output))
            {
                return output;
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
        public int AddRow(int tablePid, object[] row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (!TablesCacheDict.ContainsKey(tablePid))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tablePid];

            int primaryKeyIndex = tableModel.KeyColumnIdx;
            string primaryKey = (string)row[primaryKeyIndex];

            if (tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                return tableModel.KeyToRowIndex[primaryKey] + 1;
            }

            tableModel.SetRow(row);

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
            if (!TablesCacheDict.ContainsKey(tablePid))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tablePid];

            if (tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                return tableModel.KeyToRowIndex[primaryKey] + 1;
            }

            int columnsNumber = tableModel.ColumnCount;
            object[] row = new object[columnsNumber];
            row[tableModel.KeyColumnIdx] = primaryKey;
            tableModel.SetRow(row);

            int oneBasedRowNumber = tableModel.KeyToRowIndex[primaryKey] + 1;

            return oneBasedRowNumber;
        }

        /// <summary>
        /// Adds the specified row to the table with the specified ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="row">The row.</param>
        /// <returns>The primary key of the added row or <see langword="null"/> if the cache does not contain a table model for the specified table ID.</returns>
        public string AddRowReturnKey(int tableId, object[] row)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }
            else if (row == null)
            {
                return AddRowReturnKey(tableId);
            }

            ITableModel tableModel = TablesCacheDict[tableId];
            tableModel.SetRow(row);

            return (string)row[tableModel.KeyColumnIdx];
        }

        /// <summary>
        /// Adds a row with the specified primary key to the table with the specified ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <returns>The primary key of the added row or <see langword="null"/> if the cache does not contain a table model for the specified table ID.</returns>
        public string AddRowReturnKey(int tableId, string primaryKey)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }
            else if (primaryKey == null)
            {
                return AddRowReturnKey(tableId);
            }

            ITableModel tableModel = TablesCacheDict[tableId];
            var row = new object[tableModel.ColumnIndexesToPids.Count];
            row[tableModel.KeyColumnIdx] = primaryKey;

            return AddRowReturnKey(tableId, row);
        }

        /// <summary>
        /// Adds a row to the specified table and returns the primary key. Only used for auto-increment key tables.
        /// </summary>
        /// <param name="tableID">The ID of the table parameter.</param>
        /// <returns>The primary key of the added row.</returns>
        public string AddRowReturnKey(int tableID)
        {
            if (!TablesCacheDict.ContainsKey(tableID))
            {
                throw new NullReferenceException();
            }

            ITableModel tableModel = TablesCacheDict[tableID];

            string[] keys = GetKeys(tableID);
            string primaryKey = "1";

            if (keys.Length != 0)
            {
                primaryKey = (Array.ConvertAll(keys, Convert.ToInt32).Max() + 1).ToString();
            }

            int columnsNumber = tableModel.ColumnCount;

            object[] newRow = new object[columnsNumber];
            newRow[tableModel.KeyColumnIdx] = primaryKey;

            tableModel.SetRow(newRow);

            return primaryKey;
        }

        /// <summary>
        /// Deletes the specified rows in the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKeys">The primary keys.</param>
        /// <returns>The number of remaining rows.</returns>
        public int DeleteRow(int tableId, string[] primaryKeys)
        {
            int remainingRows = -1;

            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            if (!primaryKeys.Any())
            {
                return tableModel.KeyToRowIndex
                    .Select(x => !String.IsNullOrWhiteSpace(x.Key))
                    .Count();
            }

            foreach (string pk in primaryKeys)
            {
                int rowIndex = tableModel.KeyToRowIndex[pk];
                remainingRows = DeleteRow(tableId, rowIndex);
            }

            return remainingRows;
        }

        /// <summary>
        /// Deletes the specified row from the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The number of remaining rows.</returns>
        public int DeleteRow(int tableId, int rowIndex)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return -1;
            }

            if (IsTableEmpty(tableId))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return false;
            }

            ITableModel tableModel = TablesCacheDict[tableId];
            return tableModel.KeyToRowIndex.ContainsKey(primaryKey);
        }

        /// <summary>
        /// Gets the 1-based key position of the row with the specified primary key in the table with the specified ID.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <returns>The 1-based row position or 0 if the table does not contain a row with the specified primary key.</returns>
        public int GetKeyPosition(int tableId, string primaryKey)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return 0;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
            if (!TablesCacheDict.ContainsKey(tableId))
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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
        /// <param name="rowData">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="enableCellActions">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public object SetRow(int tableId, int rowIndex, object rowData, DateTime? timestamp = null, bool? enableCellActions = null)
        {
            // TODO: issue 33 from project internship-2022
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            int pid = tableModel.ColumnIndexesToPids[tableModel.KeyColumnIdx];

            if (tableModel.GetColumnItemCount(pid) < rowIndex + 1)
            {
                var data = (object[])rowData;
                var changes = new int[data.Length];

                tableModel.SetRow(data, timestamp);

                return changes;
            }

            return tableModel.SetExistingRow((object[])rowData, rowIndex, timestamp);
        }

        /// <summary>
        /// Sets the specified row in the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="rowData">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="enableCellActions">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public object SetRow(int tableId, string primaryKey, object rowData, DateTime? timestamp = null, bool? enableCellActions = null)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            if (!tableModel.KeyToRowIndex.ContainsKey(primaryKey))
            {
                var data = (object[])rowData;
                var changes = new int[data.Length];

                return changes;
            }

            int rowIndex = tableModel.KeyToRowIndex[primaryKey];

            return tableModel.SetExistingRow((object[])rowData, rowIndex, timestamp);
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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return true;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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
                    string pk = (string)rowData[tableModel.KeyColumnIdx];

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
        /// <returns><c>true</c></returns>
        public object FillArray(int tableId, List<object[]> columns, DateTime? timeInfo = null)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return true;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            int primaryKeyIndex = tableModel.KeyColumnIdx;

            var primaryKeyArray = columns[primaryKeyIndex].Cast<string>().ToArray();

            FillArrayNoDelete(tableId, columns, timeInfo);

            var keys = tableModel.KeyToRowIndex.Keys.ToList();

            var keysToDelete = keys.Where(primaryKey => !primaryKeyArray.Contains(primaryKey)).ToArray();

            DeleteRow(tableId, keysToDelete);

            return true;
        }

        /// <summary>
        /// Sets the content of the table to the provided content.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c></returns>
        public object FillArray(int tableId, object[] columns, DateTime? timeInfo = null)
        {
            var listOfCols = new List<object[]>();

            foreach (var col in columns)
            {
                listOfCols.Add((object[])col);
            }

            return FillArray(tableId, listOfCols, timeInfo);
        }

        /// <summary>
        /// Adds the provided rows to the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c> or <see langword="null"/> if the table cache does not contain a model for that table with the specified ID.</returns>
        public object FillArrayNoDelete(int tableId, List<object[]> columns, DateTime? timeInfo = null)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            int primaryKeyIndex = tableModel.KeyColumnIdx;

            var primaryKeyArray = columns[primaryKeyIndex].Cast<string>().ToArray();

            for (int index = 0; index < columns.Count; index++)
            {
                tableModel.SetColumn(index, primaryKeyArray, columns[index], timeInfo);
            }

            return true;
        }

        /// <summary>
        /// Fills the array no delete.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c> or <see langword="null"/> if the table cache does not contain a model for that table with the specified ID.</returns>
        public object FillArrayNoDelete(int tableId, object[] columns, DateTime? timeInfo = null)
        {
            var listOfCols = new List<object[]>();

            foreach (var col in columns)
            {
                listOfCols.Add((object[])col);
            }

            return FillArrayNoDelete(tableId, listOfCols, timeInfo);
        }

        /// <summary>
        /// Sets the specified cells of a column with the provided values.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columnID">The column identifier.</param>
        /// <param name="primaryKeys">The primary keys.</param>
        /// <param name="values">The values.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c></returns>
        /// <exception cref="ArgumentException">There should be as many primary keys as values or instead only one value.</exception>
        public object FillArrayWithColumn(int tableId, int columnID, object[] primaryKeys, object[] values, DateTime? timeInfo = null)
        {
            if (primaryKeys.Length != values.Length && (primaryKeys.Length == values.Length || values.Length != 1))
            {
                throw new ArgumentException("There should be as many primary keys as values or instead only one value.");
            }

            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return true;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            var primaryKeysArray = primaryKeys.Cast<string>().ToArray();

            var columnIdxToPid = tableModel.ColumnIndexesToPids.FirstOrDefault(x => x.Value == columnID);

            if (columnIdxToPid.Equals(default(KeyValuePair<int, int>)))
            {
                return true;
            }

            var idx = columnIdxToPid.Key;

            if (values.Length == 1)
            {
                var newValues = new object[primaryKeys.Length];

                for (int i = 0; i < newValues.Length; i++)
                {
                    newValues[i] = values[0];
                }

                tableModel.SetColumn(idx, primaryKeysArray, newValues, timeInfo);

                return true;
            }

            tableModel.SetColumn(idx, primaryKeysArray, values, timeInfo);

            return true;
        }

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columnIndexes">The column indexes.</param>
        /// <returns>A jagged array where each entry represents a column.</returns>
        public object GetTableColumns(int tableId, uint[] columnIndexes)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            object[][] columns = new object[columnIndexes.Length][];

            ITableModel tableModel = TablesCacheDict[tableId];

            int index = 0;

            foreach (var columnIdx in columnIndexes)
            {
                if (tableModel.ColumnIndexesToPids.ContainsKey((int)columnIdx))
                {
                    int pid = tableModel.ColumnIndexesToPids[(int)columnIdx];
                    object[] column = tableModel.Column(pid);
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
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            return tableModel.Column(columnPid);
        }

        /// <summary>
        /// Gets the number of rows present in the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <returns>The number of rows the table contains. If the table was not found, a value of -1 is returned.</returns>
        public int RowCount(int tableId)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return -1;
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            return tableModel.KeyToRowIndex.Count;
        }

        /// <summary>
        /// Gets the primary keys of the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <returns>The primary keys of the rows present in the table.</returns>
        public string[] GetKeys(int tableId)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                return new string[0];
            }

            ITableModel tableModel = TablesCacheDict[tableId];

            return tableModel.KeyToRowIndex.Keys.ToArray();
        }

        /// <summary>
        /// Retrieves the value of a cell in the table specified by the 1-based row and column position.
        /// </summary>
        /// <param name="iID">The ID of the table parameter.</param>
        /// <param name="iX">The 1-based position of the row.</param>
        /// <param name="iY">The 1-based position of the column.</param>
        /// <returns>The value of the cell.</returns>
        public object GetParameterIndex(int iID, int iX, int iY)
        {
            if (!TablesCacheDict.ContainsKey(iID) || iX < 1 || iY < 1)
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[iID];

            int columnsNumber = tableModel.ColumnCount;
            int rowsNumber = RowCount(iID);

            if (iX > rowsNumber || iY > columnsNumber)
            {
                return null;
            }

            var row = tableModel.Row(iX - 1);

            return row[iY - 1];
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by its 1-based row and column position, with the specified value.
        /// </summary>
        /// <param name="iID">The ID of the table parameter.</param>
        /// <param name="iX">The 1-based position of the row.</param>
        /// <param name="iY">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public bool SetParameterIndex(int iID, int iX, int iY, object value, DateTime? timeInfo = null)
        {
            if (!TablesCacheDict.ContainsKey(iID) || iX < 1 || iY < 2) // The primary key can never be updated.
            {
                return false;
            }

            ITableModel tableModel = TablesCacheDict[iID];

            int columnsNumber = tableModel.ColumnCount;
            int rowsNumber = RowCount(iID);

            if (iX > rowsNumber || iY > columnsNumber)
            {
                return false;
            }

            var row = tableModel.Row(iX - 1);
            row[iY - 1] = value;

            tableModel.SetExistingRow(row, iX - 1, timeInfo);

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
        /// <param name="iPID">The ID of the table parameter.</param>
        /// <param name="key">The primary key of the row.</param>
        /// <param name="iY">The 1-based position of the column.</param>
        /// <returns>The value of the cell.</returns>
        public object GetParameterIndexByKey(int iPID, string key, int iY)
        {
            if (!TablesCacheDict.ContainsKey(iPID) || !Exists(iPID, key) || iY < 1)
            {
                return null;
            }

            ITableModel tableModel = TablesCacheDict[iPID];

            int columnsNumber = tableModel.ColumnCount;

            if (iY > columnsNumber)
            {
                return null;
            }

            var row = tableModel.Row(key);

            return row[iY - 1];
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value.
        /// </summary>
        /// <param name="iID">The ID of the table parameter.</param>
        /// <param name="key">The primary key of the row.</param>
        /// <param name="iY">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public bool SetParameterIndexByKey(int iID, string key, int iY, object value, DateTime? timeInfo = null)
        {
            if (!TablesCacheDict.ContainsKey(iID) || !Exists(iID, key) || iY < 2) // The primary key can never be updated.
            {
                return false;
            }

            ITableModel tableModel = TablesCacheDict[iID];

            int columnsNumber = tableModel.ColumnCount;

            if (iY > columnsNumber)
            {
                return false;
            }

            var row = tableModel.Row(key);
            row[iY - 1] = value;

            int iX = tableModel.KeyToRowIndex[key] + 1;

            tableModel.SetExistingRow(row, iX - 1, timeInfo);

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
        public void GetColumnsNamesAndPids(ProtocolCache cache, int tableId, out string[] names, out int[] pids)
        {
            if (!TablesCacheDict.ContainsKey(tableId))
            {
                throw new ArgumentException($"Invalid table ID '{tableId}'", nameof(tableId));
            }

            ITableModel tableModel = TablesCacheDict[tableId];

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

        internal void LoadSetups(Mock<SLProtocol> mock)
        {
            ProtocolSetupsLoader.LoadSetups(mock, this);
        }

        /// <summary>
        /// Determines whether the table with the specified table identifier is empty.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <returns>
        ///   <c>true</c> if the table with the specified table identifier is empty; otherwise, <c>false</c>.
        /// </returns>
        private bool IsTableEmpty(int tableId)
        {
            if (TablesCacheDict.ContainsKey(tableId))
            {
                ITableModel tableModel = TablesCacheDict[tableId];

                return tableModel.KeyToRowIndex.Count == 0;
            }

            return true;
        }

        private object InternalGetRow(int tableId, int rowIndex)
        {
            ITableModel tableModel = TablesCacheDict[tableId];

            int columnsNumber = tableModel.ColumnCount;

            var row = new object[columnsNumber];

            try
            {
                row = tableModel.Row(rowIndex);
            }
            catch (ArgumentException)
            {
                return row;
            }

            return row;
        }
    }
}