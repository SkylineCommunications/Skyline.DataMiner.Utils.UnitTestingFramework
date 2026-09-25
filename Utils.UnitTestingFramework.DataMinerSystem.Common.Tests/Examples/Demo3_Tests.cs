namespace Utils.UnitTestingFramework.DataMinerSystem.Common.Tests.Examples
{
    using System;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Utils.DOM.Builders;
    using Skyline.DataMiner.Utils.DOM.Extensions;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    public class Demo3_Tests
    {
        private const string ModuleId = "demo3-module";

        private static readonly Guid DomDefinitionId = Guid.NewGuid();

        private static readonly SectionDefinitionID SectionDefinitionId = new SectionDefinitionID(Guid.NewGuid());

        private static readonly FieldDescriptorID StatusFieldId = new FieldDescriptorID(Guid.NewGuid());

        [TestMethod]
        public void DomInstanceWorkflow_TracksOnlyTargetDefinition_WhenInstancesAreCreatedUpdatedAndDeleted()
        {
            // Arrange
            var dmsMock = CreateDmsMock(out var definition);
            var domHelper = new DomHelper(dmsMock.Connection.Object.HandleMessages, ModuleId);
            var instanceId = new DomInstanceId(Guid.NewGuid());
            var instance = new DomInstanceBuilder(definition)
                .WithID(instanceId)
                .WithFieldValue(SectionDefinitionId, StatusFieldId, "Pending")
                .Build();
            var unrelatedDefinition = new DomDefinitionBuilder()
                .WithID(Guid.NewGuid())
                .WithName("Unrelated definition")
                .Build();
            var unrelatedInstance = new DomInstanceBuilder(unrelatedDefinition)
                .WithID(Guid.NewGuid())
                .Build();
            var fieldSetter = new DomFieldSetter(domHelper, SectionDefinitionId, StatusFieldId);
            domHelper.DomDefinitions.Create(unrelatedDefinition);

            using (var changeCounter = new DomInstanceChangeCounter(dmsMock.Connection.Object, DomDefinitionId))
            {
                changeCounter.StartWatching();

                // Act
                domHelper.DomInstances.Create(unrelatedInstance);
                domHelper.DomInstances.Create(instance);
                fieldSetter.Set(instanceId, "Completed");
                var updatedInstance = domHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(instanceId)).Single();
                var statusSection = updatedInstance.Sections.Single(section => section.SectionDefinitionID.Equals(SectionDefinitionId));
                var updatedStatus = statusSection.GetFieldValue<string>(StatusFieldId);
                domHelper.DomInstances.Delete(updatedInstance);

                // Assert
                Assert.AreEqual("Completed", updatedStatus);
                Assert.IsFalse(domHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(instanceId)).Any());
                Assert.IsTrue(domHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(unrelatedInstance.ID)).Any());
                Assert.AreEqual(1, changeCounter.NumberOfAdded);
                Assert.AreEqual(1, changeCounter.NumberOfUpdated);
                Assert.AreEqual(1, changeCounter.NumberOfDeleted);
            }
        }

        private static IDmsMock CreateDmsMock(out DomDefinition domDefinition)
        {
            var sectionDefinition = new SectionDefinitionBuilder()
                .WithID(SectionDefinitionId)
                .WithName("Demo status section")
                .AddFieldDescriptor(field => field
                    .WithID(StatusFieldId)
                    .WithName("Status")
                    .WithType(typeof(string)))
                .Build();

            var builtDomDefinition = new DomDefinitionBuilder()
                .WithID(DomDefinitionId)
                .WithName("Demo definition")
                .AddSectionDefinitionLink(SectionDefinitionId)
                .Build();

            domDefinition = builtDomDefinition;

            return new DmsBuilder()
                .WithSectionDefinition(ModuleId, () => sectionDefinition)
                .WithDomDefinition(ModuleId, () => builtDomDefinition)
                .Build();
        }
    }

    internal class DomFieldSetter
    {
        private readonly DomHelper domHelper;
        private readonly SectionDefinitionID sectionDefinitionId;
        private readonly FieldDescriptorID fieldId;

        public DomFieldSetter(DomHelper domHelper, SectionDefinitionID sectionDefinitionId, FieldDescriptorID fieldId)
        {
            this.domHelper = domHelper ?? throw new ArgumentNullException(nameof(domHelper));
            this.sectionDefinitionId = sectionDefinitionId ?? throw new ArgumentNullException(nameof(sectionDefinitionId));
            this.fieldId = fieldId ?? throw new ArgumentNullException(nameof(fieldId));
        }

        public void Set(DomInstanceId instanceId, string value)
        {
            var instance = domHelper.DomInstances.Read(DomInstanceExposers.Id.Equal(instanceId)).SingleOrDefault()
                ?? throw new InvalidOperationException($"DOM instance '{instanceId}' was not found.");

            var section = instance.Sections.SingleOrDefault(s => s.SectionDefinitionID.Equals(sectionDefinitionId))
                ?? throw new InvalidOperationException($"Section '{sectionDefinitionId.Id}' was not found on DOM instance '{instanceId}'.");

            section.AddOrUpdateValue(fieldId, value);

            domHelper.DomInstances.Update(instance);
        }
    }

    internal class DomInstanceChangeCounter : IDisposable
    {
        private readonly IConnection connection;
        private readonly Guid domDefinitionId;
        private readonly string subscriptionId = $"DomInstanceChangeCounter-{Guid.NewGuid()}";
        private bool isWatching;
        private bool disposed;

        public DomInstanceChangeCounter(IConnection connection, Guid domDefinitionId)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.domDefinitionId = domDefinitionId;
        }

        public int NumberOfAdded { get; private set; }

        public int NumberOfUpdated { get; private set; }

        public int NumberOfDeleted { get; private set; }

        public void StartWatching()
        {
            if (isWatching)
            {
                return;
            }

            connection.OnNewMessage += Connection_OnNewMessage;
            connection.AddSubscription(
                subscriptionId,
                new SubscriptionFilter[]
                {
                    new SubscriptionFilterElement(typeof(DomInstancesChangedEventMessage), -1, -1),
                });
            isWatching = true;
        }

        public void StopWatching()
        {
            if (!isWatching)
            {
                return;
            }

            connection.OnNewMessage -= Connection_OnNewMessage;
            connection.ClearSubscriptions(subscriptionId);
            isWatching = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    StopWatching();
                }

                disposed = true;
            }
        }

        private void Connection_OnNewMessage(object sender, NewMessageEventArgs e)
        {
            if (!(e.Message is DomInstancesChangedEventMessage message))
            {
                return;
            }

            NumberOfAdded += CountMatching(message.Created);
            NumberOfUpdated += CountMatching(message.Updated);
            NumberOfDeleted += CountMatching(message.Deleted);
        }

        private int CountMatching(System.Collections.Generic.IEnumerable<DomInstance> instances)
        {
            return instances.Count(instance => instance.DomDefinitionId.Id.Equals(domDefinitionId));
        }
    }
}
