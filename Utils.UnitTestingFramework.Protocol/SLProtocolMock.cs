using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Utils.UnitTestingFramework.Tests")]
namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using Moq;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;

    /// <summary>
    /// SLProtocol mock.
    /// </summary>
    public class SLProtocolMock : SLProtocolMock<SLProtocol>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SLProtocolMock"/> class.
        /// </summary>
        /// <param name="customPathToProtocolXml">An optional path to the protocol.xml file. If not defined, the protocol.xml file in the root of the solution will be used.</param>
        public SLProtocolMock(string customPathToProtocolXml = null) : base(customPathToProtocolXml)
        {
        }
    }

    /// <summary>
    /// SLProtocol mock that supports SLProtocolExt.
    /// </summary>
    public partial class SLProtocolMock<T> : Mock<T>
        where T : class, SLProtocol
    {
        private readonly ParametersAndTables parametersAndTables;
        private readonly NotifyProtocolHelper notifyProtocolHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SLProtocolMock"/> class.
        /// </summary>
        /// <param name="customPathToProtocolXml">An optional path to the protocol.xml file. If not defined, the protocol.xml file in the root of the solution will be used.</param>
        public SLProtocolMock(string customPathToProtocolXml = null)
        {
            var protocolModel = ProtocolModelBuilder.Build(customPathToProtocolXml);

            this.parametersAndTables = ParametersAndTablesBuilder.Build(protocolModel);

            this.notifyProtocolHelper = new NotifyProtocolHelper(parametersAndTables);

            ProtocolMockSetupHelper.Setup(this, protocolModel);
        }

        /// <summary>
        /// Asserts this instance.
        /// </summary>
        /// <returns><see cref="IAsserter"/> interface.</returns>
        public IAsserter Assert()
        {
            return new Asserter(parametersAndTables);
        }
    }
}