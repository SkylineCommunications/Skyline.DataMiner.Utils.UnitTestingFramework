namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Utils.DOM.Builders;

    [TestClass]
    public class DmsBuilderDomTests
    {
        [TestMethod]
        public void Build_KeepsAllDefinitions_WithMultipleDefinitionsForSameModule()
        {
            // Arrange
            var firstDefinitionId = Guid.NewGuid();
            var secondDefinitionId = Guid.NewGuid();

            // Act
            var dmsMock = new IDmsBuilder()
                .WithDomDefinition(moduleId: "module", createDefinition: () => CreateDefinition(firstDefinitionId, "First"))
                .WithDomDefinition(moduleId: "module", createDefinition: () => CreateDefinition(secondDefinitionId, "Second"))
                .Build();
            var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "module");

            // Assert
            Assert.AreEqual(firstDefinitionId, helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(firstDefinitionId)).Single().ID.Id);
            Assert.AreEqual(secondDefinitionId, helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(secondDefinitionId)).Single().ID.Id);
        }

        [TestMethod]
        public void Build_KeepsModuleDataSeparated_WithTwoModules()
        {
            // Arrange
            var firstDefinitionId = Guid.NewGuid();
            var secondDefinitionId = Guid.NewGuid();

            // Act
            var dmsMock = new IDmsBuilder()
                .WithDomDefinition(moduleId: "first-module", createDefinition: () => CreateDefinition(firstDefinitionId, "First definition"))
                .WithDomDefinition(moduleId: "second-module", createDefinition: () => CreateDefinition(secondDefinitionId, "Second definition"))
                .Build();
            var firstHelper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "first-module");
            var secondHelper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "second-module");

            // Assert
            Assert.AreEqual(firstDefinitionId, firstHelper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(firstDefinitionId)).Single().ID.Id);
            Assert.AreEqual(secondDefinitionId, secondHelper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(secondDefinitionId)).Single().ID.Id);
            Assert.IsFalse(firstHelper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(secondDefinitionId)).Any());
            Assert.IsFalse(secondHelper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(firstDefinitionId)).Any());
        }

        [TestMethod]
        public void Build_MakesAllDomObjectsAvailable_WithPackageBuilders()
        {
            // Arrange
            var definition = CreateDefinition(Guid.NewGuid(), "Definition");
            var sectionDefinitionId = Guid.NewGuid();
            var behaviorDefinitionId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();

            // Act
            var dmsMock = new IDmsBuilder()
                .WithDomDefinition(moduleId: "complete-module", createDefinition: () => definition)
                .WithSectionDefinition(moduleId: "complete-module", createDefinition: () => new SectionDefinitionBuilder()
                    .WithID(sectionDefinitionId)
                    .WithName("Section definition")
                    .Build())
                .WithDomBehaviorDefinition(moduleId: "complete-module", createDefinition: () => new DomBehaviorDefinitionBuilder()
                    .WithID(behaviorDefinitionId)
                    .WithName("Behavior definition")
                    .Build())
                .WithDomInstance(moduleId: "complete-module", createInstance: () => new DomInstanceBuilder(definition)
                    .WithID(instanceId)
                    .Build())
                .Build();
            var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "complete-module");

            // Assert
            Assert.AreEqual(definition.ID.Id, helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(definition.ID.Id)).Single().ID.Id);
            Assert.AreEqual(instanceId, helper.DomInstances.Read(DomInstanceExposers.Id.Equal(instanceId)).Single().ID.Id);
            Assert.IsNotNull(helper.SectionDefinitions.Read(SectionDefinitionExposers.ID.Equal(sectionDefinitionId)).Single());
            Assert.AreEqual(behaviorDefinitionId, helper.DomBehaviorDefinitions.Read(DomBehaviorDefinitionExposers.Id.Equal(behaviorDefinitionId)).Single().ID.Id);
        }

        [TestMethod]
        public void Build_MakesDefinitionAvailableThroughDmsConnection_WithDomDefinitionBuilderResult()
        {
            // Arrange
            var definitionId = Guid.NewGuid();

            // Act
            var dmsMock = new IDmsBuilder()
                .WithDomDefinition(moduleId: "dms-module", createDefinition: () => new DomDefinitionBuilder()
                    .WithID(definitionId)
                    .WithName("DMS definition")
                    .Build())
                .Build();
            var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "dms-module");

            // Assert
            var definition = helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(definitionId)).Single();
            Assert.AreEqual("DMS definition", definition.Name);
        }

        [TestMethod]
        public void Build_ThrowsInvalidOperationException_WhenDomFactoryReturnsNull()
        {
            // Arrange
            var builder = new IDmsBuilder()
                .WithDomDefinition(moduleId: "module", createDefinition: () => null);

            // Act and assert
            Assert.ThrowsExactly<InvalidOperationException>(() => builder.Build());
        }

        [TestMethod]
        public void WithDomBehaviorDefinition_ThrowsArgumentNullException_WithNullFactory()
        {
            // Arrange
            var builder = new IDmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.WithDomBehaviorDefinition(moduleId: "module", createDefinition: null));
        }

        [TestMethod]
        public void WithDomDefinition_InvokesFactory_OnBuild()
        {
            // Arrange
            var factoryInvoked = false;
            var builder = new IDmsBuilder()
                .WithDomDefinition(moduleId: "module", createDefinition: () =>
                {
                    factoryInvoked = true;
                    return CreateDefinition(Guid.NewGuid(), "Definition");
                });

            Assert.IsFalse(factoryInvoked);

            // Act
            builder.Build();

            // Assert
            Assert.IsTrue(factoryInvoked);
        }

        [TestMethod]
        public void WithDomDefinition_ThrowsArgumentException_WithEmptyModuleId()
        {
            // Arrange
            var builder = new IDmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentException>(() => builder.WithDomDefinition(
                moduleId: String.Empty,
                createDefinition: () => CreateDefinition(Guid.NewGuid(), "Definition")));
        }

        [TestMethod]
        public void WithDomDefinition_ThrowsArgumentNullException_WithNullFactory()
        {
            // Arrange
            var builder = new IDmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.WithDomDefinition(moduleId: "module", createDefinition: null));
        }

        [TestMethod]
        public void WithDomInstance_ThrowsArgumentNullException_WithNullFactory()
        {
            // Arrange
            var builder = new IDmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.WithDomInstance(moduleId: "module", createInstance: null));
        }

        [TestMethod]
        public void WithSectionDefinition_ThrowsArgumentNullException_WithNullFactory()
        {
            // Arrange
            var builder = new IDmsBuilder();

            // Act and assert
            Assert.ThrowsExactly<ArgumentNullException>(() => builder.WithSectionDefinition(moduleId: "module", createDefinition: null));
        }

        private static DomDefinition CreateDefinition(Guid id, string name)
        {
            return new DomDefinitionBuilder()
                .WithID(id)
                .WithName(name)
                .Build();
        }
    }
}
