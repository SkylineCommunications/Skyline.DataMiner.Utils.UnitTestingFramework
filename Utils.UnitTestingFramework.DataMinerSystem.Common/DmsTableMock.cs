namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Mock of an <see cref="IDmsTable"/> that is backed by an <see cref="ITableModel"/>,
    /// so that rows and cells that are set are stored and can be retrieved again.
    /// </summary>
    internal class DmsTableMock : Mock<IDmsTable>
    {
        private readonly ITableModel tableModel;
        private readonly Dictionary<string, object> columnMocks = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsTableMock"/> class.
        /// </summary>
        /// <param name="tableModel">The table model that holds the data.</param>
        /// <param name="element">The element this table belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        public DmsTableMock(ITableModel tableModel, IDmsElement element)
        {
            this.tableModel = tableModel ?? throw new ArgumentNullException(nameof(tableModel));

            Setup(t => t.Id).Returns(tableModel.TableId);
            Setup(t => t.Element).Returns(element);

            Setup(t => t.RowExists(It.IsAny<string>())).Returns((string key) => this.tableModel.RowExists(key));

            Setup(t => t.GetPrimaryKeys()).Returns(() => this.tableModel.GetAllRows().Keys.ToArray());

            Setup(t => t.GetRow(It.IsAny<string>())).Returns((string key) => this.tableModel.GetRow(key));

            Setup(t => t.GetRows()).Returns(() => this.tableModel.GetAllRows().Values.ToArray());

            Setup(t => t.GetData(It.IsAny<int>())).Returns((int keyColumnIndex) => GetData(keyColumnIndex));

            Setup(t => t.AddRow(It.IsAny<object[]>())).Callback((object[] data) => this.tableModel.SetRow(data));

            Setup(t => t.SetRow(It.IsAny<string>(), It.IsAny<object[]>())).Callback((string key, object[] data) => SetRow(key, data));

            Setup(t => t.DeleteRow(It.IsAny<string>())).Callback((string key) => this.tableModel.RemoveRows(key));

            Setup(t => t.DeleteRows(It.IsAny<IEnumerable<string>>())).Callback((IEnumerable<string> keys) => this.tableModel.RemoveRows(keys.ToArray()));

            Setup(t => t.GetColumn<It.IsAnyType>(It.IsAny<int>()))
                .Returns(new InvocationFunc(invocation =>
                {
                    var columnType = invocation.Method.GetGenericArguments()[0];
                    var columnPid = (int)invocation.Arguments[0];

                    return GetColumnObject(columnType, columnPid);
                }));
        }

        private void SetRow(string primaryKey, object[] data)
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // The primary key argument is authoritative: force the primary key column to the specified key
            // so the correct row is updated regardless of the key embedded in the provided data.
            var row = (object[])data.Clone();

            var primaryKeyColumnIndex = tableModel.Schema.PrimaryKeyColumn.Idx;
            if (primaryKeyColumnIndex < row.Length)
            {
                row[primaryKeyColumnIndex] = primaryKey;
            }

            tableModel.SetRow(row);
        }

        private object GetColumnObject(Type columnType, int columnPid)
        {
            var cacheKey = $"{columnPid}|{columnType.AssemblyQualifiedName}";

            if (!columnMocks.TryGetValue(cacheKey, out var columnMockObject))
            {
                var columnMockType = typeof(DmsColumnMock<>).MakeGenericType(columnType);
                var columnMock = (Mock)Activator.CreateInstance(columnMockType, tableModel, columnPid, Object);

                columnMockObject = columnMock.Object;
                columnMocks.Add(cacheKey, columnMockObject);
            }

            return columnMockObject;
        }

        private IDictionary<string, object[]> GetData(int keyColumnIndex)
        {
            if (keyColumnIndex < 0 || keyColumnIndex >= tableModel.Schema.ColumnCount)
            {
                throw new ArgumentException($"'{keyColumnIndex}' is not a valid key column index.", nameof(keyColumnIndex));
            }

            var data = new Dictionary<string, object[]>();

            foreach (var row in tableModel.GetAllRows().Values)
            {
                var key = System.Convert.ToString(row[keyColumnIndex]);
                data[key] = row;
            }

            return data;
        }
    }
}
