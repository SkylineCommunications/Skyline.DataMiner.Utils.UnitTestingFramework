namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class DmsTableMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Id_ReturnsTableId()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.AreEqual(900, table.Id);
        }

        [TestMethod]
        public void Element_ReturnsOwningElement()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.AreSame(mock.Object, table.Element);
        }

        [TestMethod]
        public void AddRow_RowExistsAndPrimaryKeysAreReturned()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
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
        public void GetRow_ReturnsRowData()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var row = table.GetRow("two");

            // Assert
            CollectionAssert.AreEqual(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 }, row);
        }

        [TestMethod]
        public void GetRows_ReturnsAllRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var rows = table.GetRows();

            // Assert
            Assert.AreEqual(2, rows.Length);
        }

        [TestMethod]
        public void GetData_ReturnsDataKeyedBySpecifiedColumn()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var data = table.GetData(1);

            // Assert
            CollectionAssert.AreEquivalent(new[] { "one-desc", "two-desc" }, data.Keys.ToArray());
        }

        [TestMethod]
        public void GetData_InvalidKeyColumnIndex_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetData(999));
        }

        [TestMethod]
        public void GetColumn_ReturnsColumnWithMatchingId()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            // Act
            var column = table.GetColumn<string>(902);

            // Assert
            Assert.IsNotNull(column);
            Assert.AreEqual(902, column.Id);
        }

        [TestMethod]
        public void GetColumn_UnsupportedType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => table.GetColumn<double>(903));
        }

        [TestMethod]
        public void SetRow_UpdatesRow()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.SetRow("one", new object[] { "one", "set-row-desc", 9.0, 10.0, 11.0 });

            // Assert
            Assert.AreEqual("set-row-desc", table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey));
        }

        [TestMethod]
        public void SetRow_UsesPrimaryKeyArgumentInsteadOfKeyInData()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act: the key embedded in the data differs from the primary key argument.
            table.SetRow("one", new object[] { "other-key", "authoritative-desc", 9.0, 10.0, 11.0 });

            // Assert: the row for "one" is updated (the argument key is authoritative).
            Assert.IsTrue(table.RowExists("one"));
            Assert.IsFalse(table.RowExists("other-key"));
            Assert.AreEqual("authoritative-desc", table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey));
        }

        [TestMethod]
        public void DeleteRow_RemovesRow()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.DeleteRow("one");

            // Assert
            Assert.IsFalse(table.RowExists("one"));
        }

        [TestMethod]
        public void DeleteRows_RemovesMultipleRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });
            table.AddRow(new object[] { "three", "three-desc", 9.0, 10.0, 11.0 });

            // Act
            table.DeleteRows(new[] { "one", "three" });

            // Assert
            Assert.IsFalse(table.RowExists("one"));
            Assert.IsTrue(table.RowExists("two"));
            Assert.IsFalse(table.RowExists("three"));
        }

        [TestMethod]
        public void QueryData_NoFilters_ReturnsAllRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var rows = table.QueryData(Enumerable.Empty<IColumnFilter>()).ToList();

            // Assert
            Assert.AreEqual(2, rows.Count);
        }

        [TestMethod]
        public void QueryData_EqualFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
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
        public void QueryData_GreaterThanFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
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
        public void QueryData_ReturnFilter_LimitsReturnedColumns()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
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
        public void QueryData_FilterAndReturnFilterCombined_ReturnsSelectedColumnsOfMatchingRows()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
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

        [TestMethod]
        public void StartValueMonitor_InvokesCallbackOnRowAdded()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, change => received = change, false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNotNull(received);
            Assert.IsTrue(received.UpdatedRows.ContainsKey("one"));
            Assert.AreEqual(0, received.PrimaryKeyColumnIdx);
        }

        [TestMethod]
        public void StartValueMonitor_InvokesCallbackOnRowDeleted()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, change => received = change, false);

            // Act
            table.DeleteRow("one");

            // Assert
            Assert.IsNotNull(received);
            CollectionAssert.Contains(received.DeletedRows, "one");
        }

        [TestMethod]
        public void StartValueMonitor_WithTimeSpanOverload_InvokesCallbackOnRowAdded()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, change => received = change, TimeSpan.FromSeconds(1), false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNotNull(received);
            Assert.IsTrue(received.UpdatedRows.ContainsKey("one"));
        }

        [TestMethod]
        public void StartValueMonitor_WithColumnIndexesOverload_InvokesCallbackOnRowAdded()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, new[] { 1 }, change => received = change, false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNotNull(received);
            Assert.IsTrue(received.UpdatedRows.ContainsKey("one"));
        }

        [TestMethod]
        public void StartValueMonitor_WithColumnIndexesAndTimeSpanOverload_InvokesCallbackOnRowAdded()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, new[] { 1 }, change => received = change, TimeSpan.FromSeconds(1), false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNotNull(received);
            Assert.IsTrue(received.UpdatedRows.ContainsKey("one"));
        }

        [TestMethod]
        public void StopValueMonitor_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, change => received = change, false);
            table.StopValueMonitor("source", false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void StopValueMonitor_WithTimeSpanOverload_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var table = mock.Object.GetTable(900);

            TableValueChange received = null;
            table.StartValueMonitor("source", 0, change => received = change, false);
            table.StopValueMonitor("source", TimeSpan.FromSeconds(1), false);

            // Act
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert
            Assert.IsNull(received);
        }
        [TestMethod]
        public void SetRow_OnTableMock_AddsRowThatCanBeRead()
        {
            // Arrange
            var elementMock = new IDmsMock()
                .CreateAgent(agentId: 0)
                .CreateElement(path);

            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);
            var row = new object[] { "one", "one-desc", 3.0, 4.0, 5.0 };

            // Act
            tableMock.SetRow(row);

            // Assert
            Assert.AreEqual(1, tableMock.RowCount);
            Assert.IsTrue(tableMock.RowExists("one"));
            CollectionAssert.AreEqual(row, tableMock.GetRow("one"));
        }
        [TestMethod]
        public void GetRows_OnTableMock_ReturnsOnlyRequestedRows()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);

            var firstRow = new object[] { "one", "one-desc", 3.0, 4.0, 5.0 };
            var secondRow = new object[] { "two", "two-desc", 6.0, 7.0, 8.0 };
            tableMock.SetRows(new[] { firstRow, secondRow });

            // Act
            var rows = tableMock.GetRows(new[] { "two" });

            // Assert
            Assert.AreEqual(1, rows.Count);
            CollectionAssert.AreEqual(secondRow, rows["two"]);
        }

        [TestMethod]
        public void GetAllRows_OnTableMock_ReturnsEveryRow()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);

            tableMock.SetRows(new[]
            {
                new object[] { "one", "one-desc", 3.0, 4.0, 5.0 },
                new object[] { "two", "two-desc", 6.0, 7.0, 8.0 }
            });

            // Act
            var rows = tableMock.GetAllRows();
            var allRowsProperty = tableMock.AllRows;

            // Assert
            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.ContainsKey("one"));
            Assert.IsTrue(rows.ContainsKey("two"));
            Assert.AreEqual(2, allRowsProperty.Count);
        }

        [TestMethod]
        public void SetRows_OnTableMock_AddsAndUpdatesRows()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);
            tableMock.SetRow(new object[] { "one", "old-desc", 3.0, 4.0, 5.0 });

            var updatedRow = new object[] { "one", "new-desc", 3.0, 4.0, 5.0 };
            var newRow = new object[] { "two", "two-desc", 6.0, 7.0, 8.0 };

            // Act
            tableMock.SetRows(new[] { updatedRow, newRow });

            // Assert
            Assert.AreEqual(2, tableMock.RowCount);
            CollectionAssert.AreEqual(updatedRow, tableMock.GetRow("one"));
            CollectionAssert.AreEqual(newRow, tableMock.GetRow("two"));
        }

        [TestMethod]
        public void RemoveRows_OnTableMock_RemovesOnlySpecifiedRow()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);

            tableMock.SetRows(new[]
            {
                new object[] { "one", "one-desc", 3.0, 4.0, 5.0 },
                new object[] { "two", "two-desc", 6.0, 7.0, 8.0 }
            });

            // Act
            tableMock.RemoveRows("one");

            // Assert
            Assert.AreEqual(1, tableMock.RowCount);
            Assert.IsFalse(tableMock.RowExists("one"));
            Assert.IsTrue(tableMock.RowExists("two"));
        }

        [TestMethod]
        public void RemoveAllRows_OnTableMock_ClearsTable()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);
            tableMock.SetRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            tableMock.RemoveAllRows();

            // Assert
            Assert.AreEqual(0, tableMock.RowCount);
            Assert.AreEqual(0, tableMock.GetAllRows().Count);
        }

        [TestMethod]
        public void SetCell_OnTableMock_UpdatesOneCell()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);
            tableMock.SetRow(new object[] { "one", "old-desc", 3.0, 4.0, 5.0 });

            // Act
            tableMock.SetCell("one", 902, "new-desc");

            // Assert
            Assert.AreEqual("new-desc", tableMock.GetCell("one", 902));
            Assert.AreEqual("new-desc", tableMock.GetRow("one")[1]);
        }

        [TestMethod]
        public void SetCells_OnTableMock_UpdatesSameColumnInMultipleRows()
        {
            // Arrange
            var elementMock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            elementMock.Object.GetTable(900);
            var tableMock = elementMock.GetDmsTableMock(900);

            tableMock.SetRows(new[]
            {
                new object[] { "one", "old-one", 3.0, 4.0, 5.0 },
                new object[] { "two", "old-two", 6.0, 7.0, 8.0 }
            });

            var newValues = new Dictionary<string, object>
            {
                { "one", "new-one" },
                { "two", "new-two" }
            };

            // Act
            tableMock.SetCells(newValues, 902);

            // Assert
            Assert.AreEqual("new-one", tableMock.GetCell("one", 902));
            Assert.AreEqual("new-two", tableMock.GetCell("two", 902));
        }
    }
}
