namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
