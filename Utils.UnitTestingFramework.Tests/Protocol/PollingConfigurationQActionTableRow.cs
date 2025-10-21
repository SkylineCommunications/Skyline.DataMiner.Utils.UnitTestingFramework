namespace Utils.UnitTestingFramework.Tests.Protocol
{
    using Skyline.DataMiner.Scripting;

    public class PollingConfigurationQActionTableRow : QActionTableRow
    {
        public PollingConfigurationQActionTableRow() : base(0, 5)
        {
        }
        public PollingConfigurationQActionTableRow(object[] oRow) : base(0, 5, oRow)
        {
        }
    }
}