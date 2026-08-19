namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMock_SupportedTypes_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetStandaloneParameter_StringType_IsSupported()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_NullableIntType_IsSupported()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<int?>(800);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_NullableDoubleType_IsSupported()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<double?>(1000);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_NullableDateTimeType_IsSupported()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<DateTime?>(1000);

            // Assert
            Assert.IsNotNull(parameter);
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedIntType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<int>(800));
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedDoubleType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<double>(1000));
        }

        [TestMethod]
        public void GetStandaloneParameter_UnsupportedObjectType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => mock.Object.GetStandaloneParameter<object>(1001));
        }

        [TestMethod]
        public void GetColumn_SupportedType_IsSupported()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var column = mock.Object.GetTable(900).GetColumn<string>(902);

            // Assert
            Assert.IsNotNull(column);
        }

        [TestMethod]
        public void GetColumn_UnsupportedType_ThrowsNotSupportedException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var table = mock.Object.GetTable(900);

            // Act & Assert
            Assert.ThrowsExactly<NotSupportedException>(
                () => table.GetColumn<double>(903));
        }
    }
}
