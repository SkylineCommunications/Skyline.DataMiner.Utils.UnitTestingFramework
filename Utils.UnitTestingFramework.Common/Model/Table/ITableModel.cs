namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    /// <summary>
    /// Table model interface.
    /// </summary>
    public interface ITableModel
    {
        /// <summary>
        /// Occurs when a cell value changes.
        /// </summary>
        event EventHandler<CellChangedEventArgs> CellChanged;

        /// <summary>
        /// Occurs when a row is added, removed, or updated.
        /// </summary>
        event EventHandler<RowChangedEventArgs> RowChanged;

        /// <summary>
        /// Occurs when any change is made to the table, including changes to cell values and row additions, removals, or updates.
        /// </summary>
        event EventHandler<TableChangedEventArgs> TableChanged;

        /// <summary>
        /// Gets the table identifier.
        /// </summary>
        int TableId { get; }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        TableSchema Schema { get; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        int RowCount { get; }

        /// <summary>
        /// Gets a value indicating whether the table contains the row with the specified key.
        /// </summary>
        /// <param name="primaryKey">The key for which to check existence.</param>
        /// <returns>A value indicating whether the row exists.</returns>
        bool RowExists(string primaryKey);

        /// <summary>
        /// Gets the row index for the row with the specified key, or -1 if no such row exists.
        /// </summary>
        /// <param name="primaryKey">The key for which to retrieve the row index.</param>
        /// <returns>The row index, or -1 if no such row exists.</returns>
        int GetRowIndex(string primaryKey);

        /// <summary>
        /// Gets the row key for the row with the specified index, or <see langword="null"/> if no such row exists.
        /// </summary>
        /// <param name="rowIndex">The row index.</param>
        /// <returns>The row key, or <see langword="null"/> if no such row exists.</returns>
        string GetRowPrimaryKey(int rowIndex);

        /// <summary>
        /// Gets the cell data for the cell with the specified primary key and column PID, or <see langword="null"/> if no such cell exists.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="columnPid">The column PID.</param>
        /// <returns>The cell data, or <see langword="null"/> if no such cell exists.</returns>
        IParameterValue GetCell(string primaryKey, int columnPid);

        /// <summary>
        /// Sets the cell data for the cell with the specified primary key and column PID.
        /// </summary>
        /// <param name="primaryKey">The primary key.</param>
        /// <param name="columnPid">The column PID.</param>
        /// <param name="value">The cell value.</param>
        /// <param name="timestamp">The timestamp.</param>
        void SetCell(string primaryKey, int columnPid, object value, DateTime? timestamp = null);

        /// <summary>
        /// Rows the row data of the row with the specified key, or <see langword="null"/> if no such row exists.
        /// </summary>
        /// <param name="primaryKey">The key.</param>
        /// <returns>The row data, or <see langword="null"/> if no such row exists.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="primaryKey"/> is <see langword="null"/>.</exception>
        IParameterValue[] GetRow(string primaryKey);

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns>A dictionary of rows, where the key represents the primary key.</returns>
        ReadOnlyDictionary<string, IParameterValue[]> GetAllRows();

        /// <summary>
        /// Sets the row with the specified key, adding a new row if no such row exists.
        /// </summary>
        /// <param name="row">The row to set.</param>
        /// <param name="timestamp">Optional timestamp for the row.</param>
        void SetRow(object[] row, DateTime? timestamp = null);

        /// <summary>
        /// Sets the rows, adding new rows if no such rows exist and updating existing rows if a row with the same primary key already exists.
        /// </summary>
        /// <param name="rows">The rows to set.</param>
        /// <param name="timestamp">Optional timestamp for the row.</param>
        void SetRows(IEnumerable<object[]> rows, DateTime? timestamp = null);

        /// <summary>
        /// Removes rows with the specified primary keys, ignoring any keys for which no such row exists.
        /// </summary>
        /// <param name="primaryKeys"></param>
        void RemoveRows(params string[] primaryKeys);

        /// <summary>
        /// Removes all rows from the table.
        /// </summary>
        void RemoveAllRows();

        /// <summary>
        /// No longer raises any notifications until the returned <see cref="IDisposable"/> is disposed.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, resumes notifications.</returns>
        IDisposable SuspendNotifications();
    }
}