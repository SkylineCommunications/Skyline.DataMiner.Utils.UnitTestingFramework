namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMock_StandaloneParameterMethods_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetStandaloneParameter_SetAndGetStringValue_ReturnsSetValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<string>(1001).SetValue("new value");
            var value = mock.Object.GetStandaloneParameter<string>(1001).GetValue();

            // Assert
            Assert.AreEqual("new value", value);
        }

        [TestMethod]
        public void GetStandaloneParameter_DefaultValue_ReturnsProtocolDefault()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var value = mock.Object.GetStandaloneParameter<double?>(1000).GetValue();

            // Assert
            Assert.AreEqual(10.0, value);
        }

        [TestMethod]
        public void GetStandaloneParameter_SetAndGetDoubleValue_ReturnsSetValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<double?>(1000).SetValue(42.5);
            var value = mock.Object.GetStandaloneParameter<double?>(1000).GetValue();

            // Assert
            Assert.AreEqual(42.5, value);
        }

        [TestMethod]
        public void GetStandaloneParameter_SetAndGetNullableIntValue_ReturnsSetValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<int?>(800).SetValue(7);
            var value = mock.Object.GetStandaloneParameter<int?>(800).GetValue();

            // Assert
            Assert.AreEqual(7, value);
        }

        [TestMethod]
        public void GetStandaloneParameter_ValueSetOnDifferentInstance_IsPersisted()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<string>(1001).SetValue("persisted");

            // Assert
            Assert.AreEqual("persisted", mock.Object.GetStandaloneParameter<string>(1001).GetValue());
        }

        [TestMethod]
        public void GetStandaloneParameter_ExposesIdAndElement()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.AreEqual(1001, parameter.Id);
            Assert.AreSame(mock.Object, parameter.Element);
        }

        [TestMethod]
        public void GetStandaloneParameter_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetStandaloneParameter<string>(123456));
        }
    }
}
