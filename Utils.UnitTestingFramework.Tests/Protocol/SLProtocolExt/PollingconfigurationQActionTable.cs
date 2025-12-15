namespace Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt
{
    using Skyline.DataMiner.Scripting;

    // Created this class based on how DIS generates QActionTable classes in real protocol solutions.
    public class PollingconfigurationQActionTable : QActionTable
    {
        public PollingconfigurationQActionTable(SLProtocol protocol, int tableId, string tableName) : base(protocol, tableId, tableName) { }
    }
}