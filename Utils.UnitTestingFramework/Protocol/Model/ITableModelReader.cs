namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System.Collections.Generic;

    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// Table model reader interface.
    /// </summary>
    public interface ITableModelReader
    {
        /// <summary>
        /// Gets the table identifier.
        /// </summary>
        /// <value>
        /// The table identifier.
        /// </value>
        int TableId { get; }

        /// <summary>
        /// Gets or sets the index of the key column.
        /// </summary>
        /// <value>
        /// The index of the key column.
        /// </value>
        int PrimaryKeyColumnIdx { get; }

        /// <summary>
        /// Gets the column count.
        /// </summary>
        /// <value>
        /// The column count.
        /// </value>
        int ColumnCount { get; }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        int RowCount { get; }  

        /// <summary>
        /// Gets the primary key to row index dictionary.
        /// </summary>
        /// <value>
        /// The primary key to row index dictionary.
        /// </value>
        IReadOnlyDictionary<string, int> KeyToRowIndex { get; }

        /// <summary>
        /// Gets the column index to pid mapping.
        /// </summary>
        /// <value>
        /// The column index to pid mapping.
        /// </value>
        IReadOnlyDictionary<int, int> ColumnIndexesToPids { get; }

        /// <summary>
        /// Gets the number of items present in the specified column.
        /// </summary>
        /// <param name="pid">The column pid.</param>
        /// <returns>The number of items present in the specified column.</returns>
        int GetColumnItemCount(int pid);

        /// <summary>
        /// Retrieves the column data of the column with the specified pid.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns>The column data or <see langword="null"/> if there is no column with the specified ID.</returns>
        object[] Column(int pid);

        /// <summary>
        /// Rows the row data of the row with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
        /// <exception cref="System.ArgumentException">No row with key the specified key exists.</exception>
        object[] Row(string key);

        /// <summary>
        /// Retrieves the row data of the row with the specified row index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data.</returns>
        /// <exception cref="System.ArgumentException">No row with the specified index exists.</exception>
        object[] Row(int rowIndex);

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns>a array of rows.</returns>
        object[][] AllRows();

        /// <summary>
        /// Retrieves the row with the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The row.</returns>
        TRow Row<TRow>(string key) where TRow : QActionTableRow;

        /// <summary>
        /// Retrieves the row at the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>The row.</returns>
        TRow Row<TRow>(int index) where TRow : QActionTableRow;
    }
}