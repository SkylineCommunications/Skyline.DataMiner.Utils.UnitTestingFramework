namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Model
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation;

    [TestClass]
    public class TableModelTests
    {
        [TestMethod]
        public void Row_ModifyingDoesNotModifyTable()
        {
            // Arrange
            var tableModelBuilder = new TableModelBuilder(900);

            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            var tableModel = tableModelBuilder.Build();

            object[] row = { "skyline1", "value2" };
            tableModel.SetRow(row);

            // Act
            object[] rowOutput1 = tableModel.Row("skyline1");
            rowOutput1[1] = "modifiedValue";

            // Assert
            object[] rowOutput2 = tableModel.Row("skyline1");
            Assert.AreEqual("skyline1", rowOutput2[0]);
            Assert.AreEqual("value2", rowOutput2[1]);
        }

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
            object[] rowOutput = tableModel.Row("skyline1");

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
            object[] row0Output = tableModel.Row(0);
            object[] row1Output = tableModel.Row(1);

            Assert.AreEqual("skyline2", row0Output[0]);
            Assert.AreEqual("value2", row0Output[1]);
            Assert.AreEqual("skyline3", row1Output[0]);
            Assert.AreEqual("value3", row1Output[1]);
        }

        [TestMethod]
        public void ValidSetColumn_ValidRowWithKey()
        {
            // Arrange
            object[] row0 = { "skyline4", "value4" };
            object[] row1 = { "skyline5", "value5" };
            string[] keys = { "skyline5" };
            string[] values = { "newValue6" };

            var tableModelBuilder = new TableModelBuilder(900);

            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            tableModelBuilder.Build();

            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row0);
            tableModel.SetRow(row1);
            tableModel.SetColumn(1, keys, values);
            object[] row0Output = tableModel.Row(0);
            object[] row1Output = tableModel.Row(1);

            // Assert
            Assert.AreEqual("skyline4", row0Output[0]);
            Assert.AreEqual("value4", row0Output[1]);
            Assert.AreEqual("skyline5", row1Output[0]);
            Assert.AreEqual("newValue6", row1Output[1]);
        }

        [TestMethod]
        public void InvalidPidSetColumn_Exception()
        {
            // Arrange
            object[] row0 = { "skyline4", "value4" };
            object[] row1 = { "skyline5", "value5" };
            string[] keys = { "skyline5" };
            string[] values = { "newValue6" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row0);
            tableModel.SetRow(row1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.SetColumn(2, keys, values));
        }

        [TestMethod]
        public void ValidSetColumnNewKey_ValidRowWithKey()
        {
            // Arrange
            object[] row0 = { "skyline6", "value6", "anotherValue6" };
            object[] row1 = { "skyline7", "value7", "anotherValue7" };
            string[] keys = { "skyline8" };
            string[] values = { "anotherValue8" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            tableModelBuilder.AddColumn(1203, 2, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row0);
            tableModel.SetRow(row1);
            tableModel.SetColumn(2, keys, values);
            object[] row0Output = tableModel.Row(0);
            object[] row1Output = tableModel.Row(1);
            object[] row2Output = tableModel.Row(2);

            // Assert
            Assert.AreEqual("skyline6", row0Output[0]);
            Assert.AreEqual("value6", row0Output[1]);
            Assert.AreEqual("anotherValue6", row0Output[2]);
            Assert.AreEqual("skyline7", row1Output[0]);
            Assert.AreEqual("value7", row1Output[1]);
            Assert.AreEqual("anotherValue7", row1Output[2]);
            Assert.AreEqual("skyline8", row2Output[0]);
            Assert.IsNull(row2Output[1]);
            Assert.AreEqual("anotherValue8", row2Output[2]);
        }

        [TestMethod]
        public void ValidSetColumn_ValidColumn()
        {
            // Arrange
            string[] keys = { "skyline9", "value10" };
            object[] values2nd = { "2ndColumn9", "2ndColumn10" };
            object[] values3rd = { "3ndColumn9", "3ndColumn10" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            tableModelBuilder.AddColumn(1203, 2, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetColumn(1, keys, values2nd);
            tableModel.SetColumn(2, keys, values3rd);
            object[] columnOutput = tableModel.Column(1203);

            // Assert
            Assert.AreEqual("3ndColumn9", columnOutput[0]);
            Assert.AreEqual("3ndColumn10", columnOutput[1]);
        }

        [TestMethod]
        public void ColumnWithInvalidPid_NullValueReturned()
        {
            // Arrange
            string[] keys = { "skyline9", "value10" };
            object[] values2nd = { "2ndColumn9", "2ndColumn10" };
            object[] values3rd = { "3ndColumn9", "3ndColumn10" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            tableModelBuilder.AddColumn(1203, 2, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetColumn(1, keys, values2nd);
            tableModel.SetColumn(2, keys, values3rd);
            var columnOutput = tableModel.Column(1204);

            // Assert
            Assert.IsNull(columnOutput);
        }

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

        [TestMethod]
        public void InexistentKeyRow_ArgumentException()
        {
            // Arrange
            object[] row = { "skyline1", "value2" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);

            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.Row("skyline2"));
        }

        [TestMethod]
        public void InexistentIndexRow_ArgumentException()
        {
            // Arrange
            object[] row = { "skyline1", "value2" };

            var tableModelBuilder = new TableModelBuilder(900);
            tableModelBuilder.AddColumn(1201, 0, true);
            tableModelBuilder.AddColumn(1202, 1, false);
            var tableModel = tableModelBuilder.Build();

            // Act
            tableModel.SetRow(row);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => tableModel.Row(1));
        }
    }
}