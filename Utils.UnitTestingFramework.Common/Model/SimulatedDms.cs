namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
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

        internal void NotifySubscriptions(EventMessage eventMessage)
        {
            if (eventMessage is null)
            {
                throw new ArgumentNullException(nameof(eventMessage));
            }

            foreach (SLNetConnectionMock connection in _connections)
            {
                connection.NotifySubscriptions(eventMessage);
            }
        }

        protected virtual IEnumerable<DMSMessage> HandleMessage(DMSMessage message)
        {
            // This method can be overridden in a subclass to provide custom handling logic for messages that are not handled by TryHandleMessage.
            // By default, it does nothing and returns an empty enumeration, indicating that the message was not handled.
            return Enumerable.Empty<DMSMessage>();
        }

        protected virtual bool TryHandleDomMessage(DMSMessage message, out DMSMessage response)
        {
            // This method can be overridden in a subclass to provide custom handling logic for DOM messages.
            // By default, it does not handle any messages and returns false.
            response = null;
            return false;
        }

        internal bool TryHandleMessage(DMSMessage message, out IEnumerable<DMSMessage> responses)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (TryHandleDomMessage(message, out var response))
            {
                responses = new[] { response };
                return true;
            }

            switch (message)
            {
                case GetLiteElementInfo msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetElementByIDMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetElementByNameMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetPartialTableMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetInfoMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetDataMinerByIDMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetAgentBuildInfo msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetParameterMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case ExecuteScriptMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case GetScriptInfoMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                case CheckAutomationCSharpSyntaxMessage msg:
                    responses = HandleMessage(msg);
                    return true;

                default:
                    responses = HandleMessage(message);
                    return responses.Any();
            }
        }

        private IEnumerable<DMSMessage> HandleMessage(GetLiteElementInfo msg)
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

        private IEnumerable<DMSMessage> HandleMessage(GetElementByIDMessage msg)
        {
            if (Agents.TryGetValue(msg.DataMinerID, out SimulatedDma dma) &&
                dma.Elements.TryGetValue(msg.ElementID, out SimulatedElement element))
            {
                yield return element.ToElementInfo();
            }
        }

        private IEnumerable<DMSMessage> HandleMessage(GetElementByNameMessage msg)
        {
            IEnumerable<SimulatedElement> elements = Agents.Values.SelectMany(x => x.Elements.Values);
            SimulatedElement element = elements.FirstOrDefault(x => String.Equals(x.Name, msg.ElementName));

            if (element != null)
            {
                yield return element.ToElementInfo();
            }
        }

        private IEnumerable<DMSMessage> HandleMessage(GetPartialTableMessage msg)
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

        private IEnumerable<DMSMessage> HandleMessage(GetParameterMessage msg)
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

        private IEnumerable<DMSMessage> HandleMessage(SetDataMinerInfoMessage msg)
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

        private IEnumerable<DMSMessage> HandleMessage(GetInfoMessage msg)
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

        private IEnumerable<DMSMessage> HandleElementInfoMessage()
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

        private IEnumerable<DMSMessage> HandleMessage(GetDataMinerByIDMessage msg)
        {
            yield return new GetDataMinerInfoResponseMessage
            {
                ComputerName = $"SimulatedHost{msg.ID}",
                Name = $"Simulated Agent {msg.ID}",
            };
        }

        private IEnumerable<DMSMessage> HandleMessage(GetAgentBuildInfo msg)
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

        private IEnumerable<DMSMessage> HandleDataMinerInfoMessage()
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
