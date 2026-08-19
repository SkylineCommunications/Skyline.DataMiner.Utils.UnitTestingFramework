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
        private readonly Dictionary<int, object> tableMocks = new Dictionary<int, object>();
        private readonly Dictionary<string, object> standaloneParameterMocks = new Dictionary<string, object>();

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

            if (!standaloneParameterMocks.TryGetValue(cacheKey, out var parameterMockObject))
            {
                var parameterModel = parametersAndTables.GetParameter(parameterId);

                var parameterMockType = typeof(DmsStandaloneParameterMock<>).MakeGenericType(parameterType);
                var parameterMock = (Mock)Activator.CreateInstance(parameterMockType, parameterModel, Object);

                parameterMockObject = parameterMock.Object;
                standaloneParameterMocks.Add(cacheKey, parameterMockObject);
            }

            return parameterMockObject;
        }

        private IDmsTable GetTableObject(int tableId)
        {
            if (!tableMocks.TryGetValue(tableId, out var tableMockObject))
            {
                var tableModel = parametersAndTables.GetTable(tableId);

                tableMockObject = new DmsTableMock(tableModel, Object).Object;
                tableMocks.Add(tableId, tableMockObject);
            }

            return (IDmsTable)tableMockObject;
        }
    }
}
