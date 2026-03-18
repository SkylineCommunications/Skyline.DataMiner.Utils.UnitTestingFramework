namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal static class TableModelExtensionsForProtocol
    {
        /// <summary>
        /// Adds the specified row.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="row">The row data.</param>
        /// <param name="timestamp"></param>
        /// <returns>The 1-based row number or 0 if the cache does not contain a table model for the specified table ID.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="row"/> is <see langword="null"/>.</exception>
        public static int SetRowReturnOneBasedIndex(this ITableModel tableModel, object[] row, DateTime? timestamp = null)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            var rowToSet = FitRowToCorrectSize(tableModel, row);

            tableModel.SetRow(rowToSet, timestamp);

            string key = Convert.ToString(rowToSet[tableModel.Schema.PrimaryKeyColumn.Idx]);

            int oneBasedRowNumber = tableModel.GetRowIndex(key) + 1;

            return oneBasedRowNumber;
        }

        private static object[] FitRowToCorrectSize(ITableModel tableModel, object[] row)
        {
            object[] rowToSet = row;
            if (row.Length < tableModel.Schema.ColumnDefinitions.Count)
            {
                rowToSet = new object[tableModel.Schema.ColumnDefinitions.Count];
                Array.Copy(row, rowToSet, row.Length);
            }
            else if (row.Length > tableModel.Schema.ColumnDefinitions.Count)
            {
                rowToSet = new object[tableModel.Schema.ColumnDefinitions.Count];
                Array.Copy(row, rowToSet, rowToSet.Length);
            }

            return rowToSet;
        }

        public static int SetRowReturnOneBasedIndex(this ITableModel tableModel, string primaryKey)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentException($"'{nameof(primaryKey)}' cannot be null or whitespace.", nameof(primaryKey));
            }

            var row = new object[tableModel.Schema.ColumnDefinitions.Count];
            row[tableModel.Schema.PrimaryKeyColumn.Idx] = primaryKey;

            return SetRowReturnOneBasedIndex(tableModel, row);
        }

        public static string AddRowReturnKey(this ITableModel tableModel, object[] row)
        {
            if (row == null)
            {
                return tableModel.AddRowReturnKey();
            }

            var rowToSet = FitRowToCorrectSize(tableModel, row);

            tableModel.SetRow(rowToSet);

            return (string)rowToSet[tableModel.Schema.PrimaryKeyColumn.Idx];
        }

        /// <summary>
        /// Adds a row with the specified primary key to the table with the specified ID.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <returns>The primary key of the added row or <see langword="null"/> if the cache does not contain a table model for the specified table ID.</returns>
        public static string AddRowReturnKey(this ITableModel tableModel, string primaryKey)
        {
            if (primaryKey == null)
            {
                return tableModel.AddRowReturnKey();
            }

            var row = new object[tableModel.Schema.ColumnDefinitions.Count];
            row[tableModel.Schema.PrimaryKeyColumn.Idx] = primaryKey;

            return tableModel.AddRowReturnKey(row);
        }

        /// <summary>
        /// Adds a row to the specified table and returns the primary key. Only used for auto-increment key tables.
        /// </summary>
        /// <returns>The primary key of the added row.</returns>
        public static string AddRowReturnKey(this ITableModel tableModel)
        {
            string[] keys = tableModel.GetAllRows().Keys.ToArray();
            string primaryKey = "1";

            if (keys.Length != 0)
            {
                primaryKey = (Array.ConvertAll(keys, Convert.ToInt32).Max() + 1).ToString();
            }

            object[] newRow = new object[tableModel.Schema.ColumnDefinitions.Count];
            newRow[tableModel.Schema.PrimaryKeyColumn.Idx] = primaryKey;

            tableModel.SetRow(newRow);

            return primaryKey;
        }

        public static int DeleteRowReturnRemainingRows(this ITableModel tableModel, params string[] primaryKeys)
        {
            foreach (var primaryKey in primaryKeys)
            {
                tableModel.RemoveRows(primaryKey);
            }

            return tableModel.RowCount;
        }

        /// <summary>
        /// Retrieves the row with the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The row.</returns>
        public static TRow GetRow<TRow>(this ITableModel tableModel, string key) where TRow : QActionTableRow
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var row = tableModel.GetRow(key).Select(cell => cell.Value).ToArray();

            return GetRow<TRow>(tableModel, row);
        }

        /// <summary>
        /// Retrieves the row at the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>The row.</returns>
        public static TRow GetRow<TRow>(this ITableModel tableModel, int index) where TRow : QActionTableRow
        {
            var row = tableModel.GetRow(index).Select(cell => cell.Value).ToArray();

            return GetRow<TRow>(tableModel, row);
        }

        private static TRow GetRow<TRow>(this ITableModel tableModel, object[] row) where TRow : QActionTableRow
        {
            if (row == null)
            {
                return null;
            }

            var rowAsConstructorArgument = new object[] { row };

            return (TRow)Activator.CreateInstance(typeof(TRow), rowAsConstructorArgument) ?? throw new InvalidOperationException($"Unable to create a {typeof(TRow).Name} of row");
        }

        public static object GetParameterIndexByKey(this ITableModel tableModel, string primaryKey, int oneBasedColumnIndex)
        {
            if (tableModel == null)
            {
                throw new ArgumentNullException(nameof(tableModel));
            }

            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentException($"'{nameof(primaryKey)}' cannot be null or whitespace.", nameof(primaryKey));
            }

            if (oneBasedColumnIndex < 2 || oneBasedColumnIndex > tableModel.Schema.ColumnDefinitions.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(oneBasedColumnIndex), $"'{nameof(oneBasedColumnIndex)}' must be between 2 and the number of columns in the table ({tableModel.Schema.ColumnDefinitions.Count}).");
            }

            var columnDefinition = tableModel.Schema.FindColumnDefinitionByIdx(oneBasedColumnIndex - 1);

            return tableModel.GetCell(primaryKey, columnDefinition.Pid).Value;
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by the primary key of the row and column position, with the specified value.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="primaryKey">The primary key of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public static bool SetParameterIndexByKey(this ITableModel tableModel, string primaryKey, int oneBasedColumnIndex, object value, DateTime? timeInfo = null)
        {
            if (tableModel == null)
            {
                throw new ArgumentNullException(nameof(tableModel));
            }

            if (oneBasedColumnIndex < 2 || oneBasedColumnIndex > tableModel.Schema.ColumnDefinitions.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(oneBasedColumnIndex), $"'{nameof(oneBasedColumnIndex)}' must be between 2 and the number of columns in the table ({tableModel.Schema.ColumnDefinitions.Count}).");
            }

            var columnDefinition = tableModel.Schema.FindColumnDefinitionByIdx(oneBasedColumnIndex - 1);

            tableModel.SetCell(primaryKey, columnDefinition.Pid, value, timeInfo);

            return true;
        }


        /// <summary>
        /// Sets the specified row in the specified table.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="primaryKey"></param>
        /// <param name="row">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="useClearAndLeave">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public static object SetRowReturnChanges(this ITableModel tableModel, string primaryKey, object[] row, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            var rowToSet = row.ToArray(); // Make a copy to avoid modifying the original array

            if (useClearAndLeave)
            {
                rowToSet = ConvertProtocolClearAndLeaveToActualValuesForRow(primaryKey, rowToSet, tableModel);
            }

            var oldRow = tableModel.GetRow(primaryKey);

            return UpdateCellsAndReturnChanges(tableModel, timestamp, rowToSet, oldRow);
        }

        private static object UpdateCellsAndReturnChanges(ITableModel tableModel, DateTime? timestamp, object[] rowToSet, IParameterValue[] oldRow)
        {
            var changes = new int[rowToSet.Length];

            foreach (var columnDefinition in tableModel.Schema.ColumnDefinitions)
            {
                if (rowToSet.Length <= columnDefinition.Idx)
                {
                    // If the provided row has fewer columns than the current column index, we skip
                    continue;
                }

                if (columnDefinition == tableModel.Schema.PrimaryKeyColumn)
                {
                    changes[columnDefinition.Idx] = 0;
                    continue;
                }

                var newValue = rowToSet[columnDefinition.Idx];

                var oldCell = oldRow[columnDefinition.Idx];
                var oldValue = oldCell.Value;

                changes[columnDefinition.Idx] = Equals(newValue, oldValue) ? 2 : 1;
                oldCell.Update(newValue, timestamp);
            }

            return changes;
        }

        /// <summary>
        /// Sets the specified row in the specified table.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="row">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <param name="useClearAndLeave">The enable cell actions.</param>
        /// <returns>The changes.</returns>
        public static object SetRowReturnChanges(this ITableModel tableModel, int rowIndex, object[] row, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            var rowToSet = row.ToArray(); // Make a copy to avoid modifying the original array

            if (useClearAndLeave)
            {
                rowToSet = ConvertProtocolClearAndleaveToActualValuesForRow(rowIndex, rowToSet, tableModel);
            }

            var oldRow = tableModel.GetRow(rowIndex);
            if (oldRow == null)
            {
                return new int[tableModel.Schema.ColumnDefinitions.Count];
            }

            return UpdateCellsAndReturnChanges(tableModel, timestamp, rowToSet, oldRow);
        }

        private static object[] ConvertProtocolClearAndLeaveToActualValuesForRow(string primaryKey, object[] row, ITableModel tableModel)
        {
            var existingRow = tableModel.GetRow(primaryKey).Select(cell => cell.Value).ToArray();

            return ConvertProtocolClearAndLeaveToActualValuesForRow(row, existingRow);
        }

        private static object[] ConvertProtocolClearAndLeaveToActualValuesForRow(object[] rowToConvert, object[] existingRow)
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
                else
                {
                    convertedRow[i] = rowToConvert[i];
                }
            }

            return convertedRow;
        }

        /// <summary>
        /// Sets the content of the table to the provided content.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="option">The save option.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <returns><c>true</c></returns>
        public static object FillArray(this ITableModel tableModel, List<object[]> rows, NotifyProtocol.SaveOption option, DateTime? timeInfo = null)
        {
            if (option == NotifyProtocol.SaveOption.Full)
            {
                tableModel.RemoveAllRows();

                foreach (var rowData in rows)
                {
                    tableModel.SetRow(rowData, timeInfo);
                }
            }
            else
            {
                if (rows.Count <= 0)
                {
                    throw new IndexOutOfRangeException(); // Mimic ConcreteSLProtocol behavior: it throws IndexOutOfRangeException when no rows are provided and SaveOption is Partial.
                }

                foreach (var rowData in rows)
                {
                    string primaryKey = (string)rowData[tableModel.Schema.PrimaryKeyColumn.Idx];

                    tableModel.SetRow(rowData, timeInfo);
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
        public static void FillArray(this ITableModel tableModel, object[][] columns, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            var newPrimaryKeys = columns[tableModel.Schema.PrimaryKeyColumn.Idx].Select(key => Convert.ToString(key)).ToArray();

            tableModel.FillArrayNoDelete(columns, timeInfo, useClearAndLeave);

            var primaryKeysInTable = tableModel.GetAllRows().Keys.ToList();

            var keysToDelete = primaryKeysInTable.Except(newPrimaryKeys).ToArray();

            tableModel.RemoveRows(keysToDelete);
        }

        /// <summary>
        /// Gets the table columns.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="columnIndexes">The column indexes.</param>
        /// <returns>A jagged array where each entry represents a column.</returns>
        public static object[][] GetTableColumns(this ITableModel tableModel, uint[] columnIndexes)
        {
            var columns = new object[columnIndexes.Length][];

            int index = 0;

            foreach (var columnIdx in columnIndexes)
            {
                var columnDefinition = tableModel.Schema.FindColumnDefinitionByIdx((int)columnIdx);

                if (columnDefinition != null)
                {
                    columns[index] = tableModel.GetColumnByPid(columnDefinition.Pid);
                }
                else
                {
                    columns[index] = null;
                }

                index++;
            }

            return columns.ToArray();
        }

        public static object[] GetColumnByPid(this ITableModel tableModel, int columnPid)
        {
            var column = tableModel.Schema.FindColumnDefinitionByPid(columnPid);

            return tableModel.GetAllRows().Values.Select(row => row[column.Idx].Value).ToArray();        
        }

        /// <summary>
        /// Adds the provided rows to the specified table.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <param name="useClearAndLeave"></param>
        /// <returns><c>true</c> or <see langword="null"/> if the table cache does not contain a model for that table with the specified ID.</returns>
        public static void FillArrayNoDelete(this ITableModel tableModel, object[][] columns, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            var newPrimaryKeys = columns[tableModel.Schema.PrimaryKeyColumn.Idx].Select(key => Convert.ToString(key)).ToArray();

            for (int index = 0; index < columns.Length; index++)
            {
                var columnToSet = columns[index].ToArray(); // Make a copy to avoid modifying the original array

                if (useClearAndLeave)
                {
                    columnToSet = ConvertProtocolClearAndleaveToActualValuesForColumn(newPrimaryKeys, columnToSet, tableModel, index);
                }

                int columnPid = tableModel.Schema.FindColumnDefinitionByIdx(index).Pid;

                for (int i = 0; i < newPrimaryKeys.Length; i++)
                {
                    string primaryKeyToSet = newPrimaryKeys[i];
                    if (tableModel.RowExists(primaryKeyToSet))
                    {
                        tableModel.SetCell(primaryKeyToSet, columnPid, columnToSet[i], timeInfo);
                    }
                    else
                    {
                        var emptyRow = new object[tableModel.Schema.ColumnDefinitions.Count];
                        emptyRow[tableModel.Schema.PrimaryKeyColumn.Idx] = primaryKeyToSet;
                        emptyRow[index] = columnToSet[i];

                        tableModel.SetRow(emptyRow);
                    }
                }
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
        public static void FillArrayWithColumn(this ITableModel tableModel, int columnPid, string[] primaryKeys, object[] columnValues, DateTime? timeInfo = null, bool useClearAndLeave = false)
        {
            if (primaryKeys.Length != columnValues.Length && (primaryKeys.Length == columnValues.Length || columnValues.Length != 1))
            {
                throw new ArgumentException("There should be as many primary keys as values or instead only one value.");
            }

            int columnIndex = tableModel.Schema.FindColumnDefinitionByPid(columnPid).Idx;

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

            for (int i = 0; i < primaryKeys.Length; i++)
            {
                string primaryKeyToSet = primaryKeys[i];
                if (tableModel.RowExists(primaryKeyToSet))
                {
                    tableModel.SetCell(primaryKeys[i], columnPid, columnValuesToSet[i], timeInfo);
                }
                else
                {
                    var emptyRow = new object[tableModel.Schema.ColumnDefinitions.Count];
                    emptyRow[tableModel.Schema.PrimaryKeyColumn.Idx] = primaryKeyToSet;
                    emptyRow[columnIndex] = columnValuesToSet[i];

                    tableModel.SetRow(emptyRow);
                }
            }
        }

        /// <summary>
        /// Sets columns in a table based on the provided column info and values information.
        /// </summary>
        public static void FillArrayWithColumns(this ITableModel tableModel, string[] primaryKeys, IEnumerable<KeyValuePair<int, object[]>> columnPidsToValues, DateTime? timestamp = null, bool useClearAndLeave = false)
        {
            if (columnPidsToValues == null)
            {
                throw new ArgumentNullException(nameof(columnPidsToValues));
            }

            foreach (var columnPidToValues in columnPidsToValues)
            {
                tableModel.FillArrayWithColumn(columnPidToValues.Key, primaryKeys, columnPidToValues.Value, timestamp, useClearAndLeave);
            }
        }

        private static object[] ConvertProtocolClearAndleaveToActualValuesForRow(int rowIndex, object[] row, ITableModel tableModel)
        {
            var existingRow = tableModel.GetRow(rowIndex).Select(cell => cell.Value).ToArray();
            
            return ConvertProtocolClearAndLeaveToActualValuesForRow(row, existingRow);
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
                else if (column[i].IsProtocolLeave())
                {
                    var existingRow = tableModel.GetRow(primaryKeys[i]);

                    if (existingRow == null)
                    {
                        convertedColumn[i] = column[i];
                    }
                    else
                    {
                        convertedColumn[i] = existingRow[columnIndex].Value;
                    }
                }
                else
                {
                    convertedColumn[i] = column[i];
                }
            }

            return convertedColumn;
        }

        /// <summary>
        /// Retrieves the value of a cell in the table specified by the 1-based row and column position.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="oneBasedRowIndex">The 1-based position of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <returns>The value of the cell.</returns>
        public static object GetParameterIndex(this ITableModel tableModel, int oneBasedRowIndex, int oneBasedColumnIndex)
        {
            if (oneBasedRowIndex < 1)
            {
                throw new ArgumentException("Row index must be 1 or higher.", nameof(oneBasedRowIndex));
            }

            if (oneBasedColumnIndex < 1)
            {
                throw new ArgumentException("Column index must be 1 or higher.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedColumnIndex > tableModel.Schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedRowIndex > tableModel.RowCount)
            {
                throw new ArgumentException("Row index exceeds number of rows", nameof(oneBasedRowIndex));
            }

            return tableModel.GetCell(tableModel.GetRowKey(oneBasedRowIndex - 1), tableModel.Schema.FindColumnDefinitionByIdx(oneBasedColumnIndex - 1).Pid).Value;
        }

        /// <summary>
        /// Sets the value of a cell in a table, identified by its 1-based row and column position, with the specified value.
        /// </summary>
        /// <param name="tableModel"></param>
        /// <param name="oneBasedRowIndex">The 1-based position of the row.</param>
        /// <param name="oneBasedColumnIndex">The 1-based position of the column.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="timeInfo">Time stamp.</param>
        /// <returns>Whether the cell value has changed. 'true' indicates change; otherwise, 'false'.</returns>
        public static bool SetParameterIndex(this ITableModel tableModel, int oneBasedRowIndex, int oneBasedColumnIndex, object value, DateTime? timeInfo = null)
        {
            if (oneBasedRowIndex < 1)
            {
                throw new ArgumentException("Row index must be 1 or higher.", nameof(oneBasedRowIndex));
            }

            if (oneBasedColumnIndex < 2)
            {
                throw new ArgumentException("Column index must be 2 or higher.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedColumnIndex > tableModel.Schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException("Column index exceeds number of columns.", nameof(oneBasedColumnIndex));
            }

            if (oneBasedRowIndex > tableModel.RowCount)
            {
                throw new ArgumentException("Row index exceeds number of rows", nameof(oneBasedRowIndex));
            }

            string primaryKey = tableModel.GetRowKey(oneBasedRowIndex - 1);

            var columnDefinition = tableModel.Schema.FindColumnDefinitionByIdx(oneBasedColumnIndex - 1);

            tableModel.SetCell(primaryKey, columnDefinition.Pid, value, timeInfo);

            return true;
        }
    }
}