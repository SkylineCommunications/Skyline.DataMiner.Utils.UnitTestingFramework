namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    /// <summary>
    /// Represents the protocol cache containing the standalone and table parameter values.
    /// </summary>
    /// <seealso cref="IProtocolCache" />
    public class ProtocolCache : IProtocolCache
    {
        public ParametersCache Parameters { get; } = new ParametersCache();

        public TablesCache Tables { get; } = new TablesCache();
    }
}