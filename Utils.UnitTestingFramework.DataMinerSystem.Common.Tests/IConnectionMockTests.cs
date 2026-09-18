using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("Examples/protocol.xml", "Examples")]
    public class IConnectionMockTests
    {
        private const string SubscriptionId = "ParameterChangedSubscription";
        private const string ProtocolPath = "Examples/protocol.xml";
        private const int ParameterId = 10;

        [TestMethod]
        public void Connection_GetMultipleTimes_ReturnsSameInstance()
        {
            // Arrange
            var dmsMock = new IDmsMock();

            // Act
            var firstConnection = dmsMock.Connection;
            var secondConnection = dmsMock.Connection;

            // Assert
            Assert.IsNotNull(firstConnection);
            Assert.AreSame(firstConnection, secondConnection);
        }

        [TestMethod]
        public void AddSubscription_ParameterChangesOnMultipleElements_NotifiesOnlyForTrackedElement()
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
        public void RemoveSubscription_AfterRemovingFilter_DoesNotNotify()
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
        public void ReplaceSubscription_AfterReplacingFilter_NotifiesOnlyForNewElement()
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
        public void ClearSubscriptions_AfterClearingSubscription_DoesNotNotify()
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
        public void Unsubscribe_AfterUnsubscribing_DoesNotNotify()
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

        [TestMethod]
        public void Subscribe_WithFilter_NotifiesForMatchingElement()
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
        public void AddSubscription_TwoMatchingSubscriptionIds_RaisesTwoNotifications()
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
        public void AddSubscription_TableCellChanges_NotifiesWithColumnAndRowInformation()
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
        public void AddSubscription_TableRowAdded_NotifiesOnceWithTableAndRowInformation()
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
        public void AddSubscription_TableRowDeleted_NotifiesOnceAsDeleted()
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
    }
}
