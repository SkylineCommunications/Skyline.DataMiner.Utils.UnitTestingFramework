namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal class ParameterAsserter : IParameterAsserter
    {
        public ParameterAsserter(IParameterModel parameterModel)
        {
            Value = parameterModel.Value;
            Timestamp = parameterModel.Timestamp;
        }

        public object Value { get; }

        public DateTime Timestamp { get; }
    }
}
