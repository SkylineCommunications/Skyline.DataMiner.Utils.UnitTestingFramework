namespace Utils.UnitTestingFramework.Tests.DataMinerSystem.Examples
{
    using System;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;

    internal static class Business_Code_Example_RenameElement
    {
        public static void RenameElement(IDmsElement element, string newName)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            if (element.Name.Contains(newName))
            {
                // Element name already contains "test"
                return;
            }

            element.Name = newName;
        }
    }
}
