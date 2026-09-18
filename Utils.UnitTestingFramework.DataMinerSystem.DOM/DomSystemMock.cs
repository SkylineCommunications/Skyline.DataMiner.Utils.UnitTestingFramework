namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.DOM
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Net.SubscriptionFilters;
    using Skyline.DataMiner.Utils.DOM.UnitTesting;

    /// <summary>
    /// Provides in-memory DOM behavior for a simulated DataMiner System.
    /// </summary>
    public sealed class DomSystemMock
    {
        private readonly ConcurrentDictionary<string, DomSLNetMessageHandler> moduleHandlers =
            new ConcurrentDictionary<string, DomSLNetMessageHandler>(StringComparer.Ordinal);

        private readonly Func<DMSMessage[], DMSMessage[]> sendMessages;
        private readonly Action<DomInstancesChangedEventMessage, Func<IReadOnlyCollection<SubscriptionFilter>, DomInstancesChangedEventMessage>> notifySubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomSystemMock"/> class.
        /// </summary>
        /// <param name="sendMessages">The SLNet callback used by DOM helpers and caches.</param>
        /// <param name="notifySubscriptions">The callback used to publish DOM instance changes.</param>
        public DomSystemMock(
            Func<DMSMessage[], DMSMessage[]> sendMessages,
            Action<DomInstancesChangedEventMessage, Func<IReadOnlyCollection<SubscriptionFilter>, DomInstancesChangedEventMessage>> notifySubscriptions)
        {
            this.sendMessages = sendMessages ?? throw new ArgumentNullException(nameof(sendMessages));
            this.notifySubscriptions = notifySubscriptions ?? throw new ArgumentNullException(nameof(notifySubscriptions));
        }

        /// <summary>
        /// Creates a DOM helper connected to this simulated DataMiner System.
        /// </summary>
        public DomHelper CreateHelper(string moduleId)
        {
            ValidateModuleId(moduleId);
            return new DomHelper(sendMessages, moduleId);
        }

        /// <summary>
        /// Creates an independent DOM cache connected to this simulated DataMiner System.
        /// </summary>
        public global::Skyline.DataMiner.Utils.DOM.DomCache CreateCache(string moduleId)
        {
            ValidateModuleId(moduleId);
            return new global::Skyline.DataMiner.Utils.DOM.DomCache(sendMessages, moduleId);
        }

        /// <summary>
        /// Replaces the DOM instances stored for a module.
        /// </summary>
        public void SetInstances(string moduleId, IEnumerable<DomInstance> instances)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(instances, nameof(instances));
            GetHandler(moduleId).SetInstances(values);
        }

        /// <summary>
        /// Replaces the DOM definitions stored for a module.
        /// </summary>
        public void SetDefinitions(string moduleId, IEnumerable<DomDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetHandler(moduleId).SetDefinitions(values);
        }

        /// <summary>
        /// Replaces the section definitions stored for a module.
        /// </summary>
        public void SetSectionDefinitions(string moduleId, IEnumerable<SectionDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetHandler(moduleId).SetSectionDefinitions(values);
        }

        /// <summary>
        /// Replaces the behavior definitions stored for a module.
        /// </summary>
        public void SetBehaviorDefinitions(string moduleId, IEnumerable<DomBehaviorDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetHandler(moduleId).SetBehaviorDefinitions(values);
        }

        /// <summary>
        /// Handles DOM SLNet messages sent through the shared connection mock.
        /// </summary>
        public DMSMessage[] HandleMessages(DMSMessage[] messages)
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
                    throw new ArgumentException("A message collection cannot contain null messages.", nameof(messages));
                }

                if (!TryGetModuleId(message, out var moduleId))
                {
                    throw new NotSupportedException($"Message type '{message.GetType().FullName}' is not supported by the DOM mock.");
                }

                var handler = GetHandler(moduleId);
                if (!handler.TryHandleMessage(message, out var response))
                {
                    throw new NotSupportedException($"Message type '{message.GetType().FullName}' is not supported by the DOM mock.");
                }

                if (response != null)
                {
                    responses.Add(response);
                }

                NotifyInstanceChange(message, moduleId);
            }

            return responses.ToArray();
        }

        private DomSLNetMessageHandler GetHandler(string moduleId)
        {
            return moduleHandlers.GetOrAdd(moduleId, _ => new DomSLNetMessageHandler());
        }

        private void NotifyInstanceChange(DMSMessage message, string moduleId)
        {
            var change = new DomInstancesChangedEventMessage(-1, moduleId);

            switch (message)
            {
                case ManagerStoreCreateRequest<DomInstance> request:
                    change.Created.Add(request.Object);
                    break;
                case ManagerStoreUpdateRequest<DomInstance> request:
                    change.Updated.Add(request.Object);
                    break;
                case ManagerStoreDeleteRequest<DomInstance> request:
                    change.Deleted.Add(request.Object);
                    break;
                default:
                    return;
            }

            notifySubscriptions(change, filters => ApplySubscriptionFilters(change, filters));
        }

        private static DomInstancesChangedEventMessage ApplySubscriptionFilters(
            DomInstancesChangedEventMessage change,
            IReadOnlyCollection<SubscriptionFilter> filters)
        {
            var moduleMatches = false;
            var created = change.Created.ToList();
            var updated = change.Updated.ToList();
            var deleted = change.Deleted.ToList();

            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case ModuleEventSubscriptionFilter<DomInstancesChangedEventMessage> moduleFilter:
                        moduleMatches |= moduleFilter.IsMatch(change);
                        break;
                    case SubscriptionFilter<DomInstancesChangedEventMessage, DomInstance> instanceFilter:
                        var matchesInstance = instanceFilter.Filter.getLambda();
                        created.RemoveAll(instance => !matchesInstance(instance));
                        updated.RemoveAll(instance => !matchesInstance(instance));
                        deleted.RemoveAll(instance => !matchesInstance(instance));
                        break;
                }
            }

            if (!moduleMatches || (created.Count == 0 && updated.Count == 0 && deleted.Count == 0))
            {
                return null;
            }

            var filteredChange = new DomInstancesChangedEventMessage(-1, change.ModuleId);
            filteredChange.Created.AddRange(created);
            filteredChange.Updated.AddRange(updated);
            filteredChange.Deleted.AddRange(deleted);
            return filteredChange;
        }

        private static bool TryGetModuleId(DMSMessage message, out string moduleId)
        {
            switch (message)
            {
                case ManagerStoreReadRequest<DomDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreCreateRequest<DomDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreUpdateRequest<DomDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreDeleteRequest<DomDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreReadRequest<SectionDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreCreateRequest<SectionDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreUpdateRequest<SectionDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreDeleteRequest<SectionDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreReadRequest<DomInstance> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreCreateRequest<DomInstance> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreUpdateRequest<DomInstance> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreDeleteRequest<DomInstance> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreReadRequest<DomBehaviorDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreCreateRequest<DomBehaviorDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreUpdateRequest<DomBehaviorDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                case ManagerStoreDeleteRequest<DomBehaviorDefinition> request:
                    moduleId = request.ModuleId;
                    return true;
                default:
                    moduleId = null;
                    return false;
            }
        }

        private static void ValidateModuleId(string moduleId)
        {
            if (String.IsNullOrWhiteSpace(moduleId))
            {
                throw new ArgumentException("The DOM module ID cannot be empty or white space.", nameof(moduleId));
            }
        }

        private static T[] Materialize<T>(IEnumerable<T> values, string parameterName)
        {
            if (values == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return values.ToArray();
        }
    }
}
