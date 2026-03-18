namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class ReadParameterModelCreator : ParameterModelCreatorBase
    {
        private readonly HashSet<int> excludedPids;

        public ReadParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new System.ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ElementData element, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string defaultValue = parameter.Interprete.DefaultValue?.Value;
            var parameterModel = new ParameterModel(defaultValue);

            element.AddParameter(parameterDefinition, parameterModel);
        }

        protected override void ProcessDouble(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string defaultValueString = parameter.Interprete.DefaultValue?.Value;
            var parameterModel = Double.TryParse(defaultValueString, out double defaultValue) ? new ParameterModel(defaultValue) : new ParameterModel(null);

            elementData.AddParameter(parameterDefinition, parameterModel);
        }
    }
}