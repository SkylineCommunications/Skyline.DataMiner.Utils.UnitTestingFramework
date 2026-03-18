namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class ProtocolMock_TableMethods_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetParameterIndexByKeyTest()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            var row1 = new object[] { "one", "one", 3, 4, 5 };
            var row2 = new object[] { "two", "two", 6, 7, 8 };

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Act
            var rowsNumber = mock.Object.GetParameterIndexByKey(900, "one", 3);
            var rowsString = mock.Object.GetParameterIndexByKey(900, "two", 2);

            // Assert
            Assert.AreEqual(3, rowsNumber);
            Assert.AreEqual("two", rowsString);
        }

        [TestMethod]
        public void SetParameterIndexByKeyTest()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            var row1 = new object[] { "one", "one", 3, 4, 5 };
            var row2 = new object[] { "two", "two", 6, 7, 8 };

            var row1ColumnIdx = 2;
            var row2ColumnIdx = 3;

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Act
            mock.Object.SetParameterIndexByKey(900, "one", row1ColumnIdx, "one-changed");
            mock.Object.SetParameterIndexByKey(900, "two", row2ColumnIdx, 99);


            mock.Object.GetParameterIndexByKey(900, "one", row1ColumnIdx);

            var valueRow1 = mock.Object.GetParameterIndexByKey(900, "one", row1ColumnIdx);
            var valueRow2 = mock.Object.GetParameterIndexByKey(900, "two", row2ColumnIdx);

            // Assert
            Assert.AreEqual("one-changed", valueRow1);
            Assert.AreEqual(99, valueRow2);
        }

        [TestMethod]
        public void SetParametersIndexByKeyTest()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            var row1 = new object[] { "one", "one", 3, 4, 5 };
            var row2 = new object[] { "two", "two", 6, 7, 8 };

            var row1ColumnIdx = 2;
            var row2ColumnIdx = 3;

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Act
            mock.Object.SetParametersIndexByKey(
                new int[] { 900, 900 },
                new string[] { "one", "two" },
                new int[] { row1ColumnIdx, row2ColumnIdx },
                new object[] { "one-changed", 99 });

            var valueRow1 = mock.Object.GetParameterIndexByKey(900, "one", row1ColumnIdx);
            var valueRow2 = mock.Object.GetParameterIndexByKey(900, "two", row2ColumnIdx);

            // Assert
            Assert.AreEqual("one-changed", valueRow1);
            Assert.AreEqual(99, valueRow2);
        }

        [TestMethod]
        public void AddRowTest_ValidPKRowArray_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, row1);
            var rowsNumber2 = mock.Object.AddRow(900, row2);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_DuplicatedPKInRowArray_OnlyAddsFirst()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "one", "notTwo", 6, 7, 8 };

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, row1);
            var rowsNumber2 = mock.Object.AddRow(900, row2);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(1, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_InexistentTableIdRowArray_DoesNotAdd()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.AddRow(800, row1));
        }

        [TestMethod]
        public void AddRowTest_MoreEntriesThanColumnsRowArray_CorrectAdds()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "one", "two", 3, 4, 5, 6, 7 };

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, row);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
        }

        [TestMethod]
        public void AddRowTest_LessEntriesThanColumnsRowArray_CorrectAdds()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "one", "two" };

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, row);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
        }

        [TestMethod]
        public void AddRowTest_ValidOnlyPK_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            var rowsNumber2 = mock.Object.AddRow(900, "skyline2");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_DuplicatedPK_OnlyAddsFirst()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            var rowsNumber2 = mock.Object.AddRow(900, "skyline1");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(1, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_InexistentTableId_DoesNotAdd()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.AddRow(800, "skyline1"));
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_PKExists()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            var exists = mock.Object.Exists(900, "skyline1");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_InvalidTableId()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.Exists(800, "skyline1"));
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_PKDoesNotExist()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            var exists = mock.Object.Exists(900, "skyline2");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_ValidPK_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7, 8 };

            // Act
            var primaryKey1 = mock.Object.AddRowReturnKey(900, row1);
            var primaryKey2 = mock.Object.AddRowReturnKey(900, row2);

            // Assert
            Assert.AreEqual("one", primaryKey1);
            Assert.AreEqual("notOne", primaryKey2);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_DuplicatedPK_OnlyAddsFirst()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act
            var primaryKey1 = mock.Object.AddRowReturnKey(900, row1);
            var primaryKey2 = mock.Object.AddRowReturnKey(900, row1);

            // Assert
            Assert.AreEqual("one", primaryKey1);
            Assert.AreEqual("one", primaryKey2);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_InexistentTableId_DoesNotAdd()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.AddRowReturnKey(800, row1));
        }

        [TestMethod]
        public void AddRowReturnKeyTest_MoreEntriesThanColumns_CorrectAdds()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "one", "two", 3, 4, 5, 6, 7 };

            // Act
            var primaryKey = mock.Object.AddRowReturnKey(900, row);

            // Assert
            Assert.AreEqual("one", primaryKey);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_LessEntriesThanColumns_CorrectAdds()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "one", "two" };

            // Act
            var primaryKey = mock.Object.AddRowReturnKey(900, row);

            // Assert
            Assert.AreEqual("one", primaryKey);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteOnlyFirstIndex_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow = mock.Object.DeleteRow(900, 0);

            object[] remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow);

            Assert.AreEqual("two", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.AreEqual("four", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteOnlySecondIndex_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            int remainingRowCount = mock.Object.DeleteRow(900, 1);

            var remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, remainingRowCount);

            Assert.AreEqual("one", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.AreEqual("four", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteTwoIndexes_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow1 = mock.Object.DeleteRow(900, 0);
            var deleteRow2 = mock.Object.DeleteRow(900, 1);

            var remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);
            Assert.AreEqual(2, deleteRow2);

            Assert.AreEqual("two", remainingRows1[0]);
            Assert.AreEqual("four", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteThreeIndexes_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 9, 10, 11 };
            object[] row4 = new object[] { "four", "four", 12, 13, 14 };
            object[] row5 = new object[] { "five", "five", 15, 16, 17 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);
            var pk5 = mock.Object.AddRowReturnKey(900, row5);

            var deleteRow1 = mock.Object.DeleteRow(900, 2);

            var remainingRows1_1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_1 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_1 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_1 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_1 = (object[])mock.Object.GetRow(900, 4);

            var deleteRow2 = mock.Object.DeleteRow(900, 2);

            var remainingRows1_2 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_2 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_2 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_2 = (object[])mock.Object.GetRow(900, 4);

            var deleteRow3 = mock.Object.DeleteRow(900, 2);

            var remainingRows1_3 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_3 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_3 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_3 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);
            Assert.AreEqual("five", pk5);

            Assert.AreEqual(4, deleteRow1);

            Assert.AreEqual("one", remainingRows1_1[0]);
            Assert.AreEqual("two", remainingRows2_1[0]);
            Assert.AreEqual("four", remainingRows3_1[0]);
            Assert.AreEqual("five", remainingRows4_1[0]);
            Assert.IsNull(remainingRows5_1[0]);

            Assert.AreEqual(3, deleteRow2);

            Assert.AreEqual("one", remainingRows1_2[0]);
            Assert.AreEqual("two", remainingRows2_2[0]);
            Assert.AreEqual("five", remainingRows3_2[0]);
            Assert.IsNull(remainingRows4_2[0]);
            Assert.IsNull(remainingRows5_2[0]);

            Assert.AreEqual(2, deleteRow3);

            Assert.AreEqual("one", remainingRows1_3[0]);
            Assert.AreEqual("two", remainingRows2_3[0]);
            Assert.IsNull(remainingRows3_3[0]);
            Assert.IsNull(remainingRows4_3[0]);
            Assert.IsNull(remainingRows5_3[0]);
        }

        [TestMethod]
        public void DeleteRowTest_InexistentTableId_ReturnsZero()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.DeleteRow(800, 0));
        }

        [TestMethod]
        public void DeleteRowTest_InexistentRowIndex_ReturnsRowCount()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };

            // Act
            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var deleteRow = mock.Object.DeleteRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual(1, deleteRow);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteOnlyFirstIndex_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow = mock.Object.DeleteRow(900, "one");

            object[] remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow);

            Assert.AreEqual("two", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.AreEqual("four", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteOnlySecondIndex_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow1 = mock.Object.DeleteRow(900, "two");

            var remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);

            Assert.AreEqual("one", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.AreEqual("four", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteTwoIndexes_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow1 = mock.Object.DeleteRow(900, "one");
            var deleteRow2 = mock.Object.DeleteRow(900, "two");

            var remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);
            Assert.AreEqual(2, deleteRow2);

            Assert.AreEqual("three", remainingRows1[0]);
            Assert.AreEqual("four", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteThreeIndexes_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 9, 10, 11 };
            object[] row4 = new object[] { "four", "four", 12, 13, 14 };
            object[] row5 = new object[] { "five", "five", 15, 16, 17 };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);
            var pk5 = mock.Object.AddRowReturnKey(900, row5);

            var deleteRow1 = mock.Object.DeleteRow(900, "three");

            var remainingRows1_1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_1 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_1 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_1 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_1 = (object[])mock.Object.GetRow(900, 4);

            var deleteRow2 = mock.Object.DeleteRow(900, "five");

            var remainingRows1_2 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_2 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_2 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_2 = (object[])mock.Object.GetRow(900, 4);

            var deleteRow3 = mock.Object.DeleteRow(900, "four");

            var remainingRows1_3 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2_3 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3_3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4_3 = (object[])mock.Object.GetRow(900, 3);
            var remainingRows5_3 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);
            Assert.AreEqual("five", pk5);

            Assert.AreEqual(4, deleteRow1);

            Assert.AreEqual("one", remainingRows1_1[0]);
            Assert.AreEqual("two", remainingRows2_1[0]);
            Assert.AreEqual("four", remainingRows3_1[0]);
            Assert.AreEqual("five", remainingRows4_1[0]);
            Assert.IsNull(remainingRows5_1[0]);

            Assert.AreEqual(3, deleteRow2);

            Assert.AreEqual("one", remainingRows1_2[0]);
            Assert.AreEqual("two", remainingRows2_2[0]);
            Assert.AreEqual("four", remainingRows3_2[0]);
            Assert.IsNull(remainingRows4_2[0]);
            Assert.IsNull(remainingRows5_2[0]);

            Assert.AreEqual(2, deleteRow3);

            Assert.AreEqual("one", remainingRows1_3[0]);
            Assert.AreEqual("two", remainingRows2_3[0]);
            Assert.IsNull(remainingRows3_3[0]);
            Assert.IsNull(remainingRows4_3[0]);
            Assert.IsNull(remainingRows5_3[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_InexistentTableId_ReturnsZero()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.DeleteRow(800, "one"));
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_InexistentRowIndex_ReturnsRowCount()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };

            // Act
            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var deleteRow = mock.Object.DeleteRow(900, "seven");

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual(1, deleteRow);
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_DeleteTwoPK_EqualRowNumber()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };
            string[] toDelete = new string[] { "one", "two" };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow1 = mock.Object.DeleteRow(900, toDelete);

            var remainingRows1 = (object[])mock.Object.GetRow(900, 0);
            var remainingRows2 = (object[])mock.Object.GetRow(900, 1);
            var remainingRows3 = (object[])mock.Object.GetRow(900, 2);
            var remainingRows4 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(2, deleteRow1);

            Assert.AreEqual("three", remainingRows1[0]);
            Assert.AreEqual("four", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_DeleteInexistentTableId_ReturnsZero()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            string[] toDelete = new string[] { "one", "two", "seven" };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.DeleteRow(800, toDelete));
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_EmptyArray_ReturnsZero()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };
            string[] toDelete = new string[] { };

            // Act
            mock.Object.ClearAllKeys(900);

            var pk1 = mock.Object.AddRowReturnKey(900, row1);
            var pk2 = mock.Object.AddRowReturnKey(900, row2);
            var pk3 = mock.Object.AddRowReturnKey(900, row3);
            var pk4 = mock.Object.AddRowReturnKey(900, row4);

            var deleteRow1 = mock.Object.DeleteRow(900, toDelete);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(4, deleteRow1);
        }

        [TestMethod]
        public void ClearAllKeysTest_ValidTableId_ReturnsZero()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, row1);
            var rowsNumber2 = mock.Object.AddRow(900, row2);
            var rowsLeft = mock.Object.ClearAllKeys(900);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
            Assert.AreEqual(0, rowsLeft);
        }

        [TestMethod]
        public void ClearAllKeysTest_InexistentTableId_ReturnsNegativeOne()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.ClearAllKeys(800));
        }

        [TestMethod]
        public void ClearAllKeysTest_EmptyTable_ReturnsZero()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var rowsLeft = mock.Object.ClearAllKeys(900);

            // Assert
            Assert.AreEqual(0, rowsLeft);
        }

        [TestMethod]
        public void GetKeyPositionTest_ValidOnlyPK_EqualKeyPosition()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            var rowsNumber2 = mock.Object.AddRow(900, "skyline2");
            var rowsNumber3 = mock.Object.AddRow(900, "skyline3");
            var rowsNumber4 = mock.Object.AddRow(900, "skyline4");

            var keyPosition1 = mock.Object.GetKeyPosition(900, "skyline1");
            var keyPosition2 = mock.Object.GetKeyPosition(900, "skyline2");
            var keyPosition3 = mock.Object.GetKeyPosition(900, "skyline3");
            var keyPosition4 = mock.Object.GetKeyPosition(900, "skyline4");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
            Assert.AreEqual(3, rowsNumber3);
            Assert.AreEqual(4, rowsNumber4);

            Assert.AreEqual(1, keyPosition1);
            Assert.AreEqual(2, keyPosition2);
            Assert.AreEqual(3, keyPosition3);
            Assert.AreEqual(4, keyPosition4);
        }

        [TestMethod]
        public void GetKeyPositionTest_InvalidTableId_ReturnsZero()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.GetKeyPosition(800, "skyline1"));
        }

        [TestMethod]
        public void GetKeyPositionTest_InexistentKey_ReturnsZero()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.GetKeyPosition(800, "skyline2"));
        }

        [TestMethod]
        public void SetRowWithIndexTest_InexistentTableId_ReturnsNull()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.SetRow(800, 0, row));
        }

        [TestMethod]
        public void SetRowWithIndexTest_InexistentRowIndex_ReturnsArrayWithZeros()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act
            mock.Object.ClearAllKeys(900);

            var rowsNumber1 = mock.Object.AddRow(900, "skyline1");
            int[] changes = (int[])mock.Object.SetRow(900, 10, row);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(0, changes[1]);
            Assert.AreEqual(0, changes[2]);
            Assert.AreEqual(0, changes[3]);
            Assert.AreEqual(0, changes[4]);
        }

        [TestMethod]
        public void SetRowWithIndexTest_LessEntriesThanColumns_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 10 };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            int[] changes = (int[])mock.Object.SetRow(900, 0, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, 0);

            // Assert
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(2, changes[1]);
            Assert.AreEqual(1, changes[2]);
            Assert.AreEqual("skyline1", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput[1]);
            Assert.AreEqual(10, rowOutput[2]);
            Assert.AreEqual(2, rowOutput[3]);
            Assert.AreEqual(3, rowOutput[4]);
        }

        [TestMethod]
        public void SetRowWithIndexTest_TryChangePK_DoesNotChangePK()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline1", 10 };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            int[] changes = (int[])mock.Object.SetRow(900, 0, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, 0);

            // Assert
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(2, changes[1]);
            Assert.AreEqual(1, changes[2]);
            Assert.AreEqual("skyline1", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput[1]);
            Assert.AreEqual(10, rowOutput[2]);
            Assert.AreEqual(2, rowOutput[3]);
            Assert.AreEqual(3, rowOutput[4]);
        }

        [TestMethod]
        public void SetRowWithPKTest_InexistentTableId_ReturnsNull()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.SetRow(800, "skyline1", row));
        }

        [TestMethod]
        public void SetRowWithPKTest_InexistentRowIndex_ReturnsArrayWithZeros()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.SetRow(900, "skyline2", row));
        }

        [TestMethod]
        public void SetRowWithPKTest_LessEntriesThanColumns_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 10 };

            // Act
            mock.Object.ClearAllKeys(900);
            mock.Object.AddRow(900, row1);
            int[] changes = (int[])mock.Object.SetRow(900, "skyline1", row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, 0);

            // Assert
            Assert.AreEqual(0, changes[0]);
            Assert.AreEqual(2, changes[1]);
            Assert.AreEqual(1, changes[2]);
            Assert.AreEqual("skyline1", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput[1]);
            Assert.AreEqual(10, rowOutput[2]);
            Assert.AreEqual(2, rowOutput[3]);
            Assert.AreEqual(3, rowOutput[4]);
        }

        [TestMethod]
        public void SetRowWithPrimaryKey_EntireRowWithProtocolLeave_IsEqual()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            var row = new object[5] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            mock.Object.AddRow(900, row);

            // Act
            mock.Object.SetRow(900, "skyline1", new object[5]
            {
                mock.Object.Leave,
                mock.Object.Leave,
                mock.Object.Leave,
                mock.Object.Leave,
                mock.Object.Leave,
            }, bOverrideBehaviour: true);

            // Assert
            object[] rowOutput = (object[])mock.Object.GetRow(900, 0);
            Assert.AreEqual("skyline1", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput[1]);
            Assert.AreEqual(1, rowOutput[2]);
            Assert.AreEqual(2, rowOutput[3]);
            Assert.AreEqual(3, rowOutput[4]);
        }

        [TestMethod]
        public void GetRowWithIndexTest_InexistentRowIndex_IsNull()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 4, 5, 6 };

            // Act
            mock.Object.ClearAllKeys(900);
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, 2);

            // Assert
            Assert.IsNull(rowOutput[0]);
            Assert.IsNull(rowOutput[1]);
            Assert.IsNull(rowOutput[2]);
            Assert.IsNull(rowOutput[3]);
            Assert.IsNull(rowOutput[4]);
        }

        [TestMethod]
        public void GetRowWithIndexTest_ValidTaleIdAndIndex_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            // Act
            mock.Object.ClearAllKeys(900);
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, 1);

            // Assert
            Assert.AreEqual("skyline2", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput[1]);
            Assert.AreEqual(4, rowOutput[2]);
            Assert.AreEqual(5, rowOutput[3]);
            Assert.AreEqual(6, rowOutput[4]);
        }

        [TestMethod]
        public void GetRowWithPKTest_InexistentPK_IsNull()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 4, 5, 6 };

            // Act
            mock.Object.ClearAllKeys(900);
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, "skyline3");

            // Assert
            Assert.IsNull(rowOutput[0]);
            Assert.IsNull(rowOutput[1]);
            Assert.IsNull(rowOutput[2]);
            Assert.IsNull(rowOutput[3]);
            Assert.IsNull(rowOutput[4]);
        }

        [TestMethod]
        public void GetRowWithPKTest_InexistentTableId_IsNull()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.GetRow(800, "skyline1"));
        }

        [TestMethod]
        public void GetRowWithPKTest_ValidTaleIdAndIndex_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            // Act
            mock.Object.ClearAllKeys(900);
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            object[] rowOutput = (object[])mock.Object.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline2", rowOutput[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput[1]);
            Assert.AreEqual(4, rowOutput[2]);
            Assert.AreEqual(5, rowOutput[3]);
            Assert.AreEqual(6, rowOutput[4]);
        }

        [TestMethod]
        public void FillArrayPartialTest_FillArrayBeforeAddRow_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };

            List<object[]> listOfRows = new List<object[]>
            {
                row3,
                row4,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            mock.Object.AddRow(900, row5);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("skyline3", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline3", rowOutput0[1]);
            Assert.AreEqual(13, rowOutput0[2]);
            Assert.AreEqual(14, rowOutput0[3]);
            Assert.AreEqual(15, rowOutput0[4]);

            Assert.AreEqual("skyline4", rowOutput1[0]);
            Assert.AreEqual("2ndColumnSkyline4", rowOutput1[1]);
            Assert.AreEqual(16, rowOutput1[2]);
            Assert.AreEqual(17, rowOutput1[3]);
            Assert.AreEqual(18, rowOutput1[4]);

            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput2[1]);
            Assert.AreEqual(1, rowOutput2[2]);
            Assert.AreEqual(2, rowOutput2[3]);
            Assert.AreEqual(3, rowOutput2[4]);

            Assert.AreEqual("skyline2", rowOutput3[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput3[1]);
            Assert.AreEqual(10, rowOutput3[2]);
            Assert.AreEqual(11, rowOutput3[3]);
            Assert.AreEqual(12, rowOutput3[4]);

            Assert.AreEqual("skyline5", rowOutput4[0]);
            Assert.AreEqual("2ndColumnSkyline5", rowOutput4[1]);
            Assert.AreEqual(19, rowOutput4[2]);
            Assert.AreEqual(20, rowOutput4[3]);
            Assert.AreEqual(21, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayPartialTest_FillArrayAfterAddRow_ReplaceRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
            object[] row6 = new object[] { "skyline1", "2ndColumnSkyline1", 6, 6, 6 };

            List<object[]> listOfRows = new List<object[]>
            {
                row3,
                row4,
                row6,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            mock.Object.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            mock.Object.AddRow(900, row5);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput0[1]);
            Assert.AreEqual(6, rowOutput0[2]);
            Assert.AreEqual(6, rowOutput0[3]);
            Assert.AreEqual(6, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput1[1]);
            Assert.AreEqual(10, rowOutput1[2]);
            Assert.AreEqual(11, rowOutput1[3]);
            Assert.AreEqual(12, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndColumnSkyline3", rowOutput2[1]);
            Assert.AreEqual(13, rowOutput2[2]);
            Assert.AreEqual(14, rowOutput2[3]);
            Assert.AreEqual(15, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndColumnSkyline4", rowOutput3[1]);
            Assert.AreEqual(16, rowOutput3[2]);
            Assert.AreEqual(17, rowOutput3[3]);
            Assert.AreEqual(18, rowOutput3[4]);

            Assert.AreEqual("skyline5", rowOutput4[0]);
            Assert.AreEqual("2ndColumnSkyline5", rowOutput4[1]);
            Assert.AreEqual(19, rowOutput4[2]);
            Assert.AreEqual(20, rowOutput4[3]);
            Assert.AreEqual(21, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArray_NoRows_PartialSave_ThrowsException()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            var rows = new List<object[]>(); // Empty list

            // Act & Assert
            Assert.Throws<Exception>(() => mock.Object.FillArray(900, rows, NotifyProtocol.SaveOption.Partial));
        }

        [TestMethod]
        public void FillArrayPartialTest_FillArrayAfterAddRow_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };

            List<object[]> listOfRows = new List<object[]>
            {
                row3,
                row4,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            mock.Object.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            mock.Object.AddRow(900, row5);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput1[1]);
            Assert.AreEqual(10, rowOutput1[2]);
            Assert.AreEqual(11, rowOutput1[3]);
            Assert.AreEqual(12, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndColumnSkyline3", rowOutput2[1]);
            Assert.AreEqual(13, rowOutput2[2]);
            Assert.AreEqual(14, rowOutput2[3]);
            Assert.AreEqual(15, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndColumnSkyline4", rowOutput3[1]);
            Assert.AreEqual(16, rowOutput3[2]);
            Assert.AreEqual(17, rowOutput3[3]);
            Assert.AreEqual(18, rowOutput3[4]);

            Assert.AreEqual("skyline5", rowOutput4[0]);
            Assert.AreEqual("2ndColumnSkyline5", rowOutput4[1]);
            Assert.AreEqual(19, rowOutput4[2]);
            Assert.AreEqual(20, rowOutput4[3]);
            Assert.AreEqual(21, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayPartialTest_FillArrayAfterAddRowChangesRow_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
            object[] row6 = new object[] { "skyline1", "2ndColumnSkyline1.2", null, null, null };

            List<object[]> listOfRows = new List<object[]>
            {
                row3,
                row4,
                row6,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            mock.Object.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            mock.Object.AddRow(900, row5);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline1.2", rowOutput0[1]);
            Assert.IsNull(rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndColumnSkyline2", rowOutput1[1]);
            Assert.AreEqual(10, rowOutput1[2]);
            Assert.AreEqual(11, rowOutput1[3]);
            Assert.AreEqual(12, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndColumnSkyline3", rowOutput2[1]);
            Assert.AreEqual(13, rowOutput2[2]);
            Assert.AreEqual(14, rowOutput2[3]);
            Assert.AreEqual(15, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndColumnSkyline4", rowOutput3[1]);
            Assert.AreEqual(16, rowOutput3[2]);
            Assert.AreEqual(17, rowOutput3[3]);
            Assert.AreEqual(18, rowOutput3[4]);

            Assert.AreEqual("skyline5", rowOutput4[0]);
            Assert.AreEqual("2ndColumnSkyline5", rowOutput4[1]);
            Assert.AreEqual(19, rowOutput4[2]);
            Assert.AreEqual(20, rowOutput4[3]);
            Assert.AreEqual(21, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayFullTest_FillArrayBeforeAddRow_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
            object[] row6 = new object[] { "skyline1", "2ndColumnSkyline1", 6, 6, 6 };

            List<object[]> listOfRows = new List<object[]>
            {
                row3,
                row4,
                row6,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);
            mock.Object.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Full);
            mock.Object.AddRow(900, row5);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);

            // Assert
            Assert.AreEqual("skyline3", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline3", rowOutput0[1]);
            Assert.AreEqual(13, rowOutput0[2]);
            Assert.AreEqual(14, rowOutput0[3]);
            Assert.AreEqual(15, rowOutput0[4]);

            Assert.AreEqual("skyline4", rowOutput1[0]);
            Assert.AreEqual("2ndColumnSkyline4", rowOutput1[1]);
            Assert.AreEqual(16, rowOutput1[2]);
            Assert.AreEqual(17, rowOutput1[3]);
            Assert.AreEqual(18, rowOutput1[4]);

            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput2[1]);
            Assert.AreEqual(6, rowOutput2[2]);
            Assert.AreEqual(6, rowOutput2[3]);
            Assert.AreEqual(6, rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayListColumnsTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArray(900, listOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput2[1]);
            Assert.AreEqual(7, rowOutput2[2]);
            Assert.AreEqual(8, rowOutput2[3]);
            Assert.AreEqual(9, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput3[1]);
            Assert.AreEqual(10, rowOutput3[2]);
            Assert.AreEqual(11, rowOutput3[3]);
            Assert.AreEqual(12, rowOutput3[4]);
        }

        [TestMethod]
        public void FillArrayListColumnsTest_FillArrayAfterTwoAddRow_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };
            object[] row2 = new object[] { "two", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", null, "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArray(900, listOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual(null, rowOutput2[1]);
            Assert.AreEqual(7, rowOutput2[2]);
            Assert.AreEqual(8, rowOutput2[3]);
            Assert.AreEqual(9, rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayListColumnsTest_FillArrayAfterTwoAddRow_ReplaceRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", null, "2ndSkyline4" };
            object[] col3 = new object[] { 1, null, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArray(900, listOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline2", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput0[1]);
            Assert.IsNull(rowOutput0[2]);
            Assert.AreEqual(5, rowOutput0[3]);
            Assert.AreEqual(6, rowOutput0[4]);
        }

        [TestMethod]
        public void FillArrayListColumnsTest_FillArrayAfterAddRowWithTimestamp_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            DateTime timestamps = new DateTime(2022, 7, 14);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArray(900, listOfCols, timestamps);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput2[1]);
            Assert.AreEqual(7, rowOutput2[2]);
            Assert.AreEqual(8, rowOutput2[3]);
            Assert.AreEqual(9, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput3[1]);
            Assert.AreEqual(10, rowOutput3[2]);
            Assert.AreEqual(11, rowOutput3[3]);
            Assert.AreEqual(12, rowOutput3[4]);
        }

        [TestMethod]
        public void FillArrayColumnsArrayTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[] { col1, col2, col3, col4, col5 };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArray(900, arrayOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput2[1]);
            Assert.AreEqual(7, rowOutput2[2]);
            Assert.AreEqual(8, rowOutput2[3]);
            Assert.AreEqual(9, rowOutput2[4]);

            Assert.AreEqual("skyline4", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput3[1]);
            Assert.AreEqual(10, rowOutput3[2]);
            Assert.AreEqual(11, rowOutput3[3]);
            Assert.AreEqual(12, rowOutput3[4]);
        }

        [TestMethod]
        public void FillArrayNoDeleteListColumnsTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArrayNoDelete(900, listOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", rowOutput0[0]);
            Assert.AreEqual("2ndOne", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(20, rowOutput0[3]);
            Assert.AreEqual(30, rowOutput0[4]);

            Assert.AreEqual("skyline1", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput1[1]);
            Assert.AreEqual(1, rowOutput1[2]);
            Assert.AreEqual(2, rowOutput1[3]);
            Assert.AreEqual(3, rowOutput1[4]);

            Assert.AreEqual("skyline2", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput2[1]);
            Assert.AreEqual(4, rowOutput2[2]);
            Assert.AreEqual(5, rowOutput2[3]);
            Assert.AreEqual(6, rowOutput2[4]);

            Assert.AreEqual("skyline3", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput3[1]);
            Assert.AreEqual(7, rowOutput3[2]);
            Assert.AreEqual(8, rowOutput3[3]);
            Assert.AreEqual(9, rowOutput3[4]);

            Assert.AreEqual("skyline4", rowOutput4[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput4[1]);
            Assert.AreEqual(10, rowOutput4[2]);
            Assert.AreEqual(11, rowOutput4[3]);
            Assert.AreEqual(12, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayNoDeleteListColumnsTest_FillArrayAfterAddRow_ReplaceRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArrayNoDelete(900, listOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", rowOutput0[0]);
            Assert.AreEqual("2ndOne", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(20, rowOutput0[3]);
            Assert.AreEqual(30, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput2[1]);
            Assert.AreEqual(1, rowOutput2[2]);
            Assert.AreEqual(2, rowOutput2[3]);
            Assert.AreEqual(3, rowOutput2[4]);

            Assert.AreEqual("skyline3", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput3[1]);
            Assert.AreEqual(7, rowOutput3[2]);
            Assert.AreEqual(8, rowOutput3[3]);
            Assert.AreEqual(9, rowOutput3[4]);

            Assert.AreEqual("skyline4", rowOutput4[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput4[1]);
            Assert.AreEqual(10, rowOutput4[2]);
            Assert.AreEqual(11, rowOutput4[3]);
            Assert.AreEqual(12, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayNoDeleteArrayColumnsTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[] { col1, col2, col3, col4, col5 };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArrayNoDelete(900, arrayOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", rowOutput0[0]);
            Assert.AreEqual("2ndOne", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(20, rowOutput0[3]);
            Assert.AreEqual(30, rowOutput0[4]);

            Assert.AreEqual("skyline1", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput1[1]);
            Assert.AreEqual(1, rowOutput1[2]);
            Assert.AreEqual(2, rowOutput1[3]);
            Assert.AreEqual(3, rowOutput1[4]);

            Assert.AreEqual("skyline2", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput2[1]);
            Assert.AreEqual(4, rowOutput2[2]);
            Assert.AreEqual(5, rowOutput2[3]);
            Assert.AreEqual(6, rowOutput2[4]);

            Assert.AreEqual("skyline3", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput3[1]);
            Assert.AreEqual(7, rowOutput3[2]);
            Assert.AreEqual(8, rowOutput3[3]);
            Assert.AreEqual(9, rowOutput3[4]);

            Assert.AreEqual("skyline4", rowOutput4[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput4[1]);
            Assert.AreEqual(10, rowOutput4[2]);
            Assert.AreEqual(11, rowOutput4[3]);
            Assert.AreEqual(12, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayNoDeleteArrayColumnsTest_FillArrayAfterAddRow_ReplaceRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[] { col1, col2, col3, col4, col5 };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArrayNoDelete(900, arrayOfCols);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, 0);
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, 1);
            object[] rowOutput2 = (object[])mock.Object.GetRow(900, 2);
            object[] rowOutput3 = (object[])mock.Object.GetRow(900, 3);
            object[] rowOutput4 = (object[])mock.Object.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", rowOutput0[0]);
            Assert.AreEqual("2ndOne", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(20, rowOutput0[3]);
            Assert.AreEqual(30, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("2ndSkyline2", rowOutput1[1]);
            Assert.AreEqual(4, rowOutput1[2]);
            Assert.AreEqual(5, rowOutput1[3]);
            Assert.AreEqual(6, rowOutput1[4]);

            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput2[1]);
            Assert.AreEqual(1, rowOutput2[2]);
            Assert.AreEqual(2, rowOutput2[3]);
            Assert.AreEqual(3, rowOutput2[4]);

            Assert.AreEqual("skyline3", rowOutput3[0]);
            Assert.AreEqual("2ndSkyline3", rowOutput3[1]);
            Assert.AreEqual(7, rowOutput3[2]);
            Assert.AreEqual(8, rowOutput3[3]);
            Assert.AreEqual(9, rowOutput3[4]);

            Assert.AreEqual("skyline4", rowOutput4[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput4[1]);
            Assert.AreEqual(10, rowOutput4[2]);
            Assert.AreEqual(11, rowOutput4[3]);
            Assert.AreEqual(12, rowOutput4[4]);
        }

        [TestMethod]
        public void FillArrayWithColumnColumnsTest_FillArrayWithSameUniqueValue_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] pk = new object[] { "skyline1", "skyline2" };

            object[] values = new object[] { "value1" };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.FillArrayWithColumn(900, 902, pk, values);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.IsNull(rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value1", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumnColumnsTest_FillArraysWithDifferentLengths_Exception()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] pk = new object[] { "skyline1", "skyline2", "skyline3" };

            object[] values = new object[] { "value1", "value2" };

            // Act
            mock.Object.ClearAllKeys(900);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.FillArrayWithColumn(900, 902, pk, values));
        }

        [TestMethod]
        public void FillArrayWithColumnColumnsTest_FillArray_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] pk = new object[] { "skyline1", "skyline2" };

            object[] values = new object[] { "value1", "value2" };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.FillArrayWithColumn(900, 902, pk, values);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.IsNull(rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumnColumnsTest_FillArray_ReplaceRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            object[] pk = new object[] { "skyline1", "skyline2" };

            object[] values = new object[] { "value1", "value2" };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.AddRow(900, row1);

            mock.Object.FillArrayWithColumn(900, 902, pk, values);

            object[] rowOutput0 = (object[])mock.Object.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])mock.Object.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual(2, rowOutput0[3]);
            Assert.AreEqual(3, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }
    }
}