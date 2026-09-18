namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Net.ToolsSpace.Collections;

    internal sealed class SubscriptionSet
    {
        internal SubscriptionSet(string id)
        {
            SetId = id;
        }

        internal string SetId { get; }

        internal ConcurrentHashSet<SubscriptionFilter> Filters { get; } = new ConcurrentHashSet<SubscriptionFilter>();
    }
}
