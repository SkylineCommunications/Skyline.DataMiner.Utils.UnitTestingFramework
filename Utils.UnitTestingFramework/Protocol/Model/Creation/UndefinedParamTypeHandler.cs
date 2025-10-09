namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class UndefinedParamTypeHandler : IParameterHandler
    {
        private readonly HashSet<int> excludedPids;

        public UndefinedParamTypeHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids;
        }

        public void CreateModelAndAddToCache(IProtocolCache cache, IParamsParam parameter)
        {
        }
    }
}