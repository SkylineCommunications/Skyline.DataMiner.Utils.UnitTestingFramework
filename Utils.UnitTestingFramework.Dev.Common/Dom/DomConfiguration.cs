namespace Skyline.DataMiner.Utils.UnitTestingFramework.Dev.Common.Dom
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Sections;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Dev.Common;

    /// <summary>
    /// Provides in-memory DOM behavior for a simulated DataMiner System.
    /// </summary>
    public sealed class DomConfiguration
    {
        private readonly Dictionary<string, DomModule> modules = new Dictionary<string, DomModule>(StringComparer.Ordinal);

        private Action<DMSMessage> notifySubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="DomConfiguration"/> class.
        /// </summary>
        internal DomConfiguration(IEnumerable<DomModule> modules = null)
        {
            if (modules != null)
            {
                foreach (var module in modules)
                {
                    this.modules[module.ID] = module;
                }
            }
        }

        /// <summary>
        /// Attaches the DOM configuration to a connection mock, enabling it to handle DOM-related messages.
        /// </summary>
        /// <param name="connectionMock">The connection mock to attach to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionMock"/> is <c>null</c>.</exception>
        public void AttachTo(IConnectionMock connectionMock)
        {
            if (connectionMock == null)
            {
                throw new ArgumentNullException(nameof(connectionMock));
            }

            notifySubscriptions = connectionMock.NotifySubscriptions;

            connectionMock.RegisterMessageHandler<ManagerStoreReadRequest<DomDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreUpdateRequest<DomDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreCreateRequest<DomDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreDeleteRequest<DomDefinition>>(HandleMessage);

            connectionMock.RegisterMessageHandler<ManagerStoreReadRequest<SectionDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreUpdateRequest<SectionDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreCreateRequest<SectionDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreDeleteRequest<SectionDefinition>>(HandleMessage);

            connectionMock.RegisterMessageHandler<ManagerStoreReadRequest<DomBehaviorDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreUpdateRequest<DomBehaviorDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreCreateRequest<DomBehaviorDefinition>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreDeleteRequest<DomBehaviorDefinition>>(HandleMessage);

            connectionMock.RegisterMessageHandler<ManagerStoreReadRequest<DomInstance>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreUpdateRequest<DomInstance>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreCreateRequest<DomInstance>>(HandleMessage);
            connectionMock.RegisterMessageHandler<ManagerStoreDeleteRequest<DomInstance>>(HandleMessage);
        }

        /// <summary>
        /// Replaces the DOM instances stored for a module.
        /// </summary>
        public void SetInstances(string moduleId, IEnumerable<DomInstance> instances)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(instances, nameof(instances));
            GetOrCreateCache(moduleId).SetInstances(values);
        }

        /// <summary>
        /// Replaces the DOM definitions stored for a module.
        /// </summary>
        public void SetDefinitions(string moduleId, IEnumerable<DomDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetOrCreateCache(moduleId).SetDefinitions(values);
        }

        /// <summary>
        /// Replaces the section definitions stored for a module.
        /// </summary>
        public void SetSectionDefinitions(string moduleId, IEnumerable<SectionDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetOrCreateCache(moduleId).SetSectionDefinitions(values);
        }

        /// <summary>
        /// Replaces the behavior definitions stored for a module.
        /// </summary>
        public void SetBehaviorDefinitions(string moduleId, IEnumerable<DomBehaviorDefinition> definitions)
        {
            ValidateModuleId(moduleId);
            var values = Materialize(definitions, nameof(definitions));
            GetOrCreateCache(moduleId).SetBehaviorDefinitions(values);
        }

        internal DomModule GetOrCreateCache(string moduleId)
        {
            ValidateModuleId(moduleId);

            if (!modules.TryGetValue(moduleId, out var cache))
            {
                cache = new DomModule(moduleId);
                modules.Add(moduleId, cache);
            }

            return cache;
        }

        private DMSMessage HandleMessage(DMSMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!TryHandleMessage(message, out var response))
            {
                throw new NotSupportedException($"Unsupported message type {message.GetType()}");
            }

            return response;
        }

        private bool TryHandleMessage(DMSMessage message, out DMSMessage response)
        {
            switch (message)
            {
                case ManagerStoreReadRequest<DomDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    var definitions = request.Query.ExecuteInMemory(cache.Definitions.Values).ToList();
                    response = new ManagerStoreCrudResponse<DomDefinition>(definitions);
                    return true;
                }

                case ManagerStoreCreateRequest<DomDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Definitions[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreUpdateRequest<DomDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Definitions[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreDeleteRequest<DomDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Definitions.Remove(request.Object.ID.Id);
                    response = new ManagerStoreCrudResponse<DomDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreReadRequest<SectionDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    var definitions = request.Query.ExecuteInMemory(cache.SectionDefinitions.Values).ToList();
                    response = new ManagerStoreCrudResponse<SectionDefinition>(definitions);
                    return true;
                }

                case ManagerStoreCreateRequest<SectionDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.SectionDefinitions[request.Object.GetID().Id] = request.Object;
                    response = new ManagerStoreCrudResponse<SectionDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreUpdateRequest<SectionDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.SectionDefinitions[request.Object.GetID().Id] = request.Object;
                    response = new ManagerStoreCrudResponse<SectionDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreDeleteRequest<SectionDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.SectionDefinitions.Remove(request.Object.GetID().Id);
                    response = new ManagerStoreCrudResponse<SectionDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreReadRequest<DomInstance> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    var instances = request.Query.ExecuteInMemory(cache.Instances.Values).ToList();
                    response = new ManagerStoreCrudResponse<DomInstance>(instances);
                    return true;
                }

                case ManagerStoreCreateRequest<DomInstance> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Instances[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomInstance>(request.Object);
                    NotifyInstanceChange(request.ModuleId, request.Object, DomInstanceChangeType.Created);
                    return true;
                }

                case ManagerStoreUpdateRequest<DomInstance> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Instances[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomInstance>(request.Object);
                    NotifyInstanceChange(request.ModuleId, request.Object, DomInstanceChangeType.Updated);
                    return true;
                }

                case ManagerStoreDeleteRequest<DomInstance> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.Instances.Remove(request.Object.ID.Id);
                    response = new ManagerStoreCrudResponse<DomInstance>(request.Object);
                    NotifyInstanceChange(request.ModuleId, request.Object, DomInstanceChangeType.Deleted);
                    return true;
                }

                case ManagerStoreReadRequest<DomBehaviorDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    var definitions = request.Query.ExecuteInMemory(cache.BehaviorDefinitions.Values).ToList();
                    response = new ManagerStoreCrudResponse<DomBehaviorDefinition>(definitions);
                    return true;
                }

                case ManagerStoreCreateRequest<DomBehaviorDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.BehaviorDefinitions[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomBehaviorDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreUpdateRequest<DomBehaviorDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.BehaviorDefinitions[request.Object.ID.Id] = request.Object;
                    response = new ManagerStoreCrudResponse<DomBehaviorDefinition>(request.Object);
                    return true;
                }

                case ManagerStoreDeleteRequest<DomBehaviorDefinition> request:
                {
                    var cache = GetOrCreateCache(request.ModuleId);
                    cache.BehaviorDefinitions.Remove(request.Object.ID.Id);
                    response = new ManagerStoreCrudResponse<DomBehaviorDefinition>(request.Object);
                    return true;
                }

                default:
                    response = null;
                    return false;
            }
        }

        private void NotifyInstanceChange(string moduleId, DomInstance instance, DomInstanceChangeType changeType)
        {
            var message = new DomInstancesChangedEventMessage(-1, moduleId);

            switch (changeType)
            {
                case DomInstanceChangeType.Created:
                    message.Created.Add(instance);
                    break;
                case DomInstanceChangeType.Updated:
                    message.Updated.Add(instance);
                    break;
                case DomInstanceChangeType.Deleted:
                    message.Deleted.Add(instance);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(changeType));
            }

            notifySubscriptions(message);
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

        private enum DomInstanceChangeType
        {
            Created,
            Updated,
            Deleted,
        }
    }
}
