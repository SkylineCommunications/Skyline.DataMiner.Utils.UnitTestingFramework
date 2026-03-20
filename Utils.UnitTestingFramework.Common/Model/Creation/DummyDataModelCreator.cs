namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal class DummyDataModelCreator : IDataModelCreator
    {
        public DummyDataModelCreator()
        {
        }

        public void CreateModelAndAddToDataCollection(ParametersAndTables dataCollection, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
        }
    }
}