namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Helper to convert stored parameter/cell values (stored as <see cref="object"/>) to the type requested by the caller.
    /// </summary>
    internal static class ValueConverter
    {
        /// <summary>
        /// Converts the specified value to the requested type <typeparamref name="T"/>.
        /// Supports the types that the DataMiner System interfaces work with (e.g. <see cref="int"/>?, <see cref="double"/>?, <see cref="DateTime"/>? and <see cref="string"/>).
        /// </summary>
        /// <typeparam name="T">The type to convert the value to.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value, or the default value of <typeparamref name="T"/> when <paramref name="value"/> is <see langword="null"/>.</returns>
        public static T Convert<T>(object value)
        {
            if (value == null || value is DBNull)
            {
                return default(T);
            }

            if (value is T typedValue)
            {
                return typedValue;
            }

            var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            try
            {
                var converted = System.Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return (T)converted;
            }
            catch (Exception ex) when (ex is InvalidCastException || ex is FormatException || ex is OverflowException)
            {
                throw new InvalidOperationException($"Unable to convert value '{value}' of type '{value.GetType()}' to type '{typeof(T)}'.", ex);
            }
        }
    }
}
