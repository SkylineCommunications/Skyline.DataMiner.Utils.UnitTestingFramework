namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class UndefinedParamTypeHandler : IDataModelCreator
    {
        private readonly HashSet<int> excludedPids;

        public UndefinedParamTypeHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids;
        }

        public void CreateModelAndAddToElementData(ParametersAndTables elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
        }
    }
}