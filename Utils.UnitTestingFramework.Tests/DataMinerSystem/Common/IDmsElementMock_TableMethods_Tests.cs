namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMock_TableMethods_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetTable_ExposesId()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.AreEqual(900, table.Id);
        }

        [TestMethod]
        public void GetTable_ExposesElement()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.AreSame(mock.Object, table.Element);
        }

        [TestMethod]
        public void GetTable_AddRow_RowExistsAndPrimaryKeysAreReturned()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Assert
            Assert.IsTrue(table.RowExists("one"));
            Assert.IsTrue(table.RowExists("two"));
            CollectionAssert.AreEquivalent(new[] { "one", "two" }, table.GetPrimaryKeys());
        }

        [TestMethod]
        public void GetColumn_SetAndGetStringValue_ReturnsSetValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.GetColumn<string>(902).SetValue("one", "changed-desc");
            var value = table.GetColumn<string>(902).GetValue("one");

            // Assert
            Assert.AreEqual("changed-desc", value);
        }

        [TestMethod]
        public void GetColumn_GetNumericValue_ReturnsStoredValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            var value = table.GetColumn<double?>(903).GetValue("one");

            // Assert
            Assert.AreEqual(3.0, value);
        }

        [TestMethod]
        public void GetColumn_ExposesIdAndTable()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);

            // Act
            var column = table.GetColumn<string>(902);

            // Assert
            Assert.AreEqual(902, column.Id);
            Assert.AreSame(table, column.Table);
        }

        [TestMethod]
        public void GetTable_GetRow_ReturnsRowData()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var row = table.GetRow("two");

            // Assert
            CollectionAssert.AreEqual(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 }, row);
        }

        [TestMethod]
        public void GetTable_GetRows_ReturnsAllRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var rows = table.GetRows();

            // Assert
            Assert.AreEqual(2, rows.Length);
        }

        [TestMethod]
        public void GetTable_GetData_ReturnsDataKeyedBySpecifiedColumn()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var data = table.GetData(1);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "one-desc", "two-desc" }, data.Keys.ToArray());
        }

        [TestMethod]
        public void GetTable_SetRow_UpdatesRow()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.SetRow("one", new object[] { "one", "set-row-desc", 9.0, 10.0, 11.0 });
            var value = table.GetColumn<string>(902).GetValue("one");

            // Assert
            Assert.AreEqual("set-row-desc", value);
        }

        [TestMethod]
        public void GetTable_DeleteRow_RemovesRow()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.DeleteRow("one");

            // Assert
            Assert.IsFalse(table.RowExists("one"));
        }

        [TestMethod]
        public void GetTable_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetTable(123456));
        }

        [TestMethod]
        public void GetTable_QueryData_NoFilters_ReturnsAllRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var rows = table.QueryData(Enumerable.Empty<IColumnFilter>()).ToList();

            // Assert
            Assert.AreEqual(2, rows.Count);
        }

        [TestMethod]
        public void GetTable_QueryData_EqualFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var filters = new IColumnFilter[]
            {
                new ColumnFilter { Pid = 903, Value = "3", ComparisonOperator = ComparisonOperator.Equal },
            };
            var rows = table.QueryData(filters).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("one", rows[0][0]);
        }

        [TestMethod]
        public void GetTable_QueryData_GreaterThanFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var filters = new IColumnFilter[]
            {
                new ColumnFilter { Pid = 903, Value = "4", ComparisonOperator = ComparisonOperator.GreaterThan },
            };
            var rows = table.QueryData(filters).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("two", rows[0][0]);
        }

        [TestMethod]
        public void GetTable_QueryData_ReturnFilter_LimitsReturnedColumns()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            var filters = new IColumnFilter[]
            {
                new ColumnReturnFilter { Pid = 902 },
            };
            var rows = table.QueryData(filters).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            CollectionAssert.AreEqual(new object[] { "one-desc" }, rows[0]);
        }

        [TestMethod]
        public void GetTable_QueryData_FilterAndReturnFilterCombined_ReturnsSelectedColumnsOfMatchingRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var filters = new IColumnFilter[]
            {
                new ColumnFilter { Pid = 903, Value = "6", ComparisonOperator = ComparisonOperator.Equal },
                new ColumnReturnFilter { Pid = 902 },
            };
            var rows = table.QueryData(filters).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            CollectionAssert.AreEqual(new object[] { "two-desc" }, rows[0]);
        }
    }
}
