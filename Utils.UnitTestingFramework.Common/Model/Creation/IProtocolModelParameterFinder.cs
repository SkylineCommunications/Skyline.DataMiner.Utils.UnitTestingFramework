namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;

    internal interface IProtocolModelParameterFinder
    {
        IParamsParam FindParameter(int parameterId);
    }
}