namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone
{
    using System;

    /// <summary>
    /// Provides data for parameter model change notifications.
    /// </summary>
    internal class ParameterModelChangedEventArgs : ParameterValueChangedEventArgs<ParameterDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModelChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldTimestamp">The old timestamp.</param>
        /// <param name="newTimestamp">The new timestamp.</param>
        public ParameterModelChangedEventArgs(ParameterDefinition parameterDefinition, object oldValue, object newValue, DateTime oldTimestamp, DateTime newTimestamp)
            :base(parameterDefinition, oldValue, newValue, oldTimestamp, newTimestamp)
        {

        }
    }
}
