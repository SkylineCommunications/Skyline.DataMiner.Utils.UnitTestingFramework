namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Connection
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.ToolsSpace.Collections;

    internal class SubscriptionSet
    {
        public SubscriptionSet(string setId)
        {
            SetId = setId;
        }

        public string SetId { get; }

        public ConcurrentHashSet<SubscriptionFilter> Filters { get; } = new ConcurrentHashSet<SubscriptionFilter>();
    }
}
