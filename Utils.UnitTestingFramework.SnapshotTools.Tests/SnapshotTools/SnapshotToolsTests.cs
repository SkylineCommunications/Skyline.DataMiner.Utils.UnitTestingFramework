namespace Skyline.DataMiner.Utils.UnitTestingFramework.SnapshotTools.Tests
{
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.SnapshotTools;

    using VerifyMSTest;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class SnapshotToolsTests : VerifyBase
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public Task SnapTest_CorrectOutput()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7, 8 };

            // Act
            tablesCache.SetRowReturnOneBasedIndex(900, row1);
            tablesCache.SetRowReturnOneBasedIndex(900, row2);

            // Assert
            return this.Verify(SnapshotTools.ShowTable(protocolCache, 900));
        }

        [TestMethod]
        public Task SnapTest_EmptyCell()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", null, 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7 };

            // Act
            tablesCache.SetRowReturnOneBasedIndex(900, row1);
            tablesCache.SetRowReturnOneBasedIndex(900, row2);

            // Assert
            return Verifier.Verify(SnapshotTools.ShowTable(protocolCache, 900));
        }

        [TestMethod]
        public Task SnapTest_EmptyTable()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);

            // Act

            // Assert
            return Verifier.Verify(SnapshotTools.ShowTable(protocolCache, 900));
        }

        [TestMethod]
        public Task SnapTest_MultipleTables()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7, 8 };
            object[] row3 = new object[] { "1", "1" };

            // Act
            tablesCache.SetRowReturnOneBasedIndex(900, row1);
            tablesCache.SetRowReturnOneBasedIndex(900, row2);
            tablesCache.SetRowReturnOneBasedIndex(1100, row3);

            // Assert
            return Verifier.Verify(SnapshotTools.ShowTables(protocolCache, new int[] { 900, 1100 }));
        }

        [TestMethod]
        public Task SnapTest_MultipleEmptyTables()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);

            // Act

            // Assert
            return Verifier.Verify(SnapshotTools.ShowTables(protocolCache, new int[] { 900, 1100 }));
        }

        [TestMethod]
        public Task SnapTest_MultipleTables_EmptyCells()
        {
            // Arrange
            var protocolCache = ElementDataBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "two", 3, 4 };
            object[] row2 = new object[] { "notOne", "notTwo", null, 7, 8 };
            object[] row3 = new object[] { "1", null };

            // Act
            tablesCache.SetRowReturnOneBasedIndex(900, row1);
            tablesCache.SetRowReturnOneBasedIndex(900, row2);
            tablesCache.SetRowReturnOneBasedIndex(1100, row3);

            // Assert
            return Verifier.Verify(SnapshotTools.ShowTables(protocolCache, new int[] { 900, 1100 }));
        }
    }
}