namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RowBuilder
    {
        private readonly TableDefinition tableDefinition;
        private readonly object[] row;

        internal RowBuilder(TableDefinition tableDefinition)
        {
            this.tableDefinition = tableDefinition ?? throw new ArgumentNullException(nameof(tableDefinition));

            row = new object[tableDefinition.ColumnCount];
        }

        public RowBuilder SetPrimaryKey(string primaryKey)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentException($"'{nameof(primaryKey)}' cannot be null or whitespace.", nameof(primaryKey));
            }

            var primaryKeyColumn = tableDefinition.PrimaryKeyColumn ?? throw new InvalidOperationException("Table definition does not have a primary key column.");

            return SetValue(primaryKeyColumn, primaryKey);
        }

        public RowBuilder SetValueByName(string columnName, object value)
        {
            var columnDefinition = tableDefinition.FindColumnDefinitionByName(columnName) ?? throw new ArgumentException($"Column with name {columnName} not found.", nameof(columnName));

            return SetValue(columnDefinition, value);
        }

        public RowBuilder SetValueByName(IEnumerable<string> columnNames, IEnumerable<object> values)
        {
            if (columnNames == null)
            {
                throw new ArgumentNullException(nameof(columnNames));
            }

            return SetValues(
                columnNames,
                values,
                columnName => tableDefinition.FindColumnDefinitionByName(columnName) ?? throw new ArgumentException($"Column with name {columnName} not found.", nameof(columnNames)));
        }

        public RowBuilder SetValueByPid(int columnPid, object value)
        {
            var columnDefinition = tableDefinition.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException($"Column with PID {columnPid} not found.", nameof(columnPid));

            return SetValue(columnDefinition, value);
        }

        public RowBuilder SetValueByPid(IEnumerable<int> columnPids, IEnumerable<object> values)
        {
            if (columnPids == null)
            {
                throw new ArgumentNullException(nameof(columnPids));
            }

            return SetValues(
                columnPids,
                values,
                columnPid => tableDefinition.FindColumnDefinitionByPid(columnPid) ?? throw new ArgumentException($"Column with PID {columnPid} not found.", nameof(columnPids)));
        }

        public RowBuilder SetValueByIdx(int columnIdx, object value)
        {
            var columnDefinition = tableDefinition.FindColumnDefinitionByIdx(columnIdx) ?? throw new ArgumentException($"Column with index {columnIdx} not found.", nameof(columnIdx));

            return SetValue(columnDefinition, value);
        }

        public RowBuilder SetValueByIdx(IEnumerable<int> columnIdxs, IEnumerable<object> values)
        {
            if (columnIdxs == null)
            {
                throw new ArgumentNullException(nameof(columnIdxs));
            }

            return SetValues(
                columnIdxs,
                values,
                columnIdx => tableDefinition.FindColumnDefinitionByIdx(columnIdx) ?? throw new ArgumentException($"Column with index {columnIdx} not found.", nameof(columnIdxs)));
        }

        public object[] Build()
        {
            foreach (var columnDefinition in tableDefinition.ColumnDefinitions)
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

        private RowBuilder SetValues<TIdentifier>(
            IEnumerable<TIdentifier> identifiers,
            IEnumerable<object> values,
            Func<TIdentifier, ColumnDefinition> findColumn)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            var identifierList = identifiers.ToList();
            var valueList = values.ToList();

            if (identifierList.Count != valueList.Count)
            {
                throw new ArgumentException("The number of column identifiers must match the number of values.", nameof(values));
            }

            var columns = identifierList.Select(findColumn).ToList();

            for (int index = 0; index < columns.Count; index++)
            {
                columns[index].Validate(valueList[index]);
            }

            for (int index = 0; index < columns.Count; index++)
            {
                row[columns[index].Idx] = valueList[index];
            }

            return this;
        }
    }
}
