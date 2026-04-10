namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal interface IDataModelCreator
    {
        void CreateModelAndAddToDataCollection(ParametersAndTables dataCollection, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder);
    }
}