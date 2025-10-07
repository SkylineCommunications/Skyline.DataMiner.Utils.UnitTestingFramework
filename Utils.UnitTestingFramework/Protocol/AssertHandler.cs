namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal class AssertHandler : IAssert
    {
        private readonly IProtocolCache cache;

        public AssertHandler(IProtocolCache protocolCache)
        {
            cache = protocolCache ?? throw new ArgumentNullException(nameof(protocolCache));
        }

        public IParameterModel Parameter(int parameterId)
        {
            return cache.Parameters.GetParameterModel(parameterId);
        }

        public IParameterModel Parameter(string parameterName)
        {
            return cache.Parameters.GetParameterModel(parameterName);
        }

        public ITableModelReader Table(int tablePid)
        {
            return cache.Tables.GetTable(tablePid);
        }
    }
}