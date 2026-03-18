namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
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

        ParameterDefinition Definition { get; }
    }
}