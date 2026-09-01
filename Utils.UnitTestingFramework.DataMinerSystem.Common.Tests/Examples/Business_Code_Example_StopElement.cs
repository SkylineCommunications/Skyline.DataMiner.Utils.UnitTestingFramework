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

    internal static class Business_Code_Example_FindElementByName
    {
        public static IDmsElement FindElementByName(IDma dma, string elementName)
        {
            if (dma is null)
            {
                throw new ArgumentNullException(nameof(dma));
            }

            if (string.IsNullOrEmpty(elementName))
            {
                throw new ArgumentException("Element name cannot be null or empty.", nameof(elementName));
            }

            foreach (var element in dma.GetElements())
            {
                if (element.Name.Equals(elementName, StringComparison.OrdinalIgnoreCase))
                {
                    return element;
                }
            }

            // Element not found
            return null;
        }
    }
}
