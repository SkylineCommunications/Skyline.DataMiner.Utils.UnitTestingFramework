namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone
{
    using System;

    /// <summary>
    /// Standalone parameter model.
    /// </summary>
    internal interface IParameterModel
    {
        /// <summary>
        /// Occurs when the parameter value or timestamp changes.
        /// </summary>
        event EventHandler<ParameterModelChangedEventArgs> Changed;

        /// <summary>
        /// Gets the parameter definition.
        /// </summary>
        ParameterDefinition Definition { get; }

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

        /// <summary>
        /// No longer raises any notifications until the returned <see cref="IDisposable"/> is disposed.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, resumes notifications.</returns>
        IDisposable SuspendNotifications();
    }
}