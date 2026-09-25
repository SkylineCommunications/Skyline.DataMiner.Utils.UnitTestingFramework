namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class ReadParameterModelCreator : ParameterModelCreatorBase
    {
        private readonly HashSet<int> excludedPids;

        public ReadParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ParameterAndTableDefinitions definitions, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = BuildDefinitionFromProtocolParameter(parameter, parameter.Interprete.DefaultValue?.Value);

            definitions.AddParameterDefinition(parameterDefinition);
        }

        protected override void ProcessDouble(ParameterAndTableDefinitions definitions, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            object defaultValue = null;
            if (Double.TryParse(parameter.Interprete.DefaultValue?.Value, out double parsedValue))
            {
                defaultValue = parsedValue;
            }

            var parameterDefinition = BuildDefinitionFromProtocolParameter(parameter, defaultValue);

            definitions.AddParameterDefinition(parameterDefinition);
        }
    }
}
