namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation;

    [TestClass]
    public class ParametersAndTablesTests
    {
        [TestMethod]
        public void Constructor_CreatesParameterModels_FromDefinitions()
        {
            var definitions = new ParameterAndTableDefinitions();
            var definition = new StandaloneParameterDefinition("Parameter", typeof(string), 100);
            definitions.AddParameterDefinition(definition);

            var parametersAndTables = new ParametersAndTables(definitions);

            Assert.IsTrue(parametersAndTables.ParameterExists(100));
            Assert.IsTrue(parametersAndTables.ParameterExists("Parameter"));
            Assert.AreSame(definition, parametersAndTables.GetParameter(100).Definition);
        }

        [TestMethod]
        public void Constructor_UsesDefaultValue_FromStandaloneParameterDefinition()
        {
            var definitions = new ParameterAndTableDefinitions();
            var definition = new StandaloneParameterDefinition("Parameter", typeof(string), 100, "Initial value");
            definitions.AddParameterDefinition(definition);

            var parametersAndTables = new ParametersAndTables(definitions);

            Assert.AreEqual("Initial value", parametersAndTables.GetParameter(100).Value);
        }

        [TestMethod]
        public void Constructor_CreatesTableModels_FromDefinitions()
        {
            var definitions = new ParameterAndTableDefinitions();
            var tableBuilder = new TableModelBuilder(200);
            tableBuilder.AddColumn(columnPid: 201, columnIdx: 0, isKey: true, columnName: "Key");
            var tableDefinition = tableBuilder.Build().Definition;
            definitions.AddTableDefinition(200, tableDefinition);

            var parametersAndTables = new ParametersAndTables(definitions);

            Assert.AreEqual(200, parametersAndTables.GetTable(200).TableId);
            Assert.AreSame(tableDefinition, parametersAndTables.GetTable(200).Definition);
        }

        [TestMethod]
        public void Constructor_ThrowsArgumentNullException_WithNullDefinitions()
        {
            Assert.ThrowsExactly<ArgumentNullException>(() => new ParametersAndTables(null));
        }
    }
}
