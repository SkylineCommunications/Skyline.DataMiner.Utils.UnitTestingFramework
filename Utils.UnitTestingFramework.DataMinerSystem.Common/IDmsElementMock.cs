namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;

    /// <summary>
    /// A pre-arranged mock of <see cref="IDmsElement"/>.
    /// </summary>
    /// <remarks>
    /// The mock uses a protocol.xml file to determine which standalone parameters and tables the element contains,
    /// and keeps track of the values that are set on and retrieved from them.
    /// </remarks>
    public class IDmsElementMock : Mock<IDmsElement>
    {
        private readonly ParametersAndTables parametersAndTables;
        private readonly Dictionary<int, DmsTableMock> tableMocks = new Dictionary<int, DmsTableMock>();
        private readonly Dictionary<string, Mock> standaloneParameterMocks = new Dictionary<string, Mock>();

        /// <summary>
        /// Initializes a new instance of the <see cref="IDmsElementMock"/> class.
        /// </summary>
        /// <param name="pathToProtocolXml">The path to the protocol.xml file. If not defined, the protocol.xml file in the root of the solution will be used.</param>
        public IDmsElementMock(string pathToProtocolXml = null)
        {
            parametersAndTables = ParametersAndTablesBuilder.Build(pathToProtocolXml);

            SetupStandaloneParameters();
            SetupTables();
        }

        /// <summary>
        /// Gets the standalone parameter with the specified ID, allowing values to be arranged and asserted without going through <see cref="Mock{T}.Object"/>.
        /// </summary>
        /// <remarks>Calls to this method are not recorded as invocations on the mock, so arranging data through it does not interfere with a later <see cref="Mock.Verify()"/>.</remarks>
        /// <typeparam name="T">The type of the parameter value. Only <see cref="int"/>?, <see cref="double"/>?, <see cref="DateTime"/>? and <see cref="string"/> are supported.</typeparam>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns>The standalone parameter.</returns>
        /// <exception cref="NotSupportedException"><typeparamref name="T"/> is not a supported type.</exception>
        /// <exception cref="ArgumentException">No standalone parameter with the specified <paramref name="parameterId"/> exists.</exception>
        public IDmsStandaloneParameter<T> GetStandaloneParameter<T>(int parameterId)
        {
            return (IDmsStandaloneParameter<T>)GetStandaloneParameterObject(typeof(T), parameterId);
        }

        /// <summary>
        /// Gets the table with the specified ID, allowing rows and cells to be arranged and asserted without going through <see cref="Mock{T}.Object"/>.
        /// </summary>
        /// <remarks>Calls to this method are not recorded as invocations on the mock, so arranging data through it does not interfere with a later <see cref="Mock.Verify()"/>.</remarks>
        /// <param name="tableId">The table ID.</param>
        /// <returns>The table.</returns>
        /// <exception cref="ArgumentException">No table with the specified <paramref name="tableId"/> exists.</exception>
        public IDmsTable GetTable(int tableId)
        {
            return GetTableObject(tableId);
        }

        private void SetupStandaloneParameters()
        {
            Setup(e => e.GetStandaloneParameter<It.IsAnyType>(It.IsAny<int>()))
                .Returns(new InvocationFunc(invocation =>
                {
                    var parameterType = invocation.Method.GetGenericArguments()[0];
                    var parameterId = (int)invocation.Arguments[0];

                    return GetStandaloneParameterObject(parameterType, parameterId);
                }));
        }

        private void SetupTables()
        {
            Setup(e => e.GetTable(It.IsAny<int>()))
                .Returns((int tableId) => GetTableObject(tableId));
        }

        private object GetStandaloneParameterObject(Type parameterType, int parameterId)
        {
            SupportedValueTypes.EnsureSupported(parameterType);

            var cacheKey = $"{parameterId}|{parameterType.AssemblyQualifiedName}";

            if (!standaloneParameterMocks.TryGetValue(cacheKey, out var parameterMock))
            {
                var parameterModel = parametersAndTables.GetParameter(parameterId);

                var parameterMockType = typeof(DmsStandaloneParameterMock<>).MakeGenericType(parameterType);
                parameterMock = (Mock)Activator.CreateInstance(parameterMockType, parameterModel, Object);

                standaloneParameterMocks.Add(cacheKey, parameterMock);
            }

            return parameterMock.Object;
        }

        private IDmsTable GetTableObject(int tableId)
        {
            if (!tableMocks.TryGetValue(tableId, out var tableMock))
            {
                var tableModel = parametersAndTables.GetTable(tableId);

                tableMock = new DmsTableMock(tableModel, Object);
                tableMocks.Add(tableId, tableMock);
            }

            return tableMock.Object;
        }
    }
}
