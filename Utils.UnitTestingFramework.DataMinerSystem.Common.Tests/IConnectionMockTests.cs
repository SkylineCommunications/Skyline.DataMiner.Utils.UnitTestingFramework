using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Utils.DOM.Builders;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("Examples/protocol.xml", "Examples")]
    public class IConnectionMockTests
    {
        private const string SubscriptionId = "ParameterChangedSubscription";
        private const string ProtocolPath = "Examples/protocol.xml";
        private const int ParameterId = 10;

        [TestMethod]
        public void AddSubscription_DoesNotNotifyTwice_WithDuplicateFilter()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var elementMock = dmsMock
                .CreateAgent(1, "DMA 1")
                .CreateElement(ProtocolPath, id: 11, name: "Element 11");
            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            connection.OnNewMessage += (sender, args) => numberOfInvocations++;
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });

            // Act
            elementMock.GetStandaloneParameterMock<int?>(ParameterId).UpdateValue(1);

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
        }

        [TestMethod]
        public void AddSubscription_NotifiesOnceAsDeleted_WhenTableRowIsDeleted()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");
            var tableMock = elementMock.GetDmsTableMock(100);
            tableMock.SetRow(new object[] { "row-1", "Value" });

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            ParameterTableUpdateEventMessage receivedMessage = null;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterTableUpdateEventMessage message)
                {
                    numberOfInvocations++;
                    receivedMessage = message;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });

            // Act
            tableMock.RemoveRows("row-1");

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(100, receivedMessage.ParameterID);
            Assert.AreEqual(101, receivedMessage.IndexColumnID);
            Assert.AreEqual("row-1", receivedMessage.TableIndex);
            Assert.IsTrue(receivedMessage.IsDeleted);
            Assert.AreEqual(0, receivedMessage.UpdatedRows.Length);
            CollectionAssert.AreEqual(new[] { "row-1" }, receivedMessage.DeletedRows);
        }

        [TestMethod]
        public void AddSubscription_NotifiesOnceWithTableAndRowInformation_WhenTableRowIsAdded()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");
            var tableMock = elementMock.GetDmsTableMock(100);

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            ParameterTableUpdateEventMessage receivedMessage = null;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterTableUpdateEventMessage message)
                {
                    numberOfInvocations++;
                    receivedMessage = message;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });

            // Act
            tableMock.SetRow(new object[] { "row-1", "Value" });

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(100, receivedMessage.ParameterID);
            Assert.AreEqual(101, receivedMessage.IndexColumnID);
            Assert.AreEqual("row-1", receivedMessage.TableIndex);
            Assert.IsFalse(receivedMessage.IsDeleted);
            Assert.AreEqual(1, receivedMessage.UpdatedRows.Length);
            Assert.AreEqual(0, receivedMessage.DeletedRows.Length);
        }

        [TestMethod]
        public void AddSubscription_NotifiesOnlyForTrackedElement_WithParameterChangesOnMultipleElements()
        {
            // Arrange
            var dmsMock = new IDmsMock();

            var firstDma = dmsMock.CreateAgent(1, "DMA 1");
            var secondDma = dmsMock.CreateAgent(2, "DMA 2");

            var trackedElement = firstDma.CreateElement(ProtocolPath, id: 11, name: "Tracked element");

            var otherElementOnSameDma = firstDma.CreateElement(ProtocolPath, id: 12, name: "Other element on same DMA");

            var elementWithSameIdOnOtherDma = secondDma.CreateElement(ProtocolPath, id: 11, name: "Element on other DMA");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            ParameterChangeEventMessage receivedMessage = null;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage message)
                {
                    numberOfInvocations++;
                    receivedMessage = message;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });

            connection.Subscribe();

            // Act
            otherElementOnSameDma
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            elementWithSameIdOnOtherDma
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(2);

            trackedElement
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(3);

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(1, receivedMessage.DataMinerID);
            Assert.AreEqual(11, receivedMessage.ElementID);
            Assert.AreEqual(ParameterId, receivedMessage.ParameterID);
        }

        [TestMethod]
        public void AddSubscription_NotifiesWithColumnAndRowInformation_WhenTableCellChanges()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");
            var tableMock = elementMock.GetDmsTableMock(100);
            tableMock.SetRow(new object[] { "row-1", "Old value" });

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            ParameterTableUpdateEventMessage receivedMessage = null;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterTableUpdateEventMessage message)
                {
                    numberOfInvocations++;
                    receivedMessage = message;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);
            connection.AddSubscription(SubscriptionId, new SubscriptionFilter[] { filter });
            connection.Subscribe();

            // Act
            tableMock.SetCell("row-1", 102, "New value");

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(1, receivedMessage.DataMinerID);
            Assert.AreEqual(11, receivedMessage.ElementID);
            Assert.AreEqual(100, receivedMessage.ParameterID);
            Assert.AreEqual(101, receivedMessage.IndexColumnID);
            Assert.AreEqual("row-1", receivedMessage.TableIndex);
            Assert.AreEqual(1, receivedMessage.UpdatedRows.Length);
            Assert.AreEqual("row-1", receivedMessage.UpdatedRows[0].ArrayValue[0].StringValue);
            Assert.AreEqual("New value", receivedMessage.UpdatedRows[0].ArrayValue[1].StringValue);
        }

        [TestMethod]
        public void AddSubscription_RaisesTwoNotifications_WithTwoMatchingSubscriptionIds()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage)
                {
                    numberOfInvocations++;
                }
            };

            var firstFilter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            var secondFilter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            connection.AddSubscription(
                "FirstSubscription",
                new SubscriptionFilter[] { firstFilter });

            connection.AddSubscription(
                "SecondSubscription",
                new SubscriptionFilter[] { secondFilter });

            // Act
            elementMock
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            // Assert
            Assert.AreEqual(2, numberOfInvocations);
        }

        [TestMethod]
        public void AddSubscription_ThrowsArgumentNullException_WithNullSubscriptionId()
        {
            var connection = new IConnectionMock().Object;
            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            Assert.ThrowsExactly<ArgumentNullException>(() =>
                connection.AddSubscription(null, new SubscriptionFilter[] { filter }));
        }

        [TestMethod]
        public void ClearSubscriptions_DoesNotNotify_AfterClearingSubscription()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage)
                {
                    numberOfInvocations++;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            connection.AddSubscription(
                SubscriptionId,
                new SubscriptionFilter[] { filter });

            connection.ClearSubscriptions(SubscriptionId);

            // Act
            elementMock
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            // Assert
            Assert.AreEqual(0, numberOfInvocations);
        }

        [TestMethod]
        public void ClearSubscriptions_ThrowsArgumentNullException_WithNullSubscriptionId()
        {
            var connection = new IConnectionMock().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.ClearSubscriptions(null));
        }

        [TestMethod]
        public void HandleMessage_ReturnsCustomResponse_WithRegisteredCustomMessage()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(request => new CustomResponseMessage { Value = request.Value });

            // Act
            var responses = connection.Object.HandleMessage(new CustomRequestMessage { Value = "request" });

            // Assert
            Assert.AreEqual("request", ((CustomResponseMessage)responses.Single()).Value);
        }

        [TestMethod]
        public void HandleMessages_ReturnsCustomResponse_WithRegisteredCustomMessage()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(request => new CustomResponseMessage
            {
                Value = request.Value.ToUpperInvariant(),
            });

            // Act
            var responses = connection.Object.HandleMessages(new DMSMessage[]
            {
                new CustomRequestMessage { Value = "request" },
            });

            // Assert
            var response = (CustomResponseMessage)responses.Single();
            Assert.AreEqual("REQUEST", response.Value);
        }

        [TestMethod]
        public void HandleMessages_ReturnsNoResponse_WhenCustomHandlerReturnsNull()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(_ => null);

            // Act
            var responses = connection.Object.HandleMessages(new DMSMessage[] { new CustomRequestMessage() });

            // Assert
            Assert.IsEmpty(responses);
        }

        [TestMethod]
        public void HandleMessages_ReturnsResponsesInRequestOrder_WithMultipleCustomMessages()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(request => new CustomResponseMessage { Value = request.Value });

            // Act
            var responses = connection.Object.HandleMessages(new DMSMessage[]
            {
                new CustomRequestMessage { Value = "first" },
                new CustomRequestMessage { Value = "second" },
            });

            // Assert
            Assert.AreEqual("first", ((CustomResponseMessage)responses[0]).Value);
            Assert.AreEqual("second", ((CustomResponseMessage)responses[1]).Value);
        }

        [TestMethod]
        public void HandleMessages_ThrowsArgumentException_WithNullMessageInCollection()
        {
            var connection = new IConnectionMock();

            Assert.ThrowsExactly<ArgumentException>(() => connection.Object.HandleMessages(new DMSMessage[] { null }));
        }

        [TestMethod]
        public void HandleMessages_ThrowsArgumentNullException_WithNullMessages()
        {
            var connection = new IConnectionMock();

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.Object.HandleMessages(null));
        }

        [TestMethod]
        public void HandleMessages_UsesCustomAndDomHandlers_OnSameConnection()
        {
            // Arrange
            var definitionId = Guid.NewGuid();
            var dmsMock = new IDmsBuilder()
                .WithDomDefinition(moduleId: "module", createDefinition: () => new DomDefinitionBuilder()
                    .WithID(definitionId)
                    .WithName("Definition")
                    .Build())
                .Build();
            dmsMock.Connection.RegisterMessageHandler<CustomRequestMessage>(request => new CustomResponseMessage
            {
                Value = request.Value,
            });

            // Act
            var customResponse = dmsMock.Connection.Object.HandleMessages(new DMSMessage[]
            {
                new CustomRequestMessage { Value = "custom" },
            }).Single();
            var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, "module");
            var definition = helper.DomDefinitions.Read(DomDefinitionExposers.Id.Equal(definitionId)).Single();

            // Assert
            Assert.AreEqual("custom", ((CustomResponseMessage)customResponse).Value);
            Assert.AreEqual(definitionId, definition.ID.Id);
        }

        [TestMethod]
        public void HandleSingleResponseMessage_ReturnsCustomResponse_WithRegisteredCustomMessage()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(request => new CustomResponseMessage { Value = request.Value });

            // Act
            var response = connection.Object.HandleSingleResponseMessage(new CustomRequestMessage { Value = "request" });

            // Assert
            Assert.AreEqual("request", ((CustomResponseMessage)response).Value);
        }

        [TestMethod]
        public void NotifySubscriptions_RaisesCustomMessage_WithoutApplyingFilters()
        {
            // Arrange
            var connection = new IConnectionMock();
            var expectedMessage = new CustomRequestMessage { Value = "custom" };
            DMSMessage receivedMessage = null;

            connection.Object.OnNewMessage += (sender, args) => receivedMessage = args.Message;
            connection.Object.AddSubscription(
                SubscriptionId,
                new SubscriptionFilter[] { new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11) });

            // Act
            connection.NotifySubscriptions(expectedMessage);

            // Assert
            Assert.AreSame(expectedMessage, receivedMessage);
        }

        [TestMethod]
        public void NotifySubscriptions_ThrowsArgumentNullException_WithNullMessage()
        {
            var connection = new IConnectionMock();

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.NotifySubscriptions(null));
        }

        [TestMethod]
        public void RegisterMessageHandler_ThrowsArgumentNullException_WithNullHandler()
        {
            var connection = new IConnectionMock();

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.RegisterMessageHandler<CustomRequestMessage>(null));
        }

        [TestMethod]
        public void RegisterMessageHandler_ThrowsInvalidOperationException_WhenMessageTypeIsAlreadyRegistered()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(_ => new CustomResponseMessage());

            // Act and assert
            Assert.ThrowsExactly<InvalidOperationException>(() =>
                connection.RegisterMessageHandler<CustomRequestMessage>(_ => new CustomResponseMessage()));
        }

        [TestMethod]
        public void RemoveSubscription_DoesNotNotify_AfterRemovingFilter()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage)
                {
                    numberOfInvocations++;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            var filters = new SubscriptionFilter[] { filter };

            connection.AddSubscription(SubscriptionId, filters);
            connection.RemoveSubscription(SubscriptionId, filters);

            // Act
            elementMock
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            // Assert
            Assert.AreEqual(0, numberOfInvocations);
        }

        [TestMethod]
        public void RemoveSubscription_ThrowsArgumentNullException_WithNullSubscriptionId()
        {
            var connection = new IConnectionMock().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.RemoveSubscription(null, Array.Empty<SubscriptionFilter>()));
        }

        [TestMethod]
        public void ReplaceSubscription_NotifiesOnlyForNewElement_AfterReplacingFilter()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");

            var firstElement = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var secondElement = dmaMock.CreateElement(ProtocolPath, id: 12, name: "Element 12");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;
            ParameterChangeEventMessage receivedMessage = null;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage message)
                {
                    numberOfInvocations++;
                    receivedMessage = message;
                }
            };

            var firstFilter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);
            var secondFilter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 12);

            connection.AddSubscription(
                SubscriptionId,
                new SubscriptionFilter[] { firstFilter });

            connection.ReplaceSubscription(
                SubscriptionId,
                new SubscriptionFilter[] { secondFilter });

            // Act
            firstElement
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            secondElement
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(2);

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
            Assert.IsNotNull(receivedMessage);
            Assert.AreEqual(12, receivedMessage.ElementID);
        }

        [TestMethod]
        public void ReplaceSubscription_ThrowsArgumentNullException_WithNullSubscriptionId()
        {
            var connection = new IConnectionMock().Object;

            Assert.ThrowsExactly<ArgumentNullException>(() => connection.ReplaceSubscription(null, Array.Empty<SubscriptionFilter>()));
        }

        [TestMethod]
        public void Subscribe_NotifiesForMatchingElement_WithFilter()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage)
                {
                    numberOfInvocations++;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            connection.Subscribe(
                new SubscriptionFilter[] { filter });

            // Act
            elementMock
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            // Assert
            Assert.AreEqual(1, numberOfInvocations);
        }

        [TestMethod]
        public void Subscribe_ReturnsCreateSubscriptionResponse_WithProvidedFilters()
        {
            var connection = new IConnectionMock().Object;
            var filters = new SubscriptionFilter[]
            {
                new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11),
            };

            var response = connection.Subscribe(filters);

            Assert.IsInstanceOfType<CreateSubscriptionResponseMessage>(response);
            CollectionAssert.AreEqual(filters, ((CreateSubscriptionResponseMessage)response).Filters);
        }

        [TestMethod]
        public void UnregisterMessageHandler_RemovesHandler_WhenMessageTypeIsRegistered()
        {
            // Arrange
            var connection = new IConnectionMock();
            connection.RegisterMessageHandler<CustomRequestMessage>(_ => new CustomResponseMessage());

            // Act
            var removed = connection.UnregisterMessageHandler<CustomRequestMessage>();
            var responses = connection.Object.HandleMessages(new DMSMessage[] { new CustomRequestMessage() });

            // Assert
            Assert.IsTrue(removed);
            Assert.IsEmpty(responses);
        }

        [TestMethod]
        public void UnregisterMessageHandler_ReturnsFalse_WithoutRegisteredHandler()
        {
            var connection = new IConnectionMock();

            Assert.IsFalse(connection.UnregisterMessageHandler<CustomRequestMessage>());
        }

        [TestMethod]
        public void Unsubscribe_DoesNotNotify_AfterUnsubscribing()
        {
            // Arrange
            var dmsMock = new IDmsMock();
            var dmaMock = dmsMock.CreateAgent(1, "DMA 1");
            var elementMock = dmaMock.CreateElement(ProtocolPath, id: 11, name: "Element 11");

            var connection = dmsMock.Connection.Object;
            var numberOfInvocations = 0;

            connection.OnNewMessage += (sender, args) =>
            {
                if (args.Message is ParameterChangeEventMessage)
                {
                    numberOfInvocations++;
                }
            };

            var filter = new SubscriptionFilterElement(typeof(ParameterChangeEventMessage), 1, 11);

            connection.AddSubscription(
                SubscriptionId,
                new SubscriptionFilter[] { filter });

            connection.Unsubscribe();

            // Act
            elementMock
                .GetStandaloneParameterMock<int?>(ParameterId)
                .UpdateValue(1);

            // Assert
            Assert.AreEqual(0, numberOfInvocations);
        }

        private sealed class CustomRequestMessage : DMSMessage
        {
            public string Value { get; set; }
        }

        private sealed class CustomResponseMessage : DMSMessage
        {
            public string Value { get; set; }
        }
    }
}
