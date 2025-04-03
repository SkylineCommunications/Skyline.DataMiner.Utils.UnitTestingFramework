namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Parameter
{
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal interface ISingleParameterHandler : IParameterHandler
    {
        void ProcessString(IProtocolCache cache, IParamsParam parameter);

        void ProcessDouble(IProtocolCache cache, IParamsParam parameter);

        void ProcessHighNibble(IProtocolCache cache, IParamsParam parameter);

        void ProcessUndefinedType(IProtocolCache cache, IParamsParam parameter);
    }
}