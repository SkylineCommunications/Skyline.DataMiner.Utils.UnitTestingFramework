namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class WriteParameterModelCreator : ParameterModelCreatorBase
    {
        private readonly HashSet<int> excludedPids;

        public WriteParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new System.ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            elementData.AddParameter(parameterDefinition);
        }

        protected override void ProcessDouble(ElementData elementData, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);

            elementData.AddParameter(parameterDefinition);
        }
    }
}