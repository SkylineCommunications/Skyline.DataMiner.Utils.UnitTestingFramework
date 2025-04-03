namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    /// <summary>
    /// Extended protocol model interface.
    /// </summary>
    public interface IProtocolModelExt
    {
        /// <summary>
        /// Loads the parameter values into the specified cache.
        /// </summary>
        /// <param name="cache">The cache.</param>
        /// <exception cref="ArgumentNullException"><paramref name="cache"/> is <see langword="null"/>.</exception>
        void LoadParameterValues(IProtocolCache cache);
    }
}