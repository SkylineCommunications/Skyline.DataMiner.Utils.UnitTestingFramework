namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class WriteParameterHandler : GeneralParameterHandler
    {
        private readonly HashSet<int> excludedPids;

        public WriteParameterHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new System.ArgumentNullException(nameof(excludedPids));
        }

        protected override void ProcessString(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }

        protected override void ProcessDouble(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            cache.Parameters.SetParameter(parameterId, null, null, false);
        }
    }
}