namespace Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt
{
    using Skyline.DataMiner.Scripting;

    public interface SLProtocolExt : SLProtocol
    {
        object StringParameter { get; set; }

        PollingconfigurationQActionTable PollingConfiguration { get; set; }
    }
}