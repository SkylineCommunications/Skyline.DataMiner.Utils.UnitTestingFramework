namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TableSchema
    {
        internal TableSchema(IEnumerable<ColumnDefinition> columns, ColumnDefinition primaryKeyColumn)
        {
            ColumnDefinitions = columns?.ToList() ?? throw new ArgumentNullException(nameof(columns));
            PrimaryKeyColumn = primaryKeyColumn ?? throw new ArgumentNullException(nameof(primaryKeyColumn));
        }

        /// <summary>
        /// Gets the primary key column definition.
        /// </summary>
        public ColumnDefinition PrimaryKeyColumn { get; }

        public IReadOnlyList<ColumnDefinition> ColumnDefinitions { get; }

        public int ColumnCount => ColumnDefinitions.Count;

        public ColumnDefinition FindColumnDefinitionByName(string name)
        {
            return ColumnDefinitions.FirstOrDefault(columnDefinition => columnDefinition.Name == name);
        }

        public ColumnDefinition FindColumnDefinitionByIdx(int idx)
        {
            return ColumnDefinitions.FirstOrDefault(columnDefinition => columnDefinition.Idx == idx);
        }

        public ColumnDefinition FindColumnDefinitionByPid(int pid)
        {
            return ColumnDefinitions.FirstOrDefault(columnDefinition => columnDefinition.Pid == pid);
        }

        public RowBuilder CreateRowBuilder()
        {
            return new RowBuilder(this);
        }
    }
}