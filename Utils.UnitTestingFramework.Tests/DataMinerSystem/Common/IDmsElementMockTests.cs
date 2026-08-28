namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetStandaloneParameter_ExistingId_ReturnsParameterWithMatchingId()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.IsNotNull(parameter);
            Assert.AreEqual(1001, parameter.Id);
        }

        [TestMethod]
        public void GetStandaloneParameter_CalledTwice_ReturnsSameCachedInstance()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var first = mock.Object.GetStandaloneParameter<string>(1001);
            var second = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.AreSame(first, second);
        }

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
        public void GetStandaloneParameter_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetStandaloneParameter<string>(123456));
        }

        [TestMethod]
        public void GetTable_ExistingId_ReturnsTableWithMatchingId()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var table = mock.Object.GetTable(900);

            // Assert
            Assert.IsNotNull(table);
            Assert.AreEqual(900, table.Id);
        }

        [TestMethod]
        public void GetTable_CalledTwice_ReturnsSameCachedInstance()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var first = mock.Object.GetTable(900);
            var second = mock.Object.GetTable(900);

            // Assert
            Assert.AreSame(first, second);
        }

        [TestMethod]
        public void GetTable_NonExistingId_ThrowsArgumentException()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetTable(123456));
        }
    }
}
