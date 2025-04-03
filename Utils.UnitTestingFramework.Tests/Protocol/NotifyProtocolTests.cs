namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class NotifyProtocolTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void NotifyProtocolTest_GetParameter_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            // Act
            var output = mock.Object.NotifyProtocol(73, 1000, null); // GetParameter

            // Assert
            Assert.AreEqual(10, output);
        }

        [TestMethod]
        public void NotifyProtocolTest_GetParameterByName_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            // Act
            var output = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(10, output);
        }

        [TestMethod]
        public void NotifyProtocolTest_SetParameter_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            uint dmaID = 346;
            uint elementID = 801;
            uint parameterID = 1000;

            uint[] ids = new uint[] { dmaID, elementID, parameterID };
            int value = 50;

            // Act
            var outputSet = mock.Object.NotifyProtocol(50, ids, value); // SetParameter
            var outputGet = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.AreEqual(value, outputGet);
        }

        [TestMethod]
        public void NotifyProtocolTest_SetParameterByName_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            string parameterName = "NumericParameter";
            int value = 50;

            // Act
            var outputSet = mock.Object.NotifyProtocol(84, parameterName, value); // SetParameterByName
            var outputGet = mock.Object.NotifyProtocol(85, "NumericParameter", null); // GetParameterByName

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.AreEqual(value, outputGet);
        }

        [TestMethod]
        public void NotifyProtocolTest_AddRow_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            int tableID = 900;
            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";
            string primaryKey3 = "Row 3 PK";
            string primaryKey4 = "Row 4 PK";

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputAddRow2 = mock.Object.NotifyProtocol(149, tableID, primaryKey2); // AddRow
            var outputAddRow3 = mock.Object.NotifyProtocol(149, tableID, primaryKey3); // AddRow
            var outputAddRow4 = mock.Object.NotifyProtocol(149, tableID, primaryKey4); // AddRow

            // Assert
            Assert.AreEqual(1, outputAddRow1);
            Assert.AreEqual(2, outputAddRow2);
            Assert.AreEqual(3, outputAddRow3);
            Assert.AreEqual(4, outputAddRow4);
        }

        [TestMethod]
        public void NotifyProtocolTest_AddRepeatedRow_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            int tableID = 900;
            string primaryKey1 = "Row 1 PK";
            string primaryKey2 = "Row 2 PK";
            string primaryKey3 = "Row 3 PK";
            string primaryKey4 = "Row 1 PK";

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputAddRow2 = mock.Object.NotifyProtocol(149, tableID, primaryKey2); // AddRow
            var outputAddRow3 = mock.Object.NotifyProtocol(149, tableID, primaryKey3); // AddRow
            var outputAddRow4 = mock.Object.NotifyProtocol(149, tableID, primaryKey4); // AddRow

            // Assert
            Assert.AreEqual(1, outputAddRow1);
            Assert.AreEqual(2, outputAddRow2);
            Assert.AreEqual(3, outputAddRow3);
            Assert.AreEqual(1, outputAddRow4);
        }

        [TestMethod]
        public void NotifyProtocolTest_AddRowReturnKey_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_DeleteRow_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputDeleteRow1 = mock.Object.NotifyProtocol(156, tableID, primaryKey1); // DeleteRow

            // Assert
            Assert.AreEqual(1, outputAddRow1);
            Assert.AreEqual(0, outputDeleteRow1);
        }

        [TestMethod]
        public void NotifyProtocolTest_Exists_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_GetKeyPosition_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_GetRow_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_SetRow_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            int tableID = 900;

            string primaryKey1 = "Row 1 PK";

            string col2 = "Row 1 2nd Col";
            string col3 = "Row 1 3rd Col";
            string col4 = "Row 1 4th Col";
            string col5 = "Row 1 5th Col";

            object[] rowDetails = new object[] { tableID, primaryKey1 };

            object[] rowData = new object[] { null, col2, col3, col4, col5 };

            // Act
            var outputAddRow1 = mock.Object.NotifyProtocol(149, tableID, primaryKey1); // AddRow
            var outputSetRow1 = (int[])mock.Object.NotifyProtocol(225, rowDetails, rowData); // SetRow
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowDetails, null); // GetRow

            // Assert
            Assert.AreEqual(1, outputAddRow1);

            int[] setOutputArray = { 0, 1, 1, 1, 1 };

            outputSetRow1.Should().BeEquivalentTo(setOutputArray);

            string[] row = { "Row 1 PK", "Row 1 2nd Col", "Row 1 3rd Col", "Row 1 4th Col", "Row 1 5th Col" };

            outputGetRow1.Should().BeEquivalentTo(row);
        }

        [TestMethod]
        public void NotifyProtocolTest_FillArray_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_FillArrayNoDelete_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_FillArrayWithColumn_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
            mock.Object.NotifyProtocol(220, columnInfo1, values1); // FillArrayWithColumn
            mock.Object.NotifyProtocol(220, columnInfo2, values2); // FillArrayWithColumn
            var outputGetRow1 = (object[])mock.Object.NotifyProtocol(215, rowDetails1, null); // GetRow
            var outputGetRow2 = (object[])mock.Object.NotifyProtocol(215, rowDetails2, null); // GetRow

            // Assert
            string[] row1 = { "Row 1 PK", "A", null, null, "G" };
            outputGetRow1.Should().BeEquivalentTo(row1);

            string[] row2 = { "Row 2 PK", "B", null, null, "H" };
            outputGetRow2.Should().BeEquivalentTo(row2);
        }

        [TestMethod]
        public void NotifyProtocolTest_InexistentNotifyProtocol_Exception()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

            int viewID = 10045;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.NotifyProtocol(303 /*NT_GET_VIEW_NAME*/, viewID, null));
        }

        [TestMethod]
        public void NotifyProtocolTest_GetTableColumns_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
        public void NotifyProtocolTest_GetTableColumnsInexistentIdx_IsEqual()
        {
            // Arrange
            var protocolModel = new ProtocolModelExt(path);
            var mock = new SLProtocolMock(protocolModel);

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
    }
}