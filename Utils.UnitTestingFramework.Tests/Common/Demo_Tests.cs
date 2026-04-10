namespace Utils.UnitTestingFramework.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    [TestClass]
    public class Demo_Tests
    {
        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        public void Demo_SetParameter_ArrangeViaProtocolParsing()
        {
            // Arrange: define how your DataMiner system looks like
            var simulatedDms = new SimulatedDms();

            var simulatedDma = simulatedDms.GetOrCreateAgent(1);

            var simulatedElement = simulatedDma.CreateElementBasedOnProtocolXml(1, "Element Name", "/custom/path/to/protocol.xml"); // at this point, all parameters and tables exist.

            var parameter55Model = simulatedElement.GetParameter(parameterId: 55);

            parameter55Model.Update(value: 123); // at this point, the parameter value is changed, but the element is not active yet, so no events will be sent to the IConnections subscribed to this element.

            simulatedElement.Start(); // at this point, the element is active and will report changes to its parameters and tables to all of the IConnections subscribed to it.

            var connection = simulatedDms.CreateConnection();

            // Act
            var dms = connection.GetDms();

            CodeUnderTest_SetParameter.Execute(dms, simulatedElement.Id, parameterId: 55, valueToSet: 456);

            // Assert
            parameter55Model.Value.Should().Be(456 /*new value*/);
        }

        [TestMethod]
        [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
        public void Demo_AddRow_ArrangeViaProtocolParsing()
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

            // Act
            var dms = connection.GetDms();

            CodeUnderTest_AddRow.Execute(dms, simulatedElement.Id, tableId: 66);

            // Assert
            table66Model.GetRow("PK").Should().NotBeNullOrEmpty();
        }

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

            var codeUnderTest = new CodeUnderTest_MonitorParameter();
            codeUnderTest.MonitorParameter(dms, simulatedElement.Id, parameterId: 55);

            // Act - Step 2: simulate a change on the parameter by an outside source.
            parameter55Model.Update(value: 456);

            // Assert: verify that the code-under-test received the change to the parameter value.
            codeUnderTest.Parameter55Values.Should().ContainInOrder(123 /*initial value*/, 456 /*new value*/);
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

            var codeUnderTest = new CodeUnderTest_MonitorTable();
            codeUnderTest.MonitorTable(dms, simulatedElement.Id, tableId: 66, primaryKeyColumnIdx: table66Model.Schema.PrimaryKeyColumn.Idx);

            // Act - Step 2: simulate a change on the table by an outside source.
            table66Model.SetCell(primaryKey: "primary key value", columnPid: table66Model.Schema.FindColumnDefinitionByIdx(3).Pid, value: "new value for third column");

            // Assert: verify that the code-under-test received the change to the table.
            codeUnderTest.TableChangedEventReceived.Should().BeTrue();
        }

        private static class CodeUnderTest_SetParameter
        {
            public static void Execute(IDms dms, DmsElementId elementId, int parameterId, double? valueToSet)
            {
                var element = dms.GetElement(elementId);

                var dmsStandaloneParameter55 = element.GetStandaloneParameter<double?>(parameterId);

                dmsStandaloneParameter55.SetValue(valueToSet);
            }
        }

        private static class CodeUnderTest_AddRow
        {
            public static void Execute(IDms dms, DmsElementId elementId, int tableId)
            {
                var element = dms.GetElement(elementId);
                var dmsTable66 = element.GetTable(tableId);

                var newRow = new object[4];

                dmsTable66.AddRow(newRow);
            }
        }

        private class CodeUnderTest_MonitorParameter
        {
            public List<double?> Parameter55Values { get; } = new List<double?>();

            public void MonitorParameter(IDms dms, DmsElementId elementId, int parameterId)
            {
                var element = dms.GetElement(elementId);

                var dmsStandaloneParameter55 = element.GetStandaloneParameter<double?>(parameterId);

                dmsStandaloneParameter55.StartValueMonitor("irrelevant monitor id", (ParamValueChange<double?> change) =>
                {
                    Parameter55Values.Add(change.Value);
                });
            }
        }

        private class CodeUnderTest_MonitorTable
        {
            public bool TableChangedEventReceived { get; private set; } = false;

            public void MonitorTable(IDms dms, DmsElementId elementId, int tableId, int primaryKeyColumnIdx)
            {
                var element = dms.GetElement(elementId);

                var dmsTable66 = element.GetTable(tableId);

                dmsTable66.StartValueMonitor("irrelevant monitor id", primaryKeyColumnIdx, (TableValueChange change) =>
                {
                    TableChangedEventReceived = true;
                });
            }
        }


        public void Demo_MonitorParameter_ArrangeManually()
        {
            // Arrange: define how your DataMiner system looks like
            var simulatedDms = new SimulatedDms();

            var simulatedDma = simulatedDms.GetOrCreateAgent(1);

            var simulatedElement = simulatedDma.CreateElement(1, "Element Name", "Protocol Name"); // at this point, NO parameters and tables exist.

            var parameter55Definition = new ParameterDefinition(name: "parameter name", pid: 55, type: typeof(double?)); // need to define the definition yourself

            var parameter55Model = simulatedElement.AddParameter(parameter55Definition);

            parameter55Model.Update(value: 123); // at this point, the parameter value is changed, but the element is not active yet, so no events will be sent to the IConnections subscribed to this element.

            simulatedElement.Start(); // at this point, the element is active and will report changes to its parameters and tables to all of the IConnections subscribed to it.

            var connection = simulatedDms.CreateConnection();

            // Act - Step 1: prepare the code-under-test, which uses the IDms, IDmsElement, ... from the class library
            var dms = connection.GetDms();

            var codeUnderTest = new CodeUnderTest_MonitorParameter();
            codeUnderTest.MonitorParameter(dms, simulatedElement.Id, parameterId: 55);

            // Act - Step 2: simulate a change on the parameter by an outside source.
            parameter55Model.Update(value: 456);

            // Assert: verify that the code-under-test received the change to the parameter value.
            codeUnderTest.Parameter55Values.Should().ContainInOrder(123 /*initial value*/, 456 /*new value*/);
        }

        [TestMethod]
        public void Demo_MonitorTable_ArrangeManually()
        {
            // Arrange: define how your DataMiner system looks like
            var simulatedDms = new SimulatedDms();

            var simulatedDma = simulatedDms.GetOrCreateAgent(1);

            var simulatedElement = simulatedDma.CreateElement(1, "Element Name", "protocol name"); // at this point, NO parameters and tables exist.

            var primaryKeyColumnDefinition = new ColumnDefinition(name: "primary key column", pid: 1, type: typeof(string), idx: 0);
            var secondColumnDefinition = new ColumnDefinition(name: "second column", pid: 2, type: typeof(string), idx: 1);
            var thirdColumnDefinition = new ColumnDefinition(name: "third column", pid: 3, type: typeof(string), idx: 2);

            var tableSchema = new TableSchema(new[] { primaryKeyColumnDefinition, secondColumnDefinition, thirdColumnDefinition }, primaryKeyColumnDefinition);

            var table66Model = simulatedElement.AddTable(tableId: 66, tableSchema);

            var rowToAdd = new object[table66Model.Schema.ColumnCount];
            rowToAdd[table66Model.Schema.PrimaryKeyColumn.Idx] = "primary key value";

            table66Model.SetRow(rowToAdd);

            simulatedElement.Start(); // at this point, the element is active and will report changes to its parameters and tables to all of the IConnections subscribed to it.

            var connection = simulatedDms.CreateConnection();

            // Act - Step 1: prepare the code-under-test, which uses the IDms, IDmsElement, ... from the class library
            var dms = connection.GetDms();

            var codeUnderTest = new CodeUnderTest_MonitorTable();
            codeUnderTest.MonitorTable(dms, simulatedElement.Id, tableId: 66, primaryKeyColumnIdx: table66Model.Schema.PrimaryKeyColumn.Idx);

            // Act - Step 2: simulate a change on the table by an outside source.
            table66Model.SetCell(primaryKey: "primary key value", columnPid: table66Model.Schema.FindColumnDefinitionByIdx(2).Pid, value: "new value for third column");

            // Assert: verify that the code-under-test received the change to the table.
            codeUnderTest.TableChangedEventReceived.Should().BeTrue();
        }

    }
}
