namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Model
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    [TestClass]
    public class TableModelTests
    {
        #region Properties Tests

        [TestMethod]
        public void TableId_ReturnsCorrectValue()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            // Act
            var tableModel = tableModelBuilder.Build();

            // Assert
            Assert.AreEqual(900, tableModel.TableId);
        }

        [TestMethod]
        public void Schema_ReturnsNonNull()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            // Act
            var tableModel = tableModelBuilder.Build();

            // Assert
            Assert.IsNotNull(tableModel.Schema);
        }

        [TestMethod]
        public void RowCount_EmptyTable_ReturnsZero()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            int rowCount = tableModel.RowCount;

            // Assert
            Assert.AreEqual(0, rowCount);
        }

        [TestMethod]
        public void RowCount_AfterAddingRows_ReturnsCorrectCount()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };

            // Act
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Assert
            Assert.AreEqual(3, tableModel.RowCount);
        }

        #endregion

        #region SetRow Tests

        [TestMethod]
        public void SetRow_ValidRowWithKey()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            object[] row = { "skyline1", "value2" };

            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row);

            // Assert
            var rowOutput = tableModel.GetRow("skyline1");

            Assert.AreEqual("skyline1", rowOutput[0]);
            Assert.AreEqual("value2", rowOutput[1]);
        }

        [TestMethod]
        public void SetRow_ValidRowWithIndex()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);

            object[] row0 = { "skyline2", "value2" };
            object[] row1 = { "skyline3", "value3" };

            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row0);
            tableModel.SetRow(row1);

            // Assert
            var row0Output = tableModel.GetRow(tableModel.GetRowPrimaryKey(0));
            var row1Output = tableModel.GetRow(tableModel.GetRowPrimaryKey(1));

            Assert.AreEqual("skyline2", row0Output[0]);
            Assert.AreEqual("value2", row0Output[1]);
            Assert.AreEqual("skyline3", row1Output[0]);
            Assert.AreEqual("value3", row1Output[1]);
        }

        [TestMethod]
        public void SetRow_UpdateExistingRow_UpdatesValues()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] originalRow = { "key1", "originalValue" };
            object[] updatedRow = { "key1", "updatedValue" };

            // Act
            tableModel.SetRow(originalRow);
            tableModel.SetRow(updatedRow);

            // Assert
            var row = tableModel.GetRow("key1");
            Assert.AreEqual("key1", row[0]);
            Assert.AreEqual("updatedValue", row[1]);
            Assert.AreEqual(1, tableModel.RowCount);
        }

        [TestMethod]
        public void SetRow_WithTimestamp_SetsTimestamp()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            var timestamp = new DateTime(2025, 1, 1, 12, 0, 0);

            // Act
            tableModel.SetRow(row, timestamp);

            // Assert
            var rowOutput1 = tableModel.GetLastWriteTimestamp("key1", 1201);
            var rowOutput2 = tableModel.GetLastWriteTimestamp("key1", 1202);
            Assert.AreEqual(timestamp, rowOutput1);
            Assert.AreEqual(timestamp, rowOutput2);
        }

        [TestMethod]
        public void SetRow_NullRowData_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.SetRow(null));
        }

        #endregion

        #region GetRow Tests

        [TestMethod]
        public void GetRow_ByKey_ReturnsCorrectRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var result = tableModel.GetRow("key1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key1", result[0]);
            Assert.AreEqual("value1", result[1]);
        }

        [TestMethod]
        public void GetRow_ByIndex_ReturnsCorrectRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var result = tableModel.GetRow(tableModel.GetRowPrimaryKey(0));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key1", result[0]);
            Assert.AreEqual("value1", result[1]);
        }

        [TestMethod]
        public void GetRow_NonExistentKey_ReturnsNull()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var result = tableModel.GetRow("nonExistentKey");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetRow_NonExistentIndex_ReturnsNull()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var result = tableModel.GetRow(tableModel.GetRowPrimaryKey(5));

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetRow_NullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.GetRow((string)null));
        }

        [TestMethod]
        public void GetRow_NegativeIndex_ThrowsArgumentException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => tableModel.GetRow(tableModel.GetRowPrimaryKey(-1)));
        }

        #endregion

        #region GetAllRows Tests

        [TestMethod]
        public void GetAllRows_EmptyTable_ReturnsEmptyDictionary()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            var allRows = tableModel.GetAllRows();

            // Assert
            Assert.IsNotNull(allRows);
            Assert.AreEqual(0, allRows.Count);
        }

        [TestMethod]
        public void GetAllRows_WithMultipleRows_ReturnsAllRows()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };

            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Act
            var allRows = tableModel.GetAllRows();

            // Assert
            Assert.AreEqual(3, allRows.Count);
            Assert.IsTrue(allRows.ContainsKey("key1"));
            Assert.IsTrue(allRows.ContainsKey("key2"));
            Assert.IsTrue(allRows.ContainsKey("key3"));
            Assert.AreEqual("value1", allRows["key1"][1]);
            Assert.AreEqual("value2", allRows["key2"][1]);
            Assert.AreEqual("value3", allRows["key3"][1]);
        }

        #endregion

        #region GetCell and SetCell Tests

        [TestMethod]
        public void GetCell_ValidKeyAndColumn_ReturnsCell()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var cell = tableModel.GetCell("key1", 1202);

            // Assert
            Assert.IsNotNull(cell);
            Assert.AreEqual("value1", cell);
        }

        [TestMethod]
        public void GetCell_NonExistentKey_ReturnsNull()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            var cell = tableModel.GetCell("nonExistentKey", 1202);

            // Assert
            Assert.IsNull(cell);
        }

        [TestMethod]
        public void GetCell_NullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.GetCell(null, 1202));
        }

        [TestMethod]
        public void GetCell_InvalidColumnPid_ThrowsArgumentException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.GetCell("key1", 9999));
        }

        [TestMethod]
        public void SetCell_ValidKeyAndColumn_UpdatesCell()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "originalValue" };
            tableModel.SetRow(row);

            // Act
            tableModel.SetCell("key1", 1202, "updatedValue");

            // Assert
            var cell = tableModel.GetCell("key1", 1202);
            Assert.AreEqual("updatedValue", cell);
        }

        [TestMethod]
        public void SetCell_WithTimestamp_SetsTimestamp()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            var timestamp = new DateTime(2025, 1, 1, 12, 0, 0);

            // Act
            tableModel.SetCell("key1", 1202, "newValue", timestamp);

            // Assert
            var cellTimestamp = tableModel.GetLastWriteTimestamp("key1", 1202);
            Assert.AreEqual(timestamp, cellTimestamp);
        }

        [TestMethod]
        public void SetCell_NullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.SetCell(null, 1202, "value"));
        }

        [TestMethod]
        public void SetCell_InvalidColumnPid_ThrowsArgumentException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.SetCell("key1", 9999, "value"));
        }

        [TestMethod]
        public void SetCell_NonExistentKey_ThrowsArgumentException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.SetCell("nonExistentKey", 1202, "value"));
        }

        #endregion

        #region RowExists Tests

        [TestMethod]
        public void RowExists_ExistingKey_ReturnsTrue()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            bool exists = tableModel.RowExists("key1");

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void RowExists_NonExistentKey_ReturnsFalse()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            bool exists = tableModel.RowExists("nonExistentKey");

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void RowExists_NullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.RowExists(null));
        }

        [TestMethod]
        public void RowExists_EmptyKey_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.RowExists(string.Empty));
        }

        #endregion

        #region GetRowIndex Tests

        [TestMethod]
        public void GetRowIndex_ExistingKey_ReturnsCorrectIndex()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            // Act
            int index = tableModel.GetRowIndex("key2");

            // Assert
            Assert.AreEqual(1, index);
        }

        [TestMethod]
        public void GetRowIndex_NonExistentKey_ReturnsMinusOne()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            int index = tableModel.GetRowIndex("nonExistentKey");

            // Assert
            Assert.AreEqual(-1, index);
        }

        #endregion

        #region GetRowKey Tests

        [TestMethod]
        public void GetRowKey_ValidIndex_ReturnsKey()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            string key = tableModel.GetRowPrimaryKey(0);

            // Assert
            Assert.AreEqual("key1", key);
        }

        [TestMethod]
        public void GetRowKey_InvalidIndex_ReturnsNull()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            string key = tableModel.GetRowPrimaryKey(5);

            // Assert
            Assert.AreEqual(String.Empty, key);
        }

        [TestMethod]
        public void GetRowKey_NegativeIndex_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => tableModel.GetRowPrimaryKey(-1));
        }

        #endregion

        #region RemoveRows Tests

        [TestMethod]
        public void RemoveRows_SingleRow_RemovesRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            // Act
            tableModel.RemoveRows("key1");

            // Assert
            Assert.AreEqual(1, tableModel.RowCount);
            Assert.IsFalse(tableModel.RowExists("key1"));
            Assert.IsTrue(tableModel.RowExists("key2"));
        }

        [TestMethod]
        public void RemoveRows_MultipleRows_RemovesAllSpecifiedRows()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Act
            tableModel.RemoveRows("key1", "key3");

            // Assert
            Assert.AreEqual(1, tableModel.RowCount);
            Assert.IsFalse(tableModel.RowExists("key1"));
            Assert.IsTrue(tableModel.RowExists("key2"));
            Assert.IsFalse(tableModel.RowExists("key3"));
        }

        [TestMethod]
        public void RemoveRows_UpdatesIndices()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Act
            tableModel.RemoveRows("key1");

            // Assert
            Assert.AreEqual(0, tableModel.GetRowIndex("key2"));
            Assert.AreEqual(1, tableModel.GetRowIndex("key3"));
            Assert.AreEqual("key2", tableModel.GetRowPrimaryKey(0));
            Assert.AreEqual("key3", tableModel.GetRowPrimaryKey(1));
        }

        [TestMethod]
        public void RemoveRows_NonExistentKey_DoesNotThrow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            tableModel.RemoveRows("nonExistentKey");

            // Assert
            Assert.AreEqual(1, tableModel.RowCount);
        }

        [TestMethod]
        public void RemoveRows_NullArray_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.RemoveRows(null));
        }

        #endregion

        #region RemoveAllRows Tests

        [TestMethod]
        public void RemoveAllRows_ClearsTable()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Act
            tableModel.RemoveAllRows();

            // Assert
            Assert.AreEqual(0, tableModel.RowCount);
            Assert.IsFalse(tableModel.RowExists("key1"));
            Assert.IsFalse(tableModel.RowExists("key2"));
            Assert.IsFalse(tableModel.RowExists("key3"));
        }

        [TestMethod]
        public void RemoveAllRows_EmptyTable_DoesNotThrow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.RemoveAllRows();

            // Assert
            Assert.AreEqual(0, tableModel.RowCount);
        }

        #endregion

        #region TableModelBuilder Tests

        [TestMethod]
        public void TryAddingTwoKeyColumns_InvalidOperationException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);

            // Act
            tableModelBuilder.AddColumn(1201, 0, true);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => tableModelBuilder.AddColumn(1202, 1, true));
        }

        #endregion

        #region Event Tests

        [TestMethod]
        public void CellChanged_SetCell_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            CellChangedEventArgs cellChangedEventArgs = null;

            tableModel.CellChanged += (sender, e) =>
            {
               cellChangedEventArgs = e;
            };

            // Act
            tableModel.SetCell("key1", 1202, "newValue");

            // Assert
            Assert.IsNotNull(cellChangedEventArgs);
            Assert.AreEqual("key1", cellChangedEventArgs.PrimaryKey);
            Assert.AreEqual(1202, cellChangedEventArgs.ParameterDefinition.Pid);
            Assert.AreEqual("value1", cellChangedEventArgs.OldValue);
            Assert.AreEqual("newValue", cellChangedEventArgs.NewValue);
        }

        [TestMethod]
        public void CellChanged_SetRow_NewRow_RaisesOneEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            int cellChangeEventCount = 0;

            tableModel.CellChanged += (sender, e) =>
            {
                cellChangeEventCount++;
            };

            // Act
            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Assert
            Assert.AreEqual(0, cellChangeEventCount); // None for new rows
        }

        [TestMethod]
        public void CellChanged_SetRow_UpdateExistingRow_RaisesEvents()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] originalRow = { "key1", "originalValue" };
            tableModel.SetRow(originalRow);

            CellChangedEventArgs cellChangedEventArgs = null;

            tableModel.CellChanged += (sender, e) =>
            {
                cellChangedEventArgs = e;
            };

            // Act
            object[] updatedRow = { "key1", "updatedValue" };
            tableModel.SetRow(updatedRow);

            // Assert
            Assert.IsNotNull(cellChangedEventArgs);
            Assert.AreEqual("key1", cellChangedEventArgs.PrimaryKey);
            Assert.AreEqual(1202, cellChangedEventArgs.ParameterDefinition.Pid);
            Assert.AreEqual("originalValue", cellChangedEventArgs.OldValue);
            Assert.AreEqual("updatedValue", cellChangedEventArgs.NewValue);
        }

        [TestMethod]
        public void RowChanged_SetRow_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            bool eventRaised = false;
            string eventKey = null;

            tableModel.RowChanged += (sender, e) =>
            {
                eventRaised = true;
                eventKey = e.PrimaryKey;
            };

            // Act
            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Assert
            Assert.IsTrue(eventRaised);
            Assert.AreEqual("key1", eventKey);
        }

        [TestMethod]
        public void RowChanged_SetCell_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool eventRaised = false;
            string eventKey = null;

            tableModel.RowChanged += (sender, e) =>
            {
                eventRaised = true;
                eventKey = e.PrimaryKey;
            };

            // Act
            tableModel.SetCell("key1", 1202, "newValue");

            // Assert
            Assert.IsTrue(eventRaised);
            Assert.AreEqual("key1", eventKey);
        }

        [TestMethod]
        public void RowChanged_RemoveRows_RaisesEventForEachRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            int eventCount = 0;

            tableModel.RowChanged += (sender, key) =>
            {
                eventCount++;
            };

            // Act
            tableModel.RemoveRows("key1", "key2");

            // Assert
            Assert.AreEqual(2, eventCount);
        }

        [TestMethod]
        public void RowChanged_RemoveAllRows_RaisesEventForEachRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            int eventCount = 0;

            tableModel.RowChanged += (sender, key) =>
            {
                eventCount++;
            };

            // Act
            tableModel.RemoveAllRows();

            // Assert
            Assert.AreEqual(3, eventCount);
        }

        [TestMethod]
        public void TableChanged_SetRow_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            bool eventRaised = false;

            tableModel.TableChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act
            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void TableChanged_SetCell_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool eventRaised = false;

            tableModel.TableChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act
            tableModel.SetCell("key1", 1202, "newValue");

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void TableChanged_RemoveRows_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool eventRaised = false;

            tableModel.TableChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act
            tableModel.RemoveRows("key1");

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void TableChanged_RemoveAllRows_RaisesEvent()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            bool eventRaised = false;

            tableModel.TableChanged += (sender, e) =>
            {
                eventRaised = true;
            };

            // Act
            tableModel.RemoveAllRows();

            // Assert
            Assert.IsTrue(eventRaised);
        }

        [TestMethod]
        public void SuspendNotifications_SetCell_DoesNotRaiseEvents()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool cellChangedRaised = false;
            bool rowChangedRaised = false;
            bool tableChangedRaised = false;

            tableModel.CellChanged += (sender, e) => cellChangedRaised = true;
            tableModel.RowChanged += (sender, key) => rowChangedRaised = true;
            tableModel.TableChanged += (sender, e) => tableChangedRaised = true;

            // Act
            using (tableModel.SuspendNotifications())
            {
                tableModel.SetCell("key1", 1202, "newValue");
            }

            // Assert
            Assert.IsFalse(cellChangedRaised);
            Assert.IsFalse(rowChangedRaised);
            Assert.IsFalse(tableChangedRaised);
        }

        [TestMethod]
        public void SuspendNotifications_AfterDispose_RaisesEvents()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool cellChangedRaised = false;
            bool rowChangedRaised = false;
            bool tableChangedRaised = false;

            tableModel.CellChanged += (sender, e) => cellChangedRaised = true;
            tableModel.RowChanged += (sender, key) => rowChangedRaised = true;
            tableModel.TableChanged += (sender, e) => tableChangedRaised = true;

            // Act
            using (tableModel.SuspendNotifications())
            {
                tableModel.SetCell("key1", 1202, "suspendedValue");
            }

            tableModel.SetCell("key1", 1202, "newValue");

            // Assert
            Assert.IsTrue(cellChangedRaised);
            Assert.IsTrue(rowChangedRaised);
            Assert.IsTrue(tableChangedRaised);
        }

        [TestMethod]
        public void SuspendNotifications_MultipleOperations_DoesNotRaiseEvents()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            int cellChangedCount = 0;
            int rowChangedCount = 0;
            int tableChangedCount = 0;

            tableModel.CellChanged += (sender, e) => cellChangedCount++;
            tableModel.RowChanged += (sender, key) => rowChangedCount++;
            tableModel.TableChanged += (sender, e) => tableChangedCount++;

            // Act
            using (tableModel.SuspendNotifications())
            {
                object[] row1 = { "key1", "value1" };
                object[] row2 = { "key2", "value2" };
                tableModel.SetRow(row1);
                tableModel.SetRow(row2);
                tableModel.SetCell("key1", 1202, "newValue");
                tableModel.RemoveRows("key2");
            }

            // Assert
            Assert.AreEqual(0, cellChangedCount);
            Assert.AreEqual(0, rowChangedCount);
            Assert.AreEqual(0, tableChangedCount);
        }

        [TestMethod]
        public void SuspendNotifications_NestedSuspensions_DoesNotRaiseEvents()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            bool cellChangedRaised = false;
            bool rowChangedRaised = false;
            bool tableChangedRaised = false;

            tableModel.CellChanged += (sender, e) => cellChangedRaised = true;
            tableModel.RowChanged += (sender, key) => rowChangedRaised = true;
            tableModel.TableChanged += (sender, e) => tableChangedRaised = true;

            // Act
            using (tableModel.SuspendNotifications())
            {
                using (tableModel.SuspendNotifications())
                {
                    tableModel.SetCell("key1", 1202, "newValue");
                }

                // Still suspended here
                Assert.IsFalse(cellChangedRaised);
            }

            // Assert - events should resume after all suspensions are disposed
            tableModel.SetCell("key1", 1202, "anotherValue");
            Assert.IsTrue(cellChangedRaised);
            Assert.IsTrue(rowChangedRaised);
            Assert.IsTrue(tableChangedRaised);
        }

        #endregion

        #region GetRows Tests

        [TestMethod]
        public void GetRows_ValidKeys_ReturnsCorrectRows()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            object[] row3 = { "key3", "value3" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);
            tableModel.SetRow(row3);

            // Act
            var rows = tableModel.GetRows(new[] { "key1", "key3" });

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.ContainsKey("key1"));
            Assert.IsTrue(rows.ContainsKey("key3"));
            Assert.AreEqual("value1", rows["key1"][1]);
            Assert.AreEqual("value3", rows["key3"][1]);
        }

        [TestMethod]
        public void GetRows_SingleKey_ReturnsOneRow()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var rows = tableModel.GetRows(new[] { "key1" });

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(1, rows.Count);
            Assert.IsTrue(rows.ContainsKey("key1"));
            Assert.AreEqual("value1", rows["key1"][1]);
        }

        [TestMethod]
        public void GetRows_EmptyArray_ReturnsEmptyDictionary()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var rows = tableModel.GetRows(Array.Empty<string>());

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public void GetRows_SomeNonExistentKeys_ReturnsOnlyExistingRows()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            // Act
            var rows = tableModel.GetRows(new[] { "key1", "nonExistent", "key2" });

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.ContainsKey("key1"));
            Assert.IsTrue(rows.ContainsKey("key2"));
            Assert.IsFalse(rows.ContainsKey("nonExistent"));
        }

        [TestMethod]
        public void GetRows_AllNonExistentKeys_ReturnsEmptyDictionary()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row = { "key1", "value1" };
            tableModel.SetRow(row);

            // Act
            var rows = tableModel.GetRows(new[] { "nonExistent1", "nonExistent2" });

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(0, rows.Count);
        }

        [TestMethod]
        public void GetRows_NullArray_ThrowsArgumentNullException()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => tableModel.GetRows(null));
        }

        [TestMethod]
        public void GetRows_DuplicateKeys_ReturnsUniqueRows()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            object[] row1 = { "key1", "value1" };
            object[] row2 = { "key2", "value2" };
            tableModel.SetRow(row1);
            tableModel.SetRow(row2);

            // Act
            var rows = tableModel.GetRows(new[] { "key1", "key1", "key2" });

            // Assert
            Assert.IsNotNull(rows);
            Assert.AreEqual(2, rows.Count);
            Assert.IsTrue(rows.ContainsKey("key1"));
            Assert.IsTrue(rows.ContainsKey("key2"));
        }

        #endregion
    }
}