namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Data
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using UnitTestingFramework.Protocol.Data;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class TablesCacheTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void AddRowTest_ValidPKRowArray_EqualRowNumber()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7, 8 };

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, row1);
            var rowsNumber2 = tablesCache.AddRow(900, row2);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_DuplicatedPKInRowArray_OnlyAddsFirst()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "one", "notTwo", 6, 7, 8 };

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, row1);
            var rowsNumber2 = tablesCache.AddRow(900, row2);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(1, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_InexistentTableIdRowArray_DoesNotAdd()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act
            Assert.Throws<Exception>(() => tablesCache.AddRow(800, row1));
        }

        [TestMethod]
        public void AddRowTest_MoreEntriesThanColumnsRowArray_CorrectAdds()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "one", "two", 3, 4, 5, 6, 7 };

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, row);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
        }

        [TestMethod]
        public void AddRowTest_LessEntriesThanColumnsRowArray_CorrectAdds()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "one", "two" };

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, row);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
        }

        [TestMethod]
        public void AddRowTest_ValidOnlyPK_EqualRowNumber()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            var rowsNumber2 = tablesCache.AddRow(900, "skyline2");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_DuplicatedPK_OnlyAddsFirst()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            var rowsNumber2 = tablesCache.AddRow(900, "skyline1");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(1, rowsNumber2);
        }

        [TestMethod]
        public void AddRowTest_InexistentTableId_DoesNotAdd()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.AddRow(800, "skyline1"));
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_PKExists()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            var exists = tablesCache.Exists(900, "skyline1");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_InvalidTableId()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act
            Assert.Throws<Exception>(() => tablesCache.Exists(800, "skyline1"));
        }

        [TestMethod]
        public void ExistsTest_ValidAddRow_PKDoesNotExist()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            var exists = tablesCache.Exists(900, "skyline2");

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_ValidPK_EqualRowNumber()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };
            object[] row2 = new object[] { "notOne", "notTwo", 6, 7, 8 };

            // Act
            var primaryKey1 = tablesCache.AddRowReturnKey(900, row1);
            var primaryKey2 = tablesCache.AddRowReturnKey(900, row2);

            // Assert
            Assert.AreEqual("one", primaryKey1);
            Assert.AreEqual("notOne", primaryKey2);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_DuplicatedPK_OnlyAddsFirst()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);

            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act
            var primaryKey1 = tablesCache.AddRowReturnKey(900, row1);
            var primaryKey2 = tablesCache.AddRowReturnKey(900, row1);

            // Assert
            Assert.AreEqual("one", primaryKey1);
            Assert.AreEqual("one", primaryKey2);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_InexistentTableId_DoesNotAdd()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "two", 3, 4, 5 };

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.AddRowReturnKey(800, row1));
        }

        [TestMethod]
        public void AddRowReturnKeyTest_MoreEntriesThanColumns_CorrectAdds()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "one", "two", 3, 4, 5, 6, 7 };

            // Act
            var primaryKey = tablesCache.AddRowReturnKey(900, row);

            // Assert
            Assert.AreEqual("one", primaryKey);
        }

        [TestMethod]
        public void AddRowReturnKeyTest_LessEntriesThanColumns_CorrectAdds()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "one", "two" };

            // Act
            var primaryKey = tablesCache.AddRowReturnKey(900, row);

            // Assert
            Assert.AreEqual("one", primaryKey);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteOnlyFirstIndex_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow = tablesCache.DeleteRow(900, 0);

            object[] remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow);

            Assert.AreEqual("four", remainingRows1[0]);
            Assert.AreEqual("two", remainingRows2[0]);
            Assert.AreEqual("three", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteOnlySecondIndex_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, 1);

            var remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);

            Assert.AreEqual("one", remainingRows1[0]);
            Assert.AreEqual("four", remainingRows2[0]);
            Assert.AreEqual("three", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteTwoIndexes_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, 0);
            var deleteRow2 = tablesCache.DeleteRow(900, 1);

            var remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);
            Assert.AreEqual(2, deleteRow2);

            Assert.AreEqual("four", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowTest_DeleteThreeIndexes_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 9, 10, 11 };
            object[] row4 = new object[] { "four", "four", 12, 13, 14 };
            object[] row5 = new object[] { "five", "five", 15, 16, 17 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);
            var pk5 = tablesCache.AddRowReturnKey(900, row5);

            var deleteRow1 = tablesCache.DeleteRow(900, 2);

            var remainingRows1_1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_1 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_1 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_1 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_1 = (object[])tablesCache.GetRow(900, 4);

            var deleteRow2 = tablesCache.DeleteRow(900, 2);

            var remainingRows1_2 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_2 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_2 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_2 = (object[])tablesCache.GetRow(900, 4);

            var deleteRow3 = tablesCache.DeleteRow(900, 2);

            var remainingRows1_3 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_3 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_3 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_3 = (object[])tablesCache.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);
            Assert.AreEqual("five", pk5);

            Assert.AreEqual(4, deleteRow1);

            Assert.AreEqual("one", remainingRows1_1[0]);
            Assert.AreEqual("two", remainingRows2_1[0]);
            Assert.AreEqual("five", remainingRows3_1[0]);
            Assert.AreEqual("four", remainingRows4_1[0]);
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
        public void DeleteRowTest_InexistentTableId_ReturnsZero()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.DeleteRow(800, 0));
        }

        [TestMethod]
        public void DeleteRowTest_InexistentRowIndex_ReturnsZero()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };

            // Act
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var deleteRow = tablesCache.DeleteRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual(0, deleteRow);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteOnlyFirstIndex_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow = tablesCache.DeleteRow(900, "one");

            object[] remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow);

            Assert.AreEqual("four", remainingRows1[0]);
            Assert.AreEqual("two", remainingRows2[0]);
            Assert.AreEqual("three", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteOnlySecondIndex_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, "two");

            var remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);

            Assert.AreEqual("one", remainingRows1[0]);
            Assert.AreEqual("four", remainingRows2[0]);
            Assert.AreEqual("three", remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteTwoIndexes_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, "one");
            var deleteRow2 = tablesCache.DeleteRow(900, "two");

            var remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(3, deleteRow1);
            Assert.AreEqual(2, deleteRow2);

            Assert.AreEqual("four", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_DeleteThreeIndexes_EqualRowNumber()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 9, 10, 11 };
            object[] row4 = new object[] { "four", "four", 12, 13, 14 };
            object[] row5 = new object[] { "five", "five", 15, 16, 17 };

            // Act
            tablesCache.ClearAllKeys(900);

            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);
            var pk5 = tablesCache.AddRowReturnKey(900, row5);

            var deleteRow1 = tablesCache.DeleteRow(900, "three");

            var remainingRows1_1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_1 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_1 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_1 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_1 = (object[])tablesCache.GetRow(900, 4);

            var deleteRow2 = tablesCache.DeleteRow(900, "five");

            var remainingRows1_2 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_2 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_2 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_2 = (object[])tablesCache.GetRow(900, 4);

            var deleteRow3 = tablesCache.DeleteRow(900, "four");

            var remainingRows1_3 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2_3 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3_3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4_3 = (object[])tablesCache.GetRow(900, 3);
            var remainingRows5_3 = (object[])tablesCache.GetRow(900, 4);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);
            Assert.AreEqual("five", pk5);

            Assert.AreEqual(4, deleteRow1);

            Assert.AreEqual("one", remainingRows1_1[0]);
            Assert.AreEqual("two", remainingRows2_1[0]);
            Assert.AreEqual("five", remainingRows3_1[0]);
            Assert.AreEqual("four", remainingRows4_1[0]);
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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.DeleteRow(800, "one"));
        }

        [TestMethod]
        public void DeleteRowPrimaryKeyTest_InexistentRowIndex_ReturnsZero()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };

            // Act
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var deleteRow = tablesCache.DeleteRow(900, "seven");

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual(0, deleteRow);
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_DeleteTwoPK_EqualRowNumber()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };
            string[] toDelete = new string[] { "one", "two" };

            // Act
            tablesCache.ClearAllKeys(900);
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, toDelete);

            var remainingRows1 = (object[])tablesCache.GetRow(900, 0);
            var remainingRows2 = (object[])tablesCache.GetRow(900, 1);
            var remainingRows3 = (object[])tablesCache.GetRow(900, 2);
            var remainingRows4 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("one", pk1);
            Assert.AreEqual("two", pk2);
            Assert.AreEqual("three", pk3);
            Assert.AreEqual("four", pk4);

            Assert.AreEqual(2, deleteRow1);

            Assert.AreEqual("four", remainingRows1[0]);
            Assert.AreEqual("three", remainingRows2[0]);
            Assert.IsNull(remainingRows3[0]);
            Assert.IsNull(remainingRows4[0]);
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_DeleteInexistentTableId_ReturnsZero()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            string[] toDelete = new string[] { "one", "two", "seven" };

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.DeleteRow(800, toDelete));
        }

        [TestMethod]
        public void DeleteRowPKArrayTest_EmptyArray_ReturnsZero()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };
            object[] row3 = new object[] { "three", "three", 6, 7, 8 };
            object[] row4 = new object[] { "four", "four", 6, 7, 8 };
            string[] toDelete = new string[] { };

            // Act
            tablesCache.ClearAllKeys(900);
            var pk1 = tablesCache.AddRowReturnKey(900, row1);
            var pk2 = tablesCache.AddRowReturnKey(900, row2);
            var pk3 = tablesCache.AddRowReturnKey(900, row3);
            var pk4 = tablesCache.AddRowReturnKey(900, row4);

            var deleteRow1 = tablesCache.DeleteRow(900, toDelete);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "one", "one", 3, 4, 5 };
            object[] row2 = new object[] { "two", "two", 6, 7, 8 };

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, row1);
            var rowsNumber2 = tablesCache.AddRow(900, row2);
            var rowsLeft = tablesCache.ClearAllKeys(900);

            // Assert
            Assert.AreEqual(1, rowsNumber1);
            Assert.AreEqual(2, rowsNumber2);
            Assert.AreEqual(0, rowsLeft);
        }

        [TestMethod]
        public void ClearAllKeysTest_InexistentTableId_ReturnsNegativeOne()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.ClearAllKeys(800));
        }

        [TestMethod]
        public void ClearAllKeysTest_EmptyTable_ReturnsZero()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act
            var rowsLeft = tablesCache.ClearAllKeys(900);

            // Assert
            Assert.AreEqual(0, rowsLeft);
        }

        [TestMethod]
        public void GetKeyPositionTest_ValidOnlyPK_EqualKeyPosition()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            var rowsNumber2 = tablesCache.AddRow(900, "skyline2");
            var rowsNumber3 = tablesCache.AddRow(900, "skyline3");
            var rowsNumber4 = tablesCache.AddRow(900, "skyline4");

            var keyPosition1 = tablesCache.GetOneBasedRowIndex(900, "skyline1");
            var keyPosition2 = tablesCache.GetOneBasedRowIndex(900, "skyline2");
            var keyPosition3 = tablesCache.GetOneBasedRowIndex(900, "skyline3");
            var keyPosition4 = tablesCache.GetOneBasedRowIndex(900, "skyline4");

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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.GetOneBasedRowIndex(800, "skyline1"));
        }

        [TestMethod]
        public void GetKeyPositionTest_InexistentKey_ReturnsZero()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act
            Assert.Throws<Exception>(() => tablesCache.GetOneBasedRowIndex(800, "skyline2"));
        }

        [TestMethod]
        public void SetRowWithIndexTest_InexistentTableId_ReturnsNull()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.SetRow(800, 0, row));
        }

        [TestMethod]
        public void SetRowWithIndexTest_InexistentRowIndex_ReturnsArrayWithZeros()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act
            tablesCache.ClearAllKeys(900);
            var rowsNumber1 = tablesCache.AddRow(900, "skyline1");
            int[] changes = (int[])tablesCache.SetRow(900, 10, row);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 10 };
            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            int[] changes = (int[])tablesCache.SetRow(900, 0, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, 0);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline1", 10 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            int[] changes = (int[])tablesCache.SetRow(900, 0, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, 0);

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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.SetRow(800, "skyline1", row));
        }

        [TestMethod]
        public void SetRowWithPKTest_InexistentRowIndex_ReturnsArrayWithZeros()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.SetRow(900, "skyline2", row));
        }

        [TestMethod]
        public void SetRowWithPKTest_LessEntriesThanColumns_IsEqual()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 10 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            int[] changes = (int[])tablesCache.SetRow(900, "skyline1", row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, 0);

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
        public void SetRowWithPrimaryKey_UseClearAndLeave_IsEqual()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            var row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };

            tablesCache.AddRow(900 , row1);

            // Act
            tablesCache.SetRow(900, "skyline1", new object[5] { Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE, Constants.PROTOCOL_LEAVE }, useClearAndLeave: true);

            // Assert

            var rowOutput = (object[])tablesCache.GetRow(900, 0);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, 2);

            // Assert
            Assert.IsNull(rowOutput[0]);
            Assert.IsNull(rowOutput[1]);
            Assert.IsNull(rowOutput[2]);
            Assert.IsNull(rowOutput[3]);
            Assert.IsNull(rowOutput[4]);
        }

        [TestMethod]
        public void GetRowWithIndexTest_InexistentTableId_IsNull()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object rowOutput = tablesCache.GetRow(800, 0);

            // Assert
            Assert.IsNull(rowOutput);
        }

        [TestMethod]
        public void GetRowWithIndexTest_ValidTaleIdAndIndex_IsEqual()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, 1);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline1", "2ndColumnSkyline1", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, "skyline3");

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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act &  Assert
            Assert.Throws<Exception>(() => tablesCache.GetRow(800, "skyline1"));
        }

        [TestMethod]
        public void GetRowWithPKTest_ValidTaleIdAndIndex_IsEqual()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object[] rowOutput = (object[])tablesCache.GetRow(900, "skyline2");

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
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
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.AddRow(900, row5);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
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
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);

            tablesCache.AddRow(900, row5);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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
        public void FillArrayPartialTest_FillArrayAfterAddRow_IsEqual()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
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
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            tablesCache.AddRow(900, row5);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
            object[] row6 = new object[] { "skyline1", "2ndColumnSkyline1.2", null, null, null };

            var listOfRows = new List<object[]>
            {
                row3,
                row4,
                row6,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Partial);
            tablesCache.AddRow(900, row5);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline1.2", rowOutput0[1]);
            Assert.AreEqual(null, rowOutput0[2]);
            Assert.AreEqual(null, rowOutput0[3]);
            Assert.AreEqual(null, rowOutput0[4]);

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
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;
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
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.FillArray(900, listOfRows, NotifyProtocol.SaveOption.Full);
            tablesCache.AddRow(900, row5);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);

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
        public void FillArray_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var columns = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArray(900, columns);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline4", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(11, rowOutput0[3]);
            Assert.AreEqual(12, rowOutput0[4]);

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
        }

        [TestMethod]
        public void FillArray_FillArrayAfterFillArray_UseClearAndLeave()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", null, "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var columns = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            tablesCache.ClearAllKeys(900);

            tablesCache.FillArray(900, columns);

            // Act
            col2[1] = Constants.PROTOCOL_LEAVE;
            col2[2] = "2ndSkyline3";
            col3[3] = Constants.PROTOCOL_CLEAR;

            tablesCache.FillArray(900, columns, useClearAndLeave: true);

            // Assert
            object[] row1 = (object[])tablesCache.GetRow(900, 1);
            object[] row2 = (object[])tablesCache.GetRow(900, 2);
            object[] row3 = (object[])tablesCache.GetRow(900, 3);

            Assert.AreEqual("skyline2", row1[0]);
            Assert.AreEqual("2ndSkyline2", row1[1]);
            Assert.AreEqual(4, row1[2]);
            Assert.AreEqual(5, row1[3]);
            Assert.AreEqual(6, row1[4]);

            Assert.AreEqual("skyline3", row2[0]);
            Assert.AreEqual("2ndSkyline3", row2[1]); // new value
            Assert.AreEqual(7, row2[2]);
            Assert.AreEqual(8, row2[3]);
            Assert.AreEqual(9, row2[4]);

            Assert.AreEqual("skyline4", row3[0]);
            Assert.AreEqual("2ndSkyline4", row3[1]);
            Assert.AreEqual(null, row3[2]); // protocol clear
            Assert.AreEqual(11, row3[3]);
            Assert.AreEqual(12, row3[4]);
        }

        [TestMethod]
        public void FillArray_FillArrayAfterTwoAddRow_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };
            object[] row2 = new object[] { "two", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", null, "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var columns = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArray(900, columns);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);

            // Assert
            Assert.AreEqual("skyline4", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(11, rowOutput0[3]);
            Assert.AreEqual(12, rowOutput0[4]);

            Assert.AreEqual("skyline3", rowOutput1[0]);
            Assert.IsNull(rowOutput1[1]);
            Assert.AreEqual(7, rowOutput1[2]);
            Assert.AreEqual(8, rowOutput1[3]);
            Assert.AreEqual(9, rowOutput1[4]);

            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("2ndSkyline1", rowOutput2[1]);
            Assert.AreEqual(1, rowOutput2[2]);
            Assert.AreEqual(2, rowOutput2[3]);
            Assert.AreEqual(3, rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayListColumnsTest_FillArrayAfterTwoAddRow_ReplaceRow()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", null, "2ndSkyline4" };
            object[] col3 = new object[] { 1, null, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var listOfCols = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArray(900, listOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline2");

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            DateTime timestamps = new DateTime(2022, 7, 14);

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var listOfCols = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArray(900, listOfCols, timestamps);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline4", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(11, rowOutput0[3]);
            Assert.AreEqual(12, rowOutput0[4]);

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
        }

        [TestMethod]
        public void FillArrayColumnsArrayTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndColumnSkyline1", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[][] { col1, col2, col3, col4, col5 };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArray(900, arrayOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);

            // Assert
            Assert.AreEqual("skyline4", rowOutput0[0]);
            Assert.AreEqual("2ndSkyline4", rowOutput0[1]);
            Assert.AreEqual(10, rowOutput0[2]);
            Assert.AreEqual(11, rowOutput0[3]);
            Assert.AreEqual(12, rowOutput0[4]);

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
        }

        [TestMethod]
        public void FillArrayNoDeleteListColumnsTest_FillArrayAfterAddRow_GetCorrectRows()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var listOfCols = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArrayNoDelete(900, listOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var listOfCols = new object[][]
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArrayNoDelete(900, listOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[][] { col1, col2, col3, col4, col5 };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArrayNoDelete(900, arrayOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "one", "2ndOne", 10, 20, 30 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            object[] col1 = new object[] { "skyline1", "skyline2", "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline1", "2ndSkyline2", "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { 1, 4, 7, 10 };
            object[] col4 = new object[] { 2, 5, 8, 11 };
            object[] col5 = new object[] { 3, 6, 9, 12 };

            var arrayOfCols = new object[][] { col1, col2, col3, col4, col5 };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArrayNoDelete(900, arrayOfCols);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, 0);
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, 1);
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, 2);
            object[] rowOutput3 = (object[])tablesCache.GetRow(900, 3);
            object[] rowOutput4 = (object[])tablesCache.GetRow(900, 4);

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
        public void FillArrayWithColumn_FillArrayWithSameUniqueValue_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            var primaryKeys = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { "value1" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArrayWithColumn(900, 902, primaryKeys, values);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

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
        public void FillArrayWithColumn_FillArraysWithDifferentLengths_Exception()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            var pk = new [] { "skyline1", "skyline2", "skyline3" };

            object[] values = new object[] { "value1", "value2" };

            // Act
            tablesCache.ClearAllKeys(900);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tablesCache.FillArrayWithColumn(900, 902, pk, values));
        }

        [TestMethod]
        public void FillArrayWithColumn_FillArray_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            var pk = new[] { "skyline1", "skyline2" };

            var values = new object[] { "value1", "value2" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArrayWithColumn(900, 902, pk, values);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

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
        public void FillArrayWithColumn_FillArray_ReplaceRow()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { "value1", "value2" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArrayWithColumn(900, 902, pk, values);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.AreEqual("1", rowOutput0[2]);
            Assert.AreEqual("2", rowOutput0[3]);
            Assert.AreEqual("3", rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumn_FillArray_ReplaceRowWithProtocolLeave()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { Constants.PROTOCOL_LEAVE, "value2" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArrayWithColumn(900, 902, pk, values, useClearAndLeave: true);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("2ndColumnSkyline1", rowOutput0[1]); // Value not replaced because of Protocol_Leave
            Assert.AreEqual("1", rowOutput0[2]);
            Assert.AreEqual("2", rowOutput0[3]);
            Assert.AreEqual("3", rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumn_FillArray_ReplaceRowWithProtocolClear()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { Constants.PROTOCOL_CLEAR, "value2" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);

            tablesCache.FillArrayWithColumn(900, 902, pk, values, useClearAndLeave: true);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual(null, rowOutput0[1]); // Value cleared because of Protocol_Clear
            Assert.AreEqual("1", rowOutput0[2]);
            Assert.AreEqual("2", rowOutput0[3]);
            Assert.AreEqual("3", rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumn_NotConsecutiveRows_GetCorrectRows()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { "3rdColValue1", "3rdColValue2" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArrayWithColumn(900, 903, pk, values);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.IsNull(rowOutput0[1]);
            Assert.AreEqual("3rdColValue1", rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.IsNull(rowOutput1[1]);
            Assert.AreEqual("3rdColValue2", rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void FillArrayWithColumn_InexistentColumnPid_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { "3rdColValue1", "3rdColValue2" };

            // Act
            tablesCache.ClearAllKeys(900);

            Assert.Throws<Exception>(() => tablesCache.FillArrayWithColumn(900, 906, pk, values));

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");

            // Assert
            Assert.IsNull(rowOutput0[0]);
            Assert.IsNull(rowOutput0[1]);
            Assert.IsNull(rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.IsNull(rowOutput1[0]);
            Assert.IsNull(rowOutput1[1]);
            Assert.IsNull(rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);
        }

        [TestMethod]
        public void GetColumn_IsEqual()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableId = 900;
            int columnPid = 902;

            var pk = new[] { "skyline1", "skyline2" };

            object[] values = new object[] { "3rdColValue1", "3rdColValue2" };

            // Act
            tablesCache.FillArrayWithColumn(tableId, columnPid, pk, values);
            var column = tablesCache.GetColumn(tableId, columnPid);

            // Assert
            Assert.AreEqual("3rdColValue1", column[0]);
            Assert.AreEqual("3rdColValue2", column[1]);
        }

        [TestMethod]
        public void FillArrayWithColumns_FillArraysWithColumnsDifferentLengths_Exception()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableID = 900;
            int columnID1 = 902;
            int columnID2 = 903;

            var pk = new[] { "skyline1", "skyline2", "skyline3" };
            object[] column1Values = new object[] { "value1", "value2", };
            object[] column2Values = new object[] { 1, 2, };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { columnID1, column1Values },
                { columnID2, column2Values }
            };

            // Act
            tablesCache.ClearAllKeys(900);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tablesCache.FillArrayWithColumns(tableID, pk, columnPidsToValues));
        }

        [TestMethod]
        public void FillArrayWithColumns_FillArray_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableID = 900;
            int columnID1 = 902;
            int columnID2 = 903;

            var pk = new[] { "skyline1", "skyline2", "skyline3" };
            object[] column1Values = new object[] { "value1", "value2", "value3" };
            object[] column2Values = new object[] { 1, 2, 3 };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { columnID1, column1Values },
                { columnID2, column2Values }
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArrayWithColumns(tableID, pk, columnPidsToValues);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, "skyline3");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.IsNull(rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.AreEqual(2, rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.IsNull(rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("value3", rowOutput2[1]);
            Assert.AreEqual(3, rowOutput2[2]);
            Assert.IsNull(rowOutput2[3]);
            Assert.IsNull(rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayWithColumns_FillArray_ReplaceRow()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableID = 900;
            int columnID1 = 902;
            int columnID2 = 903;

            var pk = new[] { "skyline1", "skyline2", "skyline3" };
            object[] column1Values = new object[] { "value1", "value2", "value3" };
            object[] column2Values = new object[] { 1, 2, 3 };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { columnID1, column1Values },
                { columnID2, column2Values }
            };

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1000, "2", "3" };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 2000, "22", "33" };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            tablesCache.FillArrayWithColumns(tableID, pk, columnPidsToValues);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, "skyline3");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.AreEqual("value1", rowOutput0[1]);
            Assert.AreEqual(1, rowOutput0[2]);
            Assert.AreEqual("2", rowOutput0[3]);
            Assert.AreEqual("3", rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.AreEqual("value2", rowOutput1[1]);
            Assert.AreEqual(2, rowOutput1[2]);
            Assert.AreEqual("22", rowOutput1[3]);
            Assert.AreEqual("33", rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.AreEqual("value3", rowOutput2[1]);
            Assert.AreEqual(3, rowOutput2[2]);
            Assert.IsNull(rowOutput2[3]);
            Assert.IsNull(rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayWithColumns_NotConsecutiveRows_GetCorrectRows()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableID = 900;
            int columnID1 = 903;
            int columnID2 = 905;

            var pk = new[] { "skyline1", "skyline2", "skyline3" };
            object[] column1Values = new object[] { "value1", "value2", "value3" };
            object[] column2Values = new object[] { 1, 2, 3 };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { columnID1, column1Values },
                { columnID2, column2Values }
            };

            // Act
            tablesCache.ClearAllKeys(900);

            tablesCache.FillArrayWithColumns(tableID, pk, columnPidsToValues);

            object[] rowOutput0 = (object[])tablesCache.GetRow(900, "skyline1");
            object[] rowOutput1 = (object[])tablesCache.GetRow(900, "skyline2");
            object[] rowOutput2 = (object[])tablesCache.GetRow(900, "skyline3");

            // Assert
            Assert.AreEqual("skyline1", rowOutput0[0]);
            Assert.IsNull(rowOutput0[1]);
            Assert.AreEqual("value1", rowOutput0[2]);
            Assert.IsNull(rowOutput0[3]);
            Assert.AreEqual(1, rowOutput0[4]);

            Assert.AreEqual("skyline2", rowOutput1[0]);
            Assert.IsNull(rowOutput1[1]);
            Assert.AreEqual("value2", rowOutput1[2]);
            Assert.IsNull(rowOutput1[3]);
            Assert.AreEqual(2, rowOutput1[4]);

            Assert.AreEqual("skyline3", rowOutput2[0]);
            Assert.IsNull(rowOutput2[1]);
            Assert.AreEqual("value3", rowOutput2[2]);
            Assert.IsNull(rowOutput2[3]);
            Assert.AreEqual(3, rowOutput2[4]);
        }

        [TestMethod]
        public void FillArrayWithColumns_InexistentColumnPid_ThrowsException()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int tableID = 900;
            int columnID1 = 902;
            int columnID2 = 906;    // does not exist

            var pk = new[] { "skyline1", "skyline2", "skyline3" };
            object[] column1Values = new object[] { "value1", "value2", "value3" };
            object[] column2Values = new object[] { 1, 2, 3 };

            var columnPidsToValues = new Dictionary<int, object[]>
            {
                { columnID1, column1Values },
                { columnID2, column2Values }
            };

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.FillArrayWithColumns(tableID, pk, columnPidsToValues));
        }

        [TestMethod]
        public void RowCount_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.RowCount(901));
        }

        [TestMethod]
        public void RowCount_GetCorrectCount()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int expected = 2;

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            int actual = tablesCache.RowCount(900);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RowCount_EmptyTable()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int expected = 0;

            // Act
            tablesCache.ClearAllKeys(900);
            int actual = tablesCache.RowCount(900);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RowCount_AfterDeletedRow()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 7, 8, 9 };

            int expected = 2;

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.AddRow(900, row3);

            tablesCache.DeleteRow(900, 1);
            int actual = tablesCache.RowCount(900);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetKeys_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetKeys(901));
        }

        [TestMethod]
        public void GetKeys_GetCorrectKeys()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            string[] expected = new string[] { row1[0].ToString(), row2[0].ToString() };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            string[] actual = tablesCache.GetKeys(900);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetKeys_EmptyTable()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            string[] expected = new string[] { };

            // Act
            tablesCache.ClearAllKeys(900);
            string[] actual = tablesCache.GetKeys(900);

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParameterIndex_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndex(901, 1, 1));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void GetParameterIndex_InvalidOneBasedRowIndex(int oneBasedRowIndexToGet)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndex(900, oneBasedRowIndexToGet, 0));
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(6)]
        public void GetParameterIndex_InvalidOneBasedColumnIndex(int oneBasedColumnIndex)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndex(900, 0, oneBasedColumnIndex));
        }

        [TestMethod]
        public void GetParameterIndex_GetCorrectCell()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            object expected = row1[1];

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object actual = tablesCache.GetParameterIndex(900, 1, 2);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParameterIndex_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndex(900, 0, 0));
        }

        [TestMethod]
        public void SetParameterIndex_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndex(901, 1, 2, "change"));
        }

        [TestMethod]
        public void SetParameterIndex_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndex(900, 1, 2, "change"));
        }

        [TestMethod]
        [DataRow(0)] // Too low
        [DataRow(3)] // Too high
        public void SetParameterIndex_InvalidOneBasedRowIndex(int oneBasedRowIndexToSet)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndex(900, oneBasedRowIndexToSet, 2, "change"));
        }

        [TestMethod]
        [DataRow(1)] // Too low
        [DataRow(6)] // Too high
        public void SetParameterIndex_Invalid_Y_Coordinate(int oneBasedColumnIndexToSet)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndex(900, 1, oneBasedColumnIndexToSet, "change"));
        }

        [TestMethod]
        public void SetParameterIndex_SetCorrectValue()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            bool expected_return = true;
            object expected_value = "change";

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            bool actual_return = tablesCache.SetParameterIndex(900, 1, 2, "change");
            object[] changed_row = (object[])tablesCache.GetRow(900, 0);
            object actual_value = changed_row[1];

            // Assert
            Assert.AreEqual(expected_return, actual_return);
            Assert.AreEqual(expected_value, actual_value);
        }

        [TestMethod]
        public void SetParametersIndex_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int[] ids = new int[] { 901 };
            int[] iXs = new int[] { 1 };
            int[] iYs = new int[] { 2 };
            object[] values = new object[] { "change1" };

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndex(ids, iXs, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndex_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int[] ids = new int[] { 900 };
            int[] iXs = new int[] { 1 };
            int[] iYs = new int[] { 2 };
            object[] values = new object[] { "change1" };

            // Act  & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndex(ids, iXs, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndex_InvalidOneBasedColumnIndex()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900, 900 };
            int[] iXs = new int[] { 0, 3 };
            int[] iYs = new int[] { 2, 2 };
            object[] values = new object[] { "change1", "change2" };
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndex(ids, iXs, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndex_Invalid_Y_Coordinate()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900, 900 };
            int[] iXs = new int[] { 1, 2 };
            int[] iYs = new int[] { 1, 6 };
            object[] values = new object[] { "change1", "change2" };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act  & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndex(ids, iXs, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndex_DifferentSizeArrays()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900 };
            int[] iXs = new int[] { 1, 2 };
            int[] iYs = new int[] { 2, 2 };
            object[] values = new object[] { "change1", "change2" };

            uint expected = (uint)0x80040221L;

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            uint actual = Convert.ToUInt32(tablesCache.SetParametersIndex(ids, iXs, iYs, values));

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetParametersIndex_SetCorrectValues()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900, 900 };
            int[] iXs = new int[] { 1, 2 };
            int[] iYs = new int[] { 2, 2 };
            object[] values = new object[] { "change1", "change2" };

            uint[] expected_return = new uint[] { (uint)0x0004024AL, (uint)0x0004024AL };
            object[] row1_expected = new object[] { "skyline1", "change1", 1, 2, 3 };
            object[] row2_expected = new object[] { "skyline2", "change2", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            uint[] actual_return = (uint[])tablesCache.SetParametersIndex(ids, iXs, iYs, values);
            object[] actual_row1 = (object[])tablesCache.GetRow(900, iXs[0] - 1);
            object[] actual_row2 = (object[])tablesCache.GetRow(900, iXs[1] - 1);

            // Assert
            CollectionAssert.AreEqual(expected_return, actual_return);
            CollectionAssert.AreEqual(row1_expected, actual_row1);
            CollectionAssert.AreEqual(row2_expected, actual_row2);
        }

        [TestMethod]
        public void GetParameterIndexByKey_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndexByKey(901, "value", 1));
        }

        [TestMethod]
        public void GetParameterIndexByKey_Invalid_PrimaryKey()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            string key = "Ghost";

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndexByKey(900, key, 2));
        }

        [TestMethod]
        [DataRow(0)] // Too low
        [DataRow(6)] // Too high
        public void GetParameterIndexByKey_InvalidOneBasedRowIndex(int oneBasedColumnIndexToGet)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act
            Assert.Throws<Exception>(() => tablesCache.GetParameterIndexByKey(900, "skyline1", oneBasedColumnIndexToGet));
        }

        [TestMethod]
        public void GetParameterIndexByKey_GetCorrectCell()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            object expected = row1[1];

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            object actual = tablesCache.GetParameterIndexByKey(900, "skyline1", 2);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParameterIndexByKey_UninitializedCells()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[3];

            row1[0] = "key";
            row1[2] = 1;

            object expected = null;

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            object actual = tablesCache.GetParameterIndexByKey(900, "key", 2);

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetParameterIndexByKey_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndexByKey(901, "skyline1", 2, "change"));
        }

        [TestMethod]
        public void SetParameterIndexByKey_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndexByKey(900, "Ghost", 2, "change"));
        }

        [TestMethod]
        public void SetParameterIndexByKey_Invalid_PrimaryKey()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            string key = "Ghost";

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndexByKey(900, key, 2, "change"));
        }

        [TestMethod]
        [DataRow(1)] // Too low
        [DataRow(6)] // Too high
        public void SetParameterIndexByKey_InvalidOneBasedColumnIndex(int oneBasedColumnIndexToSet)
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParameterIndexByKey(900, "skyline1", oneBasedColumnIndexToSet, "change"));
        }

        [TestMethod]
        public void SetParameterIndexByKey_SetCorrectValue()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            bool expected_return = true;
            object expected_value = "change";

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            bool actual_return = tablesCache.SetParameterIndexByKey(900, "skyline1", 2, expected_value);
            object[] changed_row = (object[])tablesCache.GetRow(900, 0);
            object actual_value = changed_row[1];

            // Assert
            Assert.AreEqual(expected_return, actual_return);
            Assert.AreEqual(expected_value, actual_value);
        }

        [TestMethod]
        public void SetParametersIndexByKey_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int[] ids = new int[] { 901 };
            string[] keys = new string[] { "skyline1" };
            int[] iYs = new int[] { 2 };
            object[] values = new object[] { "change1" };

            // Act
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndexByKey(ids, keys, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndexByKey_EmptyTable()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            int[] tablePids = new int[] { 900 };
            string[] keys = new string[] { "skyline1" };
            int[] oneBasedColumnIndices = new int[] { 2 };
            object[] values = new object[] { "change1" };

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndexByKey(tablePids, keys, oneBasedColumnIndices, values));
        }

        [TestMethod]
        public void SetParametersIndexByKey_Invalid_PrimaryKey()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900 };
            string[] keys = new string[] { "Ghost" };
            int[] iYs = new int[] { 2 };
            object[] values = new object[] { "change1" };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndexByKey(ids, keys, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndexByKey_InvalidOneBasedColumnIndex()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900, 900 };
            string[] keys = new string[] { "skyline1", "skyline2" };
            int[] iYs = new int[] { 1, 6 };
            object[] values = new object[] { "change1", "change2" };

            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.SetParametersIndexByKey(ids, keys, iYs, values));
        }

        [TestMethod]
        public void SetParametersIndexByKey_DifferentSizeArrays()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900 };
            string[] keys = new string[] { "skyline1", "skyline2" };
            int[] iYs = new int[] { 2, 2 };
            object[] values = new object[] { "change1", "change2" };

            uint expected = (uint)0x80040221L;

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            uint actual = Convert.ToUInt32(tablesCache.SetParametersIndexByKey(ids, keys, iYs, values));

            // Assert
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SetParametersIndexByKey_SetCorrectValues()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 4, 5, 6 };

            int[] ids = new int[] { 900, 900 };
            string[] keys = new string[] { "skyline1", "skyline2" };
            int[] iYs = new int[] { 2, 2 };
            object[] values = new object[] { "change1", "change2" };

            uint[] expected_return = new uint[] { (uint)0x0004024AL, (uint)0x0004024AL };
            object[] row1_expected = new object[] { "skyline1", "change1", 1, 2, 3 };
            object[] row2_expected = new object[] { "skyline2", "change2", 4, 5, 6 };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            uint[] actual_return = (uint[])tablesCache.SetParametersIndexByKey(ids, keys, iYs, values);
            object[] actual_row1 = (object[])tablesCache.GetRow(900, 0);
            object[] actual_row2 = (object[])tablesCache.GetRow(900, 1);

            // Assert
            CollectionAssert.AreEqual(expected_return, actual_return);
            CollectionAssert.AreEqual(row1_expected, actual_row1);
            CollectionAssert.AreEqual(row2_expected, actual_row2);
        }

        [TestMethod]
        public void AddRowReturnKey_InexistentTableID()
        {
            // Arrange
            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            // Act & Assert
            Assert.Throws<Exception>(() => tablesCache.AddRowReturnKey(901));
        }

        [TestMethod]
        public void AddRowReturnKey_ToEmptyTable()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            string expected = "1";
            object[] expected_row = new object[] { "1", null, null, null, null };

            // Act
            tablesCache.ClearAllKeys(900);
            string actual = tablesCache.AddRowReturnKey(900);
            object[] actual_row = (object[])tablesCache.GetRow(900, "1");

            // Assert
            Assert.AreEqual(expected, actual);
            CollectionAssert.AreEqual(expected_row, actual_row);
        }

        [TestMethod]
        public void AddRowReturnKey_ToNormalTable()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "1", null, null, null, null };

            string expected = "2";
            object[] expected_row = new object[] { "2", null, null, null, null };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            string actual = tablesCache.AddRowReturnKey(900);
            object[] actual_row = (object[])tablesCache.GetRow(900, "2");

            // Assert
            Assert.AreEqual(expected, actual);
            CollectionAssert.AreEqual(expected_row, actual_row);
        }

        [TestMethod]
        public void AddRowReturnKey_InconsistentKeys()
        {
            // Arrange


            var protocolCache = ProtocolCacheBuilder.Build(path);
            var tablesCache = protocolCache.Tables;

            object[] row1 = new object[] { "1", null, null, null, null };
            object[] row2 = new object[] { "2", null, null, null, null };
            object[] row5 = new object[] { "5", null, null, null, null };

            string expected = "6";
            object[] expected_row = new object[] { "6", null, null, null, null };

            // Act
            tablesCache.ClearAllKeys(900);
            tablesCache.AddRow(900, row1);
            tablesCache.AddRow(900, row2);
            tablesCache.AddRow(900, row5);
            string actual = tablesCache.AddRowReturnKey(900);
            object[] actual_row = (object[])tablesCache.GetRow(900, "6");

            // Assert
            Assert.AreEqual(expected, actual);
            CollectionAssert.AreEqual(expected_row, actual_row);
        }
    }
}