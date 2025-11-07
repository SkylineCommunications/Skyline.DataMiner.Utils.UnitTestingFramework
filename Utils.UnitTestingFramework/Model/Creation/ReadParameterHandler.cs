namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class ReadParameterHandler : GeneralParameterHandler
    {
        private readonly HashSet<int> excludedPids;

        public ReadParameterHandler(HashSet<int> excludedPids)
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

            string defaultValue = parameter.Interprete.DefaultValue?.Value;

            cache.Parameters.SetParameter(parameterId, defaultValue, null, false);
        }

        protected override void ProcessDouble(IProtocolCache cache, IParamsParam parameter)
        {
            int parameterId = (int)parameter.Id.Value.Value;

            if (excludedPids.Contains(parameterId))
            {
                return;
            }

            string defaultValueString = parameter.Interprete.DefaultValue?.Value;

            if (System.Int32.TryParse(defaultValueString, out int defaultValue))
            {
                cache.Parameters.SetParameter(parameterId, defaultValue, null, false);
            }
            else
            {
                cache.Parameters.SetParameter(parameterId, null, null, false);
            }
        }
    }
}