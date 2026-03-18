namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Standalone
{
    using System;

    public interface IParameterValue
    {
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        object Value { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Updates the parameter value and timestamp, returning a value indicating whether the value was actually updated (i.e. the new value is different from the current value, or the timestamp is more recent than the current timestamp).
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        bool Update(object value, DateTime? timestamp = null);
    }
}