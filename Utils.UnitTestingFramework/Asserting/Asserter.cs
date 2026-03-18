namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Asserting
{
    using System;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    internal class Asserter : IAsserter
    {
        private readonly ElementData elementData;

        public Asserter(ElementData elementData)
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