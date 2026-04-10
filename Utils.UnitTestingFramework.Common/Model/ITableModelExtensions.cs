namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    internal static class ITableModelExtensions
    {
        internal static ParameterValue ToParameterValue(this ITableModel table)
        {
            var rowList = table.GetAllRows().Values.ToList();

            var columnCount = rowList.Count > 0 ? rowList.Max(x => x.Length) : 0;

            if (columnCount == 0)
            {
                // Create at least one column that represents the keys.
                columnCount = 1;
            }

            var columns = new object[columnCount];

            for (int c = 0; c < columnCount; c++)
            {
                var columnValues = new object[rowList.Count];

                for (int r = 0; r < rowList.Count; r++)
                {
                    var row = rowList[r];

                    var cellData = new object[7];
                    cellData[0] = c < row.Length ? row[c] : null;

                    columnValues[r] = cellData;
                }

                columns[c] = columnValues;
            }

            return ParameterValue.Compose(columns);
        }
    }
}
