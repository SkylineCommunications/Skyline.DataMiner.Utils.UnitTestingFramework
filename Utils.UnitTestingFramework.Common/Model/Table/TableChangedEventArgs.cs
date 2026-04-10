namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class TableChangedEventArgs : EventArgs
    {
        public TableChangedEventArgs(IEnumerable<ChangedRow> changedRows)
        {
            ChangedRows = changedRows.ToList() ?? throw new ArgumentNullException(nameof(changedRows));
        }

        public IReadOnlyCollection<ChangedRow> ChangedRows { get; }
    }
}