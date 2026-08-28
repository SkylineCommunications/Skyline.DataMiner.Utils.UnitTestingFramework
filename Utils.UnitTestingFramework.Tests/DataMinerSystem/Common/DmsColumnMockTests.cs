namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class DmsColumnMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Id_ReturnsColumnPid()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);

            // Act
            var column = table.GetColumn<string>(902);

            // Assert
            Assert.AreEqual(902, column.Id);
        }

        [TestMethod]
        public void Table_ReturnsOwningTable()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);

            // Act
            var column = table.GetColumn<string>(902);

            // Assert
            Assert.AreSame(table, column.Table);
        }

        [TestMethod]
        public void GetValue_ReturnsStoredStringValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
#pragma warning disable CS0618 // Type or member is obsolete - the obsolete overload is verified to remain usable.
            var value = table.GetColumn<string>(902).GetValue("one");
#pragma warning restore CS0618

            // Assert
            Assert.AreEqual("one-desc", value);
        }

        [TestMethod]
        public void GetValue_WithKeyTypeOverload_ReturnsStoredValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            var value = table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey);

            // Assert
            Assert.AreEqual("one-desc", value);
        }

        [TestMethod]
        public void GetValue_ReturnsStoredNumericValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            var value = table.GetColumn<double?>(903).GetValue("one", KeyType.PrimaryKey);

            // Assert
            Assert.AreEqual(3.0, value);
        }

        [TestMethod]
        public void SetValue_ThenGetValue_ReturnsSetValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.GetColumn<string>(902).SetValue("one", "changed-desc");

            // Assert
            Assert.AreEqual("changed-desc", table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey));
        }

        [TestMethod]
        public void SetValue_WithKeyTypeOverload_PersistsValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.GetColumn<string>(902).SetValue("one", KeyType.PrimaryKey, "changed-desc");

            // Assert
            Assert.AreEqual("changed-desc", table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey));
        }

        [TestMethod]
        public void SetValue_WithKeyTypeAndExpectedChangesOverload_PersistsValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Act
            table.GetColumn<string>(902).SetValue("one", KeyType.PrimaryKey, "changed-desc", TimeSpan.FromSeconds(1), null);

            // Assert
            Assert.AreEqual("changed-desc", table.GetColumn<string>(902).GetValue("one", KeyType.PrimaryKey));
        }

        [TestMethod]
        public void StartValueMonitor_Column_InvokesCallbackOnCellChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            ColumnValueChange<string> received = null;
            column.StartValueMonitor("source", change => received = change, false);

            // Act
            column.SetValue("one", "changed-desc");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("changed-desc", received.ColumnUpdates["one"]);
        }

        [TestMethod]
        public void StartValueMonitor_Column_WithTimeSpanOverload_InvokesCallbackOnCellChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            ColumnValueChange<string> received = null;
            column.StartValueMonitor("source", change => received = change, TimeSpan.FromSeconds(1), false);

            // Act
            column.SetValue("one", "changed-desc");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("changed-desc", received.ColumnUpdates["one"]);
        }

        [TestMethod]
        public void StartValueMonitor_Cell_InvokesCallbackOnMatchingCellChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });
            var column = table.GetColumn<string>(902);

            CellValueChange<string> received = null;
            column.StartValueMonitor("source", "one", change => received = change, false);

            // Act
            column.SetValue("two", "changed-two");
            Assert.IsNull(received, "Callback should not fire for a different primary key.");

            column.SetValue("one", "changed-one");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("changed-one", received.Value);
        }

        [TestMethod]
        public void StartValueMonitor_Cell_WithTimeSpanOverload_InvokesCallbackOnMatchingCellChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            CellValueChange<string> received = null;
            column.StartValueMonitor("source", "one", change => received = change, TimeSpan.FromSeconds(1), false);

            // Act
            column.SetValue("one", "changed-one");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("changed-one", received.Value);
        }

        [TestMethod]
        public void StopValueMonitor_Column_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            ColumnValueChange<string> received = null;
            column.StartValueMonitor("source", change => received = change, false);
            column.StopValueMonitor("source", false);

            // Act
            column.SetValue("one", "changed-desc");

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void StopValueMonitor_Column_WithTimeSpanOverload_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            ColumnValueChange<string> received = null;
            column.StartValueMonitor("source", change => received = change, false);
            column.StopValueMonitor("source", TimeSpan.FromSeconds(1), false);

            // Act
            column.SetValue("one", "changed-desc");

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void StopValueMonitor_Cell_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            CellValueChange<string> received = null;
            column.StartValueMonitor("source", "one", change => received = change, false);
            column.StopValueMonitor("source", "one", false);

            // Act
            column.SetValue("one", "changed-one");

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void StopValueMonitor_Cell_WithTimeSpanOverload_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var column = table.GetColumn<string>(902);

            CellValueChange<string> received = null;
            column.StartValueMonitor("source", "one", change => received = change, false);
            column.StopValueMonitor("source", "one", TimeSpan.FromSeconds(1), false);

            // Act
            column.SetValue("one", "changed-one");

            // Assert
            Assert.IsNull(received);
        }
    }
}
