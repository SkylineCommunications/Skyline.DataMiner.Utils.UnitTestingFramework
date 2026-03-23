namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;

    public class ParameterDefinition : IEquatable<ParameterDefinition>
    {
        public ParameterDefinition(string name, Type type, int pid, bool allowNull = true, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException($"'{nameof(name)}' cannot be null or whitespace.", nameof(name));
            }

            if (pid <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pid), pid, $"'{nameof(pid)}' cannot be negative or zero");
            }

            Name = name;
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Pid = pid;
            AllowNull = allowNull;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

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
            return $"parameter {(String.IsNullOrWhiteSpace(Description) ? Name : Description)} (PID: {Pid})";
        }

        internal void Validate(object value)
        {
            if (value is null)
            {
                if (!AllowNull)
                {
                    throw new InvalidOperationException($"Parameter '{Name}' does not allow null.");
                }

                return;
            }

            // Allow int to double conversion
            if (Type == typeof(double) && value is int)
            {
                return;
            }

            if (!Type.IsInstanceOfType(value))
            {
                throw new InvalidOperationException($"Invalid value type for {this}: expected {Type} but got {value.GetType()}.");
            }
        }
    }
}