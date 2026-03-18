namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    public class ElementData
    {
        private readonly Dictionary<string, ParameterDefinition> parameterNameToDefinition = new Dictionary<string, ParameterDefinition>();
        private readonly Dictionary<int, ParameterDefinition> parameterIdToDefinition = new Dictionary<int, ParameterDefinition>();
        private readonly Dictionary<ParameterDefinition, IParameterModel> parametersToValues = new Dictionary<ParameterDefinition, IParameterModel>();

        private readonly Dictionary<int, ITableModel> tablesPerTablePid = new Dictionary<int, ITableModel>();

        /// <summary>
        /// Gets the parameter model.
        /// </summary>
        /// <param name="parameterId">The parameter ID.</param>
        /// <returns>The parameter model.</returns>
        /// <exception cref="ArgumentException">There is no parameter with ID <paramref name="parameterId"/>.</exception>
        public IParameterModel GetParameter(int parameterId)
        {
            if (!parameterIdToDefinition.TryGetValue(parameterId, out var parameterDefinition))
            {
                throw new ArgumentException($"There is no parameter with ID '{parameterId}'", nameof(parameterId));
            }

            return parametersToValues[parameterDefinition];
        }

        /// <summary>
        /// Gets the parameter model.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns>The parameter model.</returns>
        /// <exception cref="ArgumentException">There is no parameter with name <paramref name="parameterName"/>.</exception>
        public IParameterModel GetParameter(string parameterName)
        {
            if (!parameterNameToDefinition.TryGetValue(parameterName, out var parameterDefinition))
            {
                throw new ArgumentException($"There is no parameter with name '{parameterName}'", nameof(parameterName));
            }

            return parametersToValues[parameterDefinition];
        }

        /// <summary>
        /// Gets the table model for the table with the specified parameter ID.
        /// </summary>
        /// <param name="tablePid">The ID of the table parameter.</param>
        /// <returns>The table model for the table with the specified ID.</returns>
        /// <exception cref="ArgumentException">There is no table with ID " + tableId</exception>
        public ITableModel GetTable(int tablePid)
        {
            if (!tablesPerTablePid.TryGetValue(tablePid, out var tableModel))
            {
                throw new ArgumentException($"There is no table with ID '{tablePid}'");
            }

            return tableModel;
        }

        /// <summary>
        /// Adds the specified model.
        /// </summary>
        /// <param name="tableModel">The table model.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        internal void AddTable(ITableModel tableModel)
        {
            if (tableModel == null)
            {
                throw new ArgumentNullException(nameof(tableModel));
            }

            tablesPerTablePid[tableModel.TableId] = tableModel;
        }

        internal void AddParameter(ParameterDefinition parameterDefinition, IParameterModel parameterModel = null)
        {
            if (parameterDefinition == null)
            {
                throw new ArgumentNullException(nameof(parameterDefinition));
            }

            if (parameterModel == null)
            {
                throw new ArgumentNullException(nameof(parameterModel));
            }

            parameterNameToDefinition.Add(parameterDefinition.Name, parameterDefinition);
            parameterIdToDefinition.Add(parameterDefinition.Pid, parameterDefinition);
            parametersToValues.Add(parameterDefinition, parameterModel ?? new ParameterModel(null));
        }
    }
}