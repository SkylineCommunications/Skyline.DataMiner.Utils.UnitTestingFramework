namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal class Asserter : IAsserter
    {
        private readonly IProtocolCache cache;

        public Asserter(IProtocolCache protocolCache)
        {
            cache = protocolCache ?? throw new ArgumentNullException(nameof(protocolCache));
        }

        public IParameterModel Parameter(int parameterId)
        {
            try
            {
                return cache.Parameters.GetParameterModel(parameterId);
            }
            catch
            {
                return null;
            }
        }

        public IParameterModel Parameter(string parameterName)
        {
            return cache.Parameters.GetParameterModel(parameterName);
        }

        public ITableAsserter Table(int tablePid)
        {
            return new TableAsserter(cache.Tables.GetTable(tablePid));
        }
    }
}