namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// Table model.
    /// </summary>
    /// <seealso cref="ITableModel" />
    public class TableModel : ITableModel
    {
        private readonly Dictionary<string, int> keyToRowIndex = new Dictionary<string, int>();
        private readonly ReaderWriterLockSlim @lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        // List to keep track of row indexes. Each Dictionary represents a row, mapping column names to cell values.
        private readonly List<IParameterModel[]> rows = new List<IParameterModel[]>();

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
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public bool RowExists(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            using (@lock.Read())
            {
                return keyToRowIndex.ContainsKey(key);
            }
        }

        /// <inheritdoc/>
        public int GetRowIndex(string key)
        {
            using (@lock.Read())
            {
                if (!keyToRowIndex.TryGetValue(key, out int rowIndex))
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
        public string GetRowKey(int rowIndex)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex), rowIndex, $"'{nameof(rowIndex)}' cannot be negative");
            }

            using (@lock.Read())
            {
                if (rowIndex >= rows.Count)
                {
                    // Return null if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return null;
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

            var column = Schema.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException(nameof(columnPid), $"A column with PID '{columnPid}' does not exist.");

            using (@lock.Write())
            {
                column.Validate(value);

                var row = GetRow(primaryKey) ?? throw new ArgumentException($"No row with primary key '{primaryKey}' exists.", nameof(primaryKey));

                var cell = row[column.Idx];

                cell.Update(value, timestamp);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        public IParameterModel[] GetRow(string key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            using(@lock.Read())
            {
                if (!keyToRowIndex.TryGetValue(key, out int rowIndex))
                {
                    // Return null if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return null;
                }

                return GetRow(rowIndex);
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">No column with the specified PID exists.</exception>"
        /// <exception cref="ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/> or whitespace.</exception>
        public IParameterModel GetCell(string primaryKey, int columnPid)
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
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="rowIndex"/> is negative.</exception>
        public IParameterModel[] GetRow(int rowIndex)
        {
            if (rowIndex < 0)
            {
                throw new ArgumentException("Row index cannot be negative.", nameof(rowIndex));
            }

            using (@lock.Read())
            {
                if (rowIndex >= rows.Count)
                {
                    // Return null if the row does not exist.
                    // In order to support multithreading, we cannot throw an exception here, because the row might be removed by another thread right after RowExists and before this method.
                    return null;
                }

                return rows[rowIndex];
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException"><paramref name="rowData"/> is <see langword="null"/>.</exception>
        public void SetRow(object[] rowData, DateTime? timestamp = null)
        {
            if (rowData is null)
            {
                throw new ArgumentNullException(nameof(rowData));
            }

            if (rowData.Length != Schema.ColumnDefinitions.Count)
            {
                throw new ArgumentException($"Argument must contain exactly {Schema.ColumnDefinitions.Count} values, one for each column.", nameof(rowData));
            }

            using (@lock.Write())
            {
                string primaryKey = Convert.ToString(rowData[Schema.PrimaryKeyColumn.Idx]);

                var existingRow = GetRow(primaryKey);

                if (existingRow == null)
                {
                    AddNewRow(rowData, timestamp);
                }
                else
                {
                    UpdateExistingRow(rowData, timestamp, existingRow);
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

            foreach (var primaryKey in primaryKeys)
            {
                RemoveRow(primaryKey);
            }
        }

        /// <inheritdoc/>
        public void RemoveAllRows()
        {
            using (@lock.Write())
            {
                rows.Clear();
                keyToRowIndex.Clear();
            }
        }

        /// <inheritdoc/>
        public IDictionary<string, IParameterModel[]> GetAllRows()
        {
            using (@lock.Read())
            {
                return rows.ToDictionary(row => (string)row[Schema.PrimaryKeyColumn.Idx].Value, row => row);
            }
        }

        private void RemoveRow(string primaryKey)
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
            }
        }

        private void UpdateExistingRow(object[] rowData, DateTime? timestamp, IParameterModel[] existingRow)
        {
            foreach (var columnDefinition in Schema.ColumnDefinitions)
            {
                columnDefinition.Validate(rowData[columnDefinition.Idx]);

                existingRow[columnDefinition.Idx].Update(rowData[columnDefinition.Idx], timestamp);
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

                rowToAdd[columnDefinition.Idx] = new ParameterModel(valueToAdd, timestamp);
            }

            rows.Add(rowToAdd);
            keyToRowIndex[primaryKey] = rows.Count - 1;
        }
    }
}