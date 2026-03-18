namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal interface IProtocolModelParameterFinder
    {
        IParamsParam FindParameter(int parameterId);
    }
}