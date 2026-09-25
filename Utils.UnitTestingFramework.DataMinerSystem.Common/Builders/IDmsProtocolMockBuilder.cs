namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    public sealed class IDmsProtocolMockBuilder
    {
        private readonly string name;
        private readonly string version;
        private readonly Dictionary<int, StandaloneParameterDefinition> parameterDefinitions = new Dictionary<int, StandaloneParameterDefinition>();
        private readonly Dictionary<int, TableDefinition> tableDefinitions = new Dictionary<int, TableDefinition>();

        public IDmsProtocolMockBuilder(string name, string version = IDmsProtocolMock.DefaultVersion)
        {
            this.name = name;
            this.version = version;
        }

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

        public IDmsProtocolMock Build()
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

        internal IDmsProtocolMock Build(IDmsMock dmsMock)
        {
            if (dmsMock == null)
            {
                throw new ArgumentNullException(nameof(dmsMock));
            }

            var protocolMock = Build();
            dmsMock.AddProtocol(protocolMock);
            return protocolMock;
        }
    }
}
