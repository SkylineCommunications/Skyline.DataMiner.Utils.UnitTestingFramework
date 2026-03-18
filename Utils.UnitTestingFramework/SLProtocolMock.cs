using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Utils.UnitTestingFramework.Tests")]
[assembly: InternalsVisibleTo("Utils.UnitTestingFramework.SnapshotTools.Tests")]
namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using Moq;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

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
        private readonly ElementData elementData;
        private readonly NotifyProtocolHelper notifyProtocolHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="SLProtocolMock"/> class.
        /// </summary>
        /// <param name="customPathToProtocolXml">An optional path to the protocol.xml file. If not defined, the protocol.xml file in the root of the solution will be used.</param>
        public SLProtocolMock(string customPathToProtocolXml = null)
        {
            var protocolModel = ProtocolModelBuilder.Build(customPathToProtocolXml);

            this.elementData = ElementDataBuilder.Build(protocolModel);

            this.notifyProtocolHelper = new NotifyProtocolHelper(elementData);

            ProtocolMockSetupHelper.Setup(this, protocolModel);
        }

        public IParameterModel GetParameter(int parameterId)
        {
            return elementData.GetParameter(parameterId) ?? throw new InvalidOperationException($"Parameter with ID {parameterId} does not exist");
        }

        public IParameterModel GetParameter(string parameterName)
        {
            return elementData.GetParameter(parameterName) ?? throw new InvalidOperationException($"Parameter with name '{parameterName}' does not exist");
        }

        public ITableModel GetTable(int tableId)
        {
            return elementData.GetTable(tableId) ?? throw new InvalidOperationException($"Table with ID {tableId} does not exist");
        }

        /// <summary>
        /// Asserts this instance.
        /// </summary>
        /// <returns><see cref="IAsserter"/> interface.</returns>
        public IAsserter Assert()
        {
            return new Asserter(elementData);
        }
    }
}