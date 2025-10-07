using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Utils.UnitTestingFramework.Tests")]
[assembly: InternalsVisibleTo("Utils.UnitTestingFramework.SnapshotTools.Tests")]
namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using Moq;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    /// <summary>
    /// SLProtocol fake.
    /// </summary>
    public partial class SLProtocolMock : Mock<SLProtocol>
    {
        private readonly ProtocolCache protocolCache;
        private readonly NotifyProtocolHelper notifyProtocolHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SLProtocolMock"/> class.
        /// </summary>
        public SLProtocolMock(string customPathToProtocolXml = null)
        {
            this.protocolCache = ProtocolCacheBuilder.Build(customPathToProtocolXml);

            this.notifyProtocolHelper = new NotifyProtocolHelper(protocolCache);

            ProtocolMockSetupHelper.Setup(this);
        }

        /// <summary>
        /// Asserts this instance.
        /// </summary>
        /// <returns><see cref="IAsserter"/> interface.</returns>
        public IAsserter Assert()
        {
            return new Asserter(protocolCache);
        }
    }
}