namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    /// <summary>
    /// Standalone parameter model.
    /// </summary>
    public interface IParameterModel
    {
        /// <summary>
        /// Occurs when the parameter value or timestamp changes.
        /// </summary>
        event EventHandler<ParameterModelChangedEventArgs> Changed;

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
        /// Updates the parameter value and timestamp.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        void Update(object value, DateTime? timestamp = null);
    }
}