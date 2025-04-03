namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal class AssertHandler : IAssert
    {
        public AssertHandler(IProtocolCache protocolCache)
        {
            Cache = protocolCache;
        }

        private IProtocolCache Cache
        {
            get;
        }

        public IParameterModel Parameter(int parameterId)
        {
            return Cache.Parameters.GetParameterModel(parameterId);
        }

        public IParameterModel Parameter(string parameterName)
        {
            return Cache.Parameters.GetParameterModel(parameterName);
        }

        public ITableModelReader Table(int tablePid)
        {
            return Cache.Tables.GetTableModel(tablePid);
        }
    }
}