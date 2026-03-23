namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    internal sealed class CellChangedEventArgs : ParameterValueChangedEventArgs<ColumnDefinition>
    {
        public CellChangedEventArgs(string primaryKey, ColumnDefinition columnDefinition, object oldValue, object newValue, DateTime oldTimestamp, DateTime newTimestamp)
            : base(columnDefinition, oldValue, newValue, oldTimestamp, newTimestamp)
        {
            if (String.IsNullOrWhiteSpace(primaryKey))
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            PrimaryKey = primaryKey;
        }

        public string PrimaryKey { get; }
    }
}