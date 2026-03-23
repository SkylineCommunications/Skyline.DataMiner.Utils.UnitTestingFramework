namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;

    internal class Asserter : IAsserter
    {
        private readonly ParametersAndTables elementData;

        public Asserter(ParametersAndTables elementData)
        {
            this.elementData = elementData ?? throw new ArgumentNullException(nameof(elementData));
        }

        public IParameterAsserter Parameter(int parameterId)
        {
            try
            {
                return new ParameterAsserter(elementData.GetParameter(parameterId));
            }
            catch
            {
                return null;
            }
        }

        public IParameterAsserter Parameter(string parameterName)
        {
            try
            {
                return new ParameterAsserter(elementData.GetParameter(parameterName));
            }
            catch
            {
                return null;
            }
        }

        public ITableAsserter Table(int tablePid)
        {
            return new TableAsserter(elementData.GetTable(tablePid));
        }
    }
}