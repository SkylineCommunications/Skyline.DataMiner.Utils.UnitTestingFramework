namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

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
        /// Adds a column with the specified column ID and index.
        /// </summary>
        /// <param name="column">The column to add.</param>
        /// <param name="isKey">if set to <c>true</c> [is key].</param>
        /// <exception cref="InvalidOperationException">Another column has already been added as primary key column.</exception>
        internal void AddColumn(int columnPid, int columnIdx, bool isKey = false, string columnName = null)
        {
            AddColumn(new ColumnDefinition(columnName ?? "random name", typeof(object), columnPid, columnIdx), isKey);
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

            var tableSchema = new TableSchema(columns, KeyColumn);

            return new TableModel(tableId, tableSchema);
        }
    }
}