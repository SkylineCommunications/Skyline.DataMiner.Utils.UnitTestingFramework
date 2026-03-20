namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table
{
    using System;

    public sealed class ColumnDefinition : ParameterDefinition, IEquatable<ColumnDefinition>
    {
        public ColumnDefinition(string name, Type type, int pid, int idx, bool allowNull = true, string description = null) : base(name, type, pid, allowNull, description)
        {
            if (idx < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(idx), idx, $"'{nameof(idx)}' cannot be negative");
            }

            Idx = idx;
        }

        public int Idx { get; }

        public bool Equals(ColumnDefinition other)
        {
           return base.Equals(other) && Idx == other.Idx;
        }

        public override string ToString()
        {
            return $"column {(String.IsNullOrWhiteSpace(Description) ? Name : Description)} (PID: {Pid}, IDX: {Idx})";
        }
    }
}