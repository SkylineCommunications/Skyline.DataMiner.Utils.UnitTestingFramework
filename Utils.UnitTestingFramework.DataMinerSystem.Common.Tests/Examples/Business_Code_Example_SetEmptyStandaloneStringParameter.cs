namespace Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples
{
    using System;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    internal static class Business_Code_Example_SetEmptyStandaloneStringParameter
    {
        public static void SetEmptyStandaloneStringParameter(IDmsElement element, int parameterId)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            var parameter = element.GetStandaloneParameter<string>(parameterId) ?? throw new InvalidOperationException($"Parameter with ID {parameterId} not found.");

            if (!string.IsNullOrEmpty(parameter.GetValue()))
            {
                // Parameter value is already filled in, unable to update
                return;
            }

            parameter.SetValue("new value");
        }
    }
}
