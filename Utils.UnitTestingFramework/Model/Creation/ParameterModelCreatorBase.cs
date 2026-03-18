namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using Skyline.DataMiner.CICD.Models.Protocol.Enums;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal abstract class ParameterModelCreatorBase : DataModelCreatorBase, IDataModelCreator
    {
        public void CreateModelAndAddToElementData(ElementData elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            if (elementData is null)
            {
                throw new ArgumentNullException(nameof(elementData));
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
                    ProcessString(elementData, parameter);
                    break;

                case EnumParamInterpretType.Double:
                    ProcessDouble(elementData, parameter);
                    break;

                case EnumParamInterpretType.HighNibble:
                    ProcessHighNibble(elementData, parameter);
                    break;

                default:
                    ProcessUndefinedType(elementData, parameter);
                    break;
            }
        }

        protected abstract void ProcessString(ElementData elementData, IParamsParam parameter);

        protected abstract void ProcessDouble(ElementData elementData, IParamsParam parameter);

        protected virtual void ProcessHighNibble(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            elementData.AddParameter(new ParameterModel(parameterDefinition, null));
        }

        protected virtual void ProcessUndefinedType(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;
            
            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            elementData.AddParameter(new ParameterModel(parameterDefinition, null));
        }
    }
}