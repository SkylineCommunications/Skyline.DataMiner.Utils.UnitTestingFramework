namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    internal class RowBuilder
    {
        private readonly TableSchema tableSchema;
        private readonly object[] row;

        public RowBuilder(TableSchema tableSchema)
        {
            this.tableSchema = tableSchema ?? throw new ArgumentNullException(nameof(tableSchema));

            row = new object[tableSchema.ColumnCount];
        }

        public RowBuilder SetPrimaryKey(string primaryKey)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentException($"'{nameof(primaryKey)}' cannot be null or whitespace.", nameof(primaryKey));
            }

            var primaryKeyColumn = tableSchema.PrimaryKeyColumn ?? throw new InvalidOperationException("Table schema does not have a primary key column.");

            return SetValue(primaryKeyColumn, primaryKey);
        }

        public RowBuilder SetValueByName(string columnName, object value)
        {
            var columnDefinition = tableSchema.FindColumnDefinitionByName(columnName) ?? throw new ArgumentException($"Column with name {columnName} not found.", nameof(columnName));

            return SetValue(columnDefinition, value);
        }

        public RowBuilder SetValueByPid(int columnPid, object value)
        {
            var columnDefinition = tableSchema.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException($"Column with PID {columnPid} not found.", nameof(columnPid));

            return SetValue(columnDefinition, value);
        }

        public RowBuilder SetValueByIdx(int columnIdx, object value)
        {
            var columnDefinition = tableSchema.FindColumnDefinitionByIdx(columnIdx) ?? throw new ArgumentException($"Column with index {columnIdx} not found.", nameof(columnIdx));

            return SetValue(columnDefinition, value);
        }

        public object[] Build()
        {
            foreach (var columnDefinition in tableSchema.ColumnDefinitions)
            {
                columnDefinition.Validate(row[columnDefinition.Idx]);
            }

            return row;
        }

        private RowBuilder SetValue(ColumnDefinition columnDefinition, object value)
        {
            columnDefinition.Validate(value);

            row[columnDefinition.Idx] = value;

            return this;
        }
    }
}
