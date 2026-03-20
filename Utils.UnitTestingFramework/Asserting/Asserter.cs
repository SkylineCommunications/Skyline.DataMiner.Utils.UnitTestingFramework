namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    internal class Asserter : IAsserter
    {
        private readonly ParametersAndTables elementData;

        public Asserter(ParametersAndTables elementData)
        {
            this.elementData = elementData ?? throw new ArgumentNullException(nameof(elementData));
        }

        public IParameterModel Parameter(int parameterId)
        {
            try
            {
                return elementData.GetParameter(parameterId);
            }
            catch
            {
                return null;
            }
        }

        public IParameterModel Parameter(string parameterName)
        {
            return elementData.GetParameter(parameterName);
        }

        public ITableAsserter Table(int tablePid)
        {
            return new TableAsserter(elementData.GetTable(tablePid));
        }
    }
}