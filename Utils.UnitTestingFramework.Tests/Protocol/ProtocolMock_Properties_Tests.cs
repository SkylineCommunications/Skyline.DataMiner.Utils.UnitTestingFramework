namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class ProtocolMock_Properties_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Leave()
        {
            var procotolMock = new SLProtocolMock(path);

            Assert.AreEqual(Constants.PROTOCOL_LEAVE, procotolMock.Object.Leave);
        }


        [TestMethod]
        public void Clear()
        {
            var procotolMock = new SLProtocolMock(path);

            Assert.AreEqual(Constants.PROTOCOL_CLEAR, procotolMock.Object.Clear);
        }

        [TestMethod]
        public void ProtocolName()
        {
            var procotolMock = new SLProtocolMock(path);

            Assert.AreEqual("UnitTestingFrameworkUseCases", procotolMock.Object.ProtocolName);
        }

        [TestMethod]
        public void ProtocolVersion()
        {
            var procotolMock = new SLProtocolMock(path);

            Assert.AreEqual("1.0.0.1", procotolMock.Object.ProtocolVersion);
        }
    }
}
