namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    [TestClass]
    public class TableDefinitionBuilderTests
    {
        [TestMethod]
        public void AddColumn_ThrowsArgumentNullException_WithNullDefinition()
        {
            var builder = new TableDefinitionBuilder();

            Assert.ThrowsExactly<ArgumentNullException>(() => builder.AddColumn(column: null));
        }

        [TestMethod]
        public void AddColumn_ThrowsInvalidOperationException_WithSecondPrimaryKey()
        {
            var builder = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true);

            Assert.ThrowsExactly<InvalidOperationException>(() =>
                builder.AddColumn(columnPid: 202, columnIdx: 1, isPrimaryKey: true));
        }

        [TestMethod]
        public void Build_ReturnsDefinition_WithConfiguredColumnsAndPrimaryKey()
        {
            var keyColumn = new ColumnDefinition("Key", typeof(string), 201, 0, allowNull: false);

            var definition = new TableDefinitionBuilder()
                .AddColumn(keyColumn, isPrimaryKey: true)
                .AddColumn(columnPid: 202, columnIdx: 1, columnName: "Value", columnType: typeof(double))
                .Build();

            Assert.AreEqual(2, definition.ColumnCount);
            Assert.AreSame(keyColumn, definition.PrimaryKeyColumn);
            Assert.AreEqual(typeof(double), definition.FindColumnDefinitionByPid(202).Type);
        }

        [TestMethod]
        public void Build_ThrowsInvalidOperationException_WithoutPrimaryKey()
        {
            var builder = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0);

            Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());
        }
    }
}
