namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class NotifyProtocolHelper_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetParameter_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act
            var output = mock.Object.NotifyProtocol(73, 1000, null); // GetParameter

            // Assert
            Assert.AreEqual(10, output);
        }

        [TestMethod]
        public void GetParameterByName_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act
            var output = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(10, output);
        }

        [TestMethod]
        public void SetParameter_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            uint dmaID = 346;
            uint elementID = 801;
            uint parameterID = 1000;

            uint[] ids = new uint[] { dmaID, elementID, parameterID };
            int value = 50;

            // Act
            mock.Object.NotifyProtocol(50, ids, value); // SetParameter
            var outputGet = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(value, outputGet);
        }

        [TestMethod]
        public void SetParameterByName_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            string parameterName = "NumericParameter";
            int value = 50;

            // Act
            mock.Object.NotifyProtocol(84, parameterName, value); // SetParameterByName
            var outputGet = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(value, outputGet);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("900")]
        [DataRow(new[] { 900 })]
        public void AddRow_InvalidSecondArgument_ThrowsException(object notifyProtocolSecondArgument)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mock.Object.NotifyProtocol((int)NotifyType.AddRow, notifyProtocolSecondArgument, "Row 1 PK"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow(1)]
        public void AddRow_InvalidThirdArgument_ThrowsException(object notifyProtocolThirdArgument)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mock.Object.NotifyProtocol((int)NotifyType.AddRow, 900, notifyProtocolThirdArgument));
        }

        [TestMethod]
        [DataRow(new[] {"PK1", "PK2", "PK3", "PK4"}, new[] {1, 2, 3, 4})]
        [DataRow(new[] {"PK1", "PK1", "PK3", "PK4"}, new[] {1, 1, 2, 3})]
        [DataRow(new[] {"PK1", "PK2", "PK3", "PK1"}, new[] {1, 2, 3, 1})]
        [DataRow(new object[] { new[] { "PK1" }, new[] { "PK2" }, new[] { "PK3" }, new[] { "PK4" } }, new[] { 1, 2, 3, 4 })]
        [DataRow(new object[] { new[] { "PK1" }, new[] { "PK1" }, new[] { "PK3" }, new[] { "PK4" } }, new[] { 1, 1, 2, 3 })]
        [DataRow(new object[] { new[] { "PK1" }, new[] { "PK2" }, new[] { "PK3" }, new[] { "PK1" } }, new[] { 1, 2, 3, 1 })]
        public void AddRow_IsEqual(object[] notifyProtocolThirdArguments, int[] expectedRowIndexes)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol((int)NotifyType.AddRow, tableID, notifyProtocolThirdArguments[0]);
            var outputAddRow2 = mock.Object.NotifyProtocol((int)NotifyType.AddRow, tableID, notifyProtocolThirdArguments[1]);
            var outputAddRow3 = mock.Object.NotifyProtocol((int)NotifyType.AddRow, tableID, notifyProtocolThirdArguments[2]);
            var outputAddRow4 = mock.Object.NotifyProtocol((int)NotifyType.AddRow, tableID, notifyProtocolThirdArguments[3]);

            // Assert
            Assert.AreEqual(expectedRowIndexes[0], outputAddRow1);
            Assert.AreEqual(expectedRowIndexes[1], outputAddRow2);
            Assert.AreEqual(expectedRowIndexes[2], outputAddRow3);
            Assert.AreEqual(expectedRowIndexes[3], outputAddRow4);
        }

        [TestMethod]
        public void AddRowReturnKey_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;
            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";
            object[] rowIdentification1 = new object[] { tableID, primaryKey1 };
            object[] rowIdentification2 = new object[] { tableID, primaryKey2 };

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(240, tableID, primaryKey1); // AddRowReturnKey
            var outputAddRow2 = mock.Object.NotifyProtocol(240, tableID, primaryKey2); // AddRowReturnKey

            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowIdentification1, null); // GetRow
            var outputGetRow2 = (object[])mock.Object.NotifyProtocol(215, rowIdentification2, null); // GetRow

            // Assert
            Assert.AreEqual(primaryKey1, outputAddRow1);
            Assert.AreEqual(primaryKey2, outputAddRow2);

            string[] row1 = { "Row 1 PK", null, null, null, null };
            string[] row2 = { "Row 2 PK", null, null, null, null };

            outputGetRow1.Should().BeEquivalentTo(row1);
            outputGetRow2.Should().BeEquivalentTo(row2);
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow(1.1)]
        [DataRow(true)]
        [DataRow(new[] {1 , 2, 3})]
        [DataRow((object)new object[] {1 , "2", 3})]
        public void DeleteRow_InvalidSecondArgument_ThrowsException(object notifyProtocolSecondArgument)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            mock.Object.AddRow(tableID, primaryKey1);
            mock.Object.AddRow(tableID, "Row 2 PK");
            mock.Object.AddRow(tableID, "Row 3 PK");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mock.Object.NotifyProtocol((int)NotifyType.DeleteRow, notifyProtocolSecondArgument, null));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(1.1)]
        [DataRow(true)]
        [DataRow(new[] { 1, 2, 3 })]
        [DataRow((object)new object[] { 1, "2", 3 })]
        public void DeleteRow_InvalidThirdArgument_ThrowsException(object notifyProtocolThirdArgument)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            mock.Object.AddRow(tableID, primaryKey1);
            mock.Object.AddRow(tableID, "Row 2 PK");
            mock.Object.AddRow(tableID, "Row 3 PK");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mock.Object.NotifyProtocol((int)NotifyType.DeleteRow, tableID, notifyProtocolThirdArgument));
        }

        [TestMethod]
        [DataRow("Row 1 PK", 2)]
        [DataRow("Row 2 PK", 2)]
        [DataRow("Row 3 PK", 2)]
        [DataRow(new[] { "Row 1 PK" }, 2)]
        [DataRow(new[] { "Row 2 PK" }, 2)]
        [DataRow(new[] { "Row 3 PK" }, 2)]
        [DataRow(new[] { "Row 1 PK", "Row 2 PK" }, 1)]
        [DataRow(new[] { "Row 2 PK", "Row 3 PK" }, 1)]
        [DataRow(new[] { "Row 1 PK", "Row 3 PK" }, 1)]
        [DataRow(new[] { "Row 1 PK", "Row 2 PK", "Row 3 PK" }, 0)]
        public void DeleteRow_DeleteExistingRows(object primaryKeysToRemove, int expectedRemainingRows)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            mock.Object.AddRow(tableID, "Row 1 PK");
            mock.Object.AddRow(tableID, "Row 2 PK");
            mock.Object.AddRow(tableID, "Row 3 PK");

            // Act
            int remainingRows = (int)mock.Object.NotifyProtocol((int)NotifyType.DeleteRow, tableID, primaryKeysToRemove);

            // Assert
            Assert.AreEqual(expectedRemainingRows, remainingRows);
        }

        [TestMethod]
        public void Exists_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";
            string primaryKeyInexistent = "Row Inexistent PK";

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputExists1 = mock.Object.NotifyProtocol(265, tableID, primaryKey1); // Exists
            var outputExists2 = mock.Object.NotifyProtocol(265, tableID, primaryKeyInexistent); // Exists

            // Assert
            Assert.AreEqual(1, outputAddRow1);
            Assert.IsTrue((bool)outputExists1);
            Assert.IsFalse((bool)outputExists2);
        }

        [TestMethod]
        public void GetKeyPosition_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";
            string primaryKey3 = "Row 3 PK";

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputAddRow2 = mock.Object.NotifyProtocol(149, tableID, primaryKey2); // AddRow
            var outputAddRow3 = mock.Object.NotifyProtocol(149, tableID, primaryKey3); // AddRow
            var outputGetKeyPosition1 = mock.Object.NotifyProtocol(163, tableID, primaryKey1); // GetKeyPosition
            var outputGetKeyPosition2 = mock.Object.NotifyProtocol(163, tableID, primaryKey2); // GetKeyPosition
            var outputGetKeyPosition3 = mock.Object.NotifyProtocol(163, tableID, primaryKey3); // GetKeyPosition

            // Assert
            Assert.AreEqual(1, outputAddRow1);
            Assert.AreEqual(2, outputAddRow2);
            Assert.AreEqual(3, outputAddRow3);
            Assert.AreEqual(1, outputGetKeyPosition1);
            Assert.AreEqual(2, outputGetKeyPosition2);
            Assert.AreEqual(3, outputGetKeyPosition3);
        }

        [TestMethod]
        public void GetRow_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            var rowIdentification = new object[] { tableID, primaryKey1 };

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowIdentification, null); // GetRow

            // Assert
            Assert.AreEqual(1, outputAddRow1);

            string[] row1 = { "Row 1 PK", null, null, null, null };

            outputGetRow1.Should().BeEquivalentTo(row1);
        }

        [TestMethod]
        public void SetRow_UseClearAndLeave()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            string col2 = "Row 1 2nd Col";
            string col3 = "Row 1 3rd Col";
            string col4 = "Row 1 4th Col";
            string col5 = "Row 1 5th Col";

            object[] rowDetails = new object[] { tableID, primaryKey1, DateTime.Now, true };

            object[] rowData = new object[] { primaryKey1, col2, col3, col4, col5 };

            var outputAddRow1 = mock.Object.NotifyProtocol((int)NotifyType.AddRow, tableID, primaryKey1);
            mock.Object.NotifyProtocol((int)NotifyType.NT_SET_ROW, rowDetails, rowData);

            // Act
            rowData[1] = Constants.PROTOCOL_CLEAR;
            rowData[2] = Constants.PROTOCOL_LEAVE;
            rowData[3] = "Row 1 4th Col new value";

            mock.Object.NotifyProtocol((int)NotifyType.NT_SET_ROW, rowDetails, rowData);

            // Assert
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol((int)NotifyType.NT_GET_ROW, rowDetails, null);

            Assert.AreEqual(1, outputAddRow1);

            string[] row = { "Row 1 PK", null, "Row 1 3rd Col", "Row 1 4th Col new value", "Row 1 5th Col" };

            outputGetRow1.Should().BeEquivalentTo(row);
        }

        [TestMethod]
        public void FillArray_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey0 = "Row 1.1";
            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";

            var rowDetails0 = new object[] { tableID, primaryKey0 };
            var rowDetails1 = new object[] { tableID, primaryKey1 };
            var rowDetails2 = new object[] { tableID, primaryKey2 };

            var column1 = new object[] { primaryKey1, primaryKey2 };

            var column2 = new object[] { "A", "B" };
            var column3 = new object[] { "C", "D" };
            var column4 = new object[] { "E", "F" };
            var column5 = new object[] { "G", "H" };

            var tableContent = new object[] { column1, column2, column3, column4, column5 };

            var rowData = new object[] { primaryKey0, "Row 1.2", "Row 1.3", "Row 1.4", "Row 1.5" };

            // Act
            mock.Object.NotifyProtocol(149, tableID, primaryKey0); // AddRow
            mock.Object.NotifyProtocol(225, rowDetails0, rowData); // SetRow
            mock.Object.NotifyProtocol(193, tableID, tableContent); // FillArray
            var outputGetRow0 = (object[])mock.Object.NotifyProtocol(215, rowDetails0, null); // GetRow
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowDetails1, null); // GetRow
            var outputGetRow2 = (object[])mock.Object.NotifyProtocol(215, rowDetails2, null); // GetRow

            // Assert
            object[] row0 = { null, null, null, null, null };
            outputGetRow0.Should().BeEquivalentTo(row0);

            string[] row1 = { "Row 1 PK", "A", "C", "E", "G" };
            outputGetRow1.Should().BeEquivalentTo(row1);

            string[] row2 = { "Row 2 PK", "B", "D", "F", "H" };
            outputGetRow2.Should().BeEquivalentTo(row2);
        }

        [TestMethod]
        public void FillArrayNoDelete_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey0 = "Row 1.1";
            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";

            var rowDetails0 = new object[] { tableID, primaryKey0 };
            var rowDetails1 = new object[] { tableID, primaryKey1 };
            var rowDetails2 = new object[] { tableID, primaryKey2 };

            var column1 = new object[] { primaryKey1, primaryKey2 };
            var column2 = new object[] { "A", "B" };
            var column3 = new object[] { "C", "D" };
            var column4 = new object[] { "E", "F" };
            var column5 = new object[] { "G", "H" };

            var tableContent = new object[] { column1, column2, column3, column4, column5 };

            var rowData = new object[] { primaryKey0, "Row 1.2", "Row 1.3", "Row 1.4", "Row 1.5" };

            // Act
            mock.Object.NotifyProtocol(149, tableID, primaryKey0); // AddRow
            mock.Object.NotifyProtocol(225, rowDetails0, rowData); // SetRow
            mock.Object.NotifyProtocol(194, tableID, tableContent); //FillArrayNoDelete
            var outputGetRow0 = (object[])mock.Object.NotifyProtocol(215, rowDetails0, null); // GetRow
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowDetails1, null); // GetRow
            var outputGetRow2 = (object[])mock.Object.NotifyProtocol(215, rowDetails2, null); // GetRow

            // Assert
            object[] row0 = { "Row 1.1", "Row 1.2", "Row 1.3", "Row 1.4", "Row 1.5" };
            outputGetRow0.Should().BeEquivalentTo(row0);

            string[] row1 = { "Row 1 PK", "A", "C", "E", "G" };
            outputGetRow1.Should().BeEquivalentTo(row1);

            string[] row2 = { "Row 2 PK", "B", "D", "F", "H" };
            outputGetRow2.Should().BeEquivalentTo(row2);
        }

        [TestMethod]
        public void FillArrayWithColumn_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";

            var rowDetails1 = new object[] { tableID, primaryKey1 };
            var rowDetails2 = new object[] { tableID, primaryKey2 };

            int columnID1 = 902;
            int columnID2 = 905;

            var columnInfo1 = new object[] { tableID, columnID1 };
            var columnInfo2 = new object[] { tableID, columnID2 };

            var primaryKeys = new object[] { "Row 1 PK", "Row 2 PK" };

            var columnValues1 = new object[] { "A", "B" };
            var columnValues2 = new object[] { "G", "H" };

            var values1 = new object[] { primaryKeys, columnValues1 };
            var values2 = new object[] { primaryKeys, columnValues2 };

            // Act
            mock.Object.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, columnInfo1, values1);
            mock.Object.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, columnInfo2, values2);
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowDetails1, null); // GetRow
            var outputGetRow2 = (object[])mock.Object.NotifyProtocol(215, rowDetails2, null); // GetRow

            // Assert
            string[] row1 = { "Row 1 PK", "A", null, null, "G" };
            outputGetRow1.Should().BeEquivalentTo(row1);

            string[] row2 = { "Row 2 PK", "B", null, null, "H" };
            outputGetRow2.Should().BeEquivalentTo(row2);
        }

        [TestMethod]
        public void InexistentNotifyProtocol_Exception()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int viewID = 10045;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.NotifyProtocol(303 /*NT_GET_VIEW_NAME*/, viewID, null));
        }

        [TestMethod]
        public void GetTableColumns_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            uint[] columnIndexes = new uint[] { 0, 1 };

            object[] row0 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };
            object[] row1 = new object[] { "skyline2", "2ndColumnSkyline2", "4", "5", "6" };
            object[] row2 = new object[] { "skyline3", "2ndColumnSkyline3", "7", "8", "9" };
            object[] row3 = new object[] { "skyline4", "2ndColumnSkyline4", "10", "11", "12" };
            object[] row4 = new object[] { "skyline5", "2ndColumnSkyline5", "13", "14", "15" };

            var rowDetails0 = new object[] { tableID, "skyline1" };
            var rowDetails1 = new object[] { tableID, "skyline2" };
            var rowDetails2 = new object[] { tableID, "skyline3" };
            var rowDetails3 = new object[] { tableID, "skyline4" };
            var rowDetails4 = new object[] { tableID, "skyline5" };

            // Act
            mock.Object.NotifyProtocol(149, tableID, "skyline1"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline2"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline3"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline4"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline5"); // AddRow

            mock.Object.NotifyProtocol(225, rowDetails0, row0); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails1, row1); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails2, row2); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails3, row3); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails4, row4); // SetRow

            var columns = (object[])mock.Object.NotifyProtocol(321, tableID, columnIndexes); // NT_GET_TABLE_COLUMNS

            // Assert
            string[] col1 = { "skyline1", "skyline2", "skyline3", "skyline4", "skyline5" };
            string[] col2 = { "2ndColumnSkyline1", "2ndColumnSkyline2", "2ndColumnSkyline3", "2ndColumnSkyline4", "2ndColumnSkyline5" };

            object[] expectedColumns = { col1, col2 };

            columns.Should().BeEquivalentTo(expectedColumns);
        }

        [TestMethod]
        public void GetTableColumns_InexistentIdx_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int tableID = 900;

            uint[] columnIndexes = new uint[] { 0, 10 };

            object[] row0 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };
            object[] row1 = new object[] { "skyline2", "2ndColumnSkyline2", "4", "5", "6" };
            object[] row2 = new object[] { "skyline3", "2ndColumnSkyline3", "7", "8", "9" };
            object[] row3 = new object[] { "skyline4", "2ndColumnSkyline4", "10", "11", "12" };
            object[] row4 = new object[] { "skyline5", "2ndColumnSkyline5", "13", "14", "15" };

            var rowDetails0 = new object[] { tableID, "skyline1" };
            var rowDetails1 = new object[] { tableID, "skyline2" };
            var rowDetails2 = new object[] { tableID, "skyline3" };
            var rowDetails3 = new object[] { tableID, "skyline4" };
            var rowDetails4 = new object[] { tableID, "skyline5" };

            // Act
            mock.Object.NotifyProtocol(149, tableID, "skyline1"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline2"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline3"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline4"); // AddRow
            mock.Object.NotifyProtocol(149, tableID, "skyline5"); // AddRow

            mock.Object.NotifyProtocol(225, rowDetails0, row0); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails1, row1); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails2, row2); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails3, row3); // SetRow
            mock.Object.NotifyProtocol(225, rowDetails4, row4); // SetRow

            var columns = (object[])mock.Object.NotifyProtocol(321, tableID, columnIndexes); // NT_GET_TABLE_COLUMNS

            // Assert
            string[] col1 = { "skyline1", "skyline2", "skyline3", "skyline4", "skyline5" };

            object[] expectedColumns = { col1, null };

            columns.Should().BeEquivalentTo(expectedColumns);
        }

        [TestMethod]
        [DataRow(null)] // Not an array
        [DataRow(1)] // Not an array
        [DataRow("1")] // Not an array
        [DataRow(new[] { 1 })] // Incorrect array length
        [DataRow(new[] { 1, 2 })] // Incorrect array length
        [DataRow(new[] { 1, 2, 3, 4 })] // Incorrect array length
        [DataRow((object)new object[] { "1", 2, 3 })] // First item not int
        [DataRow((object)new object[] { 1, 2, "3" })] // Third item not int
        public void GetParameterIndex_InvalidSecondArgument_ThrowsException(object notifyProtocolSecondArgument)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            mock.Object.AddRow(tableID, primaryKey1);
            mock.Object.AddRow(tableID, "Row 2 PK");
            mock.Object.AddRow(tableID, "Row 3 PK");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => mock.Object.NotifyProtocol((int)NotifyType.GetParameterIndex, notifyProtocolSecondArgument, null));
        }

        [TestMethod]
        [DataRow(1, 1, "Row 1 PK")]
        [DataRow(1, 2, "value 1.1")]
        [DataRow(1, 3, "value 1.2")]
        [DataRow(1, 4, "value 1.3")]
        [DataRow(1, 5, "value 1.4")]
        [DataRow(2, 1, "Row 2 PK")]
        [DataRow(2, 2, "value 2.1")]
        [DataRow(2, 3, "value 2.2")]
        [DataRow(2, 4, "value 2.3")]
        [DataRow(2, 5, "value 2.4")]
        [DataRow(3, 1, "Row 3 PK")]
        [DataRow(3, 2, "value 3.1")]
        [DataRow(3, 3, "value 3.2")]
        [DataRow(3, 4, "value 3.3")]
        [DataRow(3, 5, "value 3.4")]
        [DataRow("Row 1 PK", 1, "Row 1 PK")]
        [DataRow("Row 1 PK", 2, "value 1.1")]
        [DataRow("Row 1 PK", 3, "value 1.2")]
        [DataRow("Row 1 PK", 4, "value 1.3")]
        [DataRow("Row 1 PK", 5, "value 1.4")]
        [DataRow("Row 2 PK", 1, "Row 2 PK")]
        [DataRow("Row 2 PK", 2, "value 2.1")]
        [DataRow("Row 2 PK", 3, "value 2.2")]
        [DataRow("Row 2 PK", 4, "value 2.3")]
        [DataRow("Row 2 PK", 5, "value 2.4")]
        [DataRow("Row 3 PK", 1, "Row 3 PK")]
        [DataRow("Row 3 PK", 2, "value 3.1")]
        [DataRow("Row 3 PK", 3, "value 3.2")]
        [DataRow("Row 3 PK", 4, "value 3.3")]
        [DataRow("Row 3 PK", 5, "value 3.4")]
        public void GetParameterIndex(object rowIndicator, int oneBasedColumnIndex, string expectedCellValue)
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            mock.Object.AddRow(tableID, new[] { primaryKey1, "value 1.1", "value 1.2", "value 1.3", "value 1.4" });
            mock.Object.AddRow(tableID, new[] { "Row 2 PK", "value 2.1", "value 2.2", "value 2.3", "value 2.4" });
            mock.Object.AddRow(tableID, new[] { "Row 3 PK", "value 3.1", "value 3.2", "value 3.3", "value 3.4" });

            // Act & Assert
            var cellValue = mock.Object.NotifyProtocol((int)NotifyType.GetParameterIndex, new[] { tableID, rowIndicator, oneBasedColumnIndex }, null);

            // Assert
            Assert.AreEqual(expectedCellValue, cellValue);
        }

        [TestMethod]
        public void FillArrayWithColumn_SingleColumn_UseProtocolLeaveAndClear()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            mock.Object.AddRow(tableID, new[] { primaryKey1, "value 1.1", "value 1.2", "value 1.3", "value 1.4" });
            mock.Object.AddRow(tableID, new[] { "Row 2 PK", "value 2.1", "value 2.2", "value 2.3", "value 2.4" });
            mock.Object.AddRow(tableID, new[] { "Row 3 PK", "value 3.1", "value 3.2", "value 3.3", "value 3.4" });

            object[] info = new object[] { tableID, 902, new object[] { true } }; // Column 902 (2nd column), with protocol_leave and protocol_clear
            object[] primaryKeys = new object[] { primaryKey1, "Row 2 PK", "Row 3 PK" };
            object[] columnValues = new object[] { Constants.PROTOCOL_LEAVE, "new value 2.1", Constants.PROTOCOL_CLEAR };

            // Act
            mock.Object.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, info, new object[] { primaryKeys, columnValues });

            // Assert
            var firstColumn = mock.Assert().Table(tableID).Column(902);

            Assert.AreEqual("value 1.1", firstColumn[0]); // Not changed because of protocol_leave
            Assert.AreEqual("new value 2.1", firstColumn[1]); // Changed
            Assert.AreEqual(null, firstColumn[2]); // Cleared because of protocol_clear
        }


        [TestMethod]
        public void FillArrayWithColumn_MultipleColumns_UseProtocolLeaveAndClear()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            int tableID = 900;

            mock.Object.AddRow(tableID, new[] { "Row 1 PK", "value 1.1", "value 1.2", "value 1.3", "value 1.4" });
            mock.Object.AddRow(tableID, new[] { "Row 2 PK", "value 2.1", "value 2.2", "value 2.3", "value 2.4" });
            mock.Object.AddRow(tableID, new[] { "Row 3 PK", "value 3.1", "value 3.2", "value 3.3", "value 3.4" });

            object[] info = new object[] { tableID, 902, 904, new object[] { true } }; // Column 902 & 903, with protocol_leave and protocol_clear
            object[] primaryKeys = new object[] { "Row 1 PK", "Row 2 PK", "Row 3 PK" };
            object[] column1Values = new object[] { Constants.PROTOCOL_LEAVE, "new value 2.1",  Constants.PROTOCOL_CLEAR };
            object[] column2Values = new object[] { Constants.PROTOCOL_LEAVE, "new value 2.3", Constants.PROTOCOL_CLEAR };

            // Act
            mock.Object.NotifyProtocol((int)NotifyType.NT_FILL_ARRAY_WITH_COLUMN, info, new object[] { primaryKeys, column1Values, column2Values });

            // Assert
            var firstColumn = mock.Assert().Table(tableID).Column(902);
            var secondColumn = mock.Assert().Table(tableID).Column(904);

            Assert.AreEqual("value 1.1", firstColumn[0]); // Not changed because of protocol_leave
            Assert.AreEqual("new value 2.1", firstColumn[1]); // Changed
            Assert.AreEqual(null, firstColumn[2]); // Cleared because of protocol_clear


            Assert.AreEqual("value 1.3", secondColumn[0]); // Not changed because of protocol_leave
            Assert.AreEqual("new value 2.3", secondColumn[1]); // Changed
            Assert.AreEqual(null, secondColumn[2]); // Cleared because of protocol_clear
        }
    }
}