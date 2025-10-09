namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System.Collections.Generic;
    using Skyline.DataMiner.Scripting;

    /// <summary>
    /// Defines methods for retrieving and asserting data from a table.
    /// </summary>
    public interface ITableAsserter
    {
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
        /// Retrieves the column data of the column with the specified pid.
        /// </summary>
        /// <param name="pid">The pid.</param>
        /// <returns>The column data or <see langword="null"/> if there is no column with the specified ID.</returns>
        object[] Column(int pid);

        /// <summary>
        /// Rows the row data of the row with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The row data or <see langword="null"/> if there is no row with the specified primary key.</returns>
        object[] Row(string key);

        /// <summary>
        /// Retrieves the row data of the row with the specified row index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>The row data or <see langword="null"/> if there is no row with the specified index.</returns>
        object[] Row(int rowIndex);

        /// <summary>
        /// Retrieves the row with the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="key">The key.</param>
        /// <returns>The row data or <see langword="null"/> if there is no row with the specified primary key.</returns>
        TRow Row<TRow>(string key) where TRow : QActionTableRow;

        /// <summary>
        /// Retrieves the row at the specified key.
        /// </summary>
        /// <typeparam name="TRow">The type of the row.</typeparam>
        /// <param name="index">The index.</param>
        /// <returns>The row data or <see langword="null"/> if there is no row with the specified index.</returns>
        TRow Row<TRow>(int index) where TRow : QActionTableRow;

        /// <summary>
        /// Gets all rows.
        /// </summary>
        /// <returns>a array of rows.</returns>
        IDictionary<string, object[]> AllRows();
    }
}
