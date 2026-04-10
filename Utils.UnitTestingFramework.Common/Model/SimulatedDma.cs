namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using Skyline.DataMiner.Net.Messages;

    public sealed class SimulatedDma
    {
        private readonly ConcurrentDictionary<int, SimulatedElement> _elements = new ConcurrentDictionary<int, SimulatedElement>();

        public SimulatedDma(SimulatedDms dms, int dmaId)
        {
            Dms = dms ?? throw new ArgumentNullException(nameof(dms));
            DmaId = dmaId;
        }

        public SimulatedDms Dms { get; }

        public int DmaId { get; }

        public IReadOnlyDictionary<int, SimulatedElement> Elements => _elements;

        public SimulatedElement CreateElement(int elementId, string name, string protocolName, string protocolVersion = "1.0.0.1")
        {
            var element = new SimulatedElement(this, elementId, name, protocolName, protocolVersion);

            if (!_elements.TryAdd(elementId, element))
            {
                throw new InvalidOperationException($"Element with ID {elementId} already exists.");
            }

            return element;
        }

        internal void NotifySubscriptions(EventMessage e)
        {
            Dms.NotifySubscriptions(e);
        }
    }
}
