namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Table
{
    using System;

    public sealed class CellChangedEventArgs : EventArgs
    {
        public CellChangedEventArgs(string primaryKey, ColumnDefinition columnDefinition, object oldValue, object newValue)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            PrimaryKey = primaryKey;
            ColumnDefinition = columnDefinition ?? throw new ArgumentNullException(nameof(columnDefinition));
            OldValue = oldValue;
            NewValue = newValue;
        }

        public string PrimaryKey { get; }

        public ColumnDefinition ColumnDefinition { get; }

        public object OldValue { get; }

        public object NewValue { get; }
    }
}