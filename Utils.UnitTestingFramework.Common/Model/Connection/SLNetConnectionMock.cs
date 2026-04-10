namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Async;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.SubscriptionFilters;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Connection;

    /// <summary>
    /// A mock implementation of <see cref="IConnection"/> used for testing purposes.
    /// </summary>
    public sealed partial class SLNetConnectionMock : IConnection
    {
        private readonly ConcurrentDictionary<string, SubscriptionSet> _subscriptions = new ConcurrentDictionary<string, SubscriptionSet>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SLNetConnectionMock"/> class.
        /// </summary>
        /// <param name="dms">Mocked DMS.</param>
        internal SLNetConnectionMock(SimulatedDms dms)
        {
            Dms = dms ?? throw new ArgumentNullException(nameof(dms));
        }

        internal SimulatedDms Dms { get; }

        public int SubscriptionCount => _subscriptions.Count;

        internal void NotifySubscriptions(EventMessage eventMessage)
        {
            if (eventMessage is DomInstancesChangedEventMessage domInstancesChangedEvent)
            {
                NotifyDomInstancesChanged(domInstancesChangedEvent);
                return;
            }

            if (eventMessage is ParameterTableUpdateEventMessage parameterTableUpdateEvent)
            {
                NotifyTableUpdate(parameterTableUpdateEvent);
                return;
            }

            foreach (var subscription in _subscriptions.Values)
            {
                if (!subscription.Filters.Any(f => f.ToTypeObject() == eventMessage.GetType()))
                {
                    continue;
                }

                InvokeOnNewMessageEvent(subscription.SetId, eventMessage);
            }
        }

        private void NotifyDomInstancesChanged(DomInstancesChangedEventMessage e)
        {
            foreach (var subscription in _subscriptions.Values)
            {
                var moduleMatch = false;
                var created = e.Created.ToList();
                var updated = e.Updated.ToList();
                var deleted = e.Deleted.ToList();

                foreach (var filter in subscription.Filters)
                {
                    switch (filter)
                    {
                        case ModuleEventSubscriptionFilter<DomInstancesChangedEventMessage> moduleFilter:
                            moduleMatch |= moduleFilter.IsMatch(e);
                            break;
                        case SubscriptionFilter<DomInstancesChangedEventMessage, DomInstance> instanceFilter:
                            var lambda = instanceFilter.Filter.getLambda();
                            created.RemoveAll(x => !lambda(x));
                            updated.RemoveAll(x => !lambda(x));
                            deleted.RemoveAll(x => !lambda(x));
                            break;
                    }
                }

                if (moduleMatch &&
                    (created.Count > 0 || updated.Count > 0 || deleted.Count > 0))
                {
                    InvokeOnNewMessageEvent(subscription.SetId, e);
                }
            }
        }

        private void NotifyTableUpdate(ParameterTableUpdateEventMessage e)
        {
            foreach (var subscription in _subscriptions.Values)
            {
                var isFilterMatch = false;

                foreach (var filter in subscription.Filters)
                {
                    switch (filter)
                    {
                        case SubscriptionFilterParameter filterParameter:
                            isFilterMatch |= filterParameter.ToTypeObject() == typeof(ParameterTableUpdateEventMessage)
                                && filterParameter.DmaID == e.DataMinerID
                                && filterParameter.ElementID == e.ElementID
                                && filterParameter.ParameterID == e.ParameterID;
                            break;
                    }
                }

                if (isFilterMatch)
                {
                    InvokeOnNewMessageEvent(subscription.SetId, e);
                }
            }
        }

        private void InvokeOnNewMessageEvent(string subscriptionSetId, EventMessage eventMessage)
        {
            var eventWithSetIds = EventWithSetIDs.Wrap(new[] { subscriptionSetId }, eventMessage);

            OnNewMessage?.Invoke(
                this,
                new NewMessageEventArgs(eventWithSetIds));
        }

        private void SendInitialEvents(string subscriptionSetId, ICollection<SubscriptionFilter> filters)
        {
            foreach (var filter in filters)
            {
                if (filter.Options.HasFlag(SubscriptionFilterOptions.SkipInitialEvents))
                {
                    continue;
                }

                switch (filter)
                {
                    case SubscriptionFilterParameter filterParameter:
                        {
                            if (filterParameter.ToTypeObject() == typeof(ParameterTableUpdateEventMessage) &&
                                Dms.Agents.TryGetValue(filterParameter.DmaID, out var dma) &&
                                dma.Elements.TryGetValue(filterParameter.ElementID, out var element) &&
                                element.TryGetTable(filterParameter.ParameterID, out var table))
                            {
                                var initialEvent = new ParameterChangeEventMessage(filterParameter.DmaID, filterParameter.ElementID, filterParameter.ParameterID)
                                {
                                    NewValue = table.ToParameterValue(),
                                };

                                InvokeOnNewMessageEvent(subscriptionSetId, initialEvent);
                            }
                        }

                        break;

                    default:
                        if (filter.ToTypeObject() == typeof(ElementStateEventMessage))
                        {
                            var elements = Dms.Agents.Values.SelectMany(x => x.Elements.Values);

                            foreach (var element in elements)
                            {
                                var initialEvent = new ElementStateEventMessage(element.Dma.DmaId, element.ElementId, ElementState.Active, AlarmLevel.Normal);
                                InvokeOnNewMessageEvent(subscriptionSetId, initialEvent);
                            }
                        }

                        break;
                }
            }
        }

        private bool TryHandleMessage(DMSMessage message, out IEnumerable<DMSMessage> responses)
        {
            switch (message)
            {
                case RequestTicketMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                default:
                    responses = Enumerable.Empty<DMSMessage>();
                    return false;
            }
        }

        private IEnumerable<DMSMessage> HandleMessage(RequestTicketMessage msg)
        {
            if (msg.TicketType != TicketType.Authentication)
            {
                throw new InvalidOperationException();
            }

            yield return new TicketResponseMessage
            {
                Ticket = "<simulated connection>",
            };
        }

        #region IConnection implementation

        /// <inheritdoc/>
        public string UserDomainName => throw new NotImplementedException();

        /// <inheritdoc/>
        public Guid ConnectionID => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsShuttingDown => false;

        /// <inheritdoc/>
        public IAsyncMessageHandler Async => new AsyncMessageHandlerMock(this);

        /// <inheritdoc/>
        public bool IsReceiving => true;

        /// <inheritdoc/>
        public ServerDetails ServerDetails => throw new NotImplementedException();

#pragma warning disable CS0067 // The event is never used
        /// <inheritdoc/>
        public event ConnectionClosedHandler OnClose;

        /// <inheritdoc/>
        public event NewMessageEventHandler OnNewMessage;

        /// <inheritdoc/>
        public event AbnormalCloseEventHandler OnAbnormalClose;

        /// <inheritdoc/>
        public event EventsDroppedEventHandler OnEventsDropped;

        /// <inheritdoc/>
        public event SubscriptionCompleteEventHandler OnSubscriptionComplete;

        /// <inheritdoc/>
        public event AuthenticationChallengeEventHandler OnAuthenticationChallenge;

        /// <inheritdoc/>
        public event EventHandler<SubscriptionStateEventArgs> OnSubscriptionState;
#pragma warning restore CS0067

        /// <summary>
        /// Gets a value indicating whether there are subscribers to the <see cref="OnNewMessage"/> event.
        /// For unit testing purposes.
        /// </summary>
        public bool HasOnNewMessageSubscribers => OnNewMessage?.GetInvocationList().Any() ?? false;

        /// <inheritdoc/>
        public DMSMessage[] HandleMessage(DMSMessage msg)
        {
            if (msg is null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            if (TryHandleMessage(msg, out var responses) ||
                Dms.TryHandleMessage(msg, out responses))
            {
                return responses.ToArray();
            }

            throw new NotSupportedException($"Unsupported message type: {msg.GetType()}");
        }

        /// <inheritdoc/>
        public DMSMessage[] HandleMessages(DMSMessage[] msgs)
        {
            if (msgs is null)
            {
                throw new ArgumentNullException(nameof(msgs));
            }

            return msgs.SelectMany(HandleMessage).ToArray();
        }

        /// <inheritdoc/>
        public DMSMessage HandleSingleResponseMessage(DMSMessage msg)
        {
            if (msg is null)
            {
                throw new ArgumentNullException(nameof(msg));
            }

            return HandleMessage(msg).SingleOrDefault();
        }

        /// <inheritdoc/>
        public CreateSubscriptionResponseMessage Subscribe(params SubscriptionFilter[] filters)
        {
            if (filters.Length > 0)
            {
                var subscriptionSet = _subscriptions.GetOrAdd(String.Empty, x => new SubscriptionSet(x));

                foreach (var filter in filters)
                {
                    subscriptionSet.Filters.TryAdd(filter);
                }

                SendInitialEvents(String.Empty, filters);
            }

            return new CreateSubscriptionResponseMessage()
            {
                Filters = filters,
            };
        }

        /// <inheritdoc/>
        public void Unsubscribe()
        {
            _subscriptions.Clear();
        }

        /// <inheritdoc/>
        public void AddSubscription(string setID, params SubscriptionFilter[] newFilters)
        {
            if (newFilters.Length == 0)
            {
                return;
            }

            var subscriptionSet = _subscriptions.GetOrAdd(setID, x => new SubscriptionSet(x));

            foreach (var filter in newFilters)
            {
                subscriptionSet.Filters.TryAdd(filter);
            }

            SendInitialEvents(setID, newFilters);
        }

        /// <inheritdoc/>
        public void RemoveSubscription(string setID, params SubscriptionFilter[] deletedFilters)
        {
            var subscriptionSet = _subscriptions.GetOrAdd(setID, x => new SubscriptionSet(x));

            foreach (var filter in deletedFilters)
            {
                subscriptionSet.Filters.TryRemove(filter);
            }
        }

        /// <inheritdoc/>
        public void ReplaceSubscription(string setID, params SubscriptionFilter[] newFilters)
        {
            ClearSubscriptions(setID);
            AddSubscription(setID);
        }

        /// <inheritdoc/>
        public void ClearSubscriptions(string setID)
        {
            _subscriptions.TryRemove(setID, out _);
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackSubscribe(params SubscriptionFilter[] filters)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackAddSubscription(string setID, params SubscriptionFilter[] newFilters)
        {
            return new TrackedSubscriptionUpdate(() => AddSubscription(setID, newFilters));
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackRemoveSubscription(string setID, params SubscriptionFilter[] deletedFilters)
        {
            return new TrackedSubscriptionUpdate(() => RemoveSubscription(setID, deletedFilters));
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackReplaceSubscription(string setID, params SubscriptionFilter[] newFilters)
        {
            return new TrackedSubscriptionUpdate(() => ReplaceSubscription(setID, newFilters));
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackClearSubscriptions(string setID)
        {
            return new TrackedSubscriptionUpdate(() => ClearSubscriptions(setID));
        }

        /// <inheritdoc/>
        public ITrackedSubscriptionUpdate TrackUpdateSubscription(UpdateSubscriptionMultiMessage multi)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SupportsFeature(CompatibilityFlags flags)
        {
            return true;
        }

        /// <inheritdoc/>
        public bool SupportsFeature(string name)
        {
            return true;
        }

        /// <inheritdoc/>
        public GetElementProtocolResponseMessage GetElementProtocol(int dmaid, int eid)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public GetProtocolInfoResponseMessage GetProtocol(string name, string version)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void FireOnAsyncResponse(AsyncResponseEvent responseEvent, ref bool handled)
        {
            // Do Nothing
        }

        /// <inheritdoc/>
        public DMSMessage[] UnPack(DMSMessage[] messages)
        {
            return messages;
        }

        /// <inheritdoc/>
        public void SafeWait(int timeout)
        {
            // No logic needed
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Unsubscribe();
        }

        #endregion
    }
}
