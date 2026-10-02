namespace Utils.UnitTestingFramework.DataMinerSystem.Common.Tests.Examples
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;
    using System.Collections.Generic;
    using System.Threading;
    using FluentAssertions;

    [TestClass]
    [DeploymentItem("Examples/protocol.xml", "Examples")]
    public class Demo_Tests
    {
        [TestMethod]
        public void FindOtherViewsOnSameDms_ReturnsAllViewsExceptSelectedView_WithBuilders()
        {
            // Arrange
            var dmsMock = new IDmsBuilder()
                .WithProtocol("Examples/protocol.xml")
                .WithView(10, "View 1")
                .WithView(20, "View 2")
                .WithView(30, "View 3")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 11, name: "Element A", protocolName: "DemoProtocol", configure: element => element
                        .UnderView(20)))
                .Build();

            var selectedView = dmsMock.GetViewMock(20);

            // Act
            var otherViews = ElementFinder.FindOtherViews(selectedView.Object);

            // Assert

            var otherA = dmsMock.GetViewMock(10);
            var otherB = dmsMock.GetViewMock(30);

            otherViews.Should().HaveCount(2);
            otherViews.Should().Contain(otherA.Object);
            otherViews.Should().Contain(otherB.Object);
            otherViews.Should().NotContain(selectedView.Object);
        }

        [TestMethod]
        public void FindOtherViewsOnSameDms_ReturnsAllViewsExceptSelectedView_WithoutBuilders()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");

            var view1 = dmsMock.CreateView(10, "View 1");
            var view2 = dmsMock.CreateView(20, "View 2");
            var view3 = dmsMock.CreateView(30, "View 3");

            var elementMock = dmaMock.CreateElement("Examples/protocol.xml", id: 11, name: "Element A");
            elementMock.AddView(20);

            // Act
            var otherViews = ElementFinder.FindOtherViews(view2.Object);

            // Assert
            otherViews.Should().HaveCount(2);
            otherViews.Should().Contain(view1.Object);
            otherViews.Should().Contain(view3.Object);
            otherViews.Should().NotContain(view2.Object);
        }

        [TestMethod]
        public void FindSimilarOnSameDma_ReturnsMatchingElementsFromSameDma_WithBuilders()
        {
            // Arrange
            var dmsMock = new IDmsBuilder()
                .WithProtocol("Examples/protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 11, name: "Element A", protocolName: "DemoProtocol")
                    .WithElement(id: 12, name: "Element B", protocolName: "DemoProtocol"))
                .WithDma(id: 2, dma => dma
                    .WithElement(id: 21, name: "Element C", protocolName: "DemoProtocol"))
                .WithDma(id: 3, dma => dma
                    .WithElement(id: 31, name: "Element D", protocolName: "DemoProtocol")
                    .WithElement(id: 32, name: "Element E", protocolName: "DemoProtocol")
                    .WithElement(id: 33, name: "Element F", protocolName: "DemoProtocol"))
                .Build();

            // Act
            var element = dmsMock.GetElementMock("Element A");
            var similarElements = ElementFinder.FindSimilarOnSameDma(element.Object);

            // Assert
            similarElements.Should().HaveCount(2);
            similarElements.Should().Contain(element.Object);
            similarElements.Should().Contain(element => element.Name == "Element B");
        }

        [TestMethod]
        public void FindSimilarOnSameDma_ReturnsMatchingElementsFromSameDma_WithoutBuilders()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstDma = dmsMock.CreateAgent(1, "DMA 1");
            var secondDma = dmsMock.CreateAgent(2, "DMA 2");
            var thirdDma = dmsMock.CreateAgent(3, "DMA 3");
            string protocolXmlPath = "Examples/protocol.xml";

            firstDma.CreateElement(protocolXmlPath, id: 11, name: "Element A");
            firstDma.CreateElement(protocolXmlPath, id: 12, name: "Element B");

            secondDma.CreateElement(protocolXmlPath, id: 21, name: "Element C");

            thirdDma.CreateElement(protocolXmlPath, id: 31, name: "Element D");
            thirdDma.CreateElement(protocolXmlPath, id: 32, name: "Element E");
            thirdDma.CreateElement(protocolXmlPath, id: 33, name: "Element F");

            // Act
            var element = dmsMock.GetElementMock("Element A");
            var similarElements = ElementFinder.FindSimilarOnSameDma(element.Object);

            // Assert
            similarElements.Should().HaveCount(2);
            similarElements.Should().Contain(element.Object);
            similarElements.Should().Contain(element => element.Name == "Element B");
        }

        [TestMethod]
        public void Repoll_ReplacesExistingRowsWithPolledRows_WithBuilders()
        {
            // Arrange
            var dmsMock = new IDmsBuilder()
                .WithProtocol("Examples/protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 11, name: "Element A", protocolName: "DemoProtocol", configure: element => element
                        .FillTable(100,
                        [
                            ["old", "Old value"]
                        ])))
                .Build();

            // Act
            var connectorApi = new ConnectorApi(dmsMock.Object, "DemoProtocol");
            connectorApi.Repoll();

            // Assert
            var expectedRows = new object[][]
            {
                ["1", "Value1"],
                ["2", "Value2"],
                ["3", "Value3"]
            };

            var dmsTableMock = dmsMock.GetElementMock("Element A").GetDmsTableMock(100);

            dmsTableMock.Verify(t => t.AddRow(It.IsAny<object[]>()), Times.Exactly(3));
            dmsTableMock.AllRows.Values.Should().BeEquivalentTo(expectedRows);
        }

        [TestMethod]
        public void Repoll_ReplacesExistingRowsWithPolledRows_WithoutBuilders()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement("Examples/protocol.xml", id: 11, name: "Element A");
            var table = elementMock.GetDmsTableMock(100);
            table.SetRow(["old", "Old value"]);

            // Act
            var connectorApi = new ConnectorApi(dmsMock.Object, "DemoProtocol");
            connectorApi.Repoll();

            // Assert
            var expectedRows = new object[][]
            {
                ["1", "Value1"],
                ["2", "Value2"],
                ["3", "Value3"]
            };

            table.Verify(t => t.AddRow(It.IsAny<object[]>()), Times.Exactly(3));
            table.AllRows.Values.Should().BeEquivalentTo(expectedRows);
        }

        [TestMethod]
        public void RestartAndEnablePolling_SetsPollingStatusToEnabledAfterCompletedStartup_WithBuilders()
        {
            // Arrange
            var dmsMock = new IDmsBuilder()
                .WithProtocol("Examples/protocol.xml")
                .WithDma(id: 1, dma => dma
                    .WithElement(id: 11, name: "Element A", protocolName: "DemoProtocol", configure: element => element
                        .SetParameter<int?>(parameterId: 10, value: 0)))
                .Build();

            var elementMock = dmsMock.GetElementMock("Element A");

            elementMock.Setup(e => e.IsStartupComplete()).Returns(() =>
            {
                elementMock.State = ElementState.Active;
                return true;
            });

            // Act
            var connectorApi = new ConnectorApi(dmsMock.Object, "DemoProtocol");
            connectorApi.RestartAndEnablePolling();

            // Assert
            elementMock.Verify(e => e.Restart(), Times.Once());
            elementMock.State.Should().Be(ElementState.Active);

            var parameterMock = elementMock.GetStandaloneParameterMock<int?>(10);
            parameterMock.Verify(p => p.SetValue(1), Times.Once());
            parameterMock.Value.Should().Be(1);
        }

        [TestMethod]
        public void RestartAndEnablePolling_SetsPollingStatusToEnabledAfterCompletedStartup_WithoutBuilders()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement("Examples/protocol.xml", id: 11, name: "Element A");
            var parameterMock = elementMock.GetStandaloneParameterMock<int?>(10);
            parameterMock.UpdateValue(10);

            elementMock.Setup(e => e.IsStartupComplete()).Returns(() =>
            {
                elementMock.State = ElementState.Active;
                return true;
            });

            // Act
            var connectorApi = new ConnectorApi(dmsMock.Object, "DemoProtocol");
            connectorApi.RestartAndEnablePolling();

            // Assert
            elementMock.Verify(e => e.Restart(), Times.Once());
            elementMock.State.Should().Be(ElementState.Active);

            parameterMock.Verify(p => p.SetValue(1), Times.Once());
            parameterMock.Value.Should().Be(1);
        }
    }

    internal static class ElementFinder
    {
        public static List<IDmsElement> FindSimilarOnSameDma(IDmsElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.Protocol == null)
            {
                throw new ArgumentException($"Element '{element.Name}' does not have a protocol defined.", nameof(element));
            }

            var dma = element.Host ?? throw new InvalidOperationException($"Element '{element.Name}' is not hosted on any DMA.");

            return dma.GetElements().Where(el => el.Protocol.Name == element.Protocol.Name && el.Protocol.ReferencedVersion == element.Protocol.ReferencedVersion).ToList();
        }

        public static List<IDmsView> FindOtherViews(IDmsView view)
        {
            if (view is null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            var element = view.Elements.FirstOrDefault() ?? throw new InvalidOperationException($"View '{view.Name}' does not contain any elements.");

            var dma = element.Host ?? throw new InvalidOperationException($"Element '{element.Name}' is not hosted on any DMA.");

            var dms = dma.Dms ?? throw new InvalidOperationException($"DMA '{dma.Name}' is not hosted on any DMS.");

            var allViews = dms.GetViews();

            return allViews.Except(new[] { view }).ToList();
        }
    }

    internal class ConnectorApi
    {
        private readonly IDmsElement element;

        internal ConnectorApi(IDms dms, string protocolName)
        {
            element = dms.GetElements().FirstOrDefault(el => el.Protocol.Name == protocolName) ?? throw new InvalidOperationException($"Element with protocol name '{protocolName}' not found.");
        }

        public void Repoll()
        {
            var table = element.GetTable(100);

            table.DeleteRows(table.GetPrimaryKeys());

            var polledRows = new object[][]
            {
                new object[] { "1", "Value1" },
                new object[] { "2", "Value2" },
                new object[] { "3", "Value3" }
            };

            foreach (var row in polledRows)
            {
                table.AddRow(row);
            }
        }

        public void RestartAndEnablePolling()
        {
            if (element.State != ElementState.Active)
            {
                throw new InvalidOperationException($"Element is not active. Current state: {element.State}");
            }

            element.Restart();

            int retries = 0;
            while (!element.IsStartupComplete() && retries < 3)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                retries++;
            }

            if (element.State != ElementState.Active)
            {
                throw new InvalidOperationException("Element did not complete startup within the expected time.");
            }

            element.GetStandaloneParameter<int?>(10).SetValue(1);
        }
    }
}
