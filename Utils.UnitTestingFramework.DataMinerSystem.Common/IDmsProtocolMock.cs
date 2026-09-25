namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;

    using Moq;

    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// A pre-arranged mock of <see cref="IDmsProtocol"/>.
    /// </summary>
    public class IDmsProtocolMock : Mock<IDmsProtocol>
    {
        public const string DefaultVersion = "1.0.0.1";

        public IDmsProtocolMock(string name, string version = DefaultVersion)
        {
            ValidateProtocolIdentifier(name, nameof(name));
            ValidateProtocolIdentifier(version, nameof(version));

            Name = name;
            ReferencedVersion = version;
            Definitions = new ParameterAndTableDefinitions();
            SetupProtocol();
        }

        internal IDmsProtocolMock(IProtocolModel protocolModel, string pathToProtocolXml = null)
        {
            if (protocolModel == null)
            {
                throw new ArgumentNullException(nameof(protocolModel));
            }

            PathToProtocolXml = pathToProtocolXml;
            Name = protocolModel.Protocol.Name?.Value;
            ReferencedVersion = protocolModel.Protocol.Version?.Value;
            Definitions = ParameterAndTablesDefinitionsBuilder.Build(protocolModel);

            var typeName = protocolModel.Protocol.Type?.Value?.ToString();
            if (!Enum.TryParse(typeName, true, out ProtocolType protocolType))
            {
                throw new ArgumentException("The protocol type is missing or invalid.", nameof(protocolModel));
            }

            Type = protocolType;
            SetupProtocol();
        }

        internal ParameterAndTableDefinitions Definitions { get; }

        internal string PathToProtocolXml { get; }

        public new string Name { get; }

        public string ReferencedVersion { get; }

        public ProtocolType Type { get; set; }

        public void AddTableDefinition(int tableId, TableDefinition tableDefinition)
        {
            Definitions.AddTableDefinition(tableId, tableDefinition);
        }

        public void AddParameterDefinition(StandaloneParameterDefinition parameterDefinition)
        {
            Definitions.AddParameterDefinition(parameterDefinition);
        }

        private void SetupProtocol()
        {
            Setup(protocol => protocol.Name).Returns(() => Name);
            Setup(protocol => protocol.ReferencedVersion).Returns(() => ReferencedVersion);
            Setup(protocol => protocol.Type).Returns(() => Type);
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
    }
}
