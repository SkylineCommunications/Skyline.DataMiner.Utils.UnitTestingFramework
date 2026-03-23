namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;

    public interface IParameterAsserter
    {
        object Value { get; }

        DateTime Timestamp { get; }
    }
}
