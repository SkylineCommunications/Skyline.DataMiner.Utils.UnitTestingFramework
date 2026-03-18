namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Standalone
{
    using System;

    /// <summary>
    /// Provides data for parameter model change notifications.
    /// </summary>
    public class ParameterModelChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModelChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldTimestamp">The old timestamp.</param>
        /// <param name="newTimestamp">The new timestamp.</param>
        public ParameterModelChangedEventArgs(object oldValue, object newValue, DateTime oldTimestamp, DateTime newTimestamp)
        {
            OldValue = oldValue;
            NewValue = newValue;
            OldTimestamp = oldTimestamp;
            NewTimestamp = newTimestamp;
        }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object NewValue { get; }

        /// <summary>
        /// Gets the old timestamp.
        /// </summary>
        public DateTime OldTimestamp { get; }

        /// <summary>
        /// Gets the new timestamp.
        /// </summary>
        public DateTime NewTimestamp { get; }
    }
}
