
namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal class TableAsserter : ITableAsserter
    {
        private readonly ITableModel tableModel;

        public TableAsserter(ITableModel tableModel)
        {
            this.tableModel = tableModel;
        }

        public int ColumnCount => tableModel?.Schema?.ColumnDefinitions?.Count ?? 0;

        public int RowCount => tableModel?.RowCount ?? 0;

        public IDictionary<string, object[]> AllRows()
        {
            return tableModel?.GetAllRows()?.ToDictionary(x => x.Key, x => x.Value.Select(cell => cell.Value).ToArray());
        }

        public object[] Column(int pid)
        {
            try
            {
                return tableModel?.GetColumnByPid(pid);
            }
            catch
            {
                return null;
            }
        }

        public object[] Row(string key)
        {
            try
            {
                return tableModel?.GetRow(key);
            }
            catch
            {
                return null;
            }
        }

        public object[] Row(int rowIndex)
        {
            try
            {
                return tableModel?.GetRow(rowIndex);
            }
            catch
            {
                return null;
            }
        }

        public TRow Row<TRow>(string key) where TRow : QActionTableRow
        {
            try
            {
                return tableModel?.GetRow<TRow>(key);
            }
            catch
            {
                return null;
            }
        }

        public TRow Row<TRow>(int index) where TRow : QActionTableRow
        {
            try
            {
                return tableModel?.GetRow<TRow>(index);
            }
            catch
            {
                return null;
            }
        }
    }
}
