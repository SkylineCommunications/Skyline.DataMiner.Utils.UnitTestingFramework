namespace Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt
{
    using Skyline.DataMiner.Scripting;

    // Created this class based on how DIS generates ConcreteSLProtocolExt in real protocol solutions.
    public class ConcreteSLProtocolExt : ConcreteSLProtocol, SLProtocolExt
    {
        public ConcreteSLProtocolExt()
        {
            PollingConfiguration = new PollingconfigurationQActionTable(this, 900, "Polling Configuration");
        }

        public object StringParameter { get { return GetParameter(1001); } set { SetParameter(1001, value); } }

        public PollingconfigurationQActionTable PollingConfiguration { get; set; }
    }
}