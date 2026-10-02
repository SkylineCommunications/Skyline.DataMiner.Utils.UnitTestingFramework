namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Builder class for constructing a <see cref="IDmsElementMock"/> with its properties and configuration.
    /// </summary>
    public class IDmsElementBuilder
    {
        private readonly int id;
        private readonly string name;
        private readonly string protocolName;
        private readonly string protocolVersion;
        private readonly List<Action<IDmsElementMock>> actions = new List<Action<IDmsElementMock>>();

        internal IDmsElementBuilder(int id, string name, string protocolName, string protocolVersion)
        {
            this.id = id;
            this.name = name;
            this.protocolName = protocolName;
            this.protocolVersion = protocolVersion;
        }

        /// <summary>
        /// Specifies that the element should be added under a specific view in the DataMiner System.
        /// </summary>
        /// <param name="viewId">The ID of the view.</param>
        /// <returns>The current <see cref="IDmsElementBuilder"/> instance.</returns>
        public IDmsElementBuilder UnderView(int viewId)
        {
            actions.Add(elementMock => elementMock.AddView(viewId));
            return this;
        }
        
        /// <summary>
        /// Fills the specified table with the provided rows.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <param name="rows">The rows to add to the table.</param>
        /// <returns>The current <see cref="IDmsElementBuilder"/> instance.</returns>
        public IDmsElementBuilder FillTable(int tableId, object[][] rows)
        {
            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (rows.Length == 0)
                return this;

            actions.Add(elementMock =>
            {
                var table = elementMock.GetDmsTableMock(tableId);
                foreach (var row in rows)
                {
                    table.TableModel.SetRow(row);
                }
            });

            return this;
        }

        /// <summary>
        /// Fills the specified table with rows constructed using the provided row builder actions.
        /// </summary>
        /// <param name="tableId">The ID of the table.</param>
        /// <param name="rowBuilderActions">The actions to configure each row.</param>
        /// <returns>The current <see cref="IDmsElementBuilder"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rowBuilderActions"/> is null.</exception>
        public IDmsElementBuilder FillTable(int tableId, params Action<RowBuilder>[] rowBuilderActions)
        {
            if (rowBuilderActions is null)
            {
                throw new ArgumentNullException(nameof(rowBuilderActions));
            }

            if(rowBuilderActions.Length == 0)
                return this;

            actions.Add(elementMock =>
            {
                var table = elementMock.GetDmsTableMock(tableId);
                foreach (var rowBuilderAction in rowBuilderActions)
                {
                    var rowBuilder = new RowBuilder(table.TableModel.Definition);
                    rowBuilderAction(rowBuilder);
                    table.SetRow(rowBuilder.Build());
                }
            });

            return this;
        }

        /// <summary>
        /// Sets the value of a parameter for the element.
        /// </summary>
        /// <typeparam name="T">The type of the parameter value.</typeparam>
        /// <param name="parameterId">The ID of the parameter.</param>
        /// <param name="value">The value to set for the parameter.</param>
        /// <returns>The current <see cref="IDmsElementBuilder"/> instance.</returns>
        public IDmsElementBuilder SetParameter<T>(int parameterId, T value)
        {
            actions.Add(elementMock => elementMock.GetStandaloneParameterMock<T>(parameterId).ParameterModel.Update(value));
            return this;
        }

        internal void Build(IDmsMock dmsMock, IDmaMock dmaMock)
        {
            var protocolMock = dmsMock.GetProtocolMock(protocolName, protocolVersion);

            if (protocolMock == null)
            {
                var version = protocolVersion == null ? String.Empty : $" with version '{protocolVersion}'";
                throw new InvalidOperationException($"Protocol '{protocolName}'{version} is not available in this DataMiner System.");
            }

            var elementMock = dmaMock.CreateElement(protocolMock, id, name ?? $"Element {id}");

            foreach (var action in actions)
            {
                action(elementMock);
            }
        }
    }
}
