using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System.Collections.Generic;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    internal sealed class Cache
    {
        private readonly IConnectionMock connectionMock = new IConnectionMock();
        private IDmsMock dmsMock;
        private readonly Dictionary<int, IDmaMock> dmaMocksById = new Dictionary<int, IDmaMock>();
        private readonly Dictionary<string, IDmaMock> dmaMocksByName = new Dictionary<string, IDmaMock>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DmsElementId, IDmsElementMock> elementMocksById = new Dictionary<DmsElementId, IDmsElementMock>();
        private readonly Dictionary<string, IDmsElementMock> elementMocksByName = new Dictionary<string, IDmsElementMock>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<DmsServiceId, IDmsServiceMock> serviceMocksById = new Dictionary<DmsServiceId, IDmsServiceMock>();
        private readonly Dictionary<string, IDmsServiceMock> serviceMocksByName = new Dictionary<string, IDmsServiceMock>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, IDmsViewMock> viewMocksById = new Dictionary<int, IDmsViewMock>();
        private readonly Dictionary<string, IDmsViewMock> viewMocksByName = new Dictionary<string, IDmsViewMock>(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Dictionary<string, IDmsProtocolMock>> protocolMocksByName = new Dictionary<string, Dictionary<string, IDmsProtocolMock>>(StringComparer.OrdinalIgnoreCase);

        internal void AddDms(IDmsMock dmsMock)
        {
            if (this.dmsMock != null)
            {
                throw new InvalidOperationException("A DataMiner System mock is already present in the cache.");
            }

            this.dmsMock = dmsMock;
        }

        internal IDmsMock GetDms()
        {
            return dmsMock;
        }

        internal IConnectionMock GetConnection()
        {
            return connectionMock;
        }

        internal void AddProtocol(IDmsProtocolMock protocolMock)
        {
            if (!protocolMocksByName.TryGetValue(protocolMock.Name, out var versions))
            {
                versions = new Dictionary<string, IDmsProtocolMock>(StringComparer.OrdinalIgnoreCase);
                protocolMocksByName.Add(protocolMock.Name, versions);
            }

            if (versions.ContainsKey(protocolMock.ReferencedVersion))
            {
                throw new ArgumentException("A protocol with the specified name and version already exists.", nameof(protocolMock));
            }

            versions.Add(protocolMock.ReferencedVersion, protocolMock);
        }

        internal IDmsProtocolMock GetProtocol(string name, string referencedVersion)
        {
            if (!protocolMocksByName.TryGetValue(name, out var versions))
            {
                return null;
            }

            versions.TryGetValue(referencedVersion, out var protocolMock);
            return protocolMock;
        }

        internal IDmsProtocolMock GetProtocol(string name)
        {
            if (!protocolMocksByName.TryGetValue(name, out var versions))
            {
                return null;
            }

            if (versions.Count != 1)
            {
                throw new InvalidOperationException($"More than one version of protocol '{name}' is available. Specify a version.");
            }

            return versions.Values.Single();
        }

        internal ICollection<IDmsProtocolMock> GetProtocols()
        {
            return protocolMocksByName.Values.SelectMany(versions => versions.Values).ToList();
        }
        internal void AddDma(IDmaMock dmaMock)
        {
            if (dmaMocksById.ContainsKey(dmaMock.Object.Id))
            {
                throw new ArgumentException("A DataMiner Agent with the specified ID already exists.", nameof(dmaMock));
            }

            if (dmaMocksByName.ContainsKey(dmaMock.Object.Name))
            {
                throw new ArgumentException("A DataMiner Agent with the specified name already exists.", nameof(dmaMock));
            }

            dmaMocksById.Add(dmaMock.Object.Id, dmaMock);
            dmaMocksByName.Add(dmaMock.Object.Name, dmaMock);
        }
        internal void AddView(IDmsViewMock viewMock)
        {
            if (viewMocksById.ContainsKey(viewMock.Object.Id))
            {
                throw new ArgumentException("A view with the specified ID already exists.", nameof(viewMock));
            }

            if (viewMocksByName.ContainsKey(viewMock.Object.Name))
            {
                throw new ArgumentException("A view with the specified name already exists.", nameof(viewMock));
            }

            viewMocksById.Add(viewMock.Object.Id, viewMock);
            viewMocksByName.Add(viewMock.Object.Name, viewMock);
        }

        internal IDmsViewMock GetView(int viewId)
        {
            viewMocksById.TryGetValue(viewId, out var viewMock);

            return viewMock;
        }

        internal IDmsViewMock GetView(string name)
        {
            viewMocksByName.TryGetValue(name, out var viewMock);

            return viewMock;
        }

        internal ICollection<IDmsViewMock> GetViews()
        {
            return viewMocksById.Values.ToList();
        }

        internal void UpdateViewName(int viewId, string oldName, string newName)
        {
            var viewMock = GetView(viewId);

            if (viewMock == null)
            {
                return;
            }

            if (viewMocksByName.TryGetValue(newName, out var existingViewMock) && !ReferenceEquals(existingViewMock, viewMock))
            {
                throw new ArgumentException("A view with the specified name already exists.", nameof(newName));
            }

            viewMocksByName.Remove(oldName);
            viewMocksByName.Add(newName, viewMock);
        }

        internal bool RemoveView(int viewId)
        {
            var viewMock = GetView(viewId);

            if (viewMock == null)
            {
                return false;
            }

            foreach (var elementMock in elementMocksById.Values)
            {
                elementMock.ViewIds.Remove(viewId);
            }

            foreach (var serviceMock in serviceMocksById.Values)
            {
                serviceMock.ViewIds.Remove(viewId);
            }

            if (viewMock.ParentViewId.HasValue)
            {
                GetView(viewMock.ParentViewId.Value)?.ChildViewIds.Remove(viewId);
            }

            foreach (var childViewId in viewMock.ChildViewIds.ToList())
            {
                var childViewMock = GetView(childViewId);

                if (childViewMock != null)
                {
                    childViewMock.ParentViewId = null;
                }
            }

            viewMocksByName.Remove(viewMock.Object.Name);

            return viewMocksById.Remove(viewId);
        }
        internal IDmaMock GetDma(int agentId)
        {
            dmaMocksById.TryGetValue(agentId, out var dmaMock);

            return dmaMock;
        }

        internal IDmaMock GetDma(string name)
        {
            dmaMocksByName.TryGetValue(name, out var dmaMock);

            return dmaMock;
        }
        internal bool RemoveDma(int agentId)
        {
            var dmaMock = GetDma(agentId);

            if (dmaMock == null)
            {
                return false;
            }

            foreach (var elementMock in GetElements(agentId).ToList())
            {
                RemoveElement(agentId, elementMock.Object.Id);
            }

            foreach (var serviceMock in GetServices(agentId).ToList())
            {
                RemoveService(agentId, serviceMock.Object.Id);
            }

            dmaMocksByName.Remove(dmaMock.Object.Name);

            return dmaMocksById.Remove(agentId);
        }

        internal void AddElement(IDmsElementMock elementMock)
        {
            var elementId = elementMock.Object.DmsElementId;

            if (elementMocksById.ContainsKey(elementId))
            {
                throw new ArgumentException("An element with the specified ID already exists.", nameof(elementMock));
            }

            if (elementMocksByName.ContainsKey(elementMock.Object.Name))
            {
                throw new ArgumentException("An element with the specified name already exists.", nameof(elementMock));
            }

            elementMocksById.Add(elementId, elementMock);
            elementMocksByName.Add(elementMock.Object.Name, elementMock);

            var dmaMock = GetDma(elementId.AgentId);

            if (dmaMock != null && !dmaMock.ElementIds.Contains(elementId.ElementId))
            {
                dmaMock.ElementIds.Add(elementId.ElementId);
            }
        }

        internal IDmsElementMock GetElement(int agentId, int elementId)
        {
            elementMocksById.TryGetValue(new DmsElementId(agentId, elementId), out var elementMock);

            return elementMock;
        }

        internal IDmsElementMock GetElement(int agentId, string name)
        {
            var elementMock = GetElement(name);

            return elementMock != null && elementMock.Object.AgentId == agentId ? elementMock : null;
        }

        internal IDmsElementMock GetElement(string name)
        {
            elementMocksByName.TryGetValue(name, out var elementMock);

            return elementMock;
        }

        internal void UpdateElementName(int agentId, int elementId, string oldName, string newName)
        {
            var elementMock = GetElement(agentId, elementId);

            if (elementMock == null)
            {
                return;
            }

            if (elementMocksByName.TryGetValue(newName, out var existingElementMock) && !ReferenceEquals(existingElementMock, elementMock))
            {
                throw new ArgumentException("An element with the specified name already exists.", nameof(newName));
            }

            elementMocksByName.Remove(oldName);
            elementMocksByName.Add(newName, elementMock);
        }

        internal bool RemoveElement(int agentId, int elementId)
        {
            var elementMock = GetElement(agentId, elementId);

            if (elementMock == null)
            {
                return false;
            }

            foreach (var viewMock in viewMocksById.Values)
            {
                viewMock.ElementIds.RemoveAll(dmsElementId => dmsElementId.AgentId == agentId && dmsElementId.ElementId == elementId);
            }

            GetDma(agentId)?.ElementIds.Remove(elementId);
            elementMocksByName.Remove(elementMock.Object.Name);

            return elementMocksById.Remove(new DmsElementId(agentId, elementId));
        }

        internal ICollection<IDmsElementMock> GetElements(int agentId)
        {
            return elementMocksById.Values.Where(element => element.Object.AgentId == agentId).ToList();
        }

        internal ICollection<IDmaMock> GetDmas()
        {
            return dmaMocksById.Values.ToList();
        }

        internal ICollection<IDmsElementMock> GetElements()
        {
            return elementMocksById.Values.ToList();
        }

        internal void AddService(IDmsServiceMock serviceMock)
        {
            var serviceId = serviceMock.Object.DmsServiceId;

            if (serviceMocksById.ContainsKey(serviceId))
            {
                throw new ArgumentException("A service with the specified ID already exists.", nameof(serviceMock));
            }

            if (serviceMocksByName.ContainsKey(serviceMock.Object.Name))
            {
                throw new ArgumentException("A service with the specified name already exists.", nameof(serviceMock));
            }

            serviceMocksById.Add(serviceId, serviceMock);
            serviceMocksByName.Add(serviceMock.Object.Name, serviceMock);

            var dmaMock = GetDma(serviceId.AgentId);

            if (dmaMock != null && !dmaMock.ServiceIds.Contains(serviceId.ServiceId))
            {
                dmaMock.ServiceIds.Add(serviceId.ServiceId);
            }
        }

        internal IDmsServiceMock GetService(int agentId, int serviceId)
        {
            serviceMocksById.TryGetValue(new DmsServiceId(agentId, serviceId), out var serviceMock);

            return serviceMock;
        }

        internal IDmsServiceMock GetService(int agentId, string name)
        {
            var serviceMock = GetService(name);

            return serviceMock != null && serviceMock.Object.AgentId == agentId ? serviceMock : null;
        }

        internal IDmsServiceMock GetService(string name)
        {
            serviceMocksByName.TryGetValue(name, out var serviceMock);

            return serviceMock;
        }

        internal void UpdateServiceName(int agentId, int serviceId, string oldName, string newName)
        {
            var serviceMock = GetService(agentId, serviceId);

            if (serviceMock == null)
            {
                return;
            }

            if (serviceMocksByName.TryGetValue(newName, out var existingServiceMock) && !ReferenceEquals(existingServiceMock, serviceMock))
            {
                throw new ArgumentException("A service with the specified name already exists.", nameof(newName));
            }

            serviceMocksByName.Remove(oldName);
            serviceMocksByName.Add(newName, serviceMock);
        }

        internal ICollection<IDmsServiceMock> GetServices(int agentId)
        {
            return serviceMocksById.Values.Where(service => service.Object.AgentId == agentId).ToList();
        }

        internal ICollection<IDmsServiceMock> GetServices()
        {
            return serviceMocksById.Values.ToList();
        }

        internal bool RemoveService(int agentId, int serviceId)
        {
            var serviceMock = GetService(agentId, serviceId);

            if (serviceMock == null)
            {
                return false;
            }

            foreach (var viewMock in viewMocksById.Values)
            {
                viewMock.ServiceIds.RemoveAll(dmsServiceId => dmsServiceId.AgentId == agentId && dmsServiceId.ServiceId == serviceId);
            }

            GetDma(agentId)?.ServiceIds.Remove(serviceId);
            serviceMocksByName.Remove(serviceMock.Object.Name);

            return serviceMocksById.Remove(new DmsServiceId(agentId, serviceId));
        }
    }
}
