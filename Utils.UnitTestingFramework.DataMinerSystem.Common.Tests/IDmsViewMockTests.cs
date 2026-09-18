namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsViewMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Name_SetValue_ReturnsUpdatedValueAndUpdatesDmsLookup()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10, name: "Original View");

            // Act
            viewMock.Object.Name = "Renamed View";

            // Assert
            Assert.AreEqual("Renamed View", viewMock.Object.Name);
            Assert.IsFalse(dmsMock.Object.ViewExists("Original View"));
            Assert.AreSame(viewMock.Object, dmsMock.Object.GetView("Renamed View"));
        }

        [TestMethod]
        public void Name_SetNull_ThrowsArgumentNullExceptionAndKeepsPreviousValue()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10, name: "Original View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => viewMock.Object.Name = null);
            Assert.AreEqual("Original View", viewMock.Object.Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow(".View")]
        [DataRow("View.")]
        [DataRow(" View")]
        [DataRow("View ")]
        [DataRow("View|Name")]
        [DataRow("View%Name%Again")]
        public void Name_SetInvalidValue_ThrowsArgumentExceptionAndKeepsPreviousValue(string invalidName)
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10, name: "Original View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => viewMock.Object.Name = invalidName);
            Assert.AreEqual("Original View", viewMock.Object.Name);
        }

        [TestMethod]
        public void Name_SetTwoHundredCharacterValue_ReturnsUpdatedValue()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);
            var validName = new string('a', 200);

            // Act
            viewMock.Object.Name = validName;

            // Assert
            Assert.AreEqual(validName, viewMock.Object.Name);
        }

        [TestMethod]
        public void Name_SetValueLongerThanTwoHundredCharacters_ThrowsArgumentException()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => viewMock.Object.Name = new string('a', 201));
        }

        [TestMethod]
        public void Name_SetValueWithOnePercentageCharacter_ReturnsUpdatedValue()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            viewMock.Object.Name = "View%Name";

            // Assert
            Assert.AreEqual("View%Name", viewMock.Object.Name);
        }

        [TestMethod]
        public void CreateView_DuplicateName_ThrowsArgumentExceptionAndKeepsExistingView()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var existingViewMock = dmsMock.CreateView(viewId: 10, name: "Existing View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmsMock.CreateView(viewId: 11, name: "Existing View"));
            Assert.AreSame(existingViewMock.Object, dmsMock.Object.GetView("Existing View"));
            Assert.AreEqual(1, dmsMock.Object.GetViews().Count);
        }

        [TestMethod]
        public void CreateView_DuplicateNameWithDifferentCasing_ThrowsArgumentExceptionAndKeepsExistingView()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var existingViewMock = dmsMock.CreateView(viewId: 10, name: "Existing View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmsMock.CreateView(viewId: 11, name: "existing view"));
            Assert.AreSame(existingViewMock.Object, dmsMock.Object.GetView("EXISTING VIEW"));
            Assert.AreEqual(1, dmsMock.Object.GetViews().Count);
        }

        [TestMethod]
        public void Name_SetToExistingName_ThrowsArgumentExceptionAndKeepsPreviousValue()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            dmsMock.CreateView(viewId: 10, name: "Existing View");
            var viewMock = dmsMock.CreateView(viewId: 11, name: "Other View");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => viewMock.Object.Name = "Existing View");
            Assert.AreEqual("Other View", viewMock.Object.Name);
        }

        [TestMethod]
        public void CreateView_InvalidName_ThrowsArgumentExceptionAndDoesNotAddView()
        {
            // Arrange
            var dmsMock = new IDmsMock();

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => dmsMock.CreateView(viewId: 10, name: "Invalid|View"));
            Assert.IsFalse(dmsMock.Object.ViewExists(10));
            Assert.IsEmpty(dmsMock.Object.GetViews());
        }

        [TestMethod]
        public void Delete_ExistingView_RemovesViewFromDms()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10, name: "Deleted View");

            // Act
            viewMock.Object.Delete();

            // Assert
            Assert.IsFalse(dmsMock.Object.ViewExists(10));
            Assert.IsFalse(dmsMock.Object.ViewExists("Deleted View"));
            Assert.IsEmpty(dmsMock.Object.GetViews());
            Assert.ThrowsExactly<ViewNotFoundException>(() => dmsMock.Object.GetView(10));
            Assert.ThrowsExactly<ViewNotFoundException>(() => dmsMock.Object.GetView("Deleted View"));
        }

        [TestMethod]
        public void Delete_DeletedView_DoesNotThrowException()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            viewMock.Object.Delete();

            // Act
            viewMock.Object.Delete();

            // Assert
            Assert.IsFalse(dmsMock.Object.ViewExists(10));
        }

        [TestMethod]
        public void Exists_ViewIsDeleted_ReturnsFalse()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.IsTrue(viewMock.Object.Exists());

            viewMock.Object.Delete();

            Assert.IsFalse(viewMock.Object.Exists());
        }

        [TestMethod]
        public void Update_ExistingView_DoesNotThrowException()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            viewMock.Object.Update();

            // Assert
            Assert.IsTrue(viewMock.Object.Exists());
        }

        [TestMethod]
        public void Display_CustomValue_ReturnsProvidedValue()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);
            viewMock.Display = "Custom Display";

            // Act
            var display = viewMock.Object.Display;

            // Assert
            Assert.AreEqual("Custom Display", display);
        }

        [TestMethod]
        public void Display_DefaultValue_ReturnsEmptyString()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            var display = viewMock.Object.Display;

            // Assert
            Assert.AreEqual(String.Empty, display);
        }

        [TestMethod]
        public void GetAlarmLevel_DefaultValue_ReturnsUndefined()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            var alarmLevel = viewMock.Object.GetAlarmLevel();

            // Assert
            Assert.AreEqual(AlarmLevel.Undefined, alarmLevel);
        }

        [TestMethod]
        [DataRow(AlarmLevel.Normal)]
        [DataRow(AlarmLevel.Warning)]
        [DataRow(AlarmLevel.Minor)]
        [DataRow(AlarmLevel.Major)]
        [DataRow(AlarmLevel.Critical)]
        [DataRow(AlarmLevel.Timeout)]
        [DataRow(AlarmLevel.Information)]
        [DataRow(AlarmLevel.Initial)]
        [DataRow(AlarmLevel.Masked)]
        [DataRow(AlarmLevel.Error)]
        [DataRow(AlarmLevel.Notice)]
        [DataRow(AlarmLevel.Suggestion)]
        public void GetAlarmLevel_CustomValue_ReturnsProvidedValue(AlarmLevel expectedAlarmLevel)
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);
            viewMock.AlarmLevel = expectedAlarmLevel;

            // Act
            var alarmLevel = viewMock.Object.GetAlarmLevel();

            // Assert
            Assert.AreEqual(expectedAlarmLevel, alarmLevel);
        }

        [TestMethod]
        public void AlarmLevel_InvalidValue_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => viewMock.AlarmLevel = (AlarmLevel)999);
            Assert.AreEqual(AlarmLevel.Undefined, viewMock.Object.GetAlarmLevel());
        }

        [TestMethod]
        public void Elements_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            var elements = viewMock.Object.Elements;

            // Assert
            Assert.IsEmpty(elements);
        }

        [TestMethod]
        public void Elements_ElementContainsView_ReturnsElement()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);
            elementMock.AddView(viewMock.Object.Id);

            // Act
            var elements = viewMock.Object.Elements;

            // Assert
            Assert.HasCount(1, elements);
            Assert.AreSame(elementMock.Object, elements.Single());
            Assert.IsTrue(elementMock.Object.Views.Contains(viewMock.Object));
        }

        [TestMethod]
        public void Elements_ElementBelongsToMultipleViews_ReturnsElementForEachView()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstViewMock = dmsMock.CreateView(viewId: 10, name: "First View");
            var secondViewMock = dmsMock.CreateView(viewId: 11, name: "Second View");
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);
            elementMock.AddView(firstViewMock.Object.Id);
            elementMock.AddView(secondViewMock.Object.Id);

            // Act & Assert
            Assert.AreSame(elementMock.Object, firstViewMock.Object.Elements.Single());
            Assert.AreSame(elementMock.Object, secondViewMock.Object.Elements.Single());
        }

        [TestMethod]
        public void Delete_ViewContainingElement_RemovesViewFromElement()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);
            elementMock.AddView(viewMock.Object.Id);

            // Act
            viewMock.Object.Delete();

            // Assert
            Assert.IsFalse(elementMock.Object.Views.Contains(viewMock.Object));
        }

        [TestMethod]
        public void Elements_ReturnedCollection_IsReadOnly()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);

            // Act
            var elements = viewMock.Object.Elements;

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(() => elements.Add(elementMock.Object));
        }

        [TestMethod]
        public void Elements_AssignedElementIsDeleted_RemovesElementFromView()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var elementMock = dmsMock.CreateAgent(agentId: 1).CreateElement(path, id: 2);
            elementMock.AddView(viewMock.Object.Id);

            // Act
            elementMock.Object.Delete();

            // Assert
            Assert.IsEmpty(viewMock.Object.Elements);
        }

        [TestMethod]
        public void Services_DefaultValue_ReturnsEmptyCollection()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            var services = viewMock.Object.Services;

            // Assert
            Assert.IsEmpty(services);
        }

        [TestMethod]
        public void Services_ServiceContainsView_ReturnsService()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var serviceMock = dmsMock.CreateAgent(agentId: 1).CreateService(serviceId: 2);
            serviceMock.AddView(viewMock.Object.Id);

            // Act
            var services = viewMock.Object.Services;

            // Assert
            Assert.AreSame(serviceMock.Object, services.Single());
        }

        [TestMethod]
        public void Services_AssignedServiceIsDeleted_RemovesServiceFromView()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var viewMock = dmsMock.CreateView(viewId: 10);
            var serviceMock = dmsMock.CreateAgent(agentId: 1).CreateService(serviceId: 2);
            serviceMock.AddView(viewMock.Object.Id);

            // Act
            serviceMock.Object.Delete();

            // Assert
            Assert.IsEmpty(viewMock.Object.Services);
        }

        [TestMethod]
        public void Properties_DefaultValue_ReturnsStableEmptyCollection()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act
            var firstProperties = viewMock.Object.Properties;
            var secondProperties = viewMock.Object.Properties;

            // Assert
            Assert.AreEqual(0, firstProperties.Count);
            Assert.IsEmpty(firstProperties.Cast<IDmsViewProperty>());
            Assert.AreSame(firstProperties, secondProperties);
        }

        [TestMethod]
        public void Properties_CustomValue_ReturnsProvidedCollection()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);
            var expectedProperties = new Mock<IPropertyCollection<IDmsViewProperty, IDmsViewPropertyDefinition>>().Object;
            viewMock.Properties = expectedProperties;

            // Act
            var properties = viewMock.Object.Properties;

            // Assert
            Assert.AreSame(expectedProperties, properties);
        }

        [TestMethod]
        public void Parent_DefaultValue_ReturnsNullAndEmptyChildViews()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.IsNull(viewMock.Object.Parent);
            Assert.IsEmpty(viewMock.Object.ChildViews);
        }

        [TestMethod]
        public void Parent_SetToView_UpdatesParentAndChildViews()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");
            var childMock = dmsMock.CreateView(viewId: 11, name: "Child");

            // Act
            childMock.Object.Parent = parentMock.Object;

            // Assert
            Assert.AreSame(parentMock.Object, childMock.Object.Parent);
            Assert.AreSame(childMock.Object, parentMock.Object.ChildViews.Single());
            Assert.IsEmpty(childMock.Object.ChildViews);
        }

        [TestMethod]
        public void Parent_Changed_UpdatesBothParentsChildViews()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var firstParentMock = dmsMock.CreateView(viewId: 10, name: "First Parent");
            var secondParentMock = dmsMock.CreateView(viewId: 11, name: "Second Parent");
            var childMock = dmsMock.CreateView(viewId: 12, name: "Child");
            childMock.Object.Parent = firstParentMock.Object;

            // Act
            childMock.Object.Parent = secondParentMock.Object;

            // Assert
            Assert.IsEmpty(firstParentMock.Object.ChildViews);
            Assert.AreSame(childMock.Object, secondParentMock.Object.ChildViews.Single());
        }

        [TestMethod]
        public void ChildViews_MultipleLevels_ReturnsOnlyImmediateChildren()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");
            var childMock = dmsMock.CreateView(viewId: 11, name: "Child");
            var grandchildMock = dmsMock.CreateView(viewId: 12, name: "Grandchild");
            childMock.Parent = parentMock.Object;
            grandchildMock.Parent = childMock.Object;

            // Act
            var childViews = parentMock.Object.ChildViews;

            // Assert
            Assert.AreEqual(1, childViews.Count);
            Assert.AreSame(childMock.Object, childViews.Single());
        }

        [TestMethod]
        public void ChildViews_ReturnedCollection_IsReadOnly()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");
            var childMock = dmsMock.CreateView(viewId: 11, name: "Child");
            childMock.Parent = parentMock.Object;

            // Act
            var childViews = parentMock.Object.ChildViews;

            // Assert
            Assert.ThrowsExactly<NotSupportedException>(() => childViews.Add(childMock.Object));
        }

        [TestMethod]
        public void Delete_ChildView_RemovesViewFromParentChildViews()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");
            var childMock = dmsMock.CreateView(viewId: 11, name: "Child");
            childMock.Parent = parentMock.Object;

            // Act
            childMock.Object.Delete();

            // Assert
            Assert.IsEmpty(parentMock.Object.ChildViews);
        }

        [TestMethod]
        public void Parent_SetNull_ThrowsArgumentNullExceptionAndKeepsPreviousParent()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");
            var childMock = dmsMock.CreateView(viewId: 11, name: "Child");
            childMock.Parent = parentMock.Object;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => childMock.Object.Parent = null);
            Assert.AreSame(parentMock.Object, childMock.Object.Parent);
        }

        [TestMethod]
        public void Parent_SetToSelf_ThrowsNotSupportedException()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => viewMock.Object.Parent = viewMock.Object);
        }

        [TestMethod]
        public void Parent_SetToDifferentReferenceWithSameId_ThrowsNotSupportedException()
        {
            // Arrange
            var viewMock = new IDmsMock().CreateView(viewId: 10);
            var sameIdViewMock = new IDmsMock().CreateView(viewId: 10);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => viewMock.Object.Parent = sameIdViewMock.Object);
        }

        [TestMethod]
        public void Parent_SetOnRootView_ThrowsNotSupportedException()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var rootMock = dmsMock.CreateView(viewId: -1, name: "Root View");
            var parentMock = dmsMock.CreateView(viewId: 10, name: "Parent");

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(() => rootMock.Object.Parent = parentMock.Object);
        }
    }
}
