namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    public interface IParameterValue
    {
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        object Value { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        DateTime Timestamp { get; }

        /// <summary>
        /// Updates the parameter value and timestamp, returning a value indicating whether the value was actually updated (i.e. the new value is different from the current value, or the timestamp is more recent than the current timestamp).
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        /// <returns>
        /// A value indicating whether the value was actually updated (i.e. the new value is different from the current value, or the timestamp is more recent than the current timestamp).
        /// </returns>
        bool Update(object value, DateTime? timestamp = null);
    }
}