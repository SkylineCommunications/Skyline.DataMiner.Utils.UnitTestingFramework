namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation;

    [TestClass]
    public class ArrayHandlerTests
    {
        [TestMethod]
        [DeploymentItem(@"TestFiles\Model\Data\protocol.xml")]
        public void LoadDefaultForParameterTest_ValidInputTable_IsEqual()
        {
            // Arrange
            var path = @"protocol.xml";
            var protocolModel = ProtocolModelBuilder.Build(path);
            var parameter = protocolModel.Protocol.Params.FirstOrDefault(x => x.Id.Value == 900);
            Assert.IsNotNull(parameter);
            HashSet<int> excludedPids = new HashSet<int>();

            // Act
            var arrayHandler = new TableModelCreator(excludedPids);
            var tableModel = arrayHandler.CreateTableModelFromArrayOptions(parameter);

            // Assert
            Assert.AreEqual(5, tableModel.ColumnIndexesToPids.Count);
            Assert.AreEqual(901, tableModel.ColumnIndexesToPids[0]);
            Assert.AreEqual(902, tableModel.ColumnIndexesToPids[1]);
            Assert.AreEqual(903, tableModel.ColumnIndexesToPids[2]);
            Assert.AreEqual(904, tableModel.ColumnIndexesToPids[3]);
            Assert.AreEqual(905, tableModel.ColumnIndexesToPids[4]);
            Assert.AreEqual(5, tableModel.GetColumnCount());
            Assert.AreEqual(900, tableModel.TableId);
            Assert.AreEqual(0, tableModel.PrimaryKeyColumn);
            Assert.AreEqual(901, tableModel.ColumnIndexesToPids[tableModel.PrimaryKeyColumn]);
        }

        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol_With_Failures.xml")]
        public void LoadDefaultForParameterTest_SamePidDifferentIdx_AddsTwoEntries()
        {
            // Arrange
            var path = @"protocol_With_Failures.xml";
            var protocolModel = ProtocolModelBuilder.Build(path);
            var parameter = protocolModel.Protocol.Params.FirstOrDefault(x => x.Id.Value == 910);
            Assert.IsNotNull(parameter);
            HashSet<int> excludedPids = new HashSet<int>();

            // Act
            var arrayHandler = new TableModelCreator(excludedPids);
            var tableModel = arrayHandler.CreateTableModelFromArrayOptions(parameter);

            // Assert
            Assert.AreEqual(2, tableModel.ColumnIndexesToPids.Count);
            Assert.AreEqual(901, tableModel.ColumnIndexesToPids[1]);
            Assert.AreEqual(901, tableModel.ColumnIndexesToPids[2]);
            Assert.AreEqual(1, tableModel.GetColumnCount());
            Assert.AreEqual(910, tableModel.TableId);
            Assert.AreEqual(1, tableModel.PrimaryKeyColumn);
            Assert.AreEqual(901, tableModel.ColumnIndexesToPids[tableModel.PrimaryKeyColumn]);
        }

        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol_With_Failures.xml")]
        public void LoadDefaultForParameterTest_SameIdxDifferentPid_AddsFirstEntry()
        {
            // Arrange
            var path = @"protocol_With_Failures.xml";
            var protocolModel = ProtocolModelBuilder.Build(path);
            var parameter = protocolModel.Protocol.Params.FirstOrDefault(x => x.Id.Value == 920);
            Assert.IsNotNull(parameter);
            HashSet<int> excludedPids = new HashSet<int>();

            // Act
            var arrayHandler = new TableModelCreator(excludedPids);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => arrayHandler.CreateTableModelFromArrayOptions(parameter));
        }

        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol_With_Failures.xml")]
        public void LoadDefaultForParameterTest_NoIdxCorrespondingToPK_()
        {
            // Arrange
            var path = @"protocol_With_Failures.xml";
            var protocolModel = ProtocolModelBuilder.Build(path);
            var parameter = protocolModel.Protocol.Params.FirstOrDefault(x => x.Id.Value == 930);
            Assert.IsNotNull(parameter);
            HashSet<int> excludedPids = new HashSet<int>();

            // Act
            var arrayHandler = new TableModelCreator(excludedPids);

            // Act & Assert
            Assert.ThrowsExactly<InvalidOperationException>(
                () => arrayHandler.CreateTableModelFromArrayOptions(parameter));
        }
    }
}