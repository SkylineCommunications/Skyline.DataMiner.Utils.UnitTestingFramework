namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;

    /// <summary>
    /// Defines a standalone parameter and its initial value.
    /// </summary>
    public class StandaloneParameterDefinition : ParameterDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StandaloneParameterDefinition"/> class.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="type">The parameter value type.</param>
        /// <param name="pid">The parameter ID.</param>
        /// <param name="defaultValue">The initial parameter value.</param>
        /// <param name="allowNull">Whether the parameter accepts a null value.</param>
        /// <param name="description">The parameter description.</param>
        public StandaloneParameterDefinition(
            string name,
            Type type,
            int pid,
            object defaultValue = null,
            bool allowNull = true,
            string description = null)
            : base(name, type, pid, allowNull, description)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the initial parameter value.
        /// </summary>
        public object DefaultValue { get; }
    }
}
