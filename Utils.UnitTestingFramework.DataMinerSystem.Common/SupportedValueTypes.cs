namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Helper that validates the generic type argument used for standalone parameters and columns.
    /// </summary>
    /// <remarks>
    /// The <see cref="Skyline.DataMiner.Core.DataMinerSystem.Common"/> interfaces only support a limited set of types
    /// (<see cref="int"/>?, <see cref="double"/>?, <see cref="DateTime"/>? and <see cref="string"/>). Any other type
    /// results in a <see cref="NotSupportedException"/>, mirroring the behavior of the real implementation.
    /// </remarks>
    internal static class SupportedValueTypes
    {
        private static readonly HashSet<Type> Supported = new HashSet<Type>
        {
            typeof(int?),
            typeof(double?),
            typeof(DateTime?),
            typeof(string),
        };

        /// <summary>
        /// Throws a <see cref="NotSupportedException"/> when the specified type is not one of the supported types.
        /// </summary>
        /// <param name="type">The type to validate.</param>
        /// <exception cref="NotSupportedException"><paramref name="type"/> is not one of the supported types.</exception>
        public static void EnsureSupported(Type type)
        {
            if (!Supported.Contains(type))
            {
                throw new NotSupportedException(
                    $"Type '{type}' is not supported. Only 'int?', 'double?', 'DateTime?' and 'string' are supported.");
            }
        }
    }
}
