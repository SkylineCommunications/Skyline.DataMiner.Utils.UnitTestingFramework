namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class TableSchema
    {
        internal TableSchema(IEnumerable<ColumnDefinition> columns, ColumnDefinition primaryKeyColum)
        {
            ColumnDefinitions = columns?.ToList() ?? throw new ArgumentNullException(nameof(columns));
            PrimaryKeyColumn = primaryKeyColum ?? throw new ArgumentNullException(nameof(primaryKeyColum));
        }

        /// <summary>
        /// Gets the primary key column definition.
        /// </summary>
        public ColumnDefinition PrimaryKeyColumn { get; }

        public IReadOnlyList<ColumnDefinition> ColumnDefinitions { get; }

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
    }
}