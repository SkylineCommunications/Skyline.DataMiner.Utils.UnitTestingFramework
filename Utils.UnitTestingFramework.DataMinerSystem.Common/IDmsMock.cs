using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System.Collections.Generic;
    using Moq;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Properties;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.DOM;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;

    /// <summary>
    /// A pre-arranged mock of <see cref="IDms"/>.
    /// </summary>
    public class IDmsMock : Mock<IDms>
    {
        private readonly Cache cache;

        /// <summary>
        /// Gets the SLNet connection mock shared by this DataMiner System and all its elements.
        /// </summary>
        public IConnectionMock Connection { get; }

        /// <summary>
        /// Gets the communication interface.
        /// </summary>
        public ICommunication Communication { get; set; } = new Mock<ICommunication>().Object;

        /// <summary>
        /// Gets the DOM state that belongs to this DataMiner System mock.
        /// </summary>
        public DomSystemMock Dom { get; }
        public IPropertyDefinitionCollection<IDmsElementPropertyDefinition> ElementPropertyDefinitions { get; set; } = CreateEmptyPropertyDefinitionCollection<IDmsElementPropertyDefinition>();
        public IPropertyDefinitionCollection<IDmsServicePropertyDefinition> ServicePropertyDefinitions { get; set; } = CreateEmptyPropertyDefinitionCollection<IDmsServicePropertyDefinition>();
        public IPropertyDefinitionCollection<IDmsViewPropertyDefinition> ViewPropertyDefinitions { get; set; } = CreateEmptyPropertyDefinitionCollection<IDmsViewPropertyDefinition>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IDmsMock"/> class.
        /// </summary>
        public IDmsMock()
        {
            cache = new Cache();
            cache.AddDms(this);
            Connection = cache.GetConnection();
            Dom = new DomSystemMock(
                Connection.HandleMessages,
                (message, applyFilters) => Connection.NotifySubscriptions(message, applyFilters));
            Connection.SetMessageHandler(Dom.HandleMessages);
            Setup(dms => dms.Communication).Returns(() => Communication);
            Setup(dms => dms.ElementPropertyDefinitions).Returns(() => ElementPropertyDefinitions);
            Setup(dms => dms.ServicePropertyDefinitions).Returns(() => ServicePropertyDefinitions);
            Setup(dms => dms.ViewPropertyDefinitions).Returns(() => ViewPropertyDefinitions);
            Setup(dms => dms.AgentExists(It.IsAny<int>())).Returns((int agentId) => cache.GetDma(agentId) != null);
            Setup(dms => dms.GetAgent(It.IsAny<int>())).Returns((int agentId) =>
            {
                var dmaMock = cache.GetDma(agentId);

                if (dmaMock == null)
                {
                    throw new AgentNotFoundException(agentId);
                }

                return dmaMock.Object;
            });
            Setup(dms => dms.GetAgentReference(It.IsAny<int>())).Returns((int agentId) =>
            {
                if (agentId < 0)
                {
                    throw new ArgumentException("The DataMiner Agent ID cannot be negative.", nameof(agentId));
                }

                var dmaMock = cache.GetDma(agentId);

                if (dmaMock != null)
                {
                    return dmaMock.Object;
                }

                var referenceMock = new Mock<IDma>();
                referenceMock.Setup(dma => dma.Id).Returns(agentId);
                referenceMock.Setup(dma => dma.Dms).Returns(Object);

                return referenceMock.Object;
            });
            Setup(dms => dms.GetAgents()).Returns(() => cache.GetDmas().Select(dma => dma.Object).ToList());
            Setup(dms => dms.ElementExists(It.IsAny<DmsElementId>())).Returns((DmsElementId elementId) =>
            {
                var elementMock = cache.GetElement(elementId.AgentId, elementId.ElementId);

                return elementMock != null && elementMock.Object.State != ElementState.Deleted;
            });
            Setup(dms => dms.ElementExists(It.IsAny<string>())).Returns((string elementName) =>
            {
                ValidateElementName(elementName);

                var elementMock = cache.GetElement(elementName);

                return elementMock != null && elementMock.Object.State != ElementState.Deleted;
            });
            Setup(dms => dms.GetElement(It.IsAny<DmsElementId>())).Returns((DmsElementId elementId) =>
            {
                ValidateElementId(elementId);

                var elementMock = cache.GetElement(elementId.AgentId, elementId.ElementId);

                if (elementMock == null || elementMock.Object.State == ElementState.Deleted)
                {
                    throw new ElementNotFoundException(elementId);
                }

                return elementMock.Object;
            });
            Setup(dms => dms.GetElement(It.IsAny<string>())).Returns((string elementName) =>
            {
                ValidateElementName(elementName);

                var elementMock = cache.GetElement(elementName);

                if (elementMock == null || elementMock.Object.State == ElementState.Deleted)
                {
                    throw new ElementNotFoundException(elementName);
                }

                return elementMock.Object;
            });
            Setup(dms => dms.GetElementReference(It.IsAny<DmsElementId>())).Returns((DmsElementId elementId) =>
            {
                var elementMock = cache.GetElement(elementId.AgentId, elementId.ElementId);

                if (elementMock != null)
                {
                    return elementMock.Object;
                }

                var referenceMock = new Mock<IDmsElement>();
                referenceMock.Setup(element => element.AgentId).Returns(elementId.AgentId);
                referenceMock.Setup(element => element.Id).Returns(elementId.ElementId);
                referenceMock.Setup(element => element.DmsElementId).Returns(elementId);
                referenceMock.Setup(element => element.Host).Returns(cache.GetDma(elementId.AgentId)?.Object);

                return referenceMock.Object;
            });
            Setup(dms => dms.GetElements()).Returns(() => cache.GetElements().Where(element => element.Object.State != ElementState.Deleted).Select(element => element.Object).ToList());
            Setup(dms => dms.ServiceExists(It.IsAny<DmsServiceId>())).Returns((DmsServiceId serviceId) =>
            {
                ValidateServiceId(serviceId);
                return cache.GetService(serviceId.AgentId, serviceId.ServiceId) != null;
            });
            Setup(dms => dms.ServiceExists(It.IsAny<string>())).Returns((string serviceName) =>
            {
                ValidateServiceName(serviceName);
                return cache.GetService(serviceName) != null;
            });
            Setup(dms => dms.GetService(It.IsAny<DmsServiceId>())).Returns((DmsServiceId serviceId) =>
            {
                ValidateServiceId(serviceId);
                var serviceMock = cache.GetService(serviceId.AgentId, serviceId.ServiceId);
                if (serviceMock == null)
                {
                    throw new ServiceNotFoundException(serviceId);
                }

                return serviceMock.Object;
            });
            Setup(dms => dms.GetService(It.IsAny<string>())).Returns((string serviceName) =>
            {
                ValidateServiceName(serviceName);
                var serviceMock = cache.GetService(serviceName);
                if (serviceMock == null)
                {
                    throw new ServiceNotFoundException(serviceName);
                }

                return serviceMock.Object;
            });
            Setup(dms => dms.GetServices()).Returns(() => cache.GetServices().Select(service => service.Object).ToList());
            Setup(dms => dms.ViewExists(It.IsAny<int>())).Returns((int viewId) =>
            {
                ValidateViewId(viewId);

                return cache.GetView(viewId) != null;
            });
            Setup(dms => dms.ViewExists(It.IsAny<string>())).Returns((string viewName) =>
            {
                ValidateViewName(viewName);

                return cache.GetView(viewName) != null;
            });
            Setup(dms => dms.CreateView(It.IsAny<ViewConfiguration>())).Returns((ViewConfiguration configuration) => CreateView(configuration));
            Setup(dms => dms.GetView(It.IsAny<int>())).Returns((int viewId) =>
            {
                ValidateViewId(viewId);

                var viewMock = cache.GetView(viewId);

                if (viewMock == null)
                {
                    throw new ViewNotFoundException(viewId);
                }

                return viewMock.Object;
            });
            Setup(dms => dms.GetView(It.IsAny<string>())).Returns((string viewName) =>
            {
                ValidateViewName(viewName);

                var viewMock = cache.GetView(viewName);

                if (viewMock == null)
                {
                    throw new ViewNotFoundException(viewName);
                }

                return viewMock.Object;
            });
            Setup(dms => dms.GetViewReference(It.IsAny<int>())).Returns((int viewId) =>
            {
                var viewMock = cache.GetView(viewId);

                if (viewMock != null)
                {
                    return viewMock.Object;
                }

                var referenceMock = new Mock<IDmsView>();
                referenceMock.Setup(view => view.Id).Returns(viewId);
                referenceMock.Setup(view => view.Dms).Returns(Object);

                return referenceMock.Object;
            });
            Setup(dms => dms.GetViews()).Returns(() => cache.GetViews().Select(view => view.Object).ToList());

            Setup(dms => dms.GetProtocols()).Returns(() => cache.GetProtocols().Select(protocol => protocol.Object).ToList());
            Setup(dms => dms.ProtocolExists(It.IsAny<string>(), It.IsAny<string>())).Returns((string protocolName, string protocolVersion) =>
            {
                ValidateProtocolIdentifier(protocolName, nameof(protocolName));
                ValidateProtocolIdentifier(protocolVersion, nameof(protocolVersion));
                return cache.GetProtocol(protocolName, protocolVersion) != null;
            });
            Setup(dms => dms.GetProtocol(It.IsAny<string>(), It.IsAny<string>())).Returns((string protocolName, string protocolVersion) =>
            {
                ValidateProtocolIdentifier(protocolName, nameof(protocolName));
                ValidateProtocolIdentifier(protocolVersion, nameof(protocolVersion));
                var protocolMock = cache.GetProtocol(protocolName, protocolVersion);

                if (protocolMock == null)
                {
                    throw new ProtocolNotFoundException(protocolName, protocolVersion);
                }

                return protocolMock.Object;
            });
        }

        public IDmsProtocolMock AddProtocol(string pathToProtocolXml)
        {
            var protocolModel = ProtocolModelBuilder.Build(pathToProtocolXml);
            var protocolMock = new IDmsProtocolMock(protocolModel, pathToProtocolXml);
            cache.AddProtocol(protocolMock);
            return protocolMock;
        }

        internal IDmsProtocolMock GetProtocolMock(string name, string version = null)
        {
            return version == null ? cache.GetProtocol(name) : cache.GetProtocol(name, version);
        }

        /// <summary>
        /// Gets an element mock from this DataMiner System by name.
        /// </summary>
        /// <param name="name">The element name.</param>
        /// <returns>The element mock, or <see langword="null"/> if it does not exist.</returns>
        public IDmsElementMock GetElementMock(string name)
        {
            return cache.GetElement(name);
        }

        public IDmsViewMock GetViewMock(int viewId)
        {
            return cache.GetView(viewId);
        }

        /// <summary>
        /// Creates a DataMiner Agent mock that belongs to this DataMiner System.
        /// </summary>
        /// <param name="agentId">The DataMiner Agent ID.</param>
        /// <param name="name">The DataMiner Agent name.</param>
        /// <returns>The created DataMiner Agent mock.</returns>
        public IDmaMock CreateAgent(int agentId, string name = "Agent")
        {
            var dmaMock = new IDmaMock(cache, agentId, name);

            cache.AddDma(dmaMock);

            return dmaMock;
        }

        /// <summary>
        /// Creates a view mock that belongs to this DataMiner System.
        /// </summary>
        /// <param name="viewId">The view ID.</param>
        /// <param name="name">The view name.</param>
        /// <returns>The created view mock.</returns>
        public IDmsViewMock CreateView(int viewId, string name = "View")
        {
            ValidateViewId(viewId);

            if (cache.GetView(viewId) != null)
            {
                throw new ArgumentException("A view with the specified ID already exists.", nameof(viewId));
            }

            var viewMock = new IDmsViewMock(cache, viewId, name);

            cache.AddView(viewMock);

            return viewMock;
        }

        private int CreateView(ViewConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var parentMock = cache.GetView(configuration.Parent.Id);

            if (parentMock == null || !ReferenceEquals(parentMock.Object, configuration.Parent))
            {
                throw new IncorrectDataException("The parent view does not belong to this DataMiner System.");
            }

            if (cache.GetView(configuration.Name) != null)
            {
                throw new IncorrectDataException("A view with the specified name already exists.");
            }

            var viewId = GetNextViewId();
            var viewMock = CreateView(viewId, configuration.Name);
            viewMock.Parent = configuration.Parent;

            return viewId;
        }

        private int GetNextViewId()
        {
            var viewIds = cache.GetViews().Select(view => view.Object.Id).Where(viewId => viewId > 0).ToList();

            return viewIds.Count == 0 ? 1 : viewIds.Max() + 1;
        }

        private static void ValidateProtocolIdentifier(string value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (String.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The protocol name or version cannot be empty or white space.", parameterName);
            }
        }
        private static void ValidateElementId(DmsElementId elementId)
        {
            if (elementId.AgentId < 1 || elementId.ElementId < 1)
            {
                throw new ArgumentException("The DataMiner Agent ID and element ID must be positive.", nameof(elementId));
            }
        }

        private static void ValidateElementName(string elementName)
        {
            if (elementName == null)
            {
                throw new ArgumentNullException(nameof(elementName));
            }

            if (String.IsNullOrWhiteSpace(elementName))
            {
                throw new ArgumentException("The element name cannot be empty or white space.", nameof(elementName));
            }
        }

        private static void ValidateViewName(string viewName)
        {
            if (viewName == null)
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (String.IsNullOrWhiteSpace(viewName))
            {
                throw new ArgumentException("The view name cannot be empty or white space.", nameof(viewName));
            }
        }

        private static void ValidateServiceId(DmsServiceId serviceId)
        {
            if (serviceId.AgentId < 1 || serviceId.ServiceId < 1)
            {
                throw new ArgumentException("The DataMiner Agent ID and service ID must be positive.", nameof(serviceId));
            }
        }

        private static void ValidateServiceName(string serviceName)
        {
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (String.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException("The service name cannot be empty or white space.", nameof(serviceName));
            }
        }

        private static void ValidateViewId(int viewId)
        {
            if (viewId < -1 || viewId == 0)
            {
                throw new ArgumentException("The view ID must be -1 or a positive number.", nameof(viewId));
            }
        }

        private static IPropertyDefinitionCollection<T> CreateEmptyPropertyDefinitionCollection<T>() where T : IDmsPropertyDefinition
        {
            var collectionMock = new Mock<IPropertyDefinitionCollection<T>>();

            collectionMock.Setup(collection => collection.Count).Returns(0);
            collectionMock.Setup(collection => collection.GetEnumerator()).Returns(() => Enumerable.Empty<T>().GetEnumerator());

            return collectionMock.Object;
        }
    }
}
