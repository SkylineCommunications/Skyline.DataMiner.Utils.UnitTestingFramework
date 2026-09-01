namespace Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples
{
    using System;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

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
