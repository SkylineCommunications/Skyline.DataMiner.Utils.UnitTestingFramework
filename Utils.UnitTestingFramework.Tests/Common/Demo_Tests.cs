namespace Utils.UnitTestingFramework.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;

    [TestClass]
    public class Demo_Tests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        public void Demo_MonitorParameter_ArrangeViaProtocolParsing()
        {
            // Arrange: define how your DataMiner system looks like
            var simulatedDms = new SimulatedDms();

            var simulatedDma = simulatedDms.GetOrCreateAgent(1);

            var simulatedElement = simulatedDma.CreateElementBasedOnProtocolXml(1, "Element Name", "/custom/path/to/protocol.xml"); // at this point, all parameters and tables exist.

            var parameter55Model = simulatedElement.GetParameter(parameterId: 55);

            parameter55Model.Update(value: 123); // at this point, the parameter value is changed, but the element is not active yet, so no events will be sent to the IConnections subscribed to this element.

            simulatedElement.Start(); // at this point, the element is active and will report changes to its parameters and tables to all of the IConnections subscribed to it.
            
            var connection = simulatedDms.CreateConnection();

            // Act - Step 1: prepare the code-under-test, which uses the IDms, IDmsElement, ... from the class library
            var dms = connection.GetDms();

            var element = dms.GetElement(simulatedElement.Id);

            var dmsStandaloneParameter55 = element.GetStandaloneParameter<double?>(parameterId: 55);

            var newParameter55Values = new List<double?>();
            dmsStandaloneParameter55.StartValueMonitor("irrelevant monitor id", (ParamValueChange<double?> change) =>
            {
                newParameter55Values.Add(change.Value);
            });

            // Act - Step 2: simulate a change on the parameter by an outside source.
            parameter55Model.Update(value: 456);

            // Assert: verify that the code-under-test received the change to the parameter value.
            newParameter55Values.Should().ContainInOrder(123 /*initial value*/, 456 /*new value*/);
        }

        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        public void Demo_MonitorTable_ArrangeViaProtocolParsing()
        {
            // Arrange: define how your DataMiner system looks like
            var simulatedDms = new SimulatedDms();

            var simulatedDma = simulatedDms.GetOrCreateAgent(1);

            var simulatedElement = simulatedDma.CreateElementBasedOnProtocolXml(1, "Element Name", "/custom/path/to/protocol.xml"); // at this point, all parameters and tables exist.

            var table66Model = simulatedElement.GetTable(tableId: 66);

            var rowToAdd = new object[table66Model.Schema.ColumnCount];
            rowToAdd[table66Model.Schema.PrimaryKeyColumn.Idx] = "primary key value";

            table66Model.SetRow(rowToAdd);

            simulatedElement.Start(); // at this point, the element is active and will report changes to its parameters and tables to all of the IConnections subscribed to it.

            var connection = simulatedDms.CreateConnection();

            // Act - Step 1: prepare the code-under-test, which uses the IDms, IDmsElement, ... from the class library
            var dms = connection.GetDms();

            var element = dms.GetElement(simulatedElement.Id);

            var dmsTable66 = element.GetTable(tableId: 66);

            bool tableChangedEventReceived = false;
            dmsTable66.StartValueMonitor("irrelevant monitor id", table66Model.Schema.PrimaryKeyColumn.Idx, (TableValueChange change) =>
            {
                tableChangedEventReceived = true;
            });

            // Act - Step 2: simulate a change on the table by an outside source.
            table66Model.SetCell(primaryKey: "primary key value", columnPid: table66Model.Schema.FindColumnDefinitionByIdx(3).Pid, value: "new value for third column");

            // Assert: verify that the code-under-test received the change to the table.
            tableChangedEventReceived.Should().BeTrue();
        }
    }
}
