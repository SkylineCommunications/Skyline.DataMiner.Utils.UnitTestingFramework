namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// Table model builder.
    /// </summary>
    public class TableModelBuilder
    {
        private readonly int tableId;
        private readonly List<ColumnDefinition> columns = new List<ColumnDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableModelBuilder"/> class.
        /// </summary>
        /// <param name="tableId">The table identifier.</param>
        public TableModelBuilder(int tableId)
        {
            this.tableId = tableId;
        }

        private ColumnDefinition KeyColumn { get; set; }

        /// <summary>
        /// Returns an empty table model.
        /// </summary>
        /// <param name="tableId">The table ID.</param>
        /// <returns>An empty table model.</returns>
        public static ITableModel EmptyTableModel(int tableId)
        {
            return new TableModel(tableId, Enumerable.Empty<ColumnDefinition>(), primaryKeyColumn: null);
        }

        /// <summary>
        /// Adds a column with the specified column ID and index.
        /// </summary>
        /// <param name="column">The column to add.</param>
        /// <param name="isKey">if set to <c>true</c> [is key].</param>
        /// <exception cref="InvalidOperationException">Another column has already been added as primary key column.</exception>
        public void AddColumn(ColumnDefinition column, bool isKey = false)
        {
            if (column is null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            columns.Add(column);

            if (isKey)
            {
                if (KeyColumn == null)
                {
                    KeyColumn = column;
                }
                else
                {
                    throw new InvalidOperationException($"Column {KeyColumn} is already the primary key column.");
                }
            }
        }

        /// <summary>
        /// Builds this instance.
        /// </summary>
        /// <returns>Table model instance.</returns>
        /// <exception cref="InvalidOperationException">A primary key column must be added.</exception>
        public ITableModel Build()
        {
            if (KeyColumn == null)
            {
                throw new InvalidOperationException("No primary key column defined.");
            }

           return new TableModel(tableId, columns, KeyColumn);
        }
    }
}