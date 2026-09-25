namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.DOM.Builders;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;

    [TestClass]
    public class DmsBuilderTests
    {
        private const string ProtocolName = "UnitTestingFrameworkUseCases";

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_AddsProtocolToDms_WithProtocolXml()
        {
            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .Build();

            // Assert
            var protocol = dmsMock.Object.GetProtocols().Single();
            Assert.AreEqual(ProtocolName, protocol.Name);
            Assert.AreEqual("1.0.0.1", protocol.ReferencedVersion);
            Assert.AreEqual(ProtocolType.Virtual, protocol.Type);
            Assert.IsTrue(dmsMock.Object.ProtocolExists(ProtocolName, "1.0.0.1"));
            Assert.AreSame(protocol, dmsMock.Object.GetProtocol(ProtocolName, "1.0.0.1"));
            Assert.IsFalse(dmsMock.Object.ProtocolExists(ProtocolName, "2.0.0.0"));
            Assert.ThrowsExactly<ProtocolNotFoundException>(() => dmsMock.Object.GetProtocol(ProtocolName, "2.0.0.0"));
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_AddsRowsToElementTable_FillTable()
        {
            // Arrange
            var row = new object[] { "one", "one-desc", 3.0, 4.0, 5.0 };

            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, configure: element => element
                        .FillTable(tableId: 900, rows: [row])
                        .FillTable(tableId: 900,
                            row => row.SetValueByIdx(new[] { 0, 1 }, new object[] { "two", "two-desc" }),
                            row => row.SetPrimaryKey("three").SetValueByIdx(1, "three-desc"))))
                .Build();

            // Assert
            var table = dmsMock.Object.GetAgent(1).GetElement("Element 33").GetTable(900);
            Assert.IsTrue(table.RowExists("one"));
            Assert.IsTrue(table.RowExists("two"));
            Assert.IsTrue(table.RowExists("three"));
            CollectionAssert.AreEqual(row, table.GetRow("one"));
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_ConnectsElementsToDmaAndView_WithElementsAndView()
        {
            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithView(viewId: 55)
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, configure: element => element
                        .UnderView(viewId: 55))
                    .WithElement(id: 44, name: "Element 44", protocolName: ProtocolName))
                .Build();

            // Assert
            var dma = dmsMock.Object.GetAgent(1);
            var firstElement = dma.GetElement("Element 33");
            var secondElement = dma.GetElement("Element 44");
            var view = dmsMock.Object.GetView(55);

            Assert.AreEqual(2, dma.GetElements().Count);
            Assert.AreSame(dma, firstElement.Host);
            Assert.AreSame(dma, secondElement.Host);
            Assert.AreSame(view, firstElement.Views.Single());
            Assert.AreSame(firstElement, view.Elements.Single());
            Assert.IsEmpty(secondElement.Views);
            Assert.AreSame(firstElement.Protocol, secondElement.Protocol);
            Assert.AreSame(firstElement.Protocol, dmsMock.Object.GetProtocols().Single());
        }

        [TestMethod]
        public void Build_CreatesDmaElementParametersTablesViewsAndDom_WithCompleteConfiguration()
        {
            // Arrange
            var parameterDefinition = new StandaloneParameterDefinition("Standalone", typeof(int), 100);
            var tableDefinition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true, columnName: "Key")
                .AddColumn(columnPid: 202, columnIdx: 1, columnName: "Value")
                .Build();
            var row = new object[] { "row-1", "value-1" };
            var domDefinitionId = Guid.NewGuid();

            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol(name: "CompleteProtocol", configure: protocol => protocol
                    .AddParameterDefinition(parameterDefinition)
                    .AddTableDefinition(tableId: 200, tableDefinition))
                .WithView(viewId: 55, name: "Complete view")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 10, name: "Complete element", protocolName: "CompleteProtocol", configure: element => element
                        .UnderView(viewId: 55)
                        .SetParameter<int?>(parameterId: 100, value: 7)
                        .FillTable(tableId: 200, rows: [row])))
                .WithDomDefinition(moduleId: "complete-module", createDefinition: () => new DomDefinitionBuilder()
                    .WithID(domDefinitionId)
                    .WithName("DOM definition")
                    .Build())
                .Build();

            // Assert
            var element = dmsMock.Object.GetElement("Complete element");
            var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "complete-module");

            Assert.IsTrue(dmsMock.Object.AgentExists(1));
            Assert.IsTrue(dmsMock.Object.ProtocolExists("CompleteProtocol", IDmsProtocolMock.DefaultVersion));
            Assert.AreEqual((int?)7, element.GetStandaloneParameter<int?>(100).GetValue());
            CollectionAssert.AreEqual(row, element.GetTable(200).GetRow("row-1"));
            Assert.AreSame(element, dmsMock.Object.GetView(55).Elements.Single());
            Assert.AreEqual(domDefinitionId, helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(domDefinitionId)).Single().ID.Id);
        }
        [TestMethod]
        public void Build_CreatesElementsWithMatchingDefinitions_WithTwoManualProtocolVersions()
        {
            // Arrange
            const string protocolName = "VersionedProtocol";
            const string firstVersion = "1.0.0.1";
            const string secondVersion = "2.0.0.0";
            var firstParameter = new StandaloneParameterDefinition("First version parameter", typeof(double), 100);
            var secondParameter = new StandaloneParameterDefinition("Second version parameter", typeof(double), 200);

            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol(name: protocolName, configure: protocol => protocol
                    .AddParameterDefinition(firstParameter), version: firstVersion)
                .WithProtocol(name: protocolName, configure: protocol => protocol
                    .AddParameterDefinition(secondParameter), version: secondVersion)
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 10, name: "First version element", protocolName: protocolName, protocolVersion: firstVersion)
                    .WithElement(id: 20, name: "Second version element", protocolName: protocolName, protocolVersion: secondVersion))
                .Build();

            // Assert
            var firstProtocol = dmsMock.Object.GetProtocol(protocolName, firstVersion);
            var secondProtocol = dmsMock.Object.GetProtocol(protocolName, secondVersion);
            var firstElement = dmsMock.Object.GetElement("First version element");
            var secondElement = dmsMock.Object.GetElement("Second version element");

            Assert.AreNotSame(firstProtocol, secondProtocol);
            Assert.AreSame(firstProtocol, firstElement.Protocol);
            Assert.AreSame(secondProtocol, secondElement.Protocol);
            Assert.AreEqual(100, firstElement.GetStandaloneParameter<double?>(100).Id);
            Assert.AreEqual(200, secondElement.GetStandaloneParameter<double?>(200).Id);
            Assert.ThrowsExactly<ArgumentException>(() => firstElement.GetStandaloneParameter<double?>(200));
            Assert.ThrowsExactly<ArgumentException>(() => secondElement.GetStandaloneParameter<double?>(100));
        }

        [TestMethod]
        public void Build_CreatesElementWithoutProtocolXml_WithManualProtocolDefinitions()
        {
            // Arrange
            var parameterDefinition = new StandaloneParameterDefinition("Standalone", typeof(double), 100);
            var tableDefinition = new TableDefinitionBuilder()
                .AddColumn(columnPid: 201, columnIdx: 0, isPrimaryKey: true, columnName: "Key")
                .AddColumn(columnPid: 202, columnIdx: 1, columnName: "Value")
                .Build();

            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol(name: "CustomProtocol", configure: protocol => protocol
                    .AddParameterDefinition(parameterDefinition)
                    .AddTableDefinition(tableId: 200, tableDefinition: tableDefinition))
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 10, name: "Element 10", protocolName: "CustomProtocol"))
                .Build();

            // Assert
            var protocol = dmsMock.Object.GetProtocol("CustomProtocol", IDmsProtocolMock.DefaultVersion);
            var element = dmsMock.Object.GetElement("Element 10");

            Assert.AreSame(protocol, element.Protocol);
            Assert.AreEqual(100, element.GetStandaloneParameter<double?>(100).Id);
            Assert.AreEqual(200, element.GetTable(200).Id);
        }

        [TestMethod]
        public void Build_CreatesViewAndDma_InSameDms()
        {
            // Act
            var dmsMock = new DmsBuilder()
                .WithView(viewId: 55)
                .WithDma(id: 1)
                .Build();

            // Assert
            Assert.IsTrue(dmsMock.Object.ViewExists(55));
            Assert.IsTrue(dmsMock.Object.AgentExists(1));
            Assert.AreSame(dmsMock.Object, dmsMock.Object.GetView(55).Dms);
            Assert.AreSame(dmsMock.Object, dmsMock.Object.GetAgent(1).Dms);
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_SetsStandaloneParameter_WithParameter()
        {
            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, configure: element => element
                        .SetParameter<int?>(parameterId: 800, value: 7)))
                .Build();

            var value = dmsMock.Object.GetElement("Element 33").GetStandaloneParameter<int?>(800).GetValue();

            // Assert
            Assert.AreEqual((int?)7, value);
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_ThrowsArgumentException_WithDuplicateProtocol()
        {
            var builder = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithProtocol("protocol.xml");

            Assert.ThrowsExactly<ArgumentException>(() => builder.Build());
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_ThrowsArgumentNullException_WithNullTableRows()
        {
            // Arrange
            var builder = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, configure: element => element
                        .FillTable(tableId: 900, rows: null)));

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.Build());
        }

        [TestMethod]
        public void Build_ThrowsFileNotFoundException_WithMissingProtocolXml()
        {
            // Arrange
            var builder = new DmsBuilder()
                .WithProtocol("missing-protocol.xml");

            // Act & Assert
            Assert.ThrowsExactly<System.IO.FileNotFoundException>(() => builder.Build());
        }

        [TestMethod]
        public void Build_ThrowsInvalidOperationException_WithUnknownProtocol()
        {
            // Arrange
            var builder = new DmsBuilder()
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: "Unknown"));

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_ThrowsInvalidOperationException_WithUnknownProtocolVersion()
        {
            // Arrange
            var builder = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, protocolVersion: "2.0.0.0"));

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void Build_UsesMatchingProtocol_WithProtocolVersion()
        {
            // Act
            var dmsMock = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, protocolVersion: "1.0.0.1"))
                .Build();

            // Assert
            var protocol = dmsMock.Object.GetProtocol(ProtocolName, "1.0.0.1");
            var element = dmsMock.Object.GetElement("Element 33");
            Assert.AreSame(protocol, element.Protocol);
        }

        [TestMethod]
        public void WithDma_RunsConfiguration_OnBuild()
        {
            // Arrange
            var configured = false;
            var builder = new DmsBuilder()
                .WithDma(id: 1, configure: dma => configured = true);

            Assert.IsFalse(configured);

            // Act
            var dmsMock = builder.Build();

            // Assert
            Assert.IsTrue(configured);
            Assert.IsTrue(dmsMock.Object.AgentExists(1));
        }

        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        [TestMethod]
        public void WithElement_RunsConfiguration_OnBuild()
        {
            // Arrange
            var configured = false;
            var builder = new DmsBuilder()
                .WithProtocol("protocol.xml")
                .WithView(viewId: 55)
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 33, name: "Element 33", protocolName: ProtocolName, configure: element =>
                    {
                        configured = true;
                        element.UnderView(viewId: 55);
                    }));

            Assert.IsFalse(configured);

            // Act
            var dmsMock = builder.Build();

            // Assert
            Assert.IsTrue(configured);
            Assert.AreSame(dmsMock.Object.GetView(55), dmsMock.Object.GetElement("Element 33").Views.Single());
        }

        [TestMethod]
        public void WithProtocol_RunsConfiguration_OnBuild()
        {
            // Arrange
            var configured = false;
            var builder = new DmsBuilder()
                .WithProtocol("Protocol", _ => configured = true);

            Assert.IsFalse(configured);

            // Act
            builder.Build();

            // Assert
            Assert.IsTrue(configured);
        }

        [TestMethod]
        public void WithProtocol_ThrowsArgumentNullException_WithNullConfiguration()
        {
            // Arrange
            var builder = new DmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.WithProtocol("Protocol", null));
        }

    }
}
