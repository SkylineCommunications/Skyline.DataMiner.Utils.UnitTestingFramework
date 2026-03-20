namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Table
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Standalone;

    public sealed class ColumnDefinition : ParameterDefinition, IEquatable<ColumnDefinition>
    {
        public ColumnDefinition(string name, Type type, int pid, int idx, bool allowNull = true) : base(name, type, pid, allowNull)
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
            return $"{Name} (PID: {Pid}, IDX: {Idx})";
        }
    }
}