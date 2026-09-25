namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Builds a table definition for a manually configured protocol.
    /// </summary>
    public sealed class TableDefinitionBuilder
    {
        private readonly List<ColumnDefinition> columns = new List<ColumnDefinition>();
        private ColumnDefinition primaryKeyColumn;

        /// <summary>
        /// Adds a column to the table definition.
        /// </summary>
        /// <param name="columnPid">The parameter ID of the column.</param>
        /// <param name="columnIdx">The zero-based column index.</param>
        /// <param name="isPrimaryKey">Whether this column is the primary key.</param>
        /// <param name="columnName">The column name.</param>
        /// <param name="columnType">The column value type.</param>
        /// <param name="allowNull">Whether the column accepts a null value.</param>
        /// <param name="description">The column description.</param>
        /// <returns>This builder.</returns>
        public TableDefinitionBuilder AddColumn(
            int columnPid,
            int columnIdx,
            bool isPrimaryKey = false,
            string columnName = null,
            Type columnType = null,
            bool allowNull = true,
            string description = null)
        {
            var column = new ColumnDefinition(
                columnName ?? $"Column {columnPid}",
                columnType ?? typeof(object),
                columnPid,
                columnIdx,
                allowNull,
                description);

            return AddColumn(column, isPrimaryKey);
        }

        /// <summary>
        /// Adds a column to the table definition.
        /// </summary>
        /// <param name="column">The column definition.</param>
        /// <param name="isPrimaryKey">Whether this column is the primary key.</param>
        /// <returns>This builder.</returns>
        public TableDefinitionBuilder AddColumn(ColumnDefinition column, bool isPrimaryKey = false)
        {
            if (column == null)
            {
                throw new ArgumentNullException(nameof(column));
            }

            if (isPrimaryKey && primaryKeyColumn != null)
            {
                throw new InvalidOperationException($"Column {primaryKeyColumn} is already the primary key column.");
            }

            columns.Add(column);

            if (isPrimaryKey)
            {
                primaryKeyColumn = column;
            }

            return this;
        }

        /// <summary>
        /// Creates the configured table definition.
        /// </summary>
        /// <returns>The table definition.</returns>
        public TableDefinition Build()
        {
            if (primaryKeyColumn == null)
            {
                throw new InvalidOperationException("No primary key column defined.");
            }

            return new TableDefinition(columns, primaryKeyColumn);
        }
    }
}
