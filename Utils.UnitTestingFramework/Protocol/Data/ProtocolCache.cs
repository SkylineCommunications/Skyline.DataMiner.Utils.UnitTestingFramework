namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    /// <summary>
    /// Represents the protocol cache containing the standalone and table parameter values.
    /// </summary>
    /// <seealso cref="IProtocolCache" />
    public class ProtocolCache : IProtocolCache
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolCache"/> class.
        /// </summary>
        public ProtocolCache()
        {
            Parameters = new ParametersCache();
            Tables = new TablesCache();
        }

        /// <summary>
        /// Gets the cache of the standalone parameters.
        /// </summary>
        /// <value>
        /// The cache of the standalone parameters.
        /// </value>
        public ParametersCache Parameters
        {
            get;
        }

        /// <summary>
        /// Gets the cache of the tables.
        /// </summary>
        /// <value>
        /// The cache of the tables.
        /// </value>
        public TablesCache Tables
        {
            get;
        }
    }
}