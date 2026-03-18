namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal interface IDataModelCreator
    {
        void CreateModelAndAddToElementData(ElementData elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder);
    }
}