namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    /// <summary>
    /// Table model interface.
    /// </summary>
    /// <seealso cref="ITableModelReader" />
    public interface ITableModel : ITableModelReader
    {
        /// <summary>
        /// Sets the column with the specified index.
        /// </summary>
        /// <param name="idx">The index.</param>
        /// <param name="keys">The keys.</param>
        /// <param name="values">The values.</param>
        /// <param name="timeInfo">The time information.</param>
        /// <exception cref="InvalidOperationException">Table cache not initialized.</exception>
        /// <exception cref="ArgumentException">Invalid pid.</exception>
        void SetColumn(int idx, string[] keys, object[] values, DateTime? timeInfo = null);

        /// <summary>
        /// Sets the specified row.
        /// </summary>
        /// <param name="rowData">The row data.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The changes.</returns>
        /// <exception cref="InvalidOperationException">Table cache not initialized.</exception>
        object SetRow(object[] rowData, DateTime? timestamp = null);

        /// <summary>
        /// Sets the existing row.
        /// </summary>
        /// <param name="rowData">The row data.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>The changes.</returns>
        /// <exception cref="InvalidOperationException">Table cache not initialized.</exception>
        object SetExistingRow(object[] rowData, int rowIndex, DateTime? timestamp = null);

        /// <summary>
        /// Removes the row with the specified index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <exception cref="InvalidOperationException">Table cache not initialized.</exception>
        void RemoveRow(int rowIndex);
    }
}