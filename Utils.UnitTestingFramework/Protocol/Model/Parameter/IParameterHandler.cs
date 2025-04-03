namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Parameter
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal interface IParameterHandler
    {
        void LoadDefaultForParameter(IProtocolCache cache, IParamsParam parameter);
    }
}