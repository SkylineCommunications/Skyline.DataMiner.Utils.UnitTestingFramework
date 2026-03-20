namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal abstract class ParameterModelCreatorBase : DataModelCreatorBase, IDataModelCreator
    {
        public void CreateModelAndAddToDataCollection(ParametersAndTables dataCollection, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            if (dataCollection is null)
            {
                throw new ArgumentNullException(nameof(dataCollection));
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
                    ProcessString(dataCollection, parameter);
                    break;

                case EnumParamInterpretType.Double:
                    ProcessDouble(dataCollection, parameter);
                    break;

                default:
                    ProcessOtherTypes(dataCollection, parameter);
                    break;
            }
        }

        protected abstract void ProcessString(ParametersAndTables dataCollection, IParamsParam parameter);

        protected abstract void ProcessDouble(ParametersAndTables dataCollection, IParamsParam parameter);

        protected virtual void ProcessOtherTypes(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            dataCollection.AddParameter(new ParameterModel(parameterDefinition, null));
        }
    }
}