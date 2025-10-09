namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    /// <summary>
    /// Parameter model.
    /// </summary>
    /// <seealso cref="IParameterModel" />
    public class ParameterModel : IParameterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModel"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public ParameterModel(object value, DateTime? timestamp = null)
        {
            Value = value;
            Timestamp = timestamp ?? DateTime.Now;
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public object Value { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp { get; }
    }
}