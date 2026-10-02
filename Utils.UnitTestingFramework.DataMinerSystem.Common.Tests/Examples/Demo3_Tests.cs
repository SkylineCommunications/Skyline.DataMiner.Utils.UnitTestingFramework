namespace Utils.UnitTestingFramework.DataMinerSystem.Common.Tests.Examples
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Utils.DOM.Builders;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    public class Demo3_Tests
    {
        private const string ModuleId = "demo3-module";

        private static readonly DomDefinitionId DomDefinitionId = new DomDefinitionId(Guid.NewGuid());

        private static readonly DomDefinitionId UnrelatedDomDefinitionId = new DomDefinitionId(Guid.NewGuid());

        private static readonly SectionDefinitionID SectionDefinitionId = new SectionDefinitionID(Guid.NewGuid());

        private static readonly SectionDefinitionID UnrelatedSectionDefinitionId = new SectionDefinitionID(Guid.NewGuid());

        private static readonly FieldDescriptorID StatusFieldId = new FieldDescriptorID(Guid.NewGuid());

        private static readonly FieldDescriptorID UnrelatedStatusFieldId = new FieldDescriptorID(Guid.NewGuid());

        private static readonly Guid relatedInstanceId = Guid.NewGuid();

        private static readonly Guid unrelatedInstanceId = Guid.NewGuid();

        private IDmsMock dmsMock;

        [TestInitialize]
        public void TestInitialize()
        {
            dmsMock = new IDmsBuilder()
                .WithSectionDefinition(ModuleId, () => new SectionDefinitionBuilder()
                    .WithID(SectionDefinitionId)
                    .WithName("Demo status section")
                    .AddFieldDescriptor(field => field
                        .WithID(StatusFieldId)
                        .WithName("Status")
                        .WithType(typeof(string)))
                    .Build())
                .WithDomDefinition(ModuleId, () => new DomDefinitionBuilder()
                    .WithID(DomDefinitionId)
                    .WithName("Demo definition")
                    .AddSectionDefinitionLink(SectionDefinitionId)
                    .Build())
                .WithDomInstance(ModuleId, () => new DomInstanceBuilder()
                    .WithID(relatedInstanceId)
                    .WithDefinition(DomDefinitionId)
                    .WithFieldValue(SectionDefinitionId, StatusFieldId, "In progress")
                    .Build())
                // Part of DOM module we won't keep track of
                .WithSectionDefinition(ModuleId, () => new SectionDefinitionBuilder()
                    .WithID(UnrelatedSectionDefinitionId)
                    .WithName("Unrelated Demo status section")
                    .AddFieldDescriptor(field => field
                        .WithID(UnrelatedStatusFieldId)
                        .WithName("Status")
                        .WithType(typeof(string)))
                    .Build())
                .WithDomDefinition(ModuleId, () => new DomDefinitionBuilder()
                    .WithID(UnrelatedDomDefinitionId)
                    .WithName("Unrelated Demo definition")
                    .AddSectionDefinitionLink(UnrelatedSectionDefinitionId)
                    .Build())
                .WithDomInstance(ModuleId, () => new DomInstanceBuilder()
                    .WithID(unrelatedInstanceId)
                    .WithDefinition(UnrelatedDomDefinitionId)
                    .WithFieldValue(UnrelatedSectionDefinitionId, UnrelatedStatusFieldId, "In progress")
                    .Build())
                .Build();
        }

        [TestMethod]
        public void DomInstanceWorkflow_TracksOnlyTargetDefinition_WhenInstancesAreCreatedUpdatedAndDeleted()
        {
            // Arrange
            var domHelper = new DomHelper(dmsMock.Connection.Object.HandleMessages, ModuleId);

            // Act
            var changeCounter = new GqiRealTimeDomUpdateTracker(dmsMock.Connection.Object, DomDefinitionId.Id);
            changeCounter.StartWatching();

            var relatedInstance = domHelper.DomInstances.GetByID(relatedInstanceId);
            var unrelatedInstance = domHelper.DomInstances.GetByID(unrelatedInstanceId);

            relatedInstance.AddOrUpdateFieldValue<string>(SectionDefinitionId, StatusFieldId, "Completed");
            domHelper.DomInstances.Update(relatedInstance);

            unrelatedInstance.AddOrUpdateFieldValue<string>(UnrelatedSectionDefinitionId, UnrelatedStatusFieldId, "Completed");
            domHelper.DomInstances.Update(unrelatedInstance);

            // Assert
            changeCounter.NumberOfUpdated.Should().Be(1);
        }
    }

    internal class GqiRealTimeDomUpdateTracker
    {
        private readonly IConnection connection;
        private readonly Guid domDefinitionId;
        private readonly string subscriptionId = $"DomInstanceChangeCounter-{Guid.NewGuid()}";

        public GqiRealTimeDomUpdateTracker(IConnection connection, Guid domDefinitionId)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.domDefinitionId = domDefinitionId;
        }

        public int NumberOfUpdated { get; private set; }

        public void StartWatching()
        {
            connection.OnNewMessage += Connection_OnNewMessage;
            connection.AddSubscription(subscriptionId, new SubscriptionFilter[] { new SubscriptionFilterElement(typeof(DomInstancesChangedEventMessage), -1, -1), });
        }

        private void Connection_OnNewMessage(object sender, NewMessageEventArgs e)
        {
            if (!(e.Message is DomInstancesChangedEventMessage message))
            {
                return;
            }

            NumberOfUpdated += CountMatching(message.Updated);
        }

        private int CountMatching(IEnumerable<DomInstance> instances)
        {
            return instances.Count(instance => instance.DomDefinitionId.Id.Equals(domDefinitionId));
        }
    }
}
