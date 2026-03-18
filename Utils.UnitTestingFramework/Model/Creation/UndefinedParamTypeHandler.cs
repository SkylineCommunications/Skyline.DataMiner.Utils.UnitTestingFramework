namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class UndefinedParamTypeHandler : IDataModelCreator
    {
        private readonly HashSet<int> excludedPids;

        public UndefinedParamTypeHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids;
        }

        public void CreateModelAndAddToElementData(ElementData elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
        }
    }
}