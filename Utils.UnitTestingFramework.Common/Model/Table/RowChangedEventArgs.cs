namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    internal sealed class RowChangedEventArgs : EventArgs
    {
        public RowChangedEventArgs(string primaryKey, RowChangeType changeType)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            PrimaryKey = primaryKey;
            ChangeType = changeType;
        }

        public string PrimaryKey { get; }

        public RowChangeType ChangeType { get; }
    }

    public enum RowChangeType
    {
        Added,
        Updated,
        Deleted,
    }
}