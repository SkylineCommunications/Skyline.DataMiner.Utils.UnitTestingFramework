namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.Advanced;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    public class SimulatedDms
    {
        private readonly ConcurrentDictionary<int, SimulatedDma> _agents = new ConcurrentDictionary<int, SimulatedDma>();
        private readonly ConcurrentBag<SLNetConnectionMock> _connections = new ConcurrentBag<SLNetConnectionMock>();

        public SimulatedDms()
        {
        }

        public IReadOnlyDictionary<int, SimulatedDma> Agents => _agents;

        public SimulatedDma GetOrCreateAgent(int dmaId)
        {
            return _agents.GetOrAdd(
                dmaId,
                id => new SimulatedDma(this, id));
        }

        public SLNetConnectionMock CreateConnection()
        {
            var connection = new SLNetConnectionMock(this);
            _connections.Add(connection);

            return connection;
        }

        protected internal void NotifySubscriptions(EventMessage eventMessage)
        {
            if (eventMessage is null)
            {
                throw new ArgumentNullException(nameof(eventMessage));
            }

            foreach (var connection in _connections)
            {
                connection.NotifySubscriptions(eventMessage);
            }
        }

        protected virtual internal bool TryHandleMessage(DMSMessage message, out IEnumerable<DMSMessage> responses)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            switch (message)
            {
                case GetLiteElementInfo msg:
                    responses = HandleGetLiteElementInfoMessage(msg);
                    return true;

                case GetElementByIDMessage msg:
                    responses = HandleGetElementByIDMessage(msg);
                    return true;

                case GetElementByNameMessage msg:
                    responses = HandleGetElementByNameMessage(msg);
                    return true;

                case GetPartialTableMessage msg:
                    responses = HandleGetPartialTableMessage(msg);
                    return true;

                case GetInfoMessage msg:
                    responses = HandleGetInfoMessage(msg);
                    return true;

                case GetDataMinerByIDMessage msg:
                    responses = HandleGetDataMinerByIDMessage(msg);
                    return true;

                case GetAgentBuildInfo msg:
                    responses = HandleGetAgentBuildInfo(msg);
                    return true;

                case GetParameterMessage msg:
                    responses = HandleGetParameterMessage(msg);
                    return true;

                case SetDataMinerInfoMessage msg:
                    responses = HandleSetDataMinerInfoMessage(msg);
                    return true;

                default:
                    responses = new DMSMessage[0];
                    return false;
            }
        }

        protected IEnumerable<DMSMessage> HandleGetLiteElementInfoMessage(GetLiteElementInfo msg)
        {
            IEnumerable<SimulatedElement> elements = Agents.Values.SelectMany(x => x.Elements.Values);

            if (!String.IsNullOrEmpty(msg.ProtocolName))
            {
                elements = elements.Where(x => String.Equals(x.ProtocolName, msg.ProtocolName));
            }

            foreach (SimulatedElement element in elements)
            {
                yield return element.ToLiteElementInfo();
            }
        }

        protected IEnumerable<DMSMessage> HandleGetElementByIDMessage(GetElementByIDMessage msg)
        {
            if (Agents.TryGetValue(msg.DataMinerID, out SimulatedDma dma) &&
                dma.Elements.TryGetValue(msg.ElementID, out SimulatedElement element))
            {
                yield return element.ToElementInfo();
            }
        }

        protected IEnumerable<DMSMessage> HandleGetElementByNameMessage(GetElementByNameMessage msg)
        {
            IEnumerable<SimulatedElement> elements = Agents.Values.SelectMany(x => x.Elements.Values);
            SimulatedElement element = elements.FirstOrDefault(x => String.Equals(x.Name, msg.ElementName));

            if (element != null)
            {
                yield return element.ToElementInfo();
            }
        }

        protected IEnumerable<DMSMessage> HandleGetPartialTableMessage(GetPartialTableMessage msg)
        {
            if (Agents.TryGetValue(msg.DataMinerID, out SimulatedDma dma) &&
                dma.Elements.TryGetValue(msg.ElementID, out SimulatedElement element) &&
                element.TryGetTable(msg.ParameterID, out ITableModel table))
            {
                yield return new ParameterChangeEventMessage(msg.DataMinerID, msg.ElementID, msg.ParameterID)
                {
                    NewValue = table.ToParameterValue(),
                };
            }
            else
            {
                throw new InvalidOperationException($"Element with ID {msg.ElementID} not found in DMA {msg.DataMinerID} or table with ID {msg.ParameterID} not found.");
            }
        }

        protected IEnumerable<DMSMessage> HandleGetParameterMessage(GetParameterMessage msg)
        {
            if (!Agents.TryGetValue(msg.DataMinerID, out SimulatedDma dma) ||
                !dma.Elements.TryGetValue(msg.ElId, out SimulatedElement element))
            {
                throw new InvalidOperationException($"Element with ID {msg.ElId} not found in DMA {msg.DataMinerID}.");
            }

            Net.Messages.ParameterValue paramValue;

            if (element.TryGetSpecialParameterValue(msg.ParameterId, out var specialValue))
            {
                paramValue = specialValue;
            }
            else if (element.TryGetParameter(msg.ParameterId, out IParameterModel param))
            {
                paramValue = param.ToParameterValue();
            }
            else
            {
                throw new InvalidOperationException($"Parameter with ID {msg.ParameterId} not found in Element {msg.ElId} on DMA {msg.DataMinerID}.");
            }

            yield return new GetParameterResponseMessage
            {
                DataMinerID = msg.DataMinerID,
                ElId = msg.ElId,
                ParameterId = msg.ParameterId,
                Value = paramValue,
            };
        }

        protected IEnumerable<DMSMessage> HandleSetDataMinerInfoMessage(SetDataMinerInfoMessage msg)
        {
            switch ((NotifyType)msg.What)
            {
                case NotifyType.GetKeyPosition:
                    {
                        int[] ids = (int[])msg.Var1;
                        string key = (string)msg.Var2;

                        if (Agents.TryGetValue(ids[0], out SimulatedDma dma) &&
                            dma.Elements.TryGetValue(ids[1], out SimulatedElement element) &&
                            element.TryGetTable(ids[2], out ITableModel table))
                        {
                            int index = table.GetRowIndex(key);

                            yield return new SetDataMinerInfoResponseMessage
                            {
                                RawData = index + 1,
                            };
                        }
                        else
                        {
                            throw new InvalidOperationException($"Element with ID {ids[1]} not found in DMA {ids[0]} or table with ID {ids[2]} not found.");
                        }
                    }

                    break;

                case NotifyType.NT_GET_ROW:
                    {
                        object[] var1 = (object[])msg.Var1;

                        if (Agents.TryGetValue((int)var1[0], out SimulatedDma dma) &&
                            dma.Elements.TryGetValue((int)var1[1], out SimulatedElement element) &&
                            element.TryGetTable((int)var1[2], out ITableModel table))
                        {
                            var row = table.GetRow((string)var1[3]);

                            yield return new SetDataMinerInfoResponseMessage
                            {
                                RawData = row,
                            };
                        }
                        else
                        {
                            throw new InvalidOperationException($"Element with ID {var1[1]} not found in DMA {var1[0]} or table with ID {var1[2]} not found.");
                        }
                    }

                    break;

                default:
                    throw new NotSupportedException($"NotifyType '{msg.What}' is not supported.");
            }
        }

        protected IEnumerable<DMSMessage> HandleGetInfoMessage(GetInfoMessage msg)
        {
            switch (msg.Type)
            {
                case InfoType.DataMinerInfo:
                    return HandleDataMinerInfoMessage();

                case InfoType.ElementInfo:
                    return HandleElementInfoMessage();

                default:
                    throw new NotSupportedException("Not Supported");
            }
        }

        protected IEnumerable<DMSMessage> HandleElementInfoMessage()
        {
            foreach (SimulatedElement element in Agents.Values.SelectMany(agent => agent.Elements.Values))
            {
                yield return new ElementInfoEventMessage
                {
                    Name = element.Name,
                    Protocol = element.ProtocolName,
                    ProtocolVersion = element.ProtocolVersion,
                    DataMinerID = element.DmaId,
                    ElementID = element.ElementId,
                    State = element.State,
                    HostingAgentID = element.HostingDmaId,
                };
            }
        }

        protected IEnumerable<DMSMessage> HandleGetDataMinerByIDMessage(GetDataMinerByIDMessage msg)
        {
            yield return new GetDataMinerInfoResponseMessage
            {
                ComputerName = $"SimulatedHost{msg.ID}",
                Name = $"Simulated Agent {msg.ID}",
            };
        }

        protected IEnumerable<DMSMessage> HandleGetAgentBuildInfo(GetAgentBuildInfo msg)
        {
            yield return new BuildInfoResponse
            {
                Agents = new[]
                {
                    new BuildInfoAgent
                    {
                        RawVersion = "10.5.6",
                        DataMinerID = msg.DataMinerID,
                    },
                },
            };
        }

        protected IEnumerable<DMSMessage> HandleDataMinerInfoMessage()
        {
            foreach (KeyValuePair<int, SimulatedDma> simulatedDma in Agents)
            {
                yield return new GetDataMinerInfoResponseMessage
                {
                    ID = simulatedDma.Key,
                };
            }
        }
    }
}
