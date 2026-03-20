namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;

    public class ParameterValueChangedEventArgs<TParameterDefinition> : EventArgs where TParameterDefinition : ParameterDefinition
    {
        public ParameterValueChangedEventArgs(TParameterDefinition parameterDefinition, object oldValue, object newValue, DateTime oldTimestamp, DateTime newTimestamp)
        {
            ParameterDefinition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
            OldValue = oldValue;
            NewValue = newValue;
            OldTimestamp = oldTimestamp;
            NewTimestamp = newTimestamp;
        }

        public TParameterDefinition ParameterDefinition { get; }

        public object OldValue { get; }

        public object NewValue { get; }

        public DateTime OldTimestamp { get; }

        public DateTime NewTimestamp { get; }
    }
}