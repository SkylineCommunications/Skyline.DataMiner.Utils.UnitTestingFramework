namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;
    using global::Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples;
    using Moq;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Templates;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;


    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMockTests
    {
        private readonly string path = "protocol.xml";

       

        [TestMethod]
        public void AdvancedSettings_DefaultValue_ReturnsNonNullInstance()
        {
            // Arrange
            var mock = new IDmsMock()
            .CreateAgent(agentId: 0)
            .CreateElement(path);

            // Act
            var advancedSettings = mock.Object.AdvancedSettings;

            // Assert
            Assert.IsNotNull(advancedSettings);
        }

        [TestMethod]
        public void AdvancedSettings_CalledTwice_ReturnsSameInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var first = mock.Object.AdvancedSettings;
            var second = mock.Object.AdvancedSettings;

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void AdvancedSettings_MultipleMocks_ReturnIndependentInstances()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            var first = firstMock.Object.AdvancedSettings;
            var second = secondMock.Object.AdvancedSettings;

            // Assert
            Assert.AreNotSame(first, second);
        }

        [TestMethod]
        public void Id_DefaultValue_ReturnsZero()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var id = mock.Object.Id;

            // Assert
            Assert.AreEqual(0, id);
        }

        [TestMethod]
        public void Id_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, id: 456);

            // Act
            var id = mock.Object.Id;

            // Assert
            Assert.AreEqual(456, id);
        }

        [TestMethod]
        public void AgentId_DefaultValue_ReturnsZero()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var agentId = mock.Object.AgentId;

            // Assert
            Assert.AreEqual(0, agentId);
        }

        [TestMethod]
        public void AgentId_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 123).CreateElement(path);

            // Act
            var agentId = mock.Object.AgentId;

            // Assert
            Assert.AreEqual(123, agentId);
        }

        [TestMethod]
        public void Identifiers_CustomValues_AreUsedInParameterValueChange()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 123).CreateElement(path, id: 456);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            var receivedAgentId = 0;
            var receivedElementId = 0;

            parameter.StartValueMonitor("source", change => { receivedAgentId = change.DataSource.AgentId; receivedElementId = change.DataSource.ElementId; }, false);

            // Act
            parameter.SetValue("new value");

            // Assert
            Assert.AreEqual(123, receivedAgentId);
            Assert.AreEqual(456, receivedElementId);
        }

        [TestMethod]
        public void DmsElementId_DefaultValues_ReturnsDefaultIdentifiers()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var dmsElementId = mock.Object.DmsElementId;

            // Assert
            Assert.AreEqual(0, dmsElementId.AgentId);
            Assert.AreEqual(0, dmsElementId.ElementId);
            Assert.AreEqual("0/0", dmsElementId.Value);
        }

        [TestMethod]
        public void DmsElementId_CustomIdentifiers_ReturnsSystemWideElementId()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 123).CreateElement(path, id: 456);

            // Act
            var dmsElementId = mock.Object.DmsElementId;

            // Assert
            Assert.AreEqual(123, dmsElementId.AgentId);
            Assert.AreEqual(456, dmsElementId.ElementId);
            Assert.AreEqual("123/456", dmsElementId.Value);
        }

        [TestMethod]
        public void Name_DefaultValue_ReturnsElement()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var name = mock.Object.Name;

            // Assert
            Assert.AreEqual("Element", name);
        }

        [TestMethod]
        public void Name_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Custom Element");

            // Act
            var name = mock.Object.Name;

            // Assert
            Assert.AreEqual("Custom Element", name);
        }

        [TestMethod]
        public void Name_SetValue_ReturnsUpdatedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Original Element");

            // Act
            mock.Object.Name = "Renamed Element";

            // Assert
            Assert.AreEqual("Renamed Element", mock.Object.Name);
        }

        [TestMethod]
        public void Name_MultipleMocks_MaintainIndependentValues()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            firstMock.Object.Name = "Renamed First Element";

            // Assert
            Assert.AreEqual("Renamed First Element", firstMock.Object.Name);
            Assert.AreEqual("Second Element", secondMock.Object.Name);
        }

        [TestMethod]
        public void Name_SetNull_ThrowsArgumentNullExceptionAndKeepsPreviousValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Original Element");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.Name = null);
            Assert.AreEqual("Original Element", mock.Object.Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(".Element")]
        [DataRow("Element.")]
        [DataRow(" Element")]
        [DataRow("Element ")]
        [DataRow("Element\\Name")]
        [DataRow("Element/Name")]
        [DataRow("Element:Name")]
        [DataRow("Element*Name")]
        [DataRow("Element?Name")]
        [DataRow("Element\"Name")]
        [DataRow("Element<Name")]
        [DataRow("Element>Name")]
        [DataRow("Element|Name")]
        [DataRow("Element°Name")]
        [DataRow("Element;Name")]
        [DataRow("Element%Name%Again")]
        public void Name_SetInvalidValue_ThrowsArgumentExceptionAndKeepsPreviousValue(string invalidName)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Original Element");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.Name = invalidName);
            Assert.AreEqual("Original Element", mock.Object.Name);
        }

        [TestMethod]
        public void GetStandaloneParameter_ExistingId_ReturnsParameterWithMatchingId()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.IsNotNull(parameter);
            Assert.AreEqual(1001, parameter.Id);
        }

        [TestMethod]
        public void Name_Set200CharacterValue_ReturnsUpdatedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var validName = new string('a', 200);

            // Act
            mock.Object.Name = validName;

            // Assert
            Assert.AreEqual(validName, mock.Object.Name);
        }

        [TestMethod]
        public void Name_SetValueLongerThan200Characters_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var invalidName = new string('a', 201);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => mock.Object.Name = invalidName);
        }

        [TestMethod]
        public void Name_SetValueWithOnePercentageCharacter_ReturnsUpdatedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            mock.Object.Name = "Element%Name";

            // Assert
            Assert.AreEqual("Element%Name", mock.Object.Name);
        }

        [TestMethod]
        public void RenameElement_NewName_UpdatesElementName()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Original Element");

            // Act
            Business_Code_Example_RenameElement.RenameElement(mock.Object,"Renamed Element");

            // Assert
            Assert.AreEqual("Renamed Element", mock.Object.Name);
        }

        [TestMethod]
        public void RenameElement_NullElement_ThrowsArgumentNullException()
        {
            // Arrange
            IDmsElement element = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Business_Code_Example_RenameElement.RenameElement(element, "New Name"));
        }

        [TestMethod]
        public void RenameElement_NameContainsRequestedText_KeepsOriginalName()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Router Test");
            var element = mock.Object;

            // Act
            Business_Code_Example_RenameElement.RenameElement(element, "Test");

            // Assert
            Assert.AreEqual("Router Test", element.Name);
        }

        [TestMethod]
        public void RenameElement_InvalidNewName_ThrowsArgumentExceptionAndKeepsOriginalName()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path, name: "Original Element");
            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Business_Code_Example_RenameElement.RenameElement(element, "Router/Invalid"));
            Assert.AreEqual("Original Element", element.Name);
        }

        [TestMethod]
        public void Description_DefaultValue_ReturnsEmptyString()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var description = mock.Object.Description;

            // Assert
            Assert.AreEqual(String.Empty, description);
        }

        [TestMethod]
        public void Description_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Description = "Element description";

            // Act
            var description = mock.Object.Description;

            // Assert
            Assert.AreEqual("Element description", description);
        }

        [TestMethod]
        public void Description_SetValue_ReturnsUpdatedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Description = "Original description";

            // Act
            mock.Object.Description = "Updated description";

            // Assert
            Assert.AreEqual("Updated description", mock.Object.Description);
        }

        [TestMethod]
        public void Description_SetEmptyString_ClearsDescription()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Description = "Existing description";

            // Act
            mock.Object.Description = String.Empty;

            // Assert
            Assert.AreEqual(String.Empty, mock.Object.Description);
        }
        [TestMethod]
        public void Description_MultipleMocks_MaintainIndependentValues()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");
            firstMock.Description = "First description";
            secondMock.Description = "Second description";

            // Act
            firstMock.Object.Description = "Updated first description";

            // Assert
            Assert.AreEqual("Updated first description", firstMock.Object.Description);
            Assert.AreEqual("Second description", secondMock.Object.Description);
        }

        [TestMethod]
        public void Type_DefaultValue_ReturnsEmptyString()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var type = mock.Object.Type;

            // Assert
            Assert.AreEqual(String.Empty, type);
        }

        [TestMethod]
        public void Type_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Type = "Modem";

            // Act
            var type = mock.Object.Type;

            // Assert
            Assert.AreEqual("Modem", type);
        }

        [TestMethod]
        public void State_DefaultValue_ReturnsActive()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var state = mock.Object.State;

            // Assert
            Assert.AreEqual(ElementState.Active, state);
        }

        [TestMethod]
        [DataRow(ElementState.Undefined)]
        [DataRow(ElementState.Active)]
        [DataRow(ElementState.Hidden)]
        [DataRow(ElementState.Paused)]
        [DataRow(ElementState.Stopped)]
        [DataRow(ElementState.Deleted)]
        [DataRow(ElementState.Error)]
        [DataRow(ElementState.Restart)]
        [DataRow(ElementState.Masked)]
        public void State_CustomValue_ReturnsProvidedValue(ElementState expectedState)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = expectedState;

            // Act
            var state = mock.Object.State;

            // Assert
            Assert.AreEqual(expectedState, state);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(5)]
        [DataRow(999)]
        public void State_InvalidValue_ThrowsArgumentOutOfRangeException(int invalidState)
        {
            // Arrange
            var state = (ElementState)invalidState;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
                mock.State = state;
            });
        }
        // state metode
        [TestMethod]
        public void Stop_ActiveElement_ChangesStateToStopped()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Stop();

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void Stop_StoppedElement_StateRemainsStopped()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Stopped;
            var element = mock.Object;

            // Act
            element.Stop();

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void Stop_CalledTwice_StateRemainsStopped()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Stop();
            element.Stop();

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void Stop_OneElement_DoesNotChangeOtherElementState()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            firstMock.Object.Stop();

            // Assert
            Assert.AreEqual(ElementState.Stopped, firstMock.Object.State);
            Assert.AreEqual(ElementState.Active, secondMock.Object.State);
        }

        [TestMethod]
        public void Start_ActiveElement_StateRemainsActive()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Start();

            // Assert
            Assert.AreEqual(ElementState.Active, element.State);
        }

        [TestMethod]
        public void Start_AfterStop_ChangesStateBackToActive()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Stop();

            // Act
            element.Start();

            // Assert
            Assert.AreEqual(ElementState.Active, element.State);
        }
        [TestMethod]
        public void Restart_ActiveElement_ChangesStateToRestart()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Restart();

            // Assert
            Assert.AreEqual(ElementState.Restart, element.State);
        }

        [TestMethod]
        public void Restart_StoppedElement_ChangesStateToRestart()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Stopped;
            var element = mock.Object;

            // Act
            element.Restart();

            // Assert
            Assert.AreEqual(ElementState.Restart, element.State);
        }

        [TestMethod]
        public void Restart_ThenStop_ChangesStateToStopped()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Restart();

            // Act
            element.Stop();

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void Pause_ActiveElement_ChangesStateToPaused()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Pause();

            // Assert
            Assert.AreEqual(ElementState.Paused, element.State);
        }

        [TestMethod]
        public void Pause_PausedElement_StateRemainsPaused()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Paused;
            var element = mock.Object;

            // Act
            element.Pause();

            // Assert
            Assert.AreEqual(ElementState.Paused, element.State);
        }

        [TestMethod]
        public void Pause_ThenStart_ChangesStateBackToActive()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Pause();

            // Act
            element.Start();

            // Assert
            Assert.AreEqual(ElementState.Active, element.State);
        }
        [TestMethod]
        public void Delete_ExistingElement_ChangesStateToDeleted()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            element.Delete();

            // Assert
            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        public void Delete_CalledTwice_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.Delete());
        }

        [TestMethod]
        public void Delete_OneElement_DoesNotDeleteOtherElement()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            firstMock.Object.Delete();

            // Assert
            Assert.AreEqual(ElementState.Deleted, firstMock.Object.State);
            Assert.AreEqual(ElementState.Active, secondMock.Object.State);
        }

        [TestMethod]
        public void Start_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.Start());
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Start_RestrictedElement_ThrowsNotSupportedException(bool isDveChild, bool isDerived)
        {
            // Arrange
            var dveSettings = new Mock<IDveSettings>();
            dveSettings.Setup(s => s.IsChild).Returns(isDveChild);

            var redundancySettings = new Mock<IRedundancySettings>();
            redundancySettings.Setup(s => s.IsDerived).Returns(isDerived);


            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Stopped;
            mock.DveSettings = dveSettings.Object;
            mock.RedundancySettings = redundancySettings.Object;
            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => element.Start());
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Stop_RestrictedElement_ThrowsNotSupportedException(bool isDveChild, bool isDerived)
        {
            // Arrange
            var dveSettings = new Mock<IDveSettings>();
            dveSettings.Setup(s => s.IsChild).Returns(isDveChild);

            var redundancySettings = new Mock<IRedundancySettings>();
            redundancySettings.Setup(s => s.IsDerived).Returns(isDerived);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Active;
            mock.DveSettings = dveSettings.Object;
            mock.RedundancySettings = redundancySettings.Object;

            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => element.Stop());
            Assert.AreEqual(ElementState.Active, element.State);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Pause_RestrictedElement_ThrowsNotSupportedException(bool isDveChild, bool isDerived)
        {
            // Arrange
            var dveSettings = new Mock<IDveSettings>();
            dveSettings.Setup(s => s.IsChild).Returns(isDveChild);

            var redundancySettings = new Mock<IRedundancySettings>();
            redundancySettings.Setup(s => s.IsDerived).Returns(isDerived);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Active;
            mock.DveSettings = dveSettings.Object;
            mock.RedundancySettings = redundancySettings.Object;

            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => element.Pause());
            Assert.AreEqual(ElementState.Active, element.State);
        }
        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Restart_RestrictedElement_ThrowsNotSupportedException(bool isDveChild, bool isDerived)
        {
            // Arrange
            var dveSettings = new Mock<IDveSettings>();
            dveSettings.Setup(s => s.IsChild).Returns(isDveChild);

            var redundancySettings = new Mock<IRedundancySettings>();
            redundancySettings.Setup(s => s.IsDerived).Returns(isDerived);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Active;
            mock.DveSettings = dveSettings.Object;
            mock.RedundancySettings = redundancySettings.Object;

            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => element.Restart());
            Assert.AreEqual(ElementState.Active, element.State);
        }

        [TestMethod]
        [DataRow(true, false)]
        [DataRow(false, true)]
        [DataRow(true, true)]
        public void Delete_RestrictedElement_ThrowsNotSupportedException(bool isDveChild, bool isDerived)
        {
            // Arrange
            var dveSettings = new Mock<IDveSettings>();
            dveSettings.Setup(s => s.IsChild).Returns(isDveChild);

            var redundancySettings = new Mock<IRedundancySettings>();
            redundancySettings.Setup(s => s.IsDerived).Returns(isDerived);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Active;
            mock.DveSettings = dveSettings.Object;
            mock.RedundancySettings = redundancySettings.Object;

            var element = mock.Object;

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => element.Delete());
            Assert.AreEqual(ElementState.Active, element.State);
        }

        [TestMethod]
        public void Pause_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.Pause());

            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        public void Restart_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.Restart());

            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        public void Stop_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.Stop());

            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        public void Protocol_DefaultValue_ReturnsNonNullInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var protocol = mock.Object.Protocol;

            // Assert
            Assert.IsNotNull(protocol);
        }

        [TestMethod]
        public void Protocol_CalledTwice_ReturnsSameInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var first = mock.Object.Protocol;
            var second = mock.Object.Protocol;

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void Protocol_ElementsUsingSameProtocol_ReturnSameCachedInstance()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var first = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var second = dmaMock.CreateElement(path, id: 2, name: "Second Element");

            // Act
            var protocol = first.Object.Protocol;

            // Assert
            Assert.AreSame(protocol, second.Object.Protocol);
            Assert.AreEqual("UnitTestingFrameworkUseCases", protocol.Name);
            Assert.AreEqual("1.0.0.1", protocol.ReferencedVersion);
            Assert.AreEqual(ProtocolType.Virtual, protocol.Type);
        }

        [TestMethod]
        public void AlarmTemplate_DefaultValue_ReturnsNull()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var alarmTemplate = mock.Object.AlarmTemplate;

            // Assert
            Assert.IsNull(alarmTemplate);
        }

        [TestMethod]
        public void AlarmTemplate_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedTemplate = new Mock<IDmsAlarmTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.AlarmTemplate = expectedTemplate;

            // Act
            var alarmTemplate = mock.Object.AlarmTemplate;

            // Assert
            Assert.AreSame(expectedTemplate, alarmTemplate);
        }

        [TestMethod]
        public void AlarmTemplate_SetValue_ReturnsUpdatedInstance()
        {
            // Arrange
            var originalTemplate = new Mock<IDmsAlarmTemplate>().Object;
            var updatedTemplate = new Mock<IDmsAlarmTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.AlarmTemplate = originalTemplate;

            // Act
            mock.Object.AlarmTemplate = updatedTemplate;

            // Assert
            Assert.AreSame(updatedTemplate, mock.Object.AlarmTemplate);
        }

        [TestMethod]
        public void AlarmTemplate_SetNull_RemovesTemplate()
        {
            // Arrange
            var alarmTemplate = new Mock<IDmsAlarmTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.AlarmTemplate = alarmTemplate;

            // Act
            mock.Object.AlarmTemplate = null;

            // Assert
            Assert.IsNull(mock.Object.AlarmTemplate);
        }

        //critical
        [TestMethod]
        public void GetActiveCriticalAlarmCount_DefaultValue_ReturnsZero()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var count = mock.Object.GetActiveCriticalAlarmCount();

            // Assert
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void GetActiveCriticalAlarmCount_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 5;
            mock.CriticalAlarmCount = 2;

            // Act
            var totalCount = mock.Object.GetActiveAlarmCount();
            var criticalCount = mock.Object.GetActiveCriticalAlarmCount();

            // Assert
            Assert.AreEqual(5, totalCount);
            Assert.AreEqual(2, criticalCount);
        }

        [TestMethod]
        [DataRow(0, -1)]
        [DataRow(2, 3)]
        public void GetActiveCriticalAlarmCount_InvalidValue_ThrowsArgumentOutOfRangeException(int activeAlarmCount, int criticalAlarmCount)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
                mock.ActiveAlarmCount = activeAlarmCount;
                mock.CriticalAlarmCount = criticalAlarmCount;
            });
        }

        [TestMethod]
        public void GetActiveCriticalAlarmCount_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 5;
            mock.CriticalAlarmCount = 2;
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.GetActiveCriticalAlarmCount());
            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        public void TrendTemplate_DefaultValue_ReturnsNull()
        {
            // Arrange
            var element = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path).Object;

            // Act
            var result = element.TrendTemplate;

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TrendTemplate_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var trendTemplate = new Mock<IDmsTrendTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.TrendTemplate = trendTemplate;

            var element = mock.Object;

            // Act
            var result = element.TrendTemplate;

            // Assert
            Assert.AreSame(trendTemplate, result);
        }

        [TestMethod]
        public void TrendTemplate_SetValue_ReturnsUpdatedInstance()
        {
            // Arrange
            var originalTemplate = new Mock<IDmsTrendTemplate>().Object;
            var updatedTemplate = new Mock<IDmsTrendTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.TrendTemplate = originalTemplate;

            var element = mock.Object;

            // Act
            element.TrendTemplate = updatedTemplate;

            // Assert
            Assert.AreSame(updatedTemplate, element.TrendTemplate);
        }

        [TestMethod]
        public void TrendTemplate_SetNull_RemovesTemplate()
        {
            // Arrange
            var trendTemplate = new Mock<IDmsTrendTemplate>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.TrendTemplate = trendTemplate;

            var element = mock.Object;

            // Act
            element.TrendTemplate = null;

            // Assert
            Assert.IsNull(element.TrendTemplate);
        }

        [TestMethod]
        public void GetActiveAlarmCount_DefaultValue_ReturnsZero()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var count = mock.Object.GetActiveAlarmCount();

            // Assert
            Assert.AreEqual(0, count);
        }

        [TestMethod]
        public void GetActiveAlarmCount_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 3;

            // Act
            var count = mock.Object.GetActiveAlarmCount();

            // Assert
            Assert.AreEqual(3, count);
        }

        [TestMethod]
        public void GetActiveAlarmCount_NegativeValue_ThrowsArgumentOutOfRangeException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
                mock.ActiveAlarmCount = -1;
            });
        }
        [TestMethod]
        public void GetActiveAlarmCount_AfterStop_ReturnsConfiguredValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 3;
            var element = mock.Object;
            element.Stop();

            // Act
            var activeAlarmCount = element.GetActiveAlarmCount();

            // Assert
            Assert.AreEqual(3, activeAlarmCount);
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void GetActiveAlarmCount_AfterDelete_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 3;
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => element.GetActiveAlarmCount());
        }
        [TestMethod]
        public void GetStandaloneParameter_CalledTwice_ReturnsSameCachedInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var first = mock.Object.GetStandaloneParameter<string>(1001);
            var second = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.AreSame(first, second);
        }
        //stop
        [TestMethod]
        public void StopElement_ActiveElementWithAlarms_StopsElement()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 2;
            var element = mock.Object;

            // Act
            Business_Code_Example_StopElement.StopElement(element);

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }
        [TestMethod]
        public void StopElement_ActiveElementWithoutAlarms_RemainsActive()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var element = mock.Object;

            // Act
            Business_Code_Example_StopElement.StopElement(element);

            // Assert
            Assert.AreEqual(ElementState.Active, element.State);
        }
        [TestMethod]
        public void StopElement_NullElement_ThrowsArgumentNullException()
        {
            // Arrange
            IDmsElement element = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Business_Code_Example_StopElement.StopElement(element));
        }
        [TestMethod]
        public void StopElement_PausedElementWithAlarms_RemainsPaused()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Paused;
            mock.ActiveAlarmCount = 2;
            var element = mock.Object;

            // Act
            Business_Code_Example_StopElement.StopElement(element);

            // Assert
            Assert.AreEqual(ElementState.Paused, element.State);
        }
        [TestMethod]
        public void StopElement_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ActiveAlarmCount = 2;
            var element = mock.Object;
            element.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => Business_Code_Example_StopElement.StopElement(element));
            Assert.AreEqual(ElementState.Deleted, element.State);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(2)]
        public void StopElement_StoppedElement_RemainsStopped(int activeAlarmCount)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.State = ElementState.Stopped;
            mock.ActiveAlarmCount = activeAlarmCount;
            var element = mock.Object;

            // Act
            Business_Code_Example_StopElement.StopElement(element);

            // Assert
            Assert.AreEqual(ElementState.Stopped, element.State);
        }

        [TestMethod]
        public void IsStartupComplete_DefaultValue_ReturnsTrue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var result = mock.Object.IsStartupComplete();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsStartupComplete_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.IsStartupComplete = false;

            // Act
            var result = mock.Object.IsStartupComplete();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetAlarmLevel_DefaultValue_ReturnsUndefined()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var alarmLevel = mock.Object.GetAlarmLevel();

            // Assert
            Assert.AreEqual(AlarmLevel.Undefined, alarmLevel);
        }

        [TestMethod]
        [DataRow(AlarmLevel.Normal)]
        [DataRow(AlarmLevel.Warning)]
        [DataRow(AlarmLevel.Minor)]
        [DataRow(AlarmLevel.Major)]
        [DataRow(AlarmLevel.Critical)]
        [DataRow(AlarmLevel.Information)]
        [DataRow(AlarmLevel.Timeout)]
        [DataRow(AlarmLevel.Initial)]
        [DataRow(AlarmLevel.Masked)]
        [DataRow(AlarmLevel.Error)]
        [DataRow(AlarmLevel.Notice)]
        [DataRow(AlarmLevel.Suggestion)]
        public void GetAlarmLevel_CustomValue_ReturnsProvidedValue(AlarmLevel expectedAlarmLevel)
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.AlarmLevel = expectedAlarmLevel;

            // Act
            var alarmLevel = mock.Object.GetAlarmLevel();

            // Assert
            Assert.AreEqual(expectedAlarmLevel, alarmLevel);
        }

        [TestMethod]
        public void GetAlarmLevel_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var invalidAlarmLevel = (AlarmLevel)999;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
            {
                var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
                mock.AlarmLevel = invalidAlarmLevel;
            });
        }

        [TestMethod]
        public void Connections_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var connections = mock.Object.Connections;
            var enumeratedCount = 0;

            foreach (var connection in connections)
            {
                enumeratedCount++;
            }

            // Assert
            Assert.IsNotNull(connections);
            Assert.AreEqual(0, connections.Length);
            Assert.AreEqual(0, enumeratedCount);
            Assert.IsFalse(connections.IsUpdateRequired());
        }

        [TestMethod]
        public void Connections_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedConnections = new Mock<IElementConnectionCollection>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Connections = expectedConnections;

            // Act
            var connections = mock.Object.Connections;

            // Assert
            Assert.AreSame(expectedConnections, connections);
        }

        [TestMethod]
        public void Connections_SetValue_ReturnsUpdatedInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            var originalConnections = mock.Object.Connections;
            var updatedConnections = new Mock<IElementConnectionCollection>().Object;

            // Act
            mock.Object.Connections = updatedConnections;

            // Assert
            Assert.AreSame(updatedConnections, mock.Object.Connections);
            Assert.AreNotSame(originalConnections, mock.Object.Connections);
        }

        [TestMethod]
        public void Connections_MultipleElements_MaintainIndependentValues()
        {
            // Arrange
            var dmaMock = new IDmsMock().CreateAgent(agentId: 1);
            var firstMock = dmaMock.CreateElement(path, id: 1, name: "First Element");
            var secondMock = dmaMock.CreateElement(path, id: 2, name: "Second Element");
            var updatedConnections = new Mock<IElementConnectionCollection>().Object;

            // Act
            firstMock.Object.Connections = updatedConnections;

            // Assert
            Assert.AreSame(updatedConnections, firstMock.Object.Connections);
            Assert.AreNotSame(updatedConnections, secondMock.Object.Connections);
            Assert.IsNotNull(secondMock.Object.Connections);
        }

        [TestMethod]
        public void FunctionSettings_DefaultValue_ReturnsNonFunctionElementSettings()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var functionSettings = mock.Object.FunctionSettings;

            // Assert
            Assert.IsNotNull(functionSettings);
            Assert.IsFalse(functionSettings.IsFunctionElement);
            Assert.AreEqual(Guid.Empty, functionSettings.FunctionId);
        }

        [TestMethod]
        public void FunctionSettings_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedFunctionId = Guid.NewGuid();
            var functionSettingsMock = new Mock<IFunctionSettings>();
            functionSettingsMock.Setup(settings => settings.IsFunctionElement).Returns(true);
            functionSettingsMock.Setup(settings => settings.FunctionId).Returns(expectedFunctionId);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.FunctionSettings = functionSettingsMock.Object;

            // Act
            var functionSettings = mock.Object.FunctionSettings;

            // Assert
            Assert.AreSame(functionSettingsMock.Object, functionSettings);
            Assert.IsTrue(functionSettings.IsFunctionElement);
            Assert.AreEqual(expectedFunctionId, functionSettings.FunctionId);
        }

        [TestMethod]
        public void Properties_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var properties = mock.Object.Properties;
            var enumeratedCount = 0;

            foreach (var property in properties)
            {
                enumeratedCount++;
            }

            // Assert
            Assert.IsNotNull(properties);
            Assert.AreEqual(0, properties.Count);
            Assert.AreEqual(0, enumeratedCount);
        }

        [TestMethod]
        public void Properties_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedProperties = new Mock<IPropertyCollection<IDmsElementProperty, IDmsElementPropertyDefinition>>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.Properties = expectedProperties;

            // Act
            var properties = mock.Object.Properties;

            // Assert
            Assert.AreSame(expectedProperties, properties);
        }

        [TestMethod]
        public void ReplicationSettings_DefaultValue_ReturnsNonReplicatedSettings()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var replicationSettings = mock.Object.ReplicationSettings;

            // Assert
            Assert.IsNotNull(replicationSettings);
            Assert.IsFalse(replicationSettings.IsReplicated);
        }

        [TestMethod]
        public void ReplicationSettings_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var replicationSettingsMock = new Mock<IReplicationSettings>();
            replicationSettingsMock.Setup(settings => settings.IsReplicated).Returns(true);

            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.ReplicationSettings = replicationSettingsMock.Object;

            // Act
            var replicationSettings = mock.Object.ReplicationSettings;

            // Assert
            Assert.AreSame(replicationSettingsMock.Object, replicationSettings);
            Assert.IsTrue(replicationSettings.IsReplicated);
        }

        [TestMethod]
        public void SpectrumAnalyzer_DefaultValue_ReturnsNull()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var spectrumAnalyzer = mock.Object.SpectrumAnalyzer;

            // Assert
            Assert.IsNull(spectrumAnalyzer);
        }

        [TestMethod]
        public void SpectrumAnalyzer_CustomValue_ReturnsProvidedInstance()
        {
            // Arrange
            var expectedSpectrumAnalyzer = new Mock<IDmsSpectrumAnalyzer>().Object;
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);
            mock.SpectrumAnalyzer = expectedSpectrumAnalyzer;

            // Act
            var spectrumAnalyzer = mock.Object.SpectrumAnalyzer;

            // Assert
            Assert.AreSame(expectedSpectrumAnalyzer, spectrumAnalyzer);
        }

        [TestMethod]
        public void GetStandaloneParameter_StringType_IsSupported()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.IsNotNull(parameter);
        }
        //example standalone parameter
        [TestMethod]
        public void SetEmptyStandaloneStringParameter_EmptyValue_SetsNewValue()
        {
            // Arrange
            var element = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path).Object;
            var parameter = element.GetStandaloneParameter<string>(1001);
            parameter.SetValue(String.Empty);

            // Act
            Business_Code_Example_SetEmptyStandaloneStringParameter.SetEmptyStandaloneStringParameter(element, 1001);

            // Assert
            Assert.AreEqual("new value", parameter.GetValue());
        }

        [TestMethod]
        public void SetEmptyStandaloneStringParameter_NullValue_SetsNewValue()
        {
            // Arrange
            var element = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path).Object;
            var parameter = element.GetStandaloneParameter<string>(1001);
            parameter.SetValue(null);

            // Act
            Business_Code_Example_SetEmptyStandaloneStringParameter.SetEmptyStandaloneStringParameter(element, 1001);

            // Assert
            Assert.AreEqual("new value", parameter.GetValue());
        }

        [TestMethod]
        public void SetEmptyStandaloneStringParameter_NullElement_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Business_Code_Example_SetEmptyStandaloneStringParameter.SetEmptyStandaloneStringParameter(null, 1001));
        }
        [TestMethod]
        public void SetEmptyStandaloneStringParameter_ExistingValue_KeepsExistingValue()
        {
            // Arrange
            var element = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path).Object;
            var parameter = element.GetStandaloneParameter<string>(1001);
            parameter.SetValue("existing value");

            // Act
            Business_Code_Example_SetEmptyStandaloneStringParameter.SetEmptyStandaloneStringParameter(element, 1001);

            // Assert
            Assert.AreEqual("existing value", parameter.GetValue());
        }

        [TestMethod]
        public void GetStandaloneParameter_NullableIntType_IsSupported()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<int?>(800);

            // Assert
            Assert.IsNotNull(parameter);
        }


        [TestMethod]
        public void GetStandaloneParameter_NullableDoubleType_IsSupported()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<double?>(1000);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_NullableDateTimeType_IsSupported()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<DateTime?>(1000);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedIntType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<int>(800));
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedDoubleType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<double>(1000));
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedObjectType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<object>(1001));
        }

        [TestMethod]
        public void GetStandaloneParameter_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetStandaloneParameter<string>(123456));
        }

        [TestMethod]
        public void GetTable_ExistingId_ReturnsTableWithMatchingId()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(900, table.Id);
        }

        [TestMethod]
        public void GetTable_CalledTwice_ReturnsSameCachedInstance()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act
            var first = mock.Object.GetTable(900);
            var second = mock.Object.GetTable(900);

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void GetTable_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 0).CreateElement(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetTable(123456));
        }

        [TestMethod]
        public void ActiveAlarmCounts_CustomValues_ReturnProvidedValues()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            mock.ActiveAlarmCount = 9;
            mock.MajorAlarmCount = 3;
            mock.MinorAlarmCount = 2;
            mock.WarningAlarmCount = 1;

            // Act
            var majorAlarmCount = mock.Object.GetActiveMajorAlarmCount();
            var minorAlarmCount = mock.Object.GetActiveMinorAlarmCount();
            var warningAlarmCount = mock.Object.GetActiveWarningAlarmCount();

            // Assert
            Assert.AreEqual(3, majorAlarmCount);
            Assert.AreEqual(2, minorAlarmCount);
            Assert.AreEqual(1, warningAlarmCount);
        }

        [TestMethod]
        public void GetActiveMajorAlarmCount_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            mock.Object.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => mock.Object.GetActiveMajorAlarmCount());
        }

        [TestMethod]
        public void Duplicate_ValidTargetAgent_CreatesElementOnTargetAgent()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var sourceAgent = dmsMock.CreateAgent(agentId: 1, name: "Source Agent");
            var targetAgent = dmsMock.CreateAgent(agentId: 2, name: "Target Agent");
            var sourceElement = sourceAgent.CreateElement(path, id: 10, name: "Original Element");
            sourceElement.Description = "Description";

            // Act
            var duplicate = sourceElement.Object.Duplicate("Duplicated Element", targetAgent.Object);

            // Assert
            Assert.AreSame(duplicate, targetAgent.Object.GetElement("Duplicated Element"));
            Assert.AreSame(targetAgent.Object, duplicate.Host);
            Assert.AreEqual(2, duplicate.AgentId);
            Assert.AreEqual("Description", duplicate.Description);
        }

        [TestMethod]
        public void Exists_ElementIsDeleted_ReturnsFalse()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);

            // Act & Assert
            Assert.IsTrue(mock.Object.Exists());

            mock.Object.Delete();

            Assert.IsFalse(mock.Object.Exists());
        }

        [TestMethod]
        public void Update_ExistingElement_DoesNotThrowException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);

            // Act
            mock.Object.Update();

            // Assert
            Assert.IsTrue(mock.Object.Exists());
        }

        [TestMethod]
        public void Update_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            mock.Object.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => mock.Object.Update());
        }

        [TestMethod]
        public void Duplicate_AgentFromDifferentDms_ThrowsAgentNotFoundException()
        {
            // Arrange
            var sourceElementMock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            var foreignAgentMock = new IDmsMock().CreateAgent(agentId: 2);

            // Act & Assert
            Assert.ThrowsExactly<AgentNotFoundException>(() => sourceElementMock.Object.Duplicate("Duplicated Element", foreignAgentMock.Object));
        }

        [TestMethod]
        public void AddView_DeletedElement_ThrowsElementNotFoundException()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);
            elementMock.Object.Delete();

            // Act & Assert
            Assert.ThrowsExactly<ElementNotFoundException>(() => elementMock.AddView(viewMock.Object.Id));
        }

        [TestMethod]
        public void StartNameMonitor_NameChanges_InvokesCallback()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            ElementNameChange receivedChange = null;
            mock.Object.StartNameMonitor("test", change => receivedChange = change);

            // Act
            mock.Object.Name = "Updated Element";

            // Assert
            Assert.IsNotNull(receivedChange);
            Assert.AreEqual("Updated Element", receivedChange.Name);
            Assert.AreEqual(1, receivedChange.DataSource.AgentId);
            Assert.AreEqual(2, receivedChange.DataSource.ElementId);
        }

        [TestMethod]
        public void StopNameMonitor_NameChanges_DoesNotInvokeCallback()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            var callbackCount = 0;
            mock.Object.StartNameMonitor("test", change => callbackCount++);
            mock.Object.StopNameMonitor("test", force: false);

            // Act
            mock.Object.Name = "Updated Element";

            // Assert
            Assert.AreEqual(0, callbackCount);
        }

        [TestMethod]
        public void StartStateMonitor_StateChanges_InvokesCallback()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            ElementStateChange receivedChange = null;
            mock.Object.StartStateMonitor("test", change => receivedChange = change, TimeSpan.FromSeconds(1));

            // Act
            mock.Object.Stop();

            // Assert
            Assert.IsNotNull(receivedChange);
            Assert.AreEqual(ElementState.Stopped, receivedChange.State);
        }

        [TestMethod]
        public void StartAlarmLevelMonitor_AlarmLevelChanges_InvokesCallback()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);
            ElementAlarmlevelChange receivedChange = null;
            mock.Object.StartAlarmLevelMonitor("test", change => receivedChange = change);

            // Act
            mock.AlarmLevel = AlarmLevel.Critical;

            // Assert
            Assert.IsNotNull(receivedChange);
            Assert.AreEqual(AlarmLevel.Critical, receivedChange.AlarmLevel);
        }

        [TestMethod]
        public void StartNameMonitor_NullArguments_ThrowArgumentNullException()
        {
            // Arrange
            var mock = new IDmsMock().CreateAgent(agentId: 1).CreateElement(path, id: 2);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.StartNameMonitor(null, change => { }));
            Assert.ThrowsExactly<ArgumentNullException>(() => mock.Object.StartNameMonitor("test", null));
        }
    }
}
