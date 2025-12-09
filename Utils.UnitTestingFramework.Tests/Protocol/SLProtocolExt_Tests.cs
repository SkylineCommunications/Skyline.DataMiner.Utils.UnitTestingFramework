namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;

    public interface SLProtocolExt : SLProtocol
    {
        object Mediatedelementstableid { get; set; }
    }

    public class ConcreteSLProtocolExt : ConcreteSLProtocol, SLProtocolExt
    {
        public System.Object Mediatedelementstableid { get { return GetParameter(1001); } set { SetParameter(1001, value); } }
    }

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class SLProtocolExt_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void SLProtocolExt_Test()
        {
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            protocolMock.Object.Mediatedelementstableid = 42;

        }
    }
}