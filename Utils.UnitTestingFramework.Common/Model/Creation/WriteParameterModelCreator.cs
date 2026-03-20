namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal class WriteParameterModelCreator : ParameterModelCreatorBase
    {
        private readonly HashSet<int> excludedPids;

        public WriteParameterModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new System.ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            ProcessAny(dataCollection, parameter);
        }

        protected override void ProcessDouble(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            ProcessAny(dataCollection, parameter);
        }

        private void ProcessAny(ParametersAndTables dataCollection, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            var parameterDefinition = new ParameterDefinition(parameter.Name.Value, GetTypeForDefinition(parameter), parameterId);
            dataCollection.AddParameter(new ParameterModel(parameterDefinition, null));
        }
    }
}