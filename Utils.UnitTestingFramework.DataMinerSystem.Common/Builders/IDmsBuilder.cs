namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Builder class for constructing a <see cref="IDmsMock"/> with its protocols, views, and DMAs.
    /// </summary>
    public class IDmsBuilder
    {
        private readonly List<string> protocolPaths = new List<string>();
        private readonly List<Action<IDmsMock>> protocols = new List<Action<IDmsMock>>();
        private readonly List<Action<IDmsMock>> views = new List<Action<IDmsMock>>();
        private readonly List<Action<IDmsMock>> dmas = new List<Action<IDmsMock>>();

        /// <summary>
        /// Adds a protocol to the DMS mock by specifying the path to its XML definition.
        /// </summary>
        /// <param name="pathToProtocolXml">The path to the protocol's XML definition.</param>
        /// <returns>The current <see cref="IDmsBuilder"/> instance.</returns>
        public IDmsBuilder WithProtocol(string pathToProtocolXml)
        {
            protocolPaths.Add(pathToProtocolXml);
            return this;
        }

        /// <summary>
        /// Adds a protocol to the DMS mock by specifying its name, configuration action, and optional version.
        /// </summary>
        /// <param name="name">The name of the protocol.</param>
        /// <param name="configure">An action to configure the protocol.</param>
        /// <param name="version">The version of the protocol.</param>
        /// <returns>The current <see cref="IDmsBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="configure"/> is null.</exception>
        public IDmsBuilder WithProtocol(string name, Action<IDmsProtocolMockBuilder> configure, string version = IDmsProtocolMock.DefaultVersion)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            protocols.Add(dmsMock =>
            {
                var protocolBuilder = new IDmsProtocolMockBuilder(name, version);
                configure(protocolBuilder);
                var protocolMock = protocolBuilder.Build();
                dmsMock.AddProtocol(protocolMock);
            });

            return this;
        }

        /// <summary>
        /// Adds a view to the DMS mock by specifying its ID and optional name (defaults to "View {viewId}").
        /// </summary>
        /// <param name="viewId">The ID of the view.</param>
        /// <param name="name">The name of the view.</param>
        /// <returns>The current <see cref="IDmsBuilder"/> instance.</returns>
        public IDmsBuilder WithView(int viewId, string name = null)
        {
            views.Add(dmsMock => dmsMock.CreateView(viewId, name ?? $"View {viewId}"));
            return this;
        }

        /// <summary>
        /// Adds a DMA to the DMS mock by specifying its ID, optional configuration action, and optional name (defaults to "DMA {id}").
        /// </summary>
        /// <param name="id">The ID of the DMA.</param>
        /// <param name="configure">An optional action to configure the DMA.</param>
        /// <param name="name">The name of the DMA.</param>
        /// <returns>The current <see cref="IDmsBuilder"/> instance.</returns>
        public IDmsBuilder WithDma(int id, Action<IDmaBuilder> configure = null, string name = null)
        {
            dmas.Add(dmsMock =>
            {
                var dmaBuilder = new IDmaBuilder(id, name ?? $"DMA {id}");
                configure?.Invoke(dmaBuilder);
                dmaBuilder.Build(dmsMock);
            });
            return this;
        }

        /// <summary>
        /// Builds the DMS mock with the configured protocols, views, and DMAs.
        /// </summary>
        /// <returns>The constructed <see cref="IDmsMock"/> instance.</returns>
        public IDmsMock Build()
        {
            var dmsMock = new IDmsMock();

            foreach (var path in protocolPaths)
            {
                dmsMock.AddProtocol(path);
            }

            foreach (var protocol in protocols)
            {
                protocol(dmsMock);
            }

            foreach (var view in views)
            {
                view(dmsMock);
            }

            foreach (var dma in dmas)
            {
                dma(dmsMock);
            }

            return dmsMock;
        }
    }
}
