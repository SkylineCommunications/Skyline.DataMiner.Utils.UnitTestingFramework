namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Communication_DefaultValue_ReturnsNonNullInstance()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var communication = mock.Object.Communication;

            // Assert
            Assert.IsNotNull(communication);
        }
        [TestMethod]
        public void Communication_CalledTwice_ReturnsSameInstance()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var firstCommunication = mock.Object.Communication;
            var secondCommunication = mock.Object.Communication;

            // Assert
            Assert.AreSame(firstCommunication, secondCommunication);

        }
        [TestMethod]
        public void Communication_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var communicationMock = new Mock<ICommunication>();
            var mock = new IDmsMock();
            mock.Communication = communicationMock.Object;

            // Act
            var communication = mock.Object.Communication;

            // Assert
            Assert.AreSame(communicationMock.Object, communication);
        }

        [TestMethod]
        public void ElementPropertyDefinitions_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var propertyDefinitions = mock.Object.ElementPropertyDefinitions;
            var enumeratedDefinitions = propertyDefinitions.Cast<IDmsElementPropertyDefinition>().ToList();

            // Assert
            Assert.IsNotNull(propertyDefinitions);
            Assert.AreEqual(0, propertyDefinitions.Count);
            Assert.AreEqual(0, enumeratedDefinitions.Count);
        }

        [TestMethod]
        public void ElementPropertyDefinitions_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedDefinitions = new Mock<IPropertyDefinitionCollection<IDmsElementPropertyDefinition>>().Object;
            var mock = new IDmsMock();
            mock.ElementPropertyDefinitions = expectedDefinitions;

            // Act
            var propertyDefinitions = mock.Object.ElementPropertyDefinitions;

            // Assert
            Assert.AreSame(expectedDefinitions, propertyDefinitions);
        }

        [TestMethod]
        public void ServicePropertyDefinitions_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var propertyDefinitions = mock.Object.ServicePropertyDefinitions;
            var enumeratedDefinitions = propertyDefinitions.Cast<IDmsServicePropertyDefinition>().ToList();

            // Assert
            Assert.IsNotNull(propertyDefinitions);
            Assert.AreEqual(0, propertyDefinitions.Count);
            Assert.AreEqual(0, enumeratedDefinitions.Count);
        }

        [TestMethod]
        public void ServicePropertyDefinitions_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedDefinitions = new Mock<IPropertyDefinitionCollection<IDmsServicePropertyDefinition>>().Object;
            var mock = new IDmsMock();
            mock.ServicePropertyDefinitions = expectedDefinitions;

            // Act
            var propertyDefinitions = mock.Object.ServicePropertyDefinitions;

            // Assert
            Assert.AreSame(expectedDefinitions, propertyDefinitions);
        }

        [TestMethod]
        public void ViewPropertyDefinitions_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var propertyDefinitions = mock.Object.ViewPropertyDefinitions;
            var enumeratedDefinitions = propertyDefinitions.Cast<IDmsViewPropertyDefinition>().ToList();

            // Assert
            Assert.IsNotNull(propertyDefinitions);
            Assert.AreEqual(0, propertyDefinitions.Count);
            Assert.AreEqual(0, enumeratedDefinitions.Count);
        }

        [TestMethod]
        public void ViewPropertyDefinitions_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedDefinitions = new Mock<IPropertyDefinitionCollection<IDmsViewPropertyDefinition>>().Object;
            var mock = new IDmsMock();
            mock.ViewPropertyDefinitions = expectedDefinitions;

            // Act
            var propertyDefinitions = mock.Object.ViewPropertyDefinitions;

            // Assert
            Assert.AreSame(expectedDefinitions, propertyDefinitions);
        }

        [TestMethod]
        public void CreateAgent_CustomValues_AddsAgentToDms()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var agentMock = mock.CreateAgent(agentId: 1, name: "Main Agent");

            // Assert
            Assert.IsTrue(mock.Object.AgentExists(1));
            Assert.AreSame(agentMock.Object, mock.Object.GetAgent(1));
            Assert.AreSame(agentMock.Object, mock.Object.GetAgents().Single());
        }

        [TestMethod]
        public void CreateView_CustomValues_AddsViewToDms()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var viewMock = mock.CreateView(viewId: 10, name: "Main View");

            // Assert
            Assert.AreEqual(10, viewMock.Object.Id);
            Assert.AreEqual("Main View", viewMock.Object.Name);
            Assert.AreSame(mock.Object, viewMock.Object.Dms);
            Assert.IsTrue(mock.Object.ViewExists(10));
            Assert.IsTrue(mock.Object.ViewExists("Main View"));
            Assert.AreSame(viewMock.Object, mock.Object.GetView(10));
            Assert.AreSame(viewMock.Object, mock.Object.GetView("Main View"));
            Assert.AreSame(viewMock.Object, mock.Object.GetViews().Single());
        }

        [TestMethod]
        public void ServiceMethods_CreatedService_ReturnServiceFromDms()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var serviceMock = dmsMock.CreateAgent(agentId: 1).CreateService(serviceId: 2, name: "Main Service");

            // Act & Assert
            Assert.IsTrue(dmsMock.Object.ServiceExists(serviceMock.Object.DmsServiceId));
            Assert.IsTrue(dmsMock.Object.ServiceExists("main service"));
            Assert.AreSame(serviceMock.Object, dmsMock.Object.GetService(serviceMock.Object.DmsServiceId));
            Assert.AreSame(serviceMock.Object, dmsMock.Object.GetService("Main Service"));
            Assert.AreSame(serviceMock.Object, dmsMock.Object.GetServices().Single());
        }

        [TestMethod]
        public void CreateView_Configuration_CreatesViewWithGeneratedIdAndParent()
        {
            // Arrange
            var mock = new IDmsMock();
            var rootMock = mock.CreateView(viewId: -1, name: "Root View");
            var firstConfiguration = new ViewConfiguration("First View", rootMock.Object);

            // Act
            var firstViewId = mock.Object.CreateView(firstConfiguration);
            var secondViewId = mock.Object.CreateView(new ViewConfiguration("Second View", rootMock.Object));

            // Assert
            Assert.AreEqual(1, firstViewId);
            Assert.AreEqual(2, secondViewId);
            Assert.AreEqual("First View", mock.Object.GetView(firstViewId).Name);
            Assert.AreSame(rootMock.Object, mock.Object.GetView(firstViewId).Parent);
            Assert.AreSame(rootMock.Object, mock.Object.GetView(secondViewId).Parent);
            Assert.AreEqual(2, rootMock.Object.ChildViews.Count);
        }

        [TestMethod]
        public void CreateView_NullConfiguration_ThrowsArgumentNullException()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.CreateView(null));
        }

        [TestMethod]
        public void CreateView_NonExistingParent_ThrowsIncorrectDataExceptionAndDoesNotCreateView()
        {
            // Arrange
            var mock = new IDmsMock();
            var parentReference = mock.Object.GetViewReference(10);
            var configuration = new ViewConfiguration("Child View", parentReference);

            // Act & Assert
            Assert.ThrowsExactly<IncorrectDataException>(() => mock.Object.CreateView(configuration));
            Assert.IsEmpty(mock.Object.GetViews());
        }

        [TestMethod]
        public void CreateView_ParentFromDifferentDms_ThrowsIncorrectDataExceptionAndDoesNotCreateView()
        {
            // Arrange
            var mock = new IDmsMock();
            var otherDmsMock = new IDmsMock();
            var parentMock = otherDmsMock.CreateView(viewId: -1, name: "Other Root View");
            var configuration = new ViewConfiguration("Child View", parentMock.Object);

            // Act & Assert
            Assert.ThrowsExactly<IncorrectDataException>(() => mock.Object.CreateView(configuration));
            Assert.IsEmpty(mock.Object.GetViews());
        }

        [TestMethod]
        public void CreateView_ConfigurationWithExistingName_ThrowsIncorrectDataExceptionAndDoesNotCreateView()
        {
            // Arrange
            var mock = new IDmsMock();
            var rootMock = mock.CreateView(viewId: -1, name: "Root View");
            var existingViewMock = mock.CreateView(viewId: 10, name: "Existing View");
            var configuration = new ViewConfiguration("Existing View", rootMock.Object);

            // Act & Assert
            Assert.ThrowsExactly<IncorrectDataException>(() => mock.Object.CreateView(configuration));
            Assert.AreSame(existingViewMock.Object, mock.Object.GetView("Existing View"));
            Assert.AreEqual(2, mock.Object.GetViews().Count);
        }

        [TestMethod]
        public void ViewExists_NonExistingView_ReturnsFalse()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.IsFalse(mock.Object.ViewExists(10));
            Assert.IsFalse(mock.Object.ViewExists("Missing View"));
        }

        [TestMethod]
        public void ViewMethods_NameWithDifferentCasing_ReturnExistingView()
        {
            // Arrange
            var mock = new IDmsMock();
            var viewMock = mock.CreateView(viewId: 10, name: "Main View");

            // Act & Assert
            Assert.IsTrue(mock.Object.ViewExists("main view"));
            Assert.AreSame(viewMock.Object, mock.Object.GetView("MAIN VIEW"));
        }

        [TestMethod]
        [DataRow(-2)]
        [DataRow(0)]
        public void ViewMethods_InvalidId_ThrowArgumentException(int viewId)
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.ViewExists(viewId));
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.GetView(viewId));
            Assert.ThrowsExactly<ArgumentException>(() => mock.CreateView(viewId));
        }

        [TestMethod]
        public void CreateView_DuplicateId_ThrowsArgumentExceptionAndKeepsExistingView()
        {
            // Arrange
            var mock = new IDmsMock();
            var existingViewMock = mock.CreateView(viewId: 10, name: "Existing View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.CreateView(viewId: 10, name: "Other View"));
            Assert.AreSame(existingViewMock.Object, mock.Object.GetView(10));
            Assert.AreEqual(1, mock.Object.GetViews().Count);
        }

        [TestMethod]
        public void GetView_NonExistingView_ThrowsViewNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ViewNotFoundException>(() => mock.Object.GetView(10));
            Assert.ThrowsExactly<ViewNotFoundException>(() => mock.Object.GetView("Missing View"));
        }

        [TestMethod]
        public void GetViewReference_ExistingView_ReturnsCachedView()
        {
            // Arrange
            var mock = new IDmsMock();
            var viewMock = mock.CreateView(viewId: 10, name: "Main View");

            // Act
            var view = mock.Object.GetViewReference(10);

            // Assert
            Assert.AreSame(viewMock.Object, view);
        }

        [TestMethod]
        public void GetViewReference_NonExistingView_ReturnsReferenceWithoutAddingView()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var view = mock.Object.GetViewReference(10);

            // Assert
            Assert.AreEqual(10, view.Id);
            Assert.AreSame(mock.Object, view.Dms);
            Assert.IsFalse(mock.Object.ViewExists(10));
            Assert.IsEmpty(mock.Object.GetViews());
        }

        [TestMethod]
        public void ViewMethods_NullName_ThrowArgumentNullException()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.ViewExists(null));
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.GetView(null));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        public void ViewMethods_EmptyOrWhiteSpaceName_ThrowArgumentException(string viewName)
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.ViewExists(viewName));
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.GetView(viewName));
        }

        [TestMethod]
        public void GetProtocols_TwoElementsUsingSameProtocol_ReturnsOneProtocol()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(agentId: 1);
            var first = dmaMock.CreateElement(path, id: 1, name: "First Element");
            dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            var protocols = dmsMock.Object.GetProtocols();

            // Assert
            Assert.AreEqual(1, protocols.Count);
            Assert.AreSame(first.Object.Protocol, protocols.Single());
        }

        [TestMethod]
        public void GetAgent_NonExistingId_ThrowsAgentNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<AgentNotFoundException>(() => mock.Object.GetAgent(123));
        }

        [TestMethod]
        public void GetAgentReference_NonExistingId_ReturnsReferenceThatDoesNotExist()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act
            var agent = mock.Object.GetAgentReference(123);

            // Assert
            Assert.AreEqual(123, agent.Id);
            Assert.IsFalse(agent.Exists());
        }

        [TestMethod]
        public void CreateElement_ThroughAgent_AddsElementToDms()
        {
            // Arrange
            var mock = new IDmsMock();
            var agentMock = mock.CreateAgent(agentId: 1);

            // Act
            var elementMock = agentMock.CreateElement(path, id: 2, name: "Test Element");

            // Assert
            Assert.IsTrue(mock.Object.ElementExists(new DmsElementId(1, 2)));
            Assert.IsTrue(mock.Object.ElementExists("Test Element"));
            Assert.AreSame(elementMock.Object, mock.Object.GetElement(new DmsElementId(1, 2)));
            Assert.AreSame(elementMock.Object, mock.Object.GetElement("Test Element"));
            Assert.AreSame(elementMock.Object, mock.Object.GetElements().Single());
        }

        [TestMethod]
        public void GetElement_NonExistingElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => mock.Object.GetElement(new DmsElementId(1, 2)));
            Assert.ThrowsExactly<ElementNotFoundException>(() => mock.Object.GetElement("Missing Element"));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 0)]
        [DataRow(-1, 1)]
        [DataRow(1, -1)]
        public void GetElement_InvalidDmsElementId_ThrowsArgumentException(int agentId, int elementId)
        {
            // Arrange
            var mock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.GetElement(new DmsElementId(agentId, elementId)));
        }

        [TestMethod]
        public void GetElementReference_NonExistingId_ReturnsReferenceWithProvidedId()
        {
            // Arrange
            var mock = new IDmsMock();
            var elementId = new DmsElementId(1, 2);

            // Act
            var element = mock.Object.GetElementReference(elementId);

            // Assert
            Assert.AreEqual(1, element.AgentId);
            Assert.AreEqual(2, element.Id);
            Assert.AreEqual(elementId, element.DmsElementId);
        }

        [TestMethod]
        public void Delete_Element_RemovesElementFromDms()
        {
            // Arrange
            var mock = new IDmsMock();
            var elementMock = mock.CreateAgent(agentId: 1).CreateElement(path, id: 2, name: "Test Element");

            // Act
            elementMock.Object.Delete();

            // Assert
            Assert.IsFalse(mock.Object.ElementExists(new DmsElementId(1, 2)));
            Assert.IsFalse(mock.Object.ElementExists("Test Element"));
            Assert.IsEmpty(mock.Object.GetElements());
        }
    }
}
