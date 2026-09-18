
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using global::Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Moq;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmaMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Id_DefaultValue_ReturnsZero()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var id = mock.Object.Id;

            // Assert
            Assert.AreEqual(0, id);
        }

        [TestMethod]
        public void Id_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 123);

            // Act
            var id = mock.Object.Id;

            // Assert
            Assert.AreEqual(123, id);
        }

        [TestMethod]
        public void Name_DefaultValue_ReturnsAgent()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var name = mock.Object.Name;

            // Assert
            Assert.AreEqual("Agent", name);
        }

        [TestMethod]
        public void Name_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0, name: "Main DMA");

            // Act
            var name = mock.Object.Name;

            // Assert
            Assert.AreEqual("Main DMA", name);
        }

        [TestMethod]
        public void HostName_DefaultValue_ReturnsLocalhost()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var hostName = mock.Object.HostName;

            // Assert
            Assert.AreEqual("localhost", hostName);
        }

        [TestMethod]
        public void HostName_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);
            mock.HostName = "dma-server-01";

            // Act
            var hostName = mock.Object.HostName;

            // Assert
            Assert.AreEqual("dma-server-01", hostName);
        }

        [TestMethod]
        public void State_DefaultValue_ReturnsRunning()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var state = mock.Object.State;

            // Assert
            Assert.AreEqual(AgentState.Running, state);
        }

        [TestMethod]
        [DataRow(AgentState.NotRunning)]
        [DataRow(AgentState.Running)]
        [DataRow(AgentState.Starting)]
        [DataRow(AgentState.Unknown)]
        [DataRow(AgentState.Switching)]
        public void State_CustomValue_ReturnsProvidedValue(AgentState expectedState)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);
            mock.State = expectedState;

            // Act
            var state = mock.Object.State;

            // Assert
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        public void State_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidState = (AgentState)999;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                var mock = new IDmsMock().CreateAgent(agentId: 0);
                mock.State = invalidState;
            });
        }

        [TestMethod]
        public void CreateElement_CreatedElement_ReturnsOwningDma()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 123);
            var elementMock = dmaMock.CreateElement(path);

            // Act
            var host = elementMock.Object.Host;

            // Assert
            Assert.AreSame(dmaMock.Object, host);
        }

        [TestMethod]
        public void Scheduler_DefaultValue_ReturnsNonNullInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var scheduler = mock.Object.Scheduler;

            // Assert
            Assert.IsNotNull(scheduler);
        }

        [TestMethod]
        public void Scheduler_CalledTwice_ReturnsSameInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var first = mock.Object.Scheduler;
            var second = mock.Object.Scheduler;

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Scheduler_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedScheduler = new Mock<IDmsScheduler>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0);
            mock.Scheduler = expectedScheduler;

            // Act
            var scheduler = mock.Object.Scheduler;

            // Assert
            Assert.AreSame(expectedScheduler, scheduler);
        }

        [TestMethod]
        public void VersionInfo_DefaultValue_ReturnsDefaultVersion()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);

            // Act
            var versionInfo = mock.Object.VersionInfo;

            // Assert
            Assert.AreEqual("0.0.0.0", versionInfo);
        }

        [TestMethod]
        public void VersionInfo_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0);
            mock.VersionInfo = "10.5.0.0";

            // Act
            var versionInfo = mock.Object.VersionInfo;

            // Assert
            Assert.AreEqual("10.5.0.0", versionInfo);
        }

        [TestMethod]
        public void GetElements_AfterCreatingElement_ReturnsCreatedElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Element");

            // Act
            var elements = dmaMock.Object.GetElements();

            // Assert
            Assert.AreEqual(1, elements.Count);
            Assert.AreSame(elementMock.Object, elements.Single());
        }

        [TestMethod]
        public void CreateService_CustomValues_AddsServiceToDma()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act
            var serviceMock = dmaMock.CreateService(serviceId: 123, name: "Main Service");

            // Assert
            Assert.AreEqual(123, serviceMock.Object.Id);
            Assert.AreEqual(1, serviceMock.Object.AgentId);
            Assert.AreEqual(new DmsServiceId(1, 123), serviceMock.Object.DmsServiceId);
            Assert.AreEqual("Main Service", serviceMock.Object.Name);
            Assert.AreSame(dmaMock.Object, serviceMock.Object.Host);
            Assert.AreSame(serviceMock.Object, dmaMock.Object.GetServices().Single());
        }

        [TestMethod]
        public void CreateService_Configuration_CreatesServiceAndReturnsId()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(agentId: 1);
            var viewMock = dmsMock.CreateView(viewId: 10);
            var configuration = new ServiceConfiguration(dmsMock.Object, "Configured Service") { Description = "Service Description" };
            configuration.Views.Add(viewMock.Object);

            // Act
            var serviceId = dmaMock.Object.CreateService(configuration);
            var service = dmaMock.Object.GetService(serviceId);

            // Assert
            Assert.AreEqual(new DmsServiceId(1, 1), serviceId);
            Assert.AreEqual("Configured Service", service.Name);
            Assert.AreEqual("Service Description", service.Description);
            Assert.AreSame(dmaMock.Object, service.Host);
            Assert.AreSame(viewMock.Object, service.Views.Single());
            Assert.AreSame(service, viewMock.Object.Services.Single());
        }

        [TestMethod]
        public void CreateService_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => dmaMock.Object.CreateService(null));
        }

        [TestMethod]
        public void CreateService_ConfigurationWithForeignView_ThrowsIncorrectDataExceptionAndDoesNotCreateService()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(agentId: 1);
            dmsMock.CreateView(viewId: 10, name: "Local View");
            var foreignViewMock = new IDmsMock().CreateView(viewId: 10, name: "Foreign View");
            var configuration = new ServiceConfiguration(dmsMock.Object, "Configured Service");
            configuration.Views.Add(foreignViewMock.Object);

            // Act & Assert
            Assert.ThrowsExactly<IncorrectDataException>(() => dmaMock.Object.CreateService(configuration));
            Assert.IsEmpty(dmaMock.Object.GetServices());
        }

        [TestMethod]
        public void ServiceExists_ExistingDmsServiceId_ReturnsTrue()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var serviceMock = dmaMock.CreateService(serviceId: 123);

            // Act
            var exists = dmaMock.Object.ServiceExists(serviceMock.Object.DmsServiceId);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ServiceExists_ServiceHostedOnDifferentDma_ReturnsFalse()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstDmaMock = dmsMock.CreateAgent(agentId: 1, name: "First DMA");
            var secondDmaMock = dmsMock.CreateAgent(agentId: 2, name: "Second DMA");
            var serviceMock = secondDmaMock.CreateService(serviceId: 123);

            // Act
            var exists = firstDmaMock.Object.ServiceExists(serviceMock.Object.DmsServiceId);

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ServiceExists_ExistingName_ReturnsTrue()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateService(serviceId: 123, name: "Main Service");

            // Act
            var exists = dmaMock.Object.ServiceExists("main service");

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ServiceExists_UnknownService_ReturnsFalse()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.IsFalse(dmaMock.Object.ServiceExists(new DmsServiceId(1, 123)));
            Assert.IsFalse(dmaMock.Object.ServiceExists("Unknown Service"));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(-1, 1)]
        [DataRow(1, -1)]
        public void ServiceExists_InvalidDmsServiceId_ThrowsArgumentException(int agentId, int serviceId)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.ServiceExists(new DmsServiceId(agentId, serviceId)));
        }

        [TestMethod]
        public void ServiceExists_NullName_ThrowsArgumentNullException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => dmaMock.Object.ServiceExists((string)null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void ServiceExists_EmptyOrWhiteSpaceName_ThrowsArgumentException(string serviceName)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.ServiceExists(serviceName));
        }

        [TestMethod]
        public void GetService_ExistingService_ReturnsCreatedService()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var serviceMock = dmaMock.CreateService(serviceId: 123, name: "Main Service");

            // Act & Assert
            Assert.AreSame(serviceMock.Object, dmaMock.Object.GetService(serviceMock.Object.DmsServiceId));
            Assert.AreSame(serviceMock.Object, dmaMock.Object.GetService("Main Service"));
        }

        [TestMethod]
        public void GetService_UnknownService_ThrowsServiceNotFoundException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ServiceNotFoundException>(() => dmaMock.Object.GetService(new DmsServiceId(1, 123)));
            Assert.ThrowsExactly<ServiceNotFoundException>(() => dmaMock.Object.GetService("Unknown Service"));
        }

        [TestMethod]
        public void ElementExists_ExistingDmsElementId_ReturnsTrue()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Element");

            // Act
            var exists = dmaMock.Object.ElementExists(elementMock.Object.DmsElementId);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ElementExists_UnknownDmsElementId_ReturnsFalse()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var unknownElementId = new DmsElementId(agentId: 1, elementId: 999);

            // Act
            var exists = dmaMock.Object.ElementExists(unknownElementId);
         
            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ElementExists_ElementHostedOnDifferentDma_ReturnsFalse()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstDmaMock = dmsMock.CreateAgent(agentId: 1, name: "First Agent");
            var secondDmaMock = dmsMock.CreateAgent(agentId: 2, name: "Second Agent");
            var elementMock = secondDmaMock.CreateElement(path, id: 123, name: "Element");

            // Act
            var exists = firstDmaMock.Object.ElementExists(elementMock.Object.DmsElementId);

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(-1, 1)]
        [DataRow(1, -1)]
        public void ElementExists_InvalidDmsElementId_ThrowsArgumentException(int agentId, int elementId)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.ElementExists(new DmsElementId(agentId, elementId)));
        }

        [TestMethod]
        public void ElementExists_ExistingName_ReturnsTrue()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateElement(path, id: 123, name: "Test Element");

            // Act
            var exists = dmaMock.Object.ElementExists("Test Element");

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void ElementExists_UnknownName_ReturnsFalse()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateElement(path, id: 123, name: "Test Element");

            // Act
            var exists = dmaMock.Object.ElementExists("Unknown Element");

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ElementExists_NullName_ThrowsArgumentNullException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => dmaMock.Object.ElementExists((string)null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void ElementExists_EmptyOrWhiteSpaceName_ThrowsArgumentException(string elementName)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.ElementExists(elementName));
        }

        [TestMethod]
        public void GetElement_ExistingDmsElementId_ReturnsCreatedElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");

            // Act
            var element = dmaMock.Object.GetElement(elementMock.Object.DmsElementId);

            // Assert
            Assert.AreSame(elementMock.Object, element);
        }

        [TestMethod]
        public void GetElement_UnknownDmsElementId_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var unknownElementId = new DmsElementId(agentId: 1, elementId: 999);

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => dmaMock.Object.GetElement(unknownElementId));
        }

        [TestMethod]
        public void GetElement_ElementHostedOnDifferentDma_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstDmaMock = dmsMock.CreateAgent(agentId: 1, name: "First Agent");
            var secondDmaMock = dmsMock.CreateAgent(agentId: 2, name: "Second Agent");
            var elementMock = secondDmaMock.CreateElement(path, id: 123, name: "Element");

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => firstDmaMock.Object.GetElement(elementMock.Object.DmsElementId));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(-1, 1)]
        [DataRow(1, -1)]
        public void GetElement_InvalidDmsElementId_ThrowsArgumentException(int agentId, int elementId)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.GetElement(new DmsElementId(agentId, elementId)));
        }

        [TestMethod]
        public void GetElement_ExistingName_ReturnsCreatedElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");

            // Act
            var element = dmaMock.Object.GetElement("Test Element");

            // Assert
            Assert.AreSame(elementMock.Object, element);
        }
        [TestMethod]
        public void GetElement_NullName_ThrowsArgumentNullException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => dmaMock.Object.GetElement((string)null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void GetElement_EmptyOrWhiteSpaceName_ThrowsArgumentException(string elementName)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmaMock.Object.GetElement(elementName));
        }

        [TestMethod]
        public void GetElement_UnknownName_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateElement(path, id: 123, name: "Existing Element");

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => dmaMock.Object.GetElement("Unknown Element"));
        }

        [TestMethod]
        public void GetElement_RenamedElement_ReturnsElementByNewName()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Old Name");
            elementMock.Object.Name = "New Name";

            // Act
            var element = dmaMock.Object.GetElement("New Name");

            // Assert
            Assert.AreSame(elementMock.Object, element);
        }

        [TestMethod]
        public void Dms_CreatedAgent_ReturnsOwningDms()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(agentId: 1);

            // Act
            var dms = dmaMock.Object.Dms;

            // Assert
            Assert.AreSame(dmsMock.Object, dms);
        }
        [TestMethod]
        public void ElementExists_DeletedElement_ReturnsFalse()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");
            elementMock.Object.Delete();

            // Act
            var exists = dmaMock.Object.ElementExists(elementMock.Object.DmsElementId);

            // Assert
            Assert.IsFalse(exists);
        }

        [TestMethod]
        public void ElementExists_DeletedElementName_ReturnsFalse()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");
            elementMock.Object.Delete();

            // Act
            var exists = dmaMock.Object.ElementExists("Test Element");

            // Assert
            Assert.IsFalse(exists);
        }
        [TestMethod]
        public void GetElement_DeletedDmsElementId_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");
            var dmsElementId = elementMock.Object.DmsElementId;
            elementMock.Object.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => dmaMock.Object.GetElement(dmsElementId));
        }
        [TestMethod]
        public void GetElement_DeletedName_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 123, name: "Test Element");
            elementMock.Object.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => dmaMock.Object.GetElement("Test Element"));
        }

        [TestMethod]
        public void GetElements_DeletedElement_ReturnsOnlyExistingElements()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var deletedElementMock = dmaMock.CreateElement(path, id: 1, name: "Deleted Element");
            var existingElementMock = dmaMock.CreateElement(path, id: 2, name: "Existing Element");
            deletedElementMock.Object.Delete();

            // Act
            var elements = dmaMock.Object.GetElements();

            // Assert
            Assert.AreEqual(1, elements.Count);
            Assert.AreSame(existingElementMock.Object, elements.Single());
        }
        [TestMethod]
        public void GetElements_NoElements_ReturnsEmptyCollection()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act
            var elements = dmaMock.Object.GetElements();

            // Assert
            Assert.IsNotNull(elements);
            Assert.HasCount(0, elements);
        }

        [TestMethod]
        public void GetElements_MultipleCreatedElements_ReturnsAllElements()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            var elements = dmaMock.Object.GetElements();

            // Assert
            Assert.AreEqual(2, elements.Count);
            CollectionAssert.Contains(elements.ToList(), firstMock.Object);
            CollectionAssert.Contains(elements.ToList(), secondMock.Object);
        }

        //business
        [TestMethod]
        public void FindElementByName_ExistingName_ReturnsMatchingElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateElement(path, id: 1, name: "First Element");
            var expectedElementMock = dmaMock.CreateElement(path, id: 2, name: "Target Element");

            // Act
            var element = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "Target Element");

            // Assert
            Assert.AreSame(expectedElementMock.Object, element);
        }

        [TestMethod]
        public void FindElementByName_DifferentLetterCase_ReturnsMatchingElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var expectedElementMock = dmaMock.CreateElement(path, id: 1, name: "Target Element");

            // Act
            var element = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "target element");

            // Assert
            Assert.AreSame(expectedElementMock.Object, element);
        }

        [TestMethod]
        public void FindElementByName_UnknownName_ReturnsNull()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            dmaMock.CreateElement(path, id: 1, name: "Existing Element");

            // Act
            var element = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "Unknown Element");

            // Assert
            Assert.IsNull(element);
        }

        [TestMethod]
        public void FindElementByName_NullDma_ThrowsArgumentNullException()
        {
            // Arrange
            IDma dma = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Business_Code_Example_FindElementByName.FindElementByName(dma, "Element"));
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public void FindElementByName_NullOrEmptyName_ThrowsArgumentException(string elementName)
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, elementName));
        }

        [TestMethod]
        public void FindElementByName_DeletedElement_ReturnsNull()
        {
            // Arrangeilho
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 1, name: "Deleted Element");
            elementMock.Object.Delete();

            // ActJenn
            var element = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "Deleted Element");

            // Assert
            Assert.IsNull(element);
        }

        [TestMethod]
        public void FindElementByName_AfterRename_ReturnsElementByNewName()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var elementMock = dmaMock.CreateElement(path, id: 1, name: "Original Element");

            // Act
            Business_Code_Example_RenameElement.RenameElement(elementMock.Object, "Renamed Element");
            var elementWithOldName = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "Original Element");
            var elementWithNewName = Business_Code_Example_FindElementByName.FindElementByName(dmaMock.Object, "Renamed Element");

            // Assert
            Assert.IsNull(elementWithOldName);
            Assert.AreSame(elementMock.Object, elementWithNewName);
        }
        //

        [TestMethod]
        public void Exists_CreatedAgent_ReturnsTrue()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);

            // Act
            var exists = dmaMock.Object.Exists();

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        public void IsVersionHigher_HigherVersion_ReturnsTrue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);
            mock.VersionInfo = "10.3.0.0";

            // Act
            var isHigher = mock.Object.IsVersionHigher("10.4.0.0");

            // Assert
            Assert.IsTrue(isHigher);
        }

        [TestMethod]
        public void IsVersionHigher_EqualVersion_ReturnsFalse()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);
            mock.VersionInfo = "10.3.0.0";

            // Act
            var isHigher = mock.Object.IsVersionHigher("10.3.0.0");

            // Assert
            Assert.IsFalse(isHigher);
        }

        [TestMethod]
        public void IsVersionHigher_LowerVersion_ReturnsFalse()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);
            mock.VersionInfo = "10.3.0.0";

            // Act
            var isHigher = mock.Object.IsVersionHigher("10.2.0.0");

            // Assert
            Assert.IsFalse(isHigher);
        }

        [TestMethod]
        public void IsVersionHigher_DifferentCuWithSameVersion_ReturnsFalse()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);
            mock.VersionInfo = "10.3.0.0-CU1";

            // Act
            var isHigher = mock.Object.IsVersionHigher("10.3.0.0-CU10");

            // Assert
            Assert.IsFalse(isHigher);
        }

        [TestMethod]
        public void IsVersionHigher_NullVersion_ThrowsArgumentNullException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.IsVersionHigher(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("10.3")]
        [DataRow("10.3.0")]
        [DataRow("10.3.0.0.1")]
        [DataRow("version")]
        [DataRow("10.3.0.0-CU")]
        [DataRow("10.3.0.0-CUabc")]
        public void IsVersionHigher_InvalidFormat_ThrowsArgumentException(string version)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.IsVersionHigher(version));
        }

    }
}
