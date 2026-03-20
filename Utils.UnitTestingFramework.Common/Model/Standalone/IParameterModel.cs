namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone
{
    using System;

    /// <summary>
    /// Standalone parameter model.
    /// </summary>
    public interface IParameterModel : IParameterValue
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
        /// No longer raises any notifications until the returned <see cref="IDisposable"/> is disposed.
        /// </summary>
        /// <returns>An <see cref="IDisposable"/> that, when disposed, resumes notifications.</returns>
        IDisposable SuspendNotifications();
    }
}