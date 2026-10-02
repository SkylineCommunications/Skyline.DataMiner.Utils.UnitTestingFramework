namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Builder class for constructing a <see cref="IDmaMock"/> with its elements.
    /// </summary>
    public class IDmaBuilder
    {
        private readonly int id;
        private readonly string name;
        private readonly List<Action<IDmsMock, IDmaMock>> elements = new List<Action<IDmsMock, IDmaMock>>();

        internal IDmaBuilder(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        /// <summary>
        /// Adds an element to the DMA with the specified properties and optional configuration.
        /// </summary>
        /// <param name="id">The ID of the element.</param>
        /// <param name="name">The name of the element.</param>
        /// <param name="protocolName">The name of the protocol.</param>
        /// <param name="protocolVersion">The version of the protocol.</param>
        /// <param name="configure">An optional action to configure the element.</param>
        /// <returns>The current <see cref="IDmaBuilder"/> instance.</returns>
        public IDmaBuilder WithElement(int id, string name, string protocolName, string protocolVersion = IDmsProtocolMock.DefaultVersion, Action<IDmsElementBuilder> configure = null)
        {
            elements.Add((dmsMock, dmaMock) =>
            {
                var elementBuilder = new IDmsElementBuilder(id, name, protocolName, protocolVersion);
                configure?.Invoke(elementBuilder);
                elementBuilder.Build(dmsMock, dmaMock);
            });
            return this;
        }

        internal void Build(IDmsMock dmsMock)
        {
            var dmaMock = dmsMock.CreateAgent(id, name ?? $"Agent {id}");

            foreach (var element in elements)
            {
                element(dmsMock, dmaMock);
            }
        }
    }
}
