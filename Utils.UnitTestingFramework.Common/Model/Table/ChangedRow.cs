namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    internal sealed class ChangedRow
    {
        public ChangedRow(string primaryKey, object[] row, RowChangeType changeType)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            PrimaryKey = primaryKey;
            Row = row ?? throw new ArgumentNullException(nameof(row));
            ChangeType = changeType;
        }

        public string PrimaryKey { get; }

        public object[] Row { get; }

        public RowChangeType ChangeType { get; }
    }
}