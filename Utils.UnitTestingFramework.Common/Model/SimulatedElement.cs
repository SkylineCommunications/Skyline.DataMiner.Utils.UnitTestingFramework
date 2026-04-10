namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Linq;
    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;
    using AlarmLevel = Net.Messages.AlarmLevel;
    using ElementState = Net.Messages.ElementState;

    public sealed class SimulatedElement
    {
        private readonly ParametersAndTables parametersAndTables = new ParametersAndTables();

        public SimulatedElement(SimulatedDma dma, int elementId, string name, string protocolName, string protocolVersion)
        {
            Dma = dma ?? throw new ArgumentNullException(nameof(dma));
            ElementId = elementId;
            Name = name;
            ProtocolName = protocolName;
            ProtocolVersion = protocolVersion;

            foreach (var table in parametersAndTables.Tables.Values)
            {
                table.TableChanged += Table_TableChanged;
            }
        }

        private void Table_TableChanged(object sender, TableChangedEventArgs e)
        {
            var table = (ITableModel)sender;

            var message = new ParameterTableUpdateEventMessage(DmaId, ElementId, table.TableId)
            {
                UpdatedRows = e.ChangedRows.Where(r => r.ChangeType == RowChangeType.Added || r.ChangeType == RowChangeType.Updated).Select(r => ToParameterValue(r.Row)).ToArray(),
                DeletedRows = e.ChangedRows.Where(r => r.ChangeType == RowChangeType.Deleted).Select(r => r.PrimaryKey).ToArray(),
            };

            Dma.NotifySubscriptions(message);
        }

        public SimulatedDma Dma { get; }

        public int ElementId { get; }

        public int DmaId => Dma.DmaId;

        public int HostingDmaId => DmaId;

        public DmsElementId Id => new DmsElementId(DmaId, ElementId);

        public string Name { get; }

        public string ProtocolName { get; }

        public string ProtocolVersion { get; }

        public ElementState State { get; private set; } = ElementState.Active;

        public void Start()
        {
            if (State != ElementState.Active)
            {
                State = ElementState.Active;

                // send events
                var e1 = new ElementStateEventMessage(DmaId, ElementId, ElementState.Active, AlarmLevel.Normal);
                Dma.NotifySubscriptions(e1);

                var e2 = new ElementStateEventMessage(DmaId, ElementId, ElementState.Active, AlarmLevel.Normal)
                {
                    IsElementStartupComplete = true,
                };
                Dma.NotifySubscriptions(e2);
            }
        }

        public void Stop()
        {
            if (State != ElementState.Stopped)
            {
                State = ElementState.Stopped;

                // send event
                var e = new ElementStateEventMessage(DmaId, ElementId, ElementState.Stopped, AlarmLevel.Normal);
                Dma.NotifySubscriptions(e);
            }
        }

        public bool TryGetTable(int tableId, out ITableModel table)
        {
            return parametersAndTables.TryGetTable(tableId, out table);
        }

        public bool TryGetParameter(int parameterId, out IParameterModel parameter)
        {
            return parametersAndTables.TryGetParameter(parameterId, out parameter);
        }

        internal LiteElementInfoEvent ToLiteElementInfo()
        {
            return new LiteElementInfoEvent
            {
                DataMinerID = DmaId,
                HostingAgentID = HostingDmaId,
                ElementID = ElementId,
                Name = Name,
                Protocol = ProtocolName,
                ProtocolVersion = ProtocolVersion,
                State = State,
            };
        }

        internal ElementInfoEventMessage ToElementInfo()
        {
            return new ElementInfoEventMessage
            {
                DataMinerID = DmaId,
                HostingAgentID = HostingDmaId,
                ElementID = ElementId,
                Name = Name,
                Protocol = ProtocolName,
                ProtocolVersion = ProtocolVersion,
                State = State,
            };
        }

        internal bool TryGetSpecialParameterValue(int parameterId, out ParameterValue specialValue)
        {
            switch (parameterId)
            {
                case 65003: // Number of active alarms
                    specialValue = new ParameterValue(0);
                    return true;
                case 65004: // Number of critical alarms
                    specialValue = new ParameterValue(0);
                    return true;
                case 65005: // Number of major alarms
                    specialValue = new ParameterValue(0);
                    return true;
                case 65006: // Number of minor alarms
                    specialValue = new ParameterValue(0);
                    return true;
                case 65007: // Number of warning alarms
                    specialValue = new ParameterValue(0);
                    return true;
                default:
                    specialValue = null;
                    return false;
            }
        }

        private ParameterValue ToParameterValue(object[] row)
        {
            var columnCount = row.Length;

            if (columnCount == 0)
            {
                // Create at least one column that represents the keys.
                columnCount = 1;
            }

            var cells = new object[columnCount];

            for (int c = 0; c < columnCount; c++)
            {
                var cellData = new object[7];
                cellData[0] = c < row.Length ? row[c] : null;

                cells[c] = cellData;
            }

            return ParameterValue.Compose(new[] { cells });
        }
    }
}
