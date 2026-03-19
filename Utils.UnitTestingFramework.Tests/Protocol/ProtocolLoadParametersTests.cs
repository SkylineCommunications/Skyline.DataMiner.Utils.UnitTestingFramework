namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class ProtocolLoadParametersTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_ValidInput_DefaultAndFixedValues()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act
            var value1 = mock.Object.GetParameter(1000);
            var value2 = mock.Object.GetParameter(1001);
            var value3 = mock.Object.GetParameter(1002);
            var value4 = mock.Object.GetParameter(1003);

            // Assert
            Assert.AreEqual(10.0, value1);
            Assert.AreEqual("15", value2);
            Assert.AreEqual(0x0A, value3);
            Assert.AreEqual("parameterValue", value4);
        }

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_InvalidInput_DefaultAndFixedValues()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value1 = mock.Object.GetParameter(1100);

            // Assert
            Assert.IsNull(value1);
        }

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_InvalidPath_DefaultAndFixedValues()
        {
            // Arrange
            string missingDirectoryPath = path + "\\..\\";

            // Act & Assert
            Assert.ThrowsExactly<DirectoryNotFoundException>(
                () => ElementDataBuilder.Build(missingDirectoryPath));
        }

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_ValidInput_LoadParameterNamesNumericValues()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value1 = mock.Object.GetParameterByName("NumericParameter");
            var value2 = mock.Object.GetParameterByName("NumericParameterFixed");

            // Assert
            Assert.AreEqual(10.0, value1);
            Assert.AreEqual(0x0A, value2);
        }

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_InvalidInput_LoadParameterNamesNumericValues()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value1 = mock.Object.GetParameterByName("NumericParameterInexistent");

            // Assert
            Assert.IsNull(value1);
        }

        [TestMethod]
        public void SLProtocolMockLoadParametersTest_ValidInput_LoadParameterNamesStringValues()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value1 = mock.Object.GetParameterByName("StringParameter");
            var value2 = mock.Object.GetParameterByName("StringParameterFixed");

            // Assert
            Assert.AreEqual("15", value1);
            Assert.AreEqual("parameterValue", value2);
        }
    }
}