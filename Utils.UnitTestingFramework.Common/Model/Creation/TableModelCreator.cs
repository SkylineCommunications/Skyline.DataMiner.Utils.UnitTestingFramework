namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Creation
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    internal class TableModelCreator : DataModelCreatorBase, IDataModelCreator
    {
        private readonly HashSet<int> excludedPids;

        public TableModelCreator(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        public void CreateModelAndAddToDataCollection(ParametersAndTables elementData, IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            var tableModel = CreateTableModelFromArrayOptions(parameter, protocolModelParameterFinder);

            if (tableModel == null)
            {
                return;
            }

            elementData.AddTable(tableModel);
        }

        public ITableModel CreateTableModelFromArrayOptions(IParamsParam parameter, IProtocolModelParameterFinder protocolModelParameterFinder)
        {
            int tableId = (int)parameter.Id.Value.Value;

            if (parameter.ArrayOptions == null)
            {
                return null;
            }

            int keyColumnIdx = (int)parameter.ArrayOptions.Index.Value.Value;

            int columnCount = parameter.ArrayOptions.Count;

            var tableModelBuilder = new TableModelBuilder(tableId);

            for (int i = 0; i < columnCount; i++)
            {
                uint columnIdx = parameter.ArrayOptions[i].Idx.Value.Value;
                uint columnPid = parameter.ArrayOptions[i].Pid.Value.Value;

                var columnParameter = protocolModelParameterFinder.FindParameter((int)columnPid) ?? throw new InvalidOperationException($"Parameter with ID {columnPid} not found.");

                var column = new ColumnDefinition(columnParameter.Name.Value, GetTypeForDefinition(columnParameter), (int)columnPid, (int)columnIdx, allowNull: true);

                tableModelBuilder.AddColumn(column, isKey: columnIdx == keyColumnIdx);

                excludedPids.Add((int)columnPid);
            }

            return tableModelBuilder.Build();
        }
    }
}