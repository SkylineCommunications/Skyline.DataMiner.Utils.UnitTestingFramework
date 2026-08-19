namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMock_MonitorMethods_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void StandaloneParameter_StartValueMonitor_InvokesCallbackOnChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, false);

            // Act
            parameter.SetValue("monitored value");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("monitored value", received.Value);
            Assert.AreEqual("source", received.MonitorSource);
        }

        [TestMethod]
        public void StandaloneParameter_StopValueMonitor_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, false);
            parameter.StopValueMonitor("source", false);

            // Act
            parameter.SetValue("value after stop");

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void Table_StartValueMonitor_InvokesCallbackOnRowAdded()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
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
        public void Table_StartValueMonitor_InvokesCallbackOnRowDeleted()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
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
        public void Table_StopValueMonitor_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
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
        public void Column_StartValueMonitor_InvokesCallbackOnCellChange()
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
        public void Column_StartCellValueMonitor_InvokesCallbackOnMatchingCellChange()
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
        public void Column_StopValueMonitor_DoesNotInvokeCallbackAfterStop()
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
        public void Table_QueryData_NoFilters_ReturnsAllRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            // Act
            var rows = table.QueryData(new IColumnFilter[0]).ToList();

            // Assert
            Assert.AreEqual(2, rows.Count);
        }

        [TestMethod]
        public void Table_QueryData_WithColumnFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            var filter = new ColumnFilter { Pid = 902, Value = "two-desc", ComparisonOperator = ComparisonOperator.Equal };

            // Act
            var rows = table.QueryData(new IColumnFilter[] { filter }).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("two", rows[0][0]);
        }

        [TestMethod]
        public void Table_QueryData_WithNumericFilter_ReturnsMatchingRows()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            table.AddRow(new object[] { "two", "two-desc", 6.0, 7.0, 8.0 });

            var filter = new ColumnFilter { Pid = 903, Value = "5", ComparisonOperator = ComparisonOperator.GreaterThan };

            // Act
            var rows = table.QueryData(new IColumnFilter[] { filter }).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual("two", rows[0][0]);
        }

        [TestMethod]
        public void Table_QueryData_WithReturnFilter_ProjectsRequestedColumns()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);
            table.AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            var returnFilter = new ColumnReturnFilter { Pid = 902 };

            // Act
            var rows = table.QueryData(new IColumnFilter[] { returnFilter }).ToList();

            // Assert
            Assert.AreEqual(1, rows.Count);
            Assert.AreEqual(1, rows[0].Length);
            Assert.AreEqual("one-desc", rows[0][0]);
        }
    }
}
