namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
