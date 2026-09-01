namespace Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples
{
    using System;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    internal static class Business_Code_Example_StopElement
    {
        public static void StopElement(IDmsElement element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.GetActiveAlarmCount() == 0)
            {
                // No active alarms, nothing to stop
                return;
            }

            if (element.State != ElementState.Active)
            {
                // Element is not active, cannot stop
                return;
            }

            element.Stop();
        }
    }
}
