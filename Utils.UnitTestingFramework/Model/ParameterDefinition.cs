namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model
{
    using System;

    public class ParameterDefinition : IEquatable<ParameterDefinition>
    {
        public ParameterDefinition(string name, Type type, int pid, bool allowNull = true)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }
            if (Type != typeof(string) && Type != typeof(double))
            {
                throw new ArgumentException("Only 'string' and 'double' parameter types are supported.", nameof(type));
            }
            if (pid <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid), pid, $"'{nameof(pid)}' cannot be negative or zero");
            }
            Name = name;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Pid = pid;
            AllowNull = allowNull;
        }

        public string Name { get; }

        public Type Type { get; }

        public int Pid { get; }

        public bool AllowNull { get; }

        public bool Equals(ParameterDefinition other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Name == other.Name && Type == other.Type && Pid == other.Pid && AllowNull == other.AllowNull;
        }

        public override string ToString()
        {
            return $"{Name} (PID: {Pid}, Type: {Type.Name}, AllowNull: {AllowNull})";
        }

        internal void Validate(object value)
        {
            if (value is null)
            {
                if (!AllowNull)
                {
                    throw new InvalidOperationException($"Parameter '{Name}' does not allow nulls.");
                }
                return;
            }

            if (!Type.IsInstanceOfType(value))
            {
                throw new InvalidOperationException($"Invalid value type for parameter '{Name}'. Expected {Type}, got {value.GetType()}.");
            }
        }
    }
}