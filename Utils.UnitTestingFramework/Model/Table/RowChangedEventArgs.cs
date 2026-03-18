namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Table
{
    using System;

    public sealed class RowChangedEventArgs : EventArgs
    {
        public RowChangedEventArgs(string primaryKey)
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