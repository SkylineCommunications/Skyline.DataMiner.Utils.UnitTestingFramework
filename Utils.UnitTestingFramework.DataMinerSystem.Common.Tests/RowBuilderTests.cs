namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    [TestClass]
    public class RowBuilderTests
    {
        [TestMethod]
        public void SetValueByIdx_SetsValues_WithIdxAndValueCollections()
        {
            var builder = CreateRowBuilder();

            var row = builder
                .SetValueByIdx(new[] { 0, 1, 2 }, new object[] { "row-1", "Description", 10.0 })
                .Build();

            CollectionAssert.AreEqual(new object[] { "row-1", "Description", 10.0 }, row);
        }

        [TestMethod]
        public void SetValueByIdx_ThrowsArgumentException_WithDifferentCollectionSizes()
        {
            var builder = CreateRowBuilder();

            Assert.ThrowsExactly<ArgumentException>(() =>
                builder.SetValueByIdx(new[] { 0, 1 }, new object[] { "row-1" }));
        }

        [TestMethod]
        public void SetValueByName_SetsValues_WithNameAndValueCollections()
        {
            var builder = CreateRowBuilder();

            var row = builder
                .SetValueByName(new[] { "Key", "Description", "Value" }, new object[] { "row-1", "Description", 10.0 })
                .Build();

            CollectionAssert.AreEqual(new object[] { "row-1", "Description", 10.0 }, row);
        }

        [TestMethod]
        public void SetValueByPid_SetsValues_WithPidAndValueCollections()
        {
            var builder = CreateRowBuilder();

            var row = builder
                .SetValueByPid(new[] { 201, 202, 203 }, new object[] { "row-1", "Description", 10.0 })
                .Build();

            CollectionAssert.AreEqual(new object[] { "row-1", "Description", 10.0 }, row);
        }

        [TestMethod]
        public void SetValueByPid_ThrowsArgumentNullException_WithNullValues()
        {
            var builder = CreateRowBuilder();

            Assert.ThrowsExactly<ArgumentNullException>(() =>
                builder.SetValueByPid(new[] { 201 }, values: null));
        }

        private static RowBuilder CreateRowBuilder()
        {
            var definition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true, columnName: "Key", columnType: typeof(string), allowNull: false)
                .AddColumn(columnPid: 202, columnIdx: 1, columnName: "Description", columnType: typeof(string))
                .AddColumn(columnPid: 203, columnIdx: 2, columnName: "Value", columnType: typeof(double))
                .Build();

            return new RowBuilder(definition);
        }
    }
}
