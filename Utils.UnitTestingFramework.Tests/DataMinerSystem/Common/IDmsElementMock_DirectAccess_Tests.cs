namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class IDmsElementMock_DirectAccess_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetStandaloneParameter_ArrangeAndAssertWithoutObject_PersistsValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.GetStandaloneParameter<string>(1001).SetValue("new value");
            var value = mock.GetStandaloneParameter<string>(1001).GetValue();

            // Assert
            Assert.AreEqual("new value", value);
        }

        [TestMethod]
        public void GetStandaloneParameter_ArrangedViaObject_AssertedWithoutObject()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<string>(1001).SetValue("shared value");

            // Assert
            Assert.AreEqual("shared value", mock.GetStandaloneParameter<string>(1001).GetValue());
        }

        [TestMethod]
        public void GetTable_ArrangeAndAssertWithoutObject_PersistsRow()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.GetTable(900).AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });
            var value = mock.GetTable(900).GetColumn<string>(902).GetValue("one");

            // Assert
            Assert.AreEqual("one-desc", value);
        }

        [TestMethod]
        public void GetTable_SameInstanceReturnedForDirectAccessAndObject()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var directTable = mock.GetTable(900);
            var objectTable = mock.Object.GetTable(900);

            // Assert
            Assert.AreSame(objectTable, directTable);
        }

        [TestMethod]
        public void ArrangeViaDirectMethods_DoesNotRecordInvocationsOnMock()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act: arrange data exclusively through the direct methods.
            mock.GetStandaloneParameter<string>(1001).SetValue("new value");
            mock.GetTable(900).AddRow(new object[] { "one", "one-desc", 3.0, 4.0, 5.0 });

            // Assert: arranging via the direct methods must not be recorded as invocations on the mock,
            // so a later Verify is not impacted.
            mock.Verify(e => e.GetStandaloneParameter<string>(It.IsAny<int>()), Times.Never);
            mock.Verify(e => e.GetTable(It.IsAny<int>()), Times.Never);
        }
    }
}
