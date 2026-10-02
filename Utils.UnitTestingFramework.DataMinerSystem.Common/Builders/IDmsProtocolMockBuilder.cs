namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Builder class for constructing a <see cref="IDmsProtocolMock"/> with its properties and configuration.
    /// </summary>
    public sealed class IDmsProtocolMockBuilder
    {
        private readonly string name;
        private readonly string version;
        private readonly Dictionary<int, StandaloneParameterDefinition> parameterDefinitions = new Dictionary<int, StandaloneParameterDefinition>();
        private readonly Dictionary<int, TableDefinition> tableDefinitions = new Dictionary<int, TableDefinition>();

        internal IDmsProtocolMockBuilder(string name, string version = IDmsProtocolMock.DefaultVersion)
        {
            this.name = name;
            this.version = version;
        }

        /// <summary>
        /// Adds a parameter definition to the protocol mock being built.
        /// </summary>
        /// <param name="parameterDefinition">The parameter definition to add.</param>
        /// <returns>The current <see cref="IDmsProtocolMockBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parameterDefinition"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if a parameter definition with the same ID has already been added.</exception>
        public IDmsProtocolMockBuilder AddParameterDefinition(StandaloneParameterDefinition parameterDefinition)
        {
            if (parameterDefinition == null)
            {
                throw new ArgumentNullException(nameof(parameterDefinition));
            }

            if (parameterDefinitions.ContainsKey(parameterDefinition.Pid))
            {
                throw new ArgumentException($"A parameter definition with ID '{parameterDefinition.Pid}' has already been added.", nameof(parameterDefinition));
            }

            parameterDefinitions.Add(parameterDefinition.Pid, parameterDefinition);
            return this;
        }

        /// <summary>
        /// Adds a table definition to the protocol mock being built.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <param name="tableDefinition">The table definition to add.</param>
        /// <returns>The current <see cref="IDmsProtocolMockBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tableDefinition"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if a table definition with the same ID has already been added.</exception>
        public IDmsProtocolMockBuilder AddTableDefinition(int tableId, TableDefinition tableDefinition)
        {
            if (tableDefinition == null)
            {
                throw new ArgumentNullException(nameof(tableDefinition));
            }

            if (tableDefinitions.ContainsKey(tableId))
            {
                throw new ArgumentException($"A table definition with ID '{tableId}' has already been added.", nameof(tableId));
            }

            tableDefinitions.Add(tableId, tableDefinition);
            return this;
        }

        internal IDmsProtocolMock Build()
        {
            var protocolMock = new IDmsProtocolMock(name, version);

            foreach (var parameterDefinition in parameterDefinitions.Values)
            {
                protocolMock.AddParameterDefinition(parameterDefinition);
            }

            foreach (var tableDefinition in tableDefinitions)
            {
                protocolMock.AddTableDefinition(tableDefinition.Key, tableDefinition.Value);
            }

            return protocolMock;
        }
    }
}
