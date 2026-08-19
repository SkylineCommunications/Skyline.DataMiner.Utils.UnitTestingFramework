namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Mock of an <see cref="IDmsTable"/> that is backed by an <see cref="ITableModel"/>,
    /// so that rows and cells that are set are stored and can be retrieved again.
    /// </summary>
    internal class DmsTableMock : Mock<IDmsTable>
    {
        private readonly ITableModel tableModel;
        private readonly IDmsElement element;
        private readonly Dictionary<string, object> columnMocks = new Dictionary<string, object>();
        private readonly Dictionary<string, EventHandler<RowChangedEventArgs>> valueMonitors =
            new Dictionary<string, EventHandler<RowChangedEventArgs>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsTableMock"/> class.
        /// </summary>
        /// <param name="tableModel">The table model that holds the data.</param>
        /// <param name="element">The element this table belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="tableModel"/> is <see langword="null"/>.</exception>
        public DmsTableMock(ITableModel tableModel, IDmsElement element)
        {
            this.tableModel = tableModel ?? throw new ArgumentNullException(nameof(tableModel));
            this.element = element;

            Setup(t => t.Id).Returns(tableModel.TableId);
            Setup(t => t.Element).Returns(element);

            Setup(t => t.RowExists(It.IsAny<string>())).Returns((string key) => this.tableModel.RowExists(key));

            Setup(t => t.GetPrimaryKeys()).Returns(() => this.tableModel.GetAllRows().Keys.ToArray());

            Setup(t => t.GetRow(It.IsAny<string>())).Returns((string key) => this.tableModel.GetRow(key));

            Setup(t => t.GetRows()).Returns(() => this.tableModel.GetAllRows().Values.ToArray());

            Setup(t => t.GetData(It.IsAny<int>())).Returns((int keyColumnIndex) => GetData(keyColumnIndex));

            Setup(t => t.QueryData(It.IsAny<IEnumerable<IColumnFilter>>())).Returns((IEnumerable<IColumnFilter> filters) => QueryData(filters));

            Setup(t => t.AddRow(It.IsAny<object[]>())).Callback((object[] data) => this.tableModel.SetRow(data));

            Setup(t => t.SetRow(It.IsAny<string>(), It.IsAny<object[]>())).Callback((string key, object[] data) => SetRow(key, data));

            Setup(t => t.DeleteRow(It.IsAny<string>())).Callback((string key) => this.tableModel.RemoveRows(key));

            Setup(t => t.DeleteRows(It.IsAny<IEnumerable<string>>())).Callback((IEnumerable<string> keys) => this.tableModel.RemoveRows(keys.ToArray()));

            Setup(t => t.GetColumn<It.IsAnyType>(It.IsAny<int>()))
                .Returns(new InvocationFunc(invocation =>
                {
                    var columnType = invocation.Method.GetGenericArguments()[0];
                    var columnPid = (int)invocation.Arguments[0];

                    return GetColumnObject(columnType, columnPid);
                }));

            SetupValueMonitors();
        }

        private void SetupValueMonitors()
        {
            Setup(t => t.StartValueMonitor(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action<TableValueChange>>(), It.IsAny<bool>()))
                .Callback((string sourceId, int primaryKeyColumnIdx, Action<TableValueChange> action, bool _) => StartValueMonitor(sourceId, primaryKeyColumnIdx, action));

            Setup(t => t.StartValueMonitor(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<Action<TableValueChange>>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, int primaryKeyColumnIdx, Action<TableValueChange> action, TimeSpan _, bool __) => StartValueMonitor(sourceId, primaryKeyColumnIdx, action));

            Setup(t => t.StartValueMonitor(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<Action<TableValueChange>>(), It.IsAny<bool>()))
                .Callback((string sourceId, int primaryKeyColumnIdx, int[] _, Action<TableValueChange> action, bool __) => StartValueMonitor(sourceId, primaryKeyColumnIdx, action));

            Setup(t => t.StartValueMonitor(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int[]>(), It.IsAny<Action<TableValueChange>>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, int primaryKeyColumnIdx, int[] _, Action<TableValueChange> action, TimeSpan __, bool ___) => StartValueMonitor(sourceId, primaryKeyColumnIdx, action));

            Setup(t => t.StopValueMonitor(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string sourceId, bool _) => StopValueMonitor(sourceId));

            Setup(t => t.StopValueMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, TimeSpan _, bool __) => StopValueMonitor(sourceId));
        }

        private void StartValueMonitor(string sourceId, int primaryKeyColumnIdx, Action<TableValueChange> action)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            StopValueMonitor(sourceId);

            void Handler(object sender, RowChangedEventArgs e)
            {
                IDictionary<string, object[]> updatedRows;
                string[] deletedRows;

                if (e.ChangeType == RowChangeType.Deleted)
                {
                    updatedRows = new Dictionary<string, object[]>();
                    deletedRows = new[] { e.PrimaryKey };
                }
                else
                {
                    var row = tableModel.GetRow(e.PrimaryKey);
                    updatedRows = new Dictionary<string, object[]>();
                    if (row != null)
                    {
                        updatedRows[e.PrimaryKey] = row;
                    }

                    deletedRows = new string[0];
                }

                var param = new Param(element?.AgentId ?? 0, element?.Id ?? 0, tableModel.TableId);

                action(new TableValueChange(param, sourceId, null, primaryKeyColumnIdx, updatedRows, deletedRows));
            }

            valueMonitors[sourceId] = Handler;
            tableModel.RowChanged += Handler;
        }

        private void StopValueMonitor(string sourceId)
        {
            if (sourceId != null && valueMonitors.TryGetValue(sourceId, out var handler))
            {
                tableModel.RowChanged -= handler;
                valueMonitors.Remove(sourceId);
            }
        }

        private IEnumerable<object[]> QueryData(IEnumerable<IColumnFilter> filters)
        {
            var filterList = filters?.ToList() ?? new List<IColumnFilter>();

            var predicates = filterList.OfType<ColumnFilter>().ToList();
            var returnColumnPids = filterList.OfType<ColumnReturnFilter>().Select(f => f.Pid).ToList();

            var rows = tableModel.GetAllRows().Values.Where(row => predicates.All(predicate => Matches(row, predicate)));

            if (returnColumnPids.Count == 0)
            {
                return rows.Select(row => (object[])row.Clone()).ToList();
            }

            return rows.Select(row => returnColumnPids.Select(pid => GetCell(row, pid)).ToArray()).ToList();
        }

        private bool Matches(object[] row, ColumnFilter filter)
        {
            var columnDefinition = tableModel.Schema.FindColumnDefinitionByPid(filter.Pid);
            if (columnDefinition == null)
            {
                return false;
            }

            return Compare(row[columnDefinition.Idx], filter.Value, filter.ComparisonOperator);
        }

        private object GetCell(object[] row, int columnPid)
        {
            var columnDefinition = tableModel.Schema.FindColumnDefinitionByPid(columnPid);
            return columnDefinition == null ? null : row[columnDefinition.Idx];
        }

        private static bool Compare(object cellValue, string filterValue, ComparisonOperator comparisonOperator)
        {
            var cellString = System.Convert.ToString(cellValue, CultureInfo.InvariantCulture) ?? String.Empty;

            int comparison;
            if (Double.TryParse(cellString, NumberStyles.Any, CultureInfo.InvariantCulture, out var cellNumber)
                && Double.TryParse(filterValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var filterNumber))
            {
                comparison = cellNumber.CompareTo(filterNumber);
            }
            else
            {
                comparison = String.Compare(cellString, filterValue ?? String.Empty, StringComparison.Ordinal);
            }

            switch (comparisonOperator)
            {
                case ComparisonOperator.Equal:
                    return comparison == 0;
                case ComparisonOperator.NotEqual:
                    return comparison != 0;
                case ComparisonOperator.GreaterThan:
                    return comparison > 0;
                case ComparisonOperator.GreaterThanOrEqual:
                    return comparison >= 0;
                case ComparisonOperator.LessThan:
                    return comparison < 0;
                case ComparisonOperator.LessThanOrEqual:
                    return comparison <= 0;
                default:
                    return false;
            }
        }

        private void SetRow(string primaryKey, object[] data)
        {
            if (primaryKey == null)
            {
                throw new ArgumentNullException(nameof(primaryKey));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            // The primary key argument is authoritative: force the primary key column to the specified key
            // so the correct row is updated regardless of the key embedded in the provided data.
            var row = (object[])data.Clone();

            var primaryKeyColumnIndex = tableModel.Schema.PrimaryKeyColumn.Idx;
            if (primaryKeyColumnIndex < row.Length)
            {
                row[primaryKeyColumnIndex] = primaryKey;
            }

            tableModel.SetRow(row);
        }

        private object GetColumnObject(Type columnType, int columnPid)
        {
            SupportedValueTypes.EnsureSupported(columnType);

            var cacheKey = $"{columnPid}|{columnType.AssemblyQualifiedName}";

            if (!columnMocks.TryGetValue(cacheKey, out var columnMockObject))
            {
                var columnMockType = typeof(DmsColumnMock<>).MakeGenericType(columnType);
                var columnMock = (Mock)Activator.CreateInstance(columnMockType, tableModel, columnPid, Object);

                columnMockObject = columnMock.Object;
                columnMocks.Add(cacheKey, columnMockObject);
            }

            return columnMockObject;
        }

        private IDictionary<string, object[]> GetData(int keyColumnIndex)
        {
            if (keyColumnIndex < 0 || keyColumnIndex >= tableModel.Schema.ColumnCount)
            {
                throw new ArgumentException($"'{keyColumnIndex}' is not a valid key column index.", nameof(keyColumnIndex));
            }

            var data = new Dictionary<string, object[]>();

            foreach (var row in tableModel.GetAllRows().Values)
            {
                var key = System.Convert.ToString(row[keyColumnIndex]);
                data[key] = row;
            }

            return data;
        }
    }
}
