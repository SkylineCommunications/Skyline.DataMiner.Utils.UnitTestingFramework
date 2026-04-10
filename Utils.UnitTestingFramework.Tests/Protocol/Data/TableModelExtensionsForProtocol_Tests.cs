namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    [TestClass]
    public class TableModelExtensionsForProtocol_Tests
    {
        #region Helpers

        /// <summary>
        /// Creates a 3-column table model (PK at idx 0).
        /// Column PIDs: 1001, 1002, 1003.
        /// </summary>
        private static ITableModel CreateThreeColumnTable()
        {
            var builder = new TableModelBuilder(900);
            builder.AddColumn(1001, 0, isKey: true);
            builder.AddColumn(1002, 1, isKey: false);
            builder.AddColumn(1003, 2, isKey: false);
            return builder.Build();
        }

        /// <summary>
        /// Creates a 5-column table model matching the PollingConfigurationQActionRow layout.
        /// Column PIDs: 901, 902, 903, 904, 905.
        /// </summary>
        private static ITableModel CreateFiveColumnTable()
        {
            var builder = new TableModelBuilder(900);
            builder.AddColumn(901, 0, isKey: true);
            builder.AddColumn(902, 1, isKey: false);
            builder.AddColumn(903, 2, isKey: false);
            builder.AddColumn(904, 3, isKey: false);
            builder.AddColumn(905, 4, isKey: false);
            return builder.Build();
        }

        #endregion

        #region SetRowReturnOneBasedIndex(ITableModel, object[])

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_NewRow_ReturnsOneBasedIndex()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] row = { "key1", "val1", "val2" };

            // Act
            int result = table.SetRowReturnOneBasedIndex(row);

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, table.RowCount);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_MultipleRows_ReturnsCorrectIndices()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            int idx1 = table.SetRowReturnOneBasedIndex(new object[] { "key1", "a", "b" });
            int idx2 = table.SetRowReturnOneBasedIndex(new object[] { "key2", "c", "d" });
            int idx3 = table.SetRowReturnOneBasedIndex(new object[] { "key3", "e", "f" });

            // Assert
            Assert.AreEqual(1, idx1);
            Assert.AreEqual(2, idx2);
            Assert.AreEqual(3, idx3);
            Assert.AreEqual(3, table.RowCount);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_DuplicatePK_UpsertsAndReturnsSameIndex()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            int idx1 = table.SetRowReturnOneBasedIndex(new object[] { "key1", "a", "b" });
            int idx2 = table.SetRowReturnOneBasedIndex(new object[] { "key1", "x", "y" });

            // Assert
            Assert.AreEqual(1, idx1);
            Assert.AreEqual(1, idx2);
            Assert.AreEqual(1, table.RowCount);

            // Verify the row was updated
            object[] row = table.GetRow("key1");
            Assert.AreEqual("x", row[1]);
            Assert.AreEqual("y", row[2]);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_ShorterThanSchema_PadsWithNulls()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] shortRow = { "key1" };

            // Act
            int result = table.SetRowReturnOneBasedIndex(shortRow);

            // Assert
            Assert.AreEqual(1, result);
            object[] row = table.GetRow("key1");
            Assert.AreEqual("key1", row[0]);
            Assert.IsNull(row[1]);
            Assert.IsNull(row[2]);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_LongerThanSchema_Truncates()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] longRow = { "key1", "val1", "val2", "extra1", "extra2" };

            // Act
            int result = table.SetRowReturnOneBasedIndex(longRow);

            // Assert
            Assert.AreEqual(1, result);
            object[] row = table.GetRow("key1");
            Assert.AreEqual(3, row.Length);
            Assert.AreEqual("key1", row[0]);
            Assert.AreEqual("val1", row[1]);
            Assert.AreEqual("val2", row[2]);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_RowArray_NullRow_ThrowsArgumentNullException()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => table.SetRowReturnOneBasedIndex((object[])null));
        }

        #endregion

        #region SetRowReturnOneBasedIndex(ITableModel, string)

        [TestMethod]
        public void SetRowReturnOneBasedIndex_PrimaryKey_NewKey_ReturnsOneBasedIndex()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            int result = table.SetRowReturnOneBasedIndex("myKey");

            // Assert
            Assert.AreEqual(1, result);
            Assert.AreEqual(1, table.RowCount);
            Assert.IsTrue(table.RowExists("myKey"));
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_PrimaryKey_DuplicateKey_ReturnsSameIndex()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            int idx1 = table.SetRowReturnOneBasedIndex("myKey");
            int idx2 = table.SetRowReturnOneBasedIndex("myKey");

            // Assert
            Assert.AreEqual(1, idx1);
            Assert.AreEqual(1, idx2);
            Assert.AreEqual(1, table.RowCount);
        }

        [TestMethod]
        public void SetRowReturnOneBasedIndex_PrimaryKey_NullOrWhitespace_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.SetRowReturnOneBasedIndex((string)null));
            Assert.ThrowsExactly<ArgumentException>(() => table.SetRowReturnOneBasedIndex("   "));
            Assert.ThrowsExactly<ArgumentException>(() => table.SetRowReturnOneBasedIndex(string.Empty));
        }

        #endregion

        #region AddRowReturnKey(ITableModel, object[])

        [TestMethod]
        public void AddRowReturnKey_RowArray_ValidRow_ReturnsPrimaryKey()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] row = { "myKey", "val1", "val2" };

            // Act
            string key = table.AddRowReturnKey(row);

            // Assert
            Assert.AreEqual("myKey", key);
            Assert.AreEqual(1, table.RowCount);
        }

        [TestMethod]
        public void AddRowReturnKey_RowArray_ShorterThanSchema_PadsWithNulls()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] row = { "key1" };

            // Act
            string key = table.AddRowReturnKey(row);

            // Assert
            Assert.AreEqual("key1", key);
            object[] stored = table.GetRow("key1");
            Assert.IsNull(stored[1]);
            Assert.IsNull(stored[2]);
        }

        [TestMethod]
        public void AddRowReturnKey_RowArray_LongerThanSchema_Truncates()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            object[] row = { "key1", "val1", "val2", "extra1", "extra2" };

            // Act
            string key = table.AddRowReturnKey(row);

            // Assert
            Assert.AreEqual("key1", key);
            object[] stored = table.GetRow("key1");
            Assert.AreEqual(3, stored.Length);
        }

        [TestMethod]
        public void AddRowReturnKey_RowArray_NullRow_DelegatesToAutoIncrement()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            string key = table.AddRowReturnKey((object[])null);

            // Assert
            Assert.AreEqual("1", key);
            Assert.AreEqual(1, table.RowCount);
        }

        #endregion

        #region AddRowReturnKey(ITableModel, string)

        [TestMethod]
        public void AddRowReturnKey_PrimaryKey_ValidKey_ReturnsSameKey()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            string key = table.AddRowReturnKey("myKey");

            // Assert
            Assert.AreEqual("myKey", key);
            Assert.IsTrue(table.RowExists("myKey"));
        }

        [TestMethod]
        public void AddRowReturnKey_PrimaryKey_NullKey_DelegatesToAutoIncrement()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            string key = table.AddRowReturnKey((string)null);

            // Assert
            Assert.AreEqual("1", key);
        }

        #endregion

        #region AddRowReturnKey(ITableModel) - Auto-increment

        [TestMethod]
        public void AddRowReturnKey_AutoIncrement_EmptyTable_ReturnsOne()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            string key = table.AddRowReturnKey();

            // Assert
            Assert.AreEqual("1", key);
            Assert.AreEqual(1, table.RowCount);
        }

        [TestMethod]
        public void AddRowReturnKey_AutoIncrement_NonEmptyTable_ReturnsMaxPlusOne()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "1", null, null });
            table.SetRow(new object[] { "2", null, null });

            // Act
            string key = table.AddRowReturnKey();

            // Assert
            Assert.AreEqual("3", key);
        }

        [TestMethod]
        public void AddRowReturnKey_AutoIncrement_NonConsecutiveKeys_ReturnsMaxPlusOne()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "1", null, null });
            table.SetRow(new object[] { "5", null, null });
            table.SetRow(new object[] { "3", null, null });

            // Act
            string key = table.AddRowReturnKey();

            // Assert
            Assert.AreEqual("6", key);
        }

        #endregion

        #region DeleteRowReturnRemainingRows

        [TestMethod]
        public void DeleteRowReturnRemainingRows_SingleKey_ReturnsRemainingCount()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });
            table.SetRow(new object[] { "key3", "e", "f" });

            // Act
            int remaining = table.DeleteRowReturnRemainingRows("key2");

            // Assert
            Assert.AreEqual(2, remaining);
            Assert.IsFalse(table.RowExists("key2"));
        }

        [TestMethod]
        public void DeleteRowReturnRemainingRows_MultipleKeys_ReturnsRemainingCount()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });
            table.SetRow(new object[] { "key3", "e", "f" });
            table.SetRow(new object[] { "key4", "g", "h" });

            // Act
            int remaining = table.DeleteRowReturnRemainingRows("key1", "key3");

            // Assert
            Assert.AreEqual(2, remaining);
            Assert.IsFalse(table.RowExists("key1"));
            Assert.IsFalse(table.RowExists("key3"));
            Assert.IsTrue(table.RowExists("key2"));
            Assert.IsTrue(table.RowExists("key4"));
        }

        [TestMethod]
        public void DeleteRowReturnRemainingRows_NonExistentKey_DoesNotThrow()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });

            // Act
            int remaining = table.DeleteRowReturnRemainingRows("nonExistent");

            // Assert
            Assert.AreEqual(1, remaining);
        }

        [TestMethod]
        public void DeleteRowReturnRemainingRows_EmptyKeys_ReturnsOriginalCount()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });

            // Act
            int remaining = table.DeleteRowReturnRemainingRows();

            // Assert
            Assert.AreEqual(2, remaining);
        }

        [TestMethod]
        public void DeleteRowReturnRemainingRows_AllKeys_ReturnsZero()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });

            // Act
            int remaining = table.DeleteRowReturnRemainingRows("key1", "key2");

            // Assert
            Assert.AreEqual(0, remaining);
        }

        #endregion

        #region GetRow<TRow>(ITableModel, string)

        [TestMethod]
        public void GetRowGeneric_ByKey_ValidKey_ReturnsTypedRow()
        {
            // Arrange
            var table = CreateFiveColumnTable();
            table.SetRow(new object[] { "key1", "desc1", 10, 20, 30 });

            // Act
            var row = table.GetRow<PollingConfigurationQActionRow>("key1");

            // Assert
            Assert.IsNotNull(row);
            Assert.AreEqual("key1", row.Pollingconfigurationinstance_901);
            Assert.AreEqual("desc1", row.Pollingconfigurationdescription_902);
            Assert.AreEqual(10, row.Pollingconfigurationperiod_903);
            Assert.AreEqual(20, row.Pollingconfigurationlastpolled_904);
            Assert.AreEqual(30, row.Pollingconfigurationconnectionid_905);
        }

        [TestMethod]
        public void GetRowGeneric_ByKey_NonExistentKey_ReturnsNull()
        {
            // Arrange
            var table = CreateFiveColumnTable();

            // Act
            var row = table.GetRow<PollingConfigurationQActionRow>("noSuchKey");

            // Assert
            Assert.IsNull(row);
        }

        [TestMethod]
        public void GetRowGeneric_ByKey_NullOrWhitespace_ThrowsArgumentNullException()
        {
            // Arrange
            var table = CreateFiveColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => table.GetRow<PollingConfigurationQActionRow>((string)null));
            Assert.ThrowsExactly<ArgumentNullException>(() => table.GetRow<PollingConfigurationQActionRow>("   "));
        }

        #endregion

        #region GetRow<TRow>(ITableModel, int)

        [TestMethod]
        public void GetRowGeneric_ByIndex_ValidIndex_ReturnsTypedRow()
        {
            // Arrange
            var table = CreateFiveColumnTable();
            table.SetRow(new object[] { "key1", "desc1", 10, 20, 30 });
            table.SetRow(new object[] { "key2", "desc2", 40, 50, 60 });

            // Act
            var row = table.GetRow<PollingConfigurationQActionRow>(1);

            // Assert
            Assert.IsNotNull(row);
            Assert.AreEqual("key2", row.Pollingconfigurationinstance_901);
        }

        [TestMethod]
        public void GetRowGeneric_ByIndex_OutOfRange_ReturnsNull()
        {
            // Arrange
            var table = CreateFiveColumnTable();
            table.SetRow(new object[] { "key1", "desc1", 10, 20, 30 });

            // Act
            var row = table.GetRow<PollingConfigurationQActionRow>(5);

            // Assert
            Assert.IsNull(row);
        }

        #endregion

        #region GetParameterIndexByKey

        [TestMethod]
        public void GetParameterIndexByKey_ValidKeyAndColumn_ReturnsCellValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            object result = table.GetParameterIndexByKey("key1", 2); // 1-based column index 2 => idx 1 => PID 1002

            // Assert
            Assert.AreEqual("val1", result);
        }

        [TestMethod]
        public void GetParameterIndexByKey_ThirdColumn_ReturnsCellValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            object result = table.GetParameterIndexByKey("key1", 3); // 1-based column index 3 => idx 2 => PID 1003

            // Assert
            Assert.AreEqual("val2", result);
        }

        [TestMethod]
        public void GetParameterIndexByKey_NullTableModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => TableModelExtensionsForProtocol.GetParameterIndexByKey(null, "key1", 1));
        }

        [TestMethod]
        public void GetParameterIndexByKey_NullPrimaryKey_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndexByKey(null, 1));
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndexByKey("  ", 1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        public void GetParameterIndexByKey_OutOfRangeColumnIndex_ThrowsArgumentOutOfRangeException(int oneBasedColumnIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => table.GetParameterIndexByKey("key1", oneBasedColumnIndex));
        }

        #endregion

        #region SetParameterIndexByKey

        [TestMethod]
        public void SetParameterIndexByKey_ValidSet_ReturnsTrue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            bool result = table.SetParameterIndexByKey("key1", 2, "newVal");

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("newVal", table.GetCell("key1", 1002));
        }

        [TestMethod]
        public void SetParameterIndexByKey_NullTableModel_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => TableModelExtensionsForProtocol.SetParameterIndexByKey(null, "key1", 1, "val"));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        public void SetParameterIndexByKey_OutOfRangeColumnIndex_ThrowsArgumentOutOfRangeException(int oneBasedColumnIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => table.SetParameterIndexByKey("key1", oneBasedColumnIndex, "newVal"));
        }

        [TestMethod]
        public void SetParameterIndexByKey_WithTimestamp_SetsValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });
            var timestamp = new DateTime(2024, 1, 1);

            // Act
            bool result = table.SetParameterIndexByKey("key1", 3, "updated", timestamp);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("updated", table.GetCell("key1", 1003));
        }

        #endregion

        #region SetRowReturnChanges(ITableModel, string, object[])

        [TestMethod]
        public void SetRowReturnChanges_ByKey_NoChanges_ReturnsArrayWithTwos()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges("key1", new object[] { "key1", "val1", "val2" });

            // Assert
            Assert.AreEqual(0, changes[0]); // PK column always 0
            Assert.AreEqual(2, changes[1]); // 2 = no change
            Assert.AreEqual(2, changes[2]); // 2 = no change
        }

        [TestMethod]
        public void SetRowReturnChanges_ByKey_WithChanges_ReturnsArrayWithOnes()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges("key1", new object[] { "key1", "newVal1", "newVal2" });

            // Assert
            Assert.AreEqual(0, changes[0]); // PK column always 0
            Assert.AreEqual(1, changes[1]); // 1 = changed
            Assert.AreEqual(1, changes[2]); // 1 = changed

            // Verify values were updated
            object[] row = table.GetRow("key1");
            Assert.AreEqual("newVal1", row[1]);
            Assert.AreEqual("newVal2", row[2]);
        }

        [TestMethod]
        public void SetRowReturnChanges_ByKey_PartialChanges_MixedResult()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges("key1", new object[] { "key1", "val1", "newVal2" });

            // Assert
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(2, changes[1]); // unchanged
            Assert.AreEqual(1, changes[2]); // changed
        }

        [TestMethod]
        public void SetRowReturnChanges_ByKey_UseClearAndLeave_LeaveKeepsOldValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges(
                "key1",
                new object[] { Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE, "newVal2" },
                useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("key1", row[0]);   // Leave => kept original
            Assert.AreEqual("val1", row[1]);    // Leave => kept original
            Assert.AreEqual("newVal2", row[2]); // Changed
        }

        [TestMethod]
        public void SetRowReturnChanges_ByKey_UseClearAndLeave_ClearSetsNull()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges(
                "key1",
                new object[] { Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_CLEAR, "val2" },
                useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("key1", row[0]);
            Assert.IsNull(row[1]);        // Clear => null
            Assert.AreEqual("val2", row[2]);
        }

        [TestMethod]
        public void SetRowReturnChanges_ByKey_ShorterRowThanColumns_OnlyUpdatesProvidedColumns()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges("key1", new object[] { "key1", "newVal1" });

            // Assert
            Assert.AreEqual(0, changes[0]); // PK
            Assert.AreEqual(1, changes[1]); // changed
            // Column at idx 2 is not in the provided row, so it should be skipped
        }

        #endregion

        #region SetRowReturnChanges(ITableModel, int, object[])

        [TestMethod]
        public void SetRowReturnChanges_ByIndex_ValidIndex_ReturnsChanges()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });
            table.SetRow(new object[] { "key2", "val3", "val4" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges(0, new object[] { "key1", "newVal1", "val2" });

            // Assert
            Assert.AreEqual(0, changes[0]); // PK
            Assert.AreEqual(1, changes[1]); // changed
            Assert.AreEqual(2, changes[2]); // unchanged
        }

        [TestMethod]
        public void SetRowReturnChanges_ByIndex_NonExistentIndex_ReturnsAllZeros()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            int[] changes = (int[])table.SetRowReturnChanges(99, new object[] { "key1", "a", "b" });

            // Assert
            Assert.AreEqual(3, changes.Length);
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(0, changes[1]);
            Assert.AreEqual(0, changes[2]);
        }

        [TestMethod]
        public void SetRowReturnChanges_ByIndex_UseClearAndLeave_LeaveKeepsOldValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            table.SetRowReturnChanges(
                0,
                new object[] { Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE, "newVal2" },
                useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("key1", row[0]);
            Assert.AreEqual("val1", row[1]);
            Assert.AreEqual("newVal2", row[2]);
        }

        #endregion

        #region FillArray(ITableModel, List<object[]>, SaveOption)

        [TestMethod]
        public void FillArray_SaveOptionFull_ClearsAndAddsRows()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "oldKey", "old1", "old2" });

            var rows = new List<object[]>
            {
                new object[] { "key1", "a", "b" },
                new object[] { "key2", "c", "d" },
            };

            // Act
            object result = table.FillArray(rows, NotifyProtocol.SaveOption.Full);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(2, table.RowCount);
            Assert.IsFalse(table.RowExists("oldKey"));
            Assert.IsTrue(table.RowExists("key1"));
            Assert.IsTrue(table.RowExists("key2"));
        }

        [TestMethod]
        public void FillArray_SaveOptionFull_EmptyList_ClearsTable()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });

            // Act
            object result = table.FillArray(new List<object[]>(), NotifyProtocol.SaveOption.Full);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(0, table.RowCount);
        }

        [TestMethod]
        public void FillArray_SaveOptionPartial_UpsertsRows()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });

            var rows = new List<object[]>
            {
                new object[] { "key1", "x", "y" }, // update
                new object[] { "key3", "e", "f" }, // new
            };

            // Act
            object result = table.FillArray(rows, NotifyProtocol.SaveOption.Partial);

            // Assert
            Assert.AreEqual(true, result);
            Assert.AreEqual(3, table.RowCount);

            object[] updatedRow = table.GetRow("key1");
            Assert.AreEqual("x", updatedRow[1]);
            Assert.AreEqual("y", updatedRow[2]);
        }

        [TestMethod]
        public void FillArray_SaveOptionPartial_EmptyList_ThrowsIndexOutOfRangeException()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<IndexOutOfRangeException>(
                () => table.FillArray(new List<object[]>(), NotifyProtocol.SaveOption.Partial));
        }

        [TestMethod]
        public void FillArray_SaveOptionFull_WithTimestamp_SetsTimestamp()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            var timestamp = new DateTime(2024, 6, 15);

            var rows = new List<object[]>
            {
                new object[] { "key1", "a", "b" },
            };

            // Act
            table.FillArray(rows, NotifyProtocol.SaveOption.Full, timestamp);

            // Assert
            Assert.AreEqual(1, table.RowCount);
            Assert.AreEqual(timestamp, table.GetLastWriteTimestamp("key1", 1002));
        }

        #endregion

        #region FillArray(ITableModel, object[][], DateTime?, bool) - column-based

        [TestMethod]
        public void FillArray_ColumnBased_ReplacesTableContent()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "oldKey", "old1", "old2" });

            object[][] columns = new object[][]
            {
                new object[] { "key1", "key2" },       // PK column
                new object[] { "val1A", "val2A" },     // col 2
                new object[] { "val1B", "val2B" },     // col 3
            };

            // Act
            table.FillArray(columns);

            // Assert
            Assert.AreEqual(2, table.RowCount);
            Assert.IsFalse(table.RowExists("oldKey")); // old row deleted

            object[] row1 = table.GetRow("key1");
            Assert.AreEqual("val1A", row1[1]);
            Assert.AreEqual("val1B", row1[2]);

            object[] row2 = table.GetRow("key2");
            Assert.AreEqual("val2A", row2[1]);
            Assert.AreEqual("val2B", row2[2]);
        }

        [TestMethod]
        public void FillArray_ColumnBased_WithClearAndLeave_HandlesCorrectly()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "original", "keep" });

            object[][] columns = new object[][]
            {
                new object[] { "key1" },
                new object[] { "newVal" },
                new object[] { Constants.PROTOCOL_LEAVE },
            };

            // Act
            table.FillArray(columns, useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("newVal", row[1]);
            Assert.AreEqual("keep", row[2]); // Leave => kept original
        }

        [TestMethod]
        public void FillArray_ColumnBased_WithClear_SetsNull()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "original", "toBeCleared" });

            object[][] columns = new object[][]
            {
                new object[] { "key1" },
                new object[] { "newVal" },
                new object[] { Constants.PROTOCOL_CLEAR },
            };

            // Act
            table.FillArray(columns, useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("newVal", row[1]);
            Assert.IsNull(row[2]); // Clear => null
        }

        [TestMethod]
        public void FillArray_ColumnBased_DeletesRowsNotInNewData()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });
            table.SetRow(new object[] { "key3", "e", "f" });

            object[][] columns = new object[][]
            {
                new object[] { "key1", "key3" },     // Only key1 and key3
                new object[] { "val1", "val3" },
                new object[] { "val1b", "val3b" },
            };

            // Act
            table.FillArray(columns);

            // Assert
            Assert.AreEqual(2, table.RowCount);
            Assert.IsTrue(table.RowExists("key1"));
            Assert.IsFalse(table.RowExists("key2")); // deleted
            Assert.IsTrue(table.RowExists("key3"));
        }

        #endregion

        #region FillArrayNoDelete

        [TestMethod]
        public void FillArrayNoDelete_AddsNewRows_KeepsExistingRows()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "existingKey", "old1", "old2" });

            object[][] columns = new object[][]
            {
                new object[] { "key1", "key2" },
                new object[] { "val1", "val2" },
                new object[] { "valA", "valB" },
            };

            // Act
            table.FillArrayNoDelete(columns);

            // Assert
            Assert.AreEqual(3, table.RowCount);
            Assert.IsTrue(table.RowExists("existingKey")); // not deleted
            Assert.IsTrue(table.RowExists("key1"));
            Assert.IsTrue(table.RowExists("key2"));
        }

        [TestMethod]
        public void FillArrayNoDelete_UpdatesExistingRows()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "old1", "old2" });

            object[][] columns = new object[][]
            {
                new object[] { "key1" },
                new object[] { "new1" },
                new object[] { "new2" },
            };

            // Act
            table.FillArrayNoDelete(columns);

            // Assert
            Assert.AreEqual(1, table.RowCount);
            object[] row = table.GetRow("key1");
            Assert.AreEqual("new1", row[1]);
            Assert.AreEqual("new2", row[2]);
        }

        [TestMethod]
        public void FillArrayNoDelete_WithClearAndLeave_HandlesCorrectly()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "original", "keep" });

            object[][] columns = new object[][]
            {
                new object[] { "key1" },
                new object[] { Constants.PROTOCOL_CLEAR },
                new object[] { Constants.PROTOCOL_LEAVE },
            };

            // Act
            table.FillArrayNoDelete(columns, useClearAndLeave: true);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.IsNull(row[1]);          // Clear => null
            Assert.AreEqual("keep", row[2]); // Leave => kept original
        }

        [TestMethod]
        public void FillArrayNoDelete_WithTimestamp_SetsTimestamp()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            var timestamp = new DateTime(2024, 3, 20);

            object[][] columns = new object[][]
            {
                new object[] { "key1" },
                new object[] { "val1" },
                new object[] { "val2" },
            };

            // Act
            table.FillArrayNoDelete(columns, timestamp);

            // Assert
            Assert.AreEqual(timestamp, table.GetLastWriteTimestamp("key1", 1002));
        }

        #endregion

        #region FillArrayWithColumn

        [TestMethod]
        public void FillArrayWithColumn_NewKeys_CreatesRows()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2" };
            object[] values = { "val1", "val2" };

            // Act
            table.FillArrayWithColumn(1002, keys, values);

            // Assert
            Assert.AreEqual(2, table.RowCount);

            object[] row1 = table.GetRow("key1");
            Assert.AreEqual("key1", row1[0]);
            Assert.AreEqual("val1", row1[1]);
            Assert.IsNull(row1[2]); // other column not set

            object[] row2 = table.GetRow("key2");
            Assert.AreEqual("val2", row2[1]);
        }

        [TestMethod]
        public void FillArrayWithColumn_ExistingKeys_UpdatesColumn()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "old1", "other1" });
            table.SetRow(new object[] { "key2", "old2", "other2" });

            string[] keys = { "key1", "key2" };
            object[] values = { "new1", "new2" };

            // Act
            table.FillArrayWithColumn(1002, keys, values);

            // Assert
            Assert.AreEqual("new1", table.GetCell("key1", 1002));
            Assert.AreEqual("new2", table.GetCell("key2", 1002));
            // Other columns unchanged
            Assert.AreEqual("other1", table.GetCell("key1", 1003));
            Assert.AreEqual("other2", table.GetCell("key2", 1003));
        }

        [TestMethod]
        public void FillArrayWithColumn_SingleValue_BroadcastsToAllKeys()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2", "key3" };
            object[] values = { "sameValue" }; // single value

            // Act
            table.FillArrayWithColumn(1002, keys, values);

            // Assert
            Assert.AreEqual(3, table.RowCount);
            Assert.AreEqual("sameValue", table.GetCell("key1", 1002));
            Assert.AreEqual("sameValue", table.GetCell("key2", 1002));
            Assert.AreEqual("sameValue", table.GetCell("key3", 1002));
        }

        [TestMethod]
        public void FillArrayWithColumn_MismatchedLengths_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2", "key3" };
            object[] values = { "val1", "val2" }; // 3 keys but 2 values (not 1 either)

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => table.FillArrayWithColumn(1002, keys, values));
        }

        [TestMethod]
        public void FillArrayWithColumn_WithProtocolLeave_KeepsExistingValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "original", "other" });

            string[] keys = { "key1" };
            object[] values = { Constants.PROTOCOL_LEAVE };

            // Act
            table.FillArrayWithColumn(1002, keys, values, useClearAndLeave: true);

            // Assert
            Assert.AreEqual("original", table.GetCell("key1", 1002));
        }

        [TestMethod]
        public void FillArrayWithColumn_WithProtocolClear_SetsNull()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "original", "other" });

            string[] keys = { "key1" };
            object[] values = { Constants.PROTOCOL_CLEAR };

            // Act
            table.FillArrayWithColumn(1002, keys, values, useClearAndLeave: true);

            // Assert
            Assert.IsNull(table.GetCell("key1", 1002));
        }

        [TestMethod]
        public void FillArrayWithColumn_NonPrimaryKeyColumn_AddsRowsWithCorrectValues()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2" };
            object[] values = { "thirdCol1", "thirdCol2" };

            // Act
            table.FillArrayWithColumn(1003, keys, values); // Third column (idx=2)

            // Assert
            object[] row1 = table.GetRow("key1");
            Assert.AreEqual("key1", row1[0]);
            Assert.IsNull(row1[1]); // second column not set
            Assert.AreEqual("thirdCol1", row1[2]);
        }

        #endregion

        #region FillArrayWithColumns

        [TestMethod]
        public void FillArrayWithColumns_MultipleColumns_SetsValues()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2" };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { 1002, new object[] { "a", "b" } },
                { 1003, new object[] { 1, 2 } },
            };

            // Act
            table.FillArrayWithColumns(keys, columnPidsToValues);

            // Assert
            Assert.AreEqual(2, table.RowCount);

            object[] row1 = table.GetRow("key1");
            Assert.AreEqual("a", row1[1]);
            Assert.AreEqual(1, row1[2]);

            object[] row2 = table.GetRow("key2");
            Assert.AreEqual("b", row2[1]);
            Assert.AreEqual(2, row2[2]);
        }

        [TestMethod]
        public void FillArrayWithColumns_NullColumnPidsToValues_ThrowsArgumentNullException()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => table.FillArrayWithColumns(new[] { "key1" }, null));
        }

        [TestMethod]
        public void FillArrayWithColumns_ExistingRows_UpdatesColumns()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "old1", "old2" });

            string[] keys = { "key1" };
            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { 1002, new object[] { "new1" } },
                { 1003, new object[] { "new2" } },
            };

            // Act
            table.FillArrayWithColumns(keys, columnPidsToValues);

            // Assert
            object[] row = table.GetRow("key1");
            Assert.AreEqual("new1", row[1]);
            Assert.AreEqual("new2", row[2]);
        }

        [TestMethod]
        public void FillArrayWithColumns_MismatchedLengths_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            string[] keys = { "key1", "key2", "key3" };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { 1002, new object[] { "a", "b" } }, // 2 values for 3 keys
            };

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => table.FillArrayWithColumns(keys, columnPidsToValues));
        }

        #endregion

        #region GetTableColumns

        [TestMethod]
        public void GetTableColumns_ValidIndexes_ReturnsColumns()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", 1 });
            table.SetRow(new object[] { "key2", "b", 2 });

            // Act
            object[][] columns = table.GetTableColumns(new uint[] { 0, 1, 2 });

            // Assert
            Assert.AreEqual(3, columns.Length);

            // Column 0 (PK)
            CollectionAssert.AreEqual(new object[] { "key1", "key2" }, columns[0]);

            // Column 1
            CollectionAssert.AreEqual(new object[] { "a", "b" }, columns[1]);

            // Column 2
            CollectionAssert.AreEqual(new object[] { 1, 2 }, columns[2]);
        }

        [TestMethod]
        public void GetTableColumns_SubsetOfIndexes_ReturnsOnlyRequestedColumns()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", 1 });
            table.SetRow(new object[] { "key2", "b", 2 });

            // Act
            object[][] columns = table.GetTableColumns(new uint[] { 0, 2 }); // skip column 1

            // Assert
            Assert.AreEqual(2, columns.Length);
            CollectionAssert.AreEqual(new object[] { "key1", "key2" }, columns[0]);
            CollectionAssert.AreEqual(new object[] { 1, 2 }, columns[1]);
        }

        [TestMethod]
        public void GetTableColumns_InvalidIndex_ReturnsNullForThatColumn()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", 1 });

            // Act
            object[][] columns = table.GetTableColumns(new uint[] { 0, 99 });

            // Assert
            Assert.AreEqual(2, columns.Length);
            Assert.IsNotNull(columns[0]);
            Assert.IsNull(columns[1]); // index 99 does not exist
        }

        [TestMethod]
        public void GetTableColumns_EmptyTable_ReturnsEmptyArrays()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            object[][] columns = table.GetTableColumns(new uint[] { 0, 1 });

            // Assert
            Assert.AreEqual(2, columns.Length);
            Assert.AreEqual(0, columns[0].Length);
            Assert.AreEqual(0, columns[1].Length);
        }

        #endregion

        #region GetColumnByPid

        [TestMethod]
        public void GetColumnByPid_ValidPid_ReturnsColumnValues()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", 1 });
            table.SetRow(new object[] { "key2", "b", 2 });
            table.SetRow(new object[] { "key3", "c", 3 });

            // Act
            object[] column = table.GetColumnByPid(1002);

            // Assert
            Assert.AreEqual(3, column.Length);
            CollectionAssert.AreEqual(new object[] { "a", "b", "c" }, column);
        }

        [TestMethod]
        public void GetColumnByPid_EmptyTable_ReturnsEmptyArray()
        {
            // Arrange
            var table = CreateThreeColumnTable();

            // Act
            object[] column = table.GetColumnByPid(1002);

            // Assert
            Assert.AreEqual(0, column.Length);
        }

        [TestMethod]
        public void GetColumnByPid_PrimaryKeyColumn_ReturnsKeys()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "a", "b" });
            table.SetRow(new object[] { "key2", "c", "d" });

            // Act
            object[] column = table.GetColumnByPid(1001);

            // Assert
            CollectionAssert.AreEqual(new object[] { "key1", "key2" }, column);
        }

        #endregion

        #region GetParameterIndex

        [TestMethod]
        public void GetParameterIndex_ValidCoordinates_ReturnsCellValue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });
            table.SetRow(new object[] { "key2", "val3", "val4" });

            // Act
            object result = table.GetParameterIndex(2, 2); // row 2, col 2 (both 1-based)

            // Assert
            Assert.AreEqual("val3", result);
        }

        [TestMethod]
        public void GetParameterIndex_FirstRowFirstColumn_ReturnsPrimaryKey()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act
            object result = table.GetParameterIndex(1, 1);

            // Assert
            Assert.AreEqual("key1", result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void GetParameterIndex_InvalidRowIndex_ThrowsArgumentException(int rowIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndex(rowIndex, 1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void GetParameterIndex_InvalidColumnIndex_ThrowsArgumentException(int colIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndex(1, colIndex));
        }

        [TestMethod]
        public void GetParameterIndex_ColumnIndexExceedsCount_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndex(1, 4));
        }

        [TestMethod]
        public void GetParameterIndex_RowIndexExceedsCount_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.GetParameterIndex(2, 1));
        }

        #endregion

        #region SetParameterIndex

        [TestMethod]
        public void SetParameterIndex_ValidCoordinates_SetsValueAndReturnsTrue()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });
            table.SetRow(new object[] { "key2", "val3", "val4" });

            // Act
            bool result = table.SetParameterIndex(1, 2, "changed"); // row 1, col 2 (1-based)

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("changed", table.GetCell("key1", 1002));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void SetParameterIndex_InvalidRowIndex_ThrowsArgumentException(int rowIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.SetParameterIndex(rowIndex, 2, "val"));
        }

        [TestMethod]
        [DataRow(1)] // Column 1 = PK => not allowed (min is 2)
        [DataRow(0)]
        public void SetParameterIndex_ColumnIndexTooLow_ThrowsArgumentException(int colIndex)
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.SetParameterIndex(1, colIndex, "val"));
        }

        [TestMethod]
        public void SetParameterIndex_ColumnIndexExceedsCount_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.SetParameterIndex(1, 4, "val"));
        }

        [TestMethod]
        public void SetParameterIndex_RowIndexExceedsCount_ThrowsArgumentException()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => table.SetParameterIndex(2, 2, "val"));
        }

        [TestMethod]
        public void SetParameterIndex_WithTimestamp_SetsValueAndTimestamp()
        {
            // Arrange
            var table = CreateThreeColumnTable();
            table.SetRow(new object[] { "key1", "val1", "val2" });
            var timestamp = new DateTime(2024, 12, 25);

            // Act
            bool result = table.SetParameterIndex(1, 3, "updated", timestamp);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual("updated", table.GetCell("key1", 1003));
            Assert.AreEqual(timestamp, table.GetLastWriteTimestamp("key1", 1003));
        }

        #endregion
    }
}
