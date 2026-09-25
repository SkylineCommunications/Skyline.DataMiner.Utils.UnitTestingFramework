namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;

    [TestClass]
    public class IDmsProtocolMockBuilderTests
    {
        [TestMethod]
        public void AddParameterDefinition_ThrowsArgumentException_WithDuplicateId()
        {
            var builder = new IDmsProtocolMockBuilder("Protocol")
                .AddParameterDefinition(new StandaloneParameterDefinition("First", typeof(double), 100));

            Assert.ThrowsExactly<ArgumentException>(() =>
                builder.AddParameterDefinition(new StandaloneParameterDefinition("Second", typeof(double), 100)));
        }

        [TestMethod]
        public void AddParameterDefinition_ThrowsArgumentNullException_WithNullDefinition()
        {
            var builder = new IDmsProtocolMockBuilder("Protocol");

            Assert.ThrowsExactly<ArgumentNullException>(() => builder.AddParameterDefinition(null));
        }

        [TestMethod]
        public void AddTableDefinition_ThrowsArgumentException_WithDuplicateId()
        {
            var firstTableDefinition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true)
                .Build();
            var secondTableDefinition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 202, columnIdx: 0, isPrimaryKey: true)
                .Build();
            var builder = new IDmsProtocolMockBuilder("Protocol")
                .AddTableDefinition(200, firstTableDefinition);

            Assert.ThrowsExactly<ArgumentException>(() =>
                builder.AddTableDefinition(200, secondTableDefinition));
        }

        [TestMethod]
        public void AddTableDefinition_ThrowsArgumentNullException_WithNullDefinition()
        {
            var builder = new IDmsProtocolMockBuilder("Protocol");

            Assert.ThrowsExactly<ArgumentNullException>(() => builder.AddTableDefinition(100, null));
        }

        [TestMethod]
        public void Build_AddsParameterAndTableDefinitions_ToProtocol()
        {
            // Arrange
            var parameterDefinition = new StandaloneParameterDefinition("Parameter", typeof(double), 100);
            var tableDefinition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true, columnName: "Key")
                .Build();

            // Act
            var protocol = new IDmsProtocolMockBuilder("Protocol")
                .AddParameterDefinition(parameterDefinition)
                .AddTableDefinition(200, tableDefinition)
                .Build();

            // Assert
            Assert.AreSame(parameterDefinition, protocol.Definitions.GetParameterDefinition(100));
            Assert.AreSame(tableDefinition, protocol.Definitions.GetTableDefinition(200));
        }

        [TestMethod]
        public void Build_ReturnsDefaultVersion_WithoutExplicitVersion()
        {
            var protocol = new IDmsProtocolMockBuilder("Protocol").Build();

            Assert.AreEqual(IDmsProtocolMock.DefaultVersion, protocol.ReferencedVersion);
        }

        [TestMethod]
        public void Build_ReturnsProvidedNameAndVersion_WithCustomIdentifiers()
        {
            var protocol = new IDmsProtocolMockBuilder("Protocol", "2.0.0.0").Build();

            Assert.AreEqual("Protocol", protocol.Name);
            Assert.AreEqual("2.0.0.0", protocol.ReferencedVersion);
        }

        [TestMethod]
        public void Build_ThrowsArgumentException_WithEmptyName()
        {
            var builder = new IDmsProtocolMockBuilder(String.Empty);

            Assert.ThrowsExactly<ArgumentException>(() => builder.Build());
        }

        [TestMethod]
        public void Build_ThrowsArgumentException_WithEmptyVersion()
        {
            var builder = new IDmsProtocolMockBuilder("Protocol", String.Empty);

            Assert.ThrowsExactly<ArgumentException>(() => builder.Build());
        }

        [TestMethod]
        public void Build_ThrowsArgumentNullException_WithNullName()
        {
            var builder = new IDmsProtocolMockBuilder(null);

            Assert.ThrowsExactly<ArgumentNullException>(() => builder.Build());
        }

        [TestMethod]
        public void Build_ThrowsArgumentNullException_WithNullVersion()
        {
            var builder = new IDmsProtocolMockBuilder("Protocol", null);

            Assert.ThrowsExactly<ArgumentNullException>(() => builder.Build());
        }
    }
}
