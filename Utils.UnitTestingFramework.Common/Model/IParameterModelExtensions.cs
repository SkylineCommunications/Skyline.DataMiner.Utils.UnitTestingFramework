namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal static class IParameterModelExtensions
    {
        internal static ParameterValue ToParameterValue(this IParameterModel parameterModel)
        {
            return ParameterValue.Compose(parameterModel.Value);
        }
    }
}
