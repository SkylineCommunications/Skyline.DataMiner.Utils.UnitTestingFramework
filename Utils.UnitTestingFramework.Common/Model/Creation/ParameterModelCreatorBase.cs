namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal abstract class ParameterModelCreatorBase : DataModelCreatorBase, IDataModelCreator
    {
        public void CreateDefinitionAndAddToCollection(ParameterAndTableDefinitions definitions, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            if (definitions is null)
            {
                throw new ArgumentNullException(nameof(definitions));
            }

            if (parameter is null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.Interprete?.Type?.Value == null)
            {
                return;
            }

            var interpreteType = parameter.Interprete.Type.Value.Value;

            switch (interpreteType)
            {
                case EnumParamInterpretType.String:
                    ProcessString(definitions, parameter);
                    break;

                case EnumParamInterpretType.Double:
                    ProcessDouble(definitions, parameter);
                    break;

                default:
                    ProcessOtherTypes(definitions, parameter);
                    break;
            }
        }

        protected StandaloneParameterDefinition BuildDefinitionFromProtocolParameter(IParamsParam parameter, object defaultValue = null, bool allowNull = true)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            return new StandaloneParameterDefinition(
                parameter.Name.Value,
                GetTypeForDefinition(parameter),
                parameterId,
                defaultValue,
                description: parameter.Description?.Value,
                allowNull: allowNull);
        }

        protected abstract void ProcessString(ParameterAndTableDefinitions definitions, IParamsParam parameter);

        protected abstract void ProcessDouble(ParameterAndTableDefinitions definitions, IParamsParam parameter);

        protected virtual void ProcessOtherTypes(ParameterAndTableDefinitions definitions, IParamsParam parameter)
        {
            var parameterDefinition = BuildDefinitionFromProtocolParameter(parameter);
            definitions.AddParameterDefinition(parameterDefinition);
        }
    }
}
