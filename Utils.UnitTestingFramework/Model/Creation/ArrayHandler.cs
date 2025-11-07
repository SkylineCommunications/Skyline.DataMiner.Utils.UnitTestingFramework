namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model.Creation
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.CICD.Models.Protocol.Read;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    internal class ArrayHandler : IParameterHandler
    {
        private readonly HashSet<int> excludedPids;

        public ArrayHandler(HashSet<int> excludedPids)
        {
            this.excludedPids = excludedPids ?? throw new ArgumentNullException(nameof(excludedPids));
        }

        public void CreateModelAndAddToCache(IProtocolCache cache, IParamsParam parameter)
        {
            var tableModel = CreateTableModelFromArrayOptions(parameter);

            cache.Tables.AddTable(tableModel);
        }

        public ITableModel CreateTableModelFromArrayOptions(IParamsParam parameter)
        {
            int tableId = (int)parameter.Id.Value.Value;

            if (parameter.ArrayOptions == null)
            {
                return TableModelBuilder.EmptyTableModel(tableId);
            }

            var tableModelBuilder = new TableModelBuilder(tableId);

            int keyColumnIdx = (int)parameter.ArrayOptions.Index.Value.Value;
            bool foundPKIndex = false;

            int columnsNumber = parameter.ArrayOptions.Count;

            for (int index = 0; index < columnsNumber; index++)
            {
                uint idx = parameter.ArrayOptions[index].Idx.Value.Value;
                uint pid = parameter.ArrayOptions[index].Pid.Value.Value;

                var isKeyColumn = idx == keyColumnIdx;

                tableModelBuilder.AddColumn((int)pid, (int)idx, isKeyColumn);
                excludedPids.Add((int)pid);

                if (isKeyColumn)
                {
                    foundPKIndex = true;
                }
            }

            if (!foundPKIndex)
            {
                throw new InvalidOperationException($"Did not find an idx corresponding to the PK index '{keyColumnIdx}'.");
            }

            return tableModelBuilder.Build();
        }
    }
}