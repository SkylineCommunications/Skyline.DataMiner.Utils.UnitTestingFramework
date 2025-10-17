namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal interface IParameterHandler
    {
        void CreateModelAndAddToCache(IProtocolCache cache, IParamsParam parameter);
    }
}