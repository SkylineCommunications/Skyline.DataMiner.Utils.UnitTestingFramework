namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    public sealed class Column
    {
        public Column(string name, Type type, int pid, int idx, bool allowNull = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            if (Type != typeof(string) && Type != typeof(double))
            {
                throw new ArgumentException("Only 'string' and 'double' column types are supported.", nameof(type));
            }

            if (pid <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid), pid, $"'{nameof(pid)}' cannot be negative or zero");
            }

            if (idx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idx), idx, $"'{nameof(idx)}' cannot be negative");
            }

            Name = name;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Pid = pid;
            Idx = idx;
            AllowNull = allowNull;
        }

        public string Name { get; }

        public Type Type { get; }

        public int Pid { get; set; }

        public int Idx { get; set; }

        public bool AllowNull { get; }

        internal void Validate(object value)
        {
            if (value is null)
            {
                if (!AllowNull)
                {
                    throw new InvalidOperationException($"Column '{Name}' does not allow nulls.");
                }

                return;
            }

            if (!Type.IsInstanceOfType(value))
            {
                throw new InvalidOperationException($"Invalid value type for column '{Name}'. Expected {Type}, got {value.GetType()}.");
            }
        }
    }
}