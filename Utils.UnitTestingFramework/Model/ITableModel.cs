namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Table model interface.
    /// </summary>
    public interface ITableModel
    {
        /// <summary>
        /// Gets the table identifier.
        /// </summary>
        /// <value>
        /// The table identifier.
        /// </value>
        int TableId { get; }

        TableSchema Schema { get; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        int RowCount { get; }

        bool RowExists(string key);

        int GetRowIndex(string key);

        string GetRowKey(int rowIndex);

        IParameterModel GetCell(string primaryKey, int columnPid);

        void SetCell(string primaryKey, int columnPid, object value, DateTime? timestamp = null);

        /// <summary>
        /// Rows the row data of the row with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">No row with key the specified key exists.</exception>
        IParameterModel[] GetRow(string key);

        /// <summary>
        /// Retrieves the row data of the row with the specified row index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="System.ArgumentException">No row with the specified index exists.</exception>
        IParameterModel[] GetRow(int rowIndex);

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns>a array of rows.</returns>
        IDictionary<string, IParameterModel[]> GetAllRows();

        void SetRow(object[] rowData, DateTime? timestamp = null);

        void RemoveRows(params string[] primaryKeys);

        void RemoveAllRows();
    }
}