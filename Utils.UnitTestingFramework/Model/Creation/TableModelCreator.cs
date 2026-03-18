namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal interface IProtocolModelParameterFinder
    {
        IParamsParam FindParameter(int parameterId);
    }

    internal class ProtocolModelParameterFinder : IProtocolModelParameterFinder
    {
        private readonly IProtocolModel protocolModel;

        public ProtocolModelParameterFinder(IProtocolModel protocolModel)
        {
            this.protocolModel = protocolModel ?? throw new ArgumentNullException(nameof(protocolModel));
        }

        public IParamsParam FindParameter(int parameterId)
        {
            var parameter = protocolModel.Protocol.Params.SingleOrDefault(p => p.Id.Value.Value == parameterId);

            return parameter;
        }
    }

    internal class TableModelCreator : DataModelCreatorBase, IDataModelCreator
    {
        private readonly HashSet<int> excludedPids;

        public TableModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        public void CreateModelAndAddToElementData(ElementData elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            var tableModel = CreateTableModelFromArrayOptions(parameter, protocolModelParameterFinder);

            elementData.AddTable(tableModel);
        }

        public ITableModel CreateTableModelFromArrayOptions(IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            int tableId = (int)parameter.Id.Value.Value;

            if (parameter.ArrayOptions == null)
            {
                return TableModelBuilder.EmptyTableModel(tableId);
            }

            int keyColumnIdx = (int)parameter.ArrayOptions.Index.Value.Value;

            int columnCount = parameter.ArrayOptions.Count;

            var columns = new List<ColumnDefinition>();
            ColumnDefinition keyColumn = null;

            for (int i = 0; i < columnCount; i++)
            {
                uint columnIdx = parameter.ArrayOptions[i].Idx.Value.Value;
                uint columnPid = parameter.ArrayOptions[i].Pid.Value.Value;

                var columnParameter = protocolModelParameterFinder.FindParameter((int)columnPid) ?? throw new InvalidOperationException($"Parameter with ID {columnPid} not found.");

                var column = new ColumnDefinition(columnParameter.Name.Value, GetTypeForDefinition(columnParameter), (int)columnPid, (int)columnIdx, allowNull: false);
                columns.Add(column);

                excludedPids.Add((int)columnPid);

                bool isKeyColumn = columnIdx == keyColumnIdx;
                if (isKeyColumn)
                {
                    keyColumn = column;
                }
            }

            return new TableModel(tableId, columns, keyColumn);
        }
    }
}