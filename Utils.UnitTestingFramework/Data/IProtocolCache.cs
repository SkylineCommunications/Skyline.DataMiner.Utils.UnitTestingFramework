namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    /// <summary>
    /// Protocol data cache interface.
    /// </summary>
    public interface IProtocolCache
    {
        /// <summary>
        /// Gets the cache of the standalone parameters.
        /// </summary>
        /// <value>
        /// The cache of the standalone parameters.
        /// </value>
        ParametersCache Parameters
        {
            get;
        }

        /// <summary>
        /// Gets the cache of the tables.
        /// </summary>
        /// <value>
        /// The cache of the tables.
        /// </value>
        TablesCache Tables
        {
            get;
        }
    }
}