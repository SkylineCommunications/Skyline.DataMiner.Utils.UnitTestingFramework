namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Skyline.DataMiner.Net.Messages;

    public class SimulatedDma
    {
        public SimulatedDma(SimulatedDms dms, int dmaId)
        {
            Dms = dms ?? throw new ArgumentNullException(nameof(dms));
            DmaId = dmaId;
        }

        public SimulatedDms Dms { get; }

        public int DmaId { get; }

        public IReadOnlyDictionary<int, SimulatedElement> Elements => ElementsInternal;

        protected ConcurrentDictionary<int, SimulatedElement> ElementsInternal { get; } = new ConcurrentDictionary<int, SimulatedElement>();

        public SimulatedElement CreateElement(int elementId, string name, string protocolName, string protocolVersion = "1.0.0.1")
        {
            var element = new SimulatedElement(this, elementId, name, protocolName, protocolVersion);

            if (!ElementsInternal.TryAdd(elementId, element))
            {
                throw new InvalidOperationException($"Element with ID {elementId} already exists.");
            }

            return element;
        }

        public SimulatedElement CreateElementBasedOnProtocolXml(int elementId, string name, string pathToProtocolXml)
        {
            var protocolModel = ProtocolModelBuilder.Build(pathToProtocolXml);

            var parametersAndTables = ParametersAndTablesBuilder.Build(protocolModel);

            var element = new SimulatedElement(this, elementId, name, protocolModel.Protocol.Name.Value, protocolModel.Protocol.Version.Value, parametersAndTables);

            if (!ElementsInternal.TryAdd(elementId, element))
            {
                throw new InvalidOperationException($"Element with ID {elementId} already exists.");
            }

            return element;
        }

        protected internal void NotifySubscriptions(EventMessage e)
        {
            Dms.NotifySubscriptions(e);
        }
    }
}
