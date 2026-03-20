namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal class ReadParameterModelCreator : ParameterModelCreatorBase
    {
        private readonly HashSet<int> excludedPids;

        public ReadParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string defaultValue = parameter.Interprete.DefaultValue?.Value;
            var parameterModel = new ParameterModel(parameterDefinition, defaultValue);

            dataCollection.AddParameter(parameterModel);
        }

        protected override void ProcessDouble(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            string defaultValueString = parameter.Interprete.DefaultValue?.Value;
            var parameterModel = Double.TryParse(defaultValueString, out double defaultValue) ? new ParameterModel(parameterDefinition, defaultValue) : new ParameterModel(parameterDefinition, null);

            dataCollection.AddParameter(parameterModel);
        }
    }
}