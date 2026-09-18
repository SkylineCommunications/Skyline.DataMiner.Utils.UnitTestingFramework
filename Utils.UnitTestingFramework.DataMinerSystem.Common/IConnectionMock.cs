namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages;

    /// <summary>
    /// A pre-arranged mock of an SLNet <see cref="IConnection"/>.
    /// </summary>
    public class IConnectionMock : Mock<IConnection>
    {
        private readonly ConcurrentDictionary<string, SubscriptionSet> subscriptions = new ConcurrentDictionary<string, SubscriptionSet>(StringComparer.Ordinal);
        private Func<DMSMessage[], DMSMessage[]> messageHandler = messages => Array.Empty<DMSMessage>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IConnectionMock"/> class.
        /// </summary>
        public IConnectionMock()
        {
            Setup(connection => connection.Subscribe(It.IsAny<SubscriptionFilter[]>()))
                .Callback((SubscriptionFilter[] filters) => Subscribe(filters))
                .Returns((SubscriptionFilter[] filters) => new CreateSubscriptionResponseMessage
                {
                    Filters = filters,
                });
            Setup(connection => connection.Unsubscribe()).Callback(Unsubscribe);
            Setup(connection => connection.AddSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => AddSubscription(subscriptionId, filters));
            Setup(connection => connection.RemoveSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => RemoveSubscription(subscriptionId, filters));
            Setup(connection => connection.ReplaceSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => ReplaceSubscription(subscriptionId, filters));
            Setup(connection => connection.ClearSubscriptions(It.IsAny<string>()))
                .Callback((string subscriptionId) => ClearSubscriptions(subscriptionId));
        }

        /// <summary>
        /// Raises <see cref="IConnection.OnNewMessage"/> when an active subscription matches the message.
        /// </summary>
        /// <param name="message">The parameter change message to publish.</param>
        internal void NotifySubscriptions(ParameterChangeEventMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            NotifySubscriptions(
                message,
                filters => filters.Any(filter => Matches(filter, message)) ? message : null);
        }

        /// <summary>
        /// Sends SLNet request messages to the handler attached to this connection.
        /// </summary>
        /// <param name="messages">The messages to send.</param>
        /// <returns>The response messages.</returns>
        public DMSMessage[] HandleMessages(DMSMessage[] messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            return messageHandler(messages) ?? Array.Empty<DMSMessage>();
        }

        internal void SetMessageHandler(Func<DMSMessage[], DMSMessage[]> handler)
        {
            messageHandler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        internal void NotifySubscriptions<TMessage>(
            TMessage message,
            Func<IReadOnlyCollection<SubscriptionFilter>, TMessage> applyFilters)
            where TMessage : DMSMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (applyFilters == null)
            {
                throw new ArgumentNullException(nameof(applyFilters));
            }

            foreach (var subscription in subscriptions.Values)
            {
                var filteredMessage = applyFilters(subscription.Filters.ToArray());
                if (filteredMessage != null)
                {
                    RaiseOnNewMessage(subscription.SetId, filteredMessage);
                }
            }
        }

        private void Subscribe(SubscriptionFilter[] filters)
        {
            if (filters == null || filters.Length == 0)
            {
                return;
            }

            AddSubscription(String.Empty, filters);
        }

        private void Unsubscribe()
        {
            subscriptions.Clear();
        }

        private void AddSubscription(string subscriptionId, SubscriptionFilter[] filters)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            if (filters == null || filters.Length == 0)
            {
                return;
            }

            var subscription = subscriptions.GetOrAdd(subscriptionId, id => new SubscriptionSet(id));

            foreach (var filter in filters.Where(filter => filter != null))
            {
                subscription.Filters.TryAdd(filter);
            }
        }

        private void RemoveSubscription(string subscriptionId, SubscriptionFilter[] filters)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            if (!subscriptions.TryGetValue(subscriptionId, out var subscription) || filters == null)
            {
                return;
            }

            foreach (var filter in filters)
            {
                subscription.Filters.TryRemove(filter);
            }
        }

        private void ReplaceSubscription(string subscriptionId, SubscriptionFilter[] filters)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            ClearSubscriptions(subscriptionId);
            AddSubscription(subscriptionId, filters);
        }

        private void ClearSubscriptions(string subscriptionId)
        {
            if (subscriptionId == null)
            {
                throw new ArgumentNullException(nameof(subscriptionId));
            }

            subscriptions.TryRemove(subscriptionId, out _);
        }

        private static bool Matches(SubscriptionFilter filter, ParameterChangeEventMessage message)
        {
            var messageType = filter.ToTypeObject();

            if (messageType == null || !messageType.IsInstanceOfType(message))
            {
                return false;
            }

            if (!(filter is SubscriptionFilterElement elementFilter))
            {
                return true;
            }

            var agentMatches = elementFilter.DmaID < 0 || elementFilter.DmaID == message.DataMinerID;
            var elementMatches = elementFilter.ElementID < 0 || elementFilter.ElementID == message.ElementID;
            return agentMatches && elementMatches;
        }

        private void RaiseOnNewMessage(string subscriptionId, DMSMessage message)
        {
            var eventWithSetIds = EventWithSetIDs.Wrap(new[] { subscriptionId }, message);

            Raise(
                connection => connection.OnNewMessage += null,
                Object,
                new NewMessageEventArgs(eventWithSetIds));
        }
    }
}
