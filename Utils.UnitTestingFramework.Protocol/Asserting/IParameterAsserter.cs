namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;

    /// <summary>
    /// Defines the interface for asserting a parameter value and timestamp.
    /// </summary>
    public interface IParameterAsserter
    {
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        object Value { get; }
        
        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        DateTime Timestamp { get; }
    }
}
