namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Table model builder.
    /// </summary>
    public class TableModelBuilder
    {
        private readonly int tableId;
        private readonly Dictionary<int, int> columnIndexesToPids;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModelBuilder"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        public TableModelBuilder(int tableId)
        {
            this.tableId = tableId;
            columnIndexesToPids = new Dictionary<int, int>();
        }

        /// <summary>
        /// Gets a value indicating whether a key column exists.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a key column exists; otherwise, <c>false</c>.
        /// </value>
        private bool KeyColumnExists { get; set; }

        /// <summary>
        /// Gets or sets the index of the key column.
        /// </summary>
        /// <value>
        /// The index of the key column.
        /// </value>
        private int KeyColumnIdx { get; set; }

        /// <summary>
        /// Returns an empty table model.
        /// </summary>
        /// <param name="tableId">The table ID.</param>
        /// <returns>An empty table model.</returns>
        public static ITableModel EmptyTableModel(int tableId)
        {
            return new TableModel(tableId);
        }

        /// <summary>
        /// Adds a column with the specified column ID and index.
        /// </summary>
        /// <param name="columnPid">The column pid.</param>
        /// <param name="idx">The index.</param>
        /// <param name="isKey">if set to <c>true</c> [is key].</param>
        /// <exception cref="InvalidOperationException">Another column has already been added as primary key column.</exception>
        public void AddColumn(int columnPid, int idx, bool isKey = false)
        {
            columnIndexesToPids[idx] = columnPid;

            if (!isKey)
            {
                return;
            }

            if (KeyColumnExists)
            {
                throw new InvalidOperationException($"Column with pid '{columnIndexesToPids[KeyColumnIdx]}' is already the primary key column.");
            }

            KeyColumnExists = true;
            KeyColumnIdx = idx;
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Table model instance.</returns>
        /// <exception cref="InvalidOperationException">A primary key column must be added.</exception>
        public ITableModel Build()
        {
            if (!KeyColumnExists)
            {
                throw new InvalidOperationException("A primary key column must be added.");
            }

            TableModel tableModel = new TableModel(tableId);

            foreach (var kvp in columnIndexesToPids)
            {
                tableModel.AddColumn(kvp.Value, kvp.Key, KeyColumnIdx == kvp.Key);
            }

            return tableModel;
        }
    }
}