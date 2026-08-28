namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.DataMinerSystem.Common
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class DmsStandaloneParameterMockTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void Id_ReturnsParameterId()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.AreEqual(1001, parameter.Id);
        }

        [TestMethod]
        public void Element_ReturnsOwningElement()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Assert
            Assert.AreSame(mock.Object, parameter.Element);
        }

        [TestMethod]
        public void GetValue_DefaultValue_ReturnsProtocolDefault()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            var value = mock.Object.GetStandaloneParameter<double?>(1000).GetValue();

            // Assert
            Assert.AreEqual(10.0, value);
        }

        [TestMethod]
        public void SetValue_ThenGetValue_ReturnsSetStringValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Act
            parameter.SetValue("new value");

            // Assert
            Assert.AreEqual("new value", parameter.GetValue());
        }

        [TestMethod]
        public void SetValue_ThenGetValue_ReturnsSetDoubleValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<double?>(1000);

            // Act
            parameter.SetValue(42.5);

            // Assert
            Assert.AreEqual(42.5, parameter.GetValue());
        }

        [TestMethod]
        public void SetValue_ThenGetValue_ReturnsSetNullableIntValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<int?>(800);

            // Act
            parameter.SetValue(7);

            // Assert
            Assert.AreEqual(7, parameter.GetValue());
        }

        [TestMethod]
        public void SetValue_WithExpectedChangesOverload_PersistsValue()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            // Act
            parameter.SetValue("changed", System.TimeSpan.FromSeconds(1), null);

            // Assert
            Assert.AreEqual("changed", parameter.GetValue());
        }

        [TestMethod]
        public void SetValue_ValueSetOnDifferentInstance_IsPersisted()
        {
            // Arrange
            var mock = new IDmsElementMock(path);

            // Act
            mock.Object.GetStandaloneParameter<string>(1001).SetValue("persisted");

            // Assert
            Assert.AreEqual("persisted", mock.Object.GetStandaloneParameter<string>(1001).GetValue());
        }

        [TestMethod]
        public void StartValueMonitor_InvokesCallbackOnChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, false);

            // Act
            parameter.SetValue("monitored value");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("monitored value", received.Value);
            Assert.AreEqual("source", received.MonitorSource);
        }

        [TestMethod]
        public void StartValueMonitor_WithTimeSpanOverload_InvokesCallbackOnChange()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, System.TimeSpan.FromSeconds(1), false);

            // Act
            parameter.SetValue("monitored value");

            // Assert
            Assert.IsNotNull(received);
            Assert.AreEqual("monitored value", received.Value);
        }

        [TestMethod]
        public void StopValueMonitor_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, false);
            parameter.StopValueMonitor("source", false);

            // Act
            parameter.SetValue("value after stop");

            // Assert
            Assert.IsNull(received);
        }

        [TestMethod]
        public void StopValueMonitor_WithTimeSpanOverload_DoesNotInvokeCallbackAfterStop()
        {
            // Arrange
            var mock = new IDmsElementMock(path);
            var parameter = mock.Object.GetStandaloneParameter<string>(1001);

            ParamValueChange<string> received = null;
            parameter.StartValueMonitor("source", change => received = change, false);
            parameter.StopValueMonitor("source", System.TimeSpan.FromSeconds(1), false);

            // Act
            parameter.SetValue("value after stop");

            // Assert
            Assert.IsNull(received);
        }
    }
}
