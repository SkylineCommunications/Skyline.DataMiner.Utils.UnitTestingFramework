namespace Utils.UnitTestingFramework.Tests.Protocol.SLProtocolExt
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class SLProtocolExt_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void SLProtocolExt_SetParameterViaExtended()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            // Act
            protocolMock.Object.StringParameter = "42";

            // Assert
            Assert.AreEqual("42", protocolMock.Object.StringParameter);
            Assert.AreEqual("42", protocolMock.Object.GetParameter(1001));
        }

        [TestMethod]
        public void SLProtocolExt_SetParameterViaNonExtended()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            // Act
            protocolMock.Object.SetParameter(1001, "42");

            // Assert
            Assert.AreEqual("42", protocolMock.Object.StringParameter);
            Assert.AreEqual("42", protocolMock.Object.GetParameter(1001));
        }

        [TestMethod]
        public void SLProtocolExt_GetParameterViaExtended()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            protocolMock.Object.SetParameter(1001, "42");

            // Act
            var parameterValue = protocolMock.Object.StringParameter;

            // Assert
            parameterValue.Should().Be("42");
        }

        [TestMethod]
        public void SLProtocolExt_GetParameterViaNonExtended()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            protocolMock.Object.SetParameter(1001, "42");

            // Act
            var parameterValue = protocolMock.Object.GetParameter(1001);

            // Assert
            parameterValue.Should().Be("42");
        }

        [TestMethod]
        public void SLProtocolExt_QActionTable_AddRow()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            // Act
            var rowToAdd = new PollingConfigurationQActionRow
            {
                Pollingconfigurationinstance_901 = "key",
                Pollingconfigurationdescription_902 = "description",
                Pollingconfigurationperiod_903 = 30,
                Pollingconfigurationlastpolled_904 = 12,
                Pollingconfigurationconnectionid_905 = 1,
            };

            protocolMock.Object.PollingConfiguration.AddRow(rowToAdd);

            // Assert
            protocolMock.Assert().Table(900).Row<PollingConfigurationQActionRow>("key").Should().BeEquivalentTo(rowToAdd);
        }

        [TestMethod]
        public void SLProtocolExt_QActionTable_GetRow()
        {
            // Arrange
            var protocolMock = new SLProtocolMock<ConcreteSLProtocolExt>(path);

            var addedRow = new PollingConfigurationQActionRow
            {
                Pollingconfigurationinstance_901 = "key",
                Pollingconfigurationdescription_902 = "description",
                Pollingconfigurationperiod_903 = 30,
                Pollingconfigurationlastpolled_904 = 12,
                Pollingconfigurationconnectionid_905 = 1,
            };

            protocolMock.Object.PollingConfiguration.AddRow(addedRow);

            // Act
            var row = protocolMock.Object.PollingConfiguration.GetRow("key");

            // Assert
            row.Should().Equal(addedRow.ToObjectArray());
        }
    }
}