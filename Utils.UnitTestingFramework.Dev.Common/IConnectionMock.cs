namespace Skyline.DataMiner.Utils.UnitTestingFramework.Dev.Common
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Messages;

    /// <summary>
    /// A pre-arranged mock of an <see cref="IConnection"/>.
    /// </summary>
    public class IConnectionMock : Mock<IConnection>
    {
        private readonly ConcurrentDictionary<string, SubscriptionSet> subscriptions = new ConcurrentDictionary<string, SubscriptionSet>(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<Type, Func<DMSMessage, DMSMessage>> messageHandlers = new ConcurrentDictionary<Type, Func<DMSMessage, DMSMessage>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IConnectionMock"/> class.
        /// </summary>
        public IConnectionMock()
        {
            Setup(connection => connection.Subscribe(It.IsAny<SubscriptionFilter[]>()))
                .Callback((SubscriptionFilter[] filters) => Subscribe(filters))
                .Returns((SubscriptionFilter[] filters) => new CreateSubscriptionResponseMessage { Filters = filters });

            Setup(connection => connection.Unsubscribe()).Callback(Unsubscribe);

            Setup(connection => connection.AddSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => AddSubscription(subscriptionId, filters));

            Setup(connection => connection.RemoveSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => RemoveSubscription(subscriptionId, filters));

            Setup(connection => connection.ReplaceSubscription(It.IsAny<string>(), It.IsAny<SubscriptionFilter[]>()))
                .Callback((string subscriptionId, SubscriptionFilter[] filters) => ReplaceSubscription(subscriptionId, filters));

            Setup(connection => connection.ClearSubscriptions(It.IsAny<string>()))
                .Callback((string subscriptionId) => ClearSubscriptions(subscriptionId));

            Setup(connection => connection.HandleMessages(It.IsAny<DMSMessage[]>()))
                .Returns((DMSMessage[] messages) => HandleMessages(messages));

            Setup(connection => connection.HandleMessage(It.IsAny<DMSMessage>()))
                .Returns((DMSMessage message) => HandleMessages(new[] { message }));

            Setup(connection => connection.HandleSingleResponseMessage(It.IsAny<DMSMessage>()))
                .Returns((DMSMessage message) => HandleMessages(new[] { message }).SingleOrDefault());
        }

        /// <summary>
        /// Raises <see cref="IConnection.OnNewMessage"/> for every active subscription, ignoring the filters.
        /// </summary>
        /// <param name="message">The message to publish.</param>
        public void NotifySubscriptions(DMSMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            NotifySubscriptions(message, (filters, msg) => true);
        }

        /// <summary>
        /// Raises <see cref="IConnection.OnNewMessage"/> when an active subscription matches the message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message to publish.</param>
        /// <param name="passesFilters">A function to determine if the message passes the subscription filters.</param>
        public void NotifySubscriptions<TMessage>(TMessage message, Func<IReadOnlyCollection<SubscriptionFilter>, TMessage, bool> passesFilters ) where TMessage : DMSMessage
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            foreach (var subscription in subscriptions.Values)
            {
                bool passes = passesFilters(subscription.Filters.ToArray(), message);
                if (passes)
                {
                    RaiseOnNewMessage(subscription.SetId, message);
                }
            }
        }

        private DMSMessage[] HandleMessages(DMSMessage[] messages)
        {
            if (messages == null)
            {
                throw new ArgumentNullException(nameof(messages));
            }

            var responses = new List<DMSMessage>();

            foreach (var message in messages)
            {
                if (message == null)
                {
                    throw new ArgumentException("The message collection cannot contain null values.", nameof(messages));
                }

                if (messageHandlers.TryGetValue(message.GetType(), out var customHandler))
                {
                    var customResponse = customHandler(message);
                    if (customResponse != null)
                    {
                        responses.Add(customResponse);
                    }
                }
                else
                {
                    // TODO how to handle messages without a registered handler? For now, we just ignore them.
                }
            }

            return responses.ToArray();
        }

        /// <summary>
        /// Registers a request handler for a custom SLNet message type.
        /// Registered handlers are evaluated before the built-in message handler.
        /// </summary>
        /// <typeparam name="TMessage">The request message type.</typeparam>
        /// <param name="handler">The handler that creates the response message.</param>
        public void RegisterMessageHandler<TMessage>(Func<TMessage, DMSMessage> handler)
            where TMessage : DMSMessage
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            if (!messageHandlers.TryAdd(typeof(TMessage), message => handler((TMessage)message)))
            {
                throw new InvalidOperationException($"A message handler for '{typeof(TMessage).FullName}' is already registered.");
            }
        }

        /// <summary>
        /// Removes the custom request handler registered for an SLNet message type.
        /// </summary>
        /// <typeparam name="TMessage">The request message type.</typeparam>
        /// <returns><see langword="true"/> when a handler was removed; otherwise, <see langword="false"/>.</returns>
        public bool UnregisterMessageHandler<TMessage>()
            where TMessage : DMSMessage
        {
            return messageHandlers.TryRemove(typeof(TMessage), out _);
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
