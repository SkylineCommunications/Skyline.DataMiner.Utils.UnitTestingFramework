namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using Skyline.DataMiner.CICD.Models.Protocol.Read.Interfaces;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using static Skyline.DataMiner.Scripting.NotifyProtocol;

    public partial class SLProtocolMock<T> where T : class, SLProtocol
    {
        internal static class ProtocolMockSetupHelper
        {
            public static void Setup(SLProtocolMock<T> mock, IProtocolModel protocolModel)
            {
                SetupProperties(mock, protocolModel);

                SetupParameterGets(mock);
                SetupParameterSets(mock);

                SetupTableAddsAndExists(mock, mock.parametersAndTables);
                SetupTableDeleteAndClearKeys(mock, mock.parametersAndTables);
                SetupParametersIndexByKeys(mock, mock.parametersAndTables);
                SetupParametersIndexByCoordinates(mock, mock.parametersAndTables);
                SetupGetRowsAndKeyPosition(mock, mock.parametersAndTables);
                SetupSetRows(mock, mock.parametersAndTables);
                SetupFillArray(mock, mock.parametersAndTables);
                SetupCounts(mock, mock.parametersAndTables);

                mock.Setup(p => p.NotifyProtocol(It.IsAny<int>(), It.IsAny<object>(), It.IsAny<object>()))
                    .Returns(
                    (int notifyType, object value1, object value2) =>
                    {
                        return mock.notifyProtocolHelper.Execute(notifyType, value1, value2);
                    });
            }

            private static void SetupProperties(Mock<T> mock, IProtocolModel protocolModel)
            {
                mock.SetupGet(p => p.ProtocolName).Returns(protocolModel.Protocol.Name.Value);
                mock.SetupGet(p => p.ProtocolVersion).Returns(protocolModel.Protocol.Version.Value);
                mock.SetupGet(p => p.Leave).Returns(Constants.Constants.PROTOCOL_LEAVE);
                mock.SetupGet(p => p.Clear).Returns(Constants.Constants.PROTOCOL_CLEAR);
            }

            private static void SetupParameterGets(SLProtocolMock<T> protocolMock)
            {
                protocolMock.Setup(p => p.GetParameter(It.IsAny<int>()))
                    .Returns(
                    (int pid) =>
                    {
                        return protocolMock.parametersAndTables.TryGetParameter(pid, out var parameterModel) ? parameterModel.Value : null;
                    });

                protocolMock.Setup(p => p.GetParameterByName(It.IsAny<string>()))
                    .Returns(
                    (string parameterName) =>
                    {
                        return protocolMock.parametersAndTables.TryGetParameter(parameterName, out var parameterModel) ? parameterModel.Value : null;
                    });

                protocolMock.Setup(p => p.GetParameters(It.IsAny<object>()))
                    .Returns(
                    (uint[] parameters) =>
                    {
                        if (parameters.GetType() != typeof(uint[]))
                        {
                            throw new ArgumentException("Argument should be of type uint[]");
                        }

                        return parameters.Select(pid => protocolMock.parametersAndTables.TryGetParameter((int)pid, out var parameterModel) ? parameterModel.Value : null).ToArray();
                    });

                protocolMock.Setup(p => p.IsEmpty(It.IsAny<int>()))
                    .Returns(
                    (int pid) =>
                    {
                        return (protocolMock.parametersAndTables.TryGetParameter(pid, out var parameterModel) ? parameterModel.Value : null) == null;
                    });
            }

            private static void SetupParameterSets(SLProtocolMock<T> mock)
            {
                mock.Setup(p => p.SetParameter(It.IsAny<int>(), It.IsAny<object>()))
                    .Returns(
                    (int pid, object value) =>
                    {
                        if (mock.parametersAndTables.TryGetParameter(pid, out var parameterModel))
                        {
                            parameterModel.Update(value);
                            return 0;
                        }

                        return Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                    });

                mock.Setup(p => p.SetParameter(It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                    .Returns(
                    (int pid, object value, DateTime timestamp) =>
                    {
                        if (mock.parametersAndTables.TryGetParameter(pid, out var parameterModel))
                        {
                            parameterModel.Update(value, timestamp);
                            return 0;
                        }

                        return Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                    });

                mock.Setup(p => p.SetParameterByName(It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(
                    (string name, object value) =>
                    {
                        if (mock.parametersAndTables.TryGetParameter(name, out var parameterModel))
                        {
                            parameterModel.Update(value);
                            return 0;
                        }
                        else
                        {
                            return Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                        }
                    });

                mock.Setup(p => p.SetParametersByName(It.IsAny<string[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (string[] names, object[] values) =>
                    {
                        if (names.Length != values.Length)
                        {
                            return Constants.Constants.HRESULT_FAIL_DIFFLEN;
                        }

                        var result = new int[names.Length];
                        for (int i = 0; i < names.Length; i++)
                        {
                            if (mock.parametersAndTables.TryGetParameter(names[i], out var parameterModel))
                            {
                                parameterModel.Update(values[i]);
                                result[i] = 0;
                            }
                            else
                            {
                                result[i] = Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                            }
                        }

                        return result;
                    });

                mock.Setup(p => p.SetParameters(It.IsAny<int[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (int[] parameterIDs, object[] values) =>
                    {
                        if (parameterIDs.Length != values.Length)
                        {
                            return Constants.Constants.HRESULT_FAIL_DIFFLEN;
                        }

                        var result = new int[parameterIDs.Length];
                        for (int i = 0; i < parameterIDs.Length; i++)
                        {
                            if (mock.parametersAndTables.TryGetParameter(parameterIDs[i], out var parameterModel))
                            {
                                parameterModel.Update(values[i]);
                                result[i] = 0;
                            }
                            else
                            {
                                result[i] = Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                            }
                        }

                        return result;
                    });

                mock.Setup(p => p.SetParameters(It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                    .Returns(
                    (int[] parameterIDs, object[] values, DateTime[] timestamps) =>
                    {
                        if (parameterIDs.Length != values.Length || parameterIDs.Length != timestamps.Length)
                        {
                            return Constants.Constants.HRESULT_FAIL_DIFFLEN;
                        }

                        var result = new int[parameterIDs.Length];
                        for (int i = 0; i < parameterIDs.Length; i++)
                        {
                            if (mock.parametersAndTables.TryGetParameter(parameterIDs[i], out var parameterModel))
                            {
                                parameterModel.Update(values[i], timestamps[i]);
                                result[i] = 0;
                            }
                            else
                            {
                                result[i] = (int)Constants.Constants.HRESULT_FAIL_IDINEXISTENT;
                            }
                        }

                        return result;
                    });
            }

            private static void SetupTableAddsAndExists(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<bool[]>()))
                    .Callback(
                    (int tableId, object[] row, bool[] keyMask) =>
                    {
                        elementData.GetTable(tableId).SetRowReturnOneBasedIndex(row);
                    });

                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] row) =>
                    {
                        return elementData.GetTable(tableId).SetRowReturnOneBasedIndex(row);
                    });

                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(
                    (int tableId, string primaryKey) =>
                    {
                        return elementData.GetTable(tableId).SetRowReturnOneBasedIndex(primaryKey);
                    });

                mock.Setup(p => p.AddRowReturnKey(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] row) =>
                    {
                        return elementData.GetTable(tableId).AddRowReturnKey(row);
                    });

                mock.Setup(p => p.AddRowReturnKey(It.IsAny<int>()))
                    .Returns(
                    (int tableId) =>
                    {
                        return elementData.GetTable(tableId).AddRowReturnKey();
                    });

                mock.Setup(p => p.Exists(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(
                    (int tableId, string primaryKey) =>
                    {
                        return elementData.GetTable(tableId).RowExists(primaryKey);
                    });
            }

            private static void SetupTableDeleteAndClearKeys(Mock<T> mock, ParametersAndTables element)
            {
                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<string[]>()))
                   .Returns(
                   (int tableId, string[] primaryKeys) =>
                   {
                       return element.GetTable(tableId).DeleteRowReturnRemainingRows(primaryKeys);
                   });

                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int tableId, int rowIndex) =>
                   {
                       var tableModel = element.GetTable(tableId);

                       string rowkey = tableModel.GetRowPrimaryKey(rowIndex);
                       if (!String.IsNullOrWhiteSpace(rowkey))
                       {
                           tableModel.RemoveRows(tableModel.GetRowPrimaryKey(rowIndex));
                       }
    
                       return tableModel.RowCount;
                   });

                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       return element.GetTable(tableId).DeleteRowReturnRemainingRows(primaryKey);
                   });

                mock.Setup(p => p.ClearAllKeys(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       var table = element.GetTable(tableId);
                       table.RemoveAllRows();
                       return 0;
                   });
            }

            private static void SetupGetRowsAndKeyPosition(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.GetKeyPosition(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       return elementData.GetTable(tableId).GetRowIndex(primaryKey) + 1;
                   });

                mock.Setup(p => p.GetRow(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int tableId, int rowIndex) =>
                   {
                       var table = elementData.GetTable(tableId);
                       var row = table.GetRow(table.GetRowPrimaryKey(rowIndex));
                       if (row == null)
                       {
                            return new object[table.Schema.ColumnDefinitions.Count];
                       }
                       else
                       {
                            return row.Select(cell => cell.Value).ToArray();
                       }
                   });

                mock.Setup(p => p.GetRow(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       var table = elementData.GetTable(tableId);
                       var row = table.GetRow(primaryKey);
                       if (row == null)
                       {
                           return new object[table.Schema.ColumnDefinitions.Count];
                       }
                       else
                       {
                           return row.Select(cell => cell.Value).ToArray();
                       }
                   });
                mock.Setup(p => p.GetKeys(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       return elementData.GetTable(tableId).GetAllRows().Keys.ToArray();
                   });
            }

            private static void SetupSetRows(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, DateTime timestamp, bool useClearAndLeave) =>
                   {
                       if (!(rowData is object[] row))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(rowIndex, row, timestamp, useClearAndLeave);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData) =>
                   {
                       if (!(rowData is object[] row))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(rowIndex, row);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, bool useClearAndLeave) =>
                   {
                       if (!(rowData is object[] row))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(rowIndex, row, timestamp: null, useClearAndLeave);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, DateTime timestamp) =>
                   {
                       if (!(rowData is object[] row))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(rowIndex, row, timestamp);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, DateTime timestamp, bool useClearAndLeave) =>
                   {
                       if (!(rowData is object[] rowValues))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(primaryKey, rowValues, timestamp, useClearAndLeave);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData) =>
                   {
                       if (!(rowData is object[] rowValues))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(primaryKey, rowValues);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, bool useClearAndLeave) =>
                   {
                       if (!(rowData is object[] rowValues))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(primaryKey, rowValues, timestamp: null, useClearAndLeave);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, DateTime timestamp) =>
                   {
                       if (!(rowData is object[] rowValues))
                       {
                           throw new ArgumentException($"Expected type object[], but got {rowData?.GetType()} instead.");
                       }

                       return elementData.GetTable(tableId).SetRowReturnChanges(primaryKey, rowValues, timestamp);
                   });
            }

            private static void SetupFillArray(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<SaveOption>(), It.IsAny<DateTime?>()))
                   .Returns(
                   (int tableId, List<object[]> rows, SaveOption option, DateTime? timeInfo) =>
                   {
                       return elementData.GetTable(tableId).FillArray(rows, option, timeInfo);
                   });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<SaveOption>()))
                    .Returns(
                    (int tableId, List<object[]> rows, SaveOption option) =>
                    {
                        return elementData.GetTable(tableId).FillArray(rows, option);
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>()))
                    .Returns(
                    (int tableId, List<object[]> columns) =>
                    {
                        elementData.GetTable(tableId).FillArray(columns.ToArray());
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, List<object[]> columns, DateTime? timeInfo) =>
                    {
                        elementData.GetTable(tableId).FillArray(columns.ToArray(), timeInfo);
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] columns) =>
                    {
                        if (columns.Any(columnValues => !(columnValues is object[])))
                        {
                            throw new ArgumentException("One or more items are not of type object[]", nameof(columns));
                        }

                        elementData.GetTable(tableId).FillArray(columns.Cast<object[]>().ToArray());
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, object[] columns, DateTime? timeInfo) =>
                    {
                        if (columns.Any(columnValues => !(columnValues is object[])))
                        {
                            throw new ArgumentException("One or more items are not of type object[]", nameof(columns));
                        }

                        elementData.GetTable(tableId).FillArray(columns.Cast<object[]>().ToArray(), timeInfo);
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<List<object[]>>()))
                    .Returns(
                    (int tableId, List<object[]> columns) =>
                    {
                        elementData.GetTable(tableId).FillArrayNoDelete(columns.ToArray());
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, List<object[]> columns, DateTime? timeInfo) =>
                    {
                        elementData.GetTable(tableId).FillArrayNoDelete(columns.ToArray(), timeInfo);
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] columns) =>
                    {
                        if (columns.Any(columnValues => !(columnValues is object[])))
                        {
                            throw new ArgumentException("One or more items are not of type object[]", nameof(columns));
                        }

                        elementData.GetTable(tableId).FillArrayNoDelete(columns.Cast<object[]>().ToArray());
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, object[] columns, DateTime? timeInfo) =>
                    {
                        if (columns.Any(columnValues => !(columnValues is object[])))
                        {
                            throw new ArgumentException("One or more items are not of type object[]", nameof(columns));
                        }

                        elementData.GetTable(tableId).FillArrayNoDelete(columns.Cast<object[]>().ToArray(), timeInfo);

                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayWithColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, int columnPid, object[] keys, object[] values, DateTime? timeInfo) =>
                    {
                        elementData.GetTable(tableId).FillArrayWithColumn(columnPid, Array.ConvertAll(keys, Convert.ToString), values, timeInfo);
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayWithColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, int columnPid, object[] keys, object[] values) =>
                    {
                        elementData.GetTable(tableId).FillArrayWithColumn(columnPid, Array.ConvertAll(keys, Convert.ToString), values);
                        return null; // Irrelevant return value.
                    });
            }

            private static void SetupParametersIndexByKeys(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.GetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(
                    (int tableId, string key, int oneBasedColumnIndex) =>
                    {
                        return elementData.GetTable(tableId).GetParameterIndexByKey(key, oneBasedColumnIndex);
                    });

                mock.Setup(p => p.SetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, string key, int oneBasedColumnIndex, object value, DateTime timeInfo) =>
                   {
                       return elementData.GetTable(tableId).SetParameterIndexByKey(key, oneBasedColumnIndex, value, timeInfo);
                   });

                mock.Setup(p => p.SetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                   (int tableId, string key, int oneBasedColumnIndex, object value) =>
                   {
                       return elementData.GetTable(tableId).SetParameterIndexByKey(key, oneBasedColumnIndex, value);
                   });

                mock.Setup(p => p.SetParametersIndexByKey(It.IsAny<int[]>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                   .Returns(
                   (int[] tableIds, string[] keys, int[] oneBasedColumnIndices, object[] values, DateTime?[] timeInfos) =>
                   {
                       return SetParametersIndexByKey(elementData, tableIds, keys, oneBasedColumnIndices, values, timeInfos);
                   });

                mock.Setup(p => p.SetParametersIndexByKey(It.IsAny<int[]>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<object[]>()))
                   .Returns(
                   (int[] tableIds, string[] keys, int[] oneBasedColumnIndices, object[] values) =>
                   {
                       return SetParametersIndexByKey(elementData, tableIds, keys, oneBasedColumnIndices, values);
                   });
            }

            private static void SetupParametersIndexByCoordinates(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.GetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int tableId, int oneBasedRowIndex, int oneBasedColumnIndex) =>
                   {
                       return elementData.GetTable(tableId).GetParameterIndex(oneBasedRowIndex, oneBasedColumnIndex);
                   });

                mock.Setup(p => p.SetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, int oneBasedRowIndex, int oneBasedColumnIndex, object value, DateTime timeInfo) =>
                   {
                       return elementData.GetTable(tableId).SetParameterIndex(oneBasedRowIndex, oneBasedColumnIndex, value, timeInfo);
                   });

                mock.Setup(p => p.SetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                    (int tableId, int oneBasedRowIndex, int oneBasedColumnIndex, object value) =>
                    {
                        return elementData.GetTable(tableId).SetParameterIndex(oneBasedRowIndex, oneBasedColumnIndex, value);
                    });

                mock.Setup(p => p.SetParametersIndex(It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                   .Returns(
                   (int[] tableIds, int[] oneBasedRowIndexes, int[] oneBasedColumnIndexes, object[] values, DateTime?[] timeInfos) =>
                   {
                       return SetParametersIndex(elementData, tableIds, oneBasedRowIndexes, oneBasedColumnIndexes, values, timeInfos);
                   });

                mock.Setup(p => p.SetParametersIndex(It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<object[]>()))
                   .Returns(
                   (int[] tableIds, int[] oneBasedRowIndexes, int[] oneBasedColumnIndexes, object[] values) =>
                   {
                       return SetParametersIndex(elementData, tableIds, oneBasedRowIndexes, oneBasedColumnIndexes, values);
                   });
            }

            private static void SetupCounts(Mock<T> mock, ParametersAndTables elementData)
            {
                mock.Setup(p => p.RowCount(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       return elementData.GetTable(tableId).RowCount;
                   });
            }

            private static object SetParametersIndex(ParametersAndTables elementData, int[] tableIds, int[] oneBasedRowIndexes, int[] oneBasedColumnIndexes, object[] values, DateTime?[] timeInfos = null)
            {
                if (!(tableIds.Length == oneBasedRowIndexes.Length
                        && tableIds.Length == oneBasedColumnIndexes.Length
                        && tableIds.Length == values.Length))
                {
                    return 0x80040221L; // Invalid data.
                }

                if (timeInfos == null)
                {
                    timeInfos = new DateTime?[5];
                }

                uint[] results = new uint[tableIds.Length];

                for (int i = 0; i < tableIds.Length; i++)
                {
                    var tableModel = elementData.GetTable(tableIds[i]);

                    if (tableModel.SetParameterIndex(oneBasedRowIndexes[i], oneBasedColumnIndexes[i], values[i], timeInfos[i]))
                    {
                        results[i] = (uint)0x0004024AL; //// Parameter changed
                    }
                    else
                    {
                        results[i] = (uint)0x800402A4L; //// Action not performed;
                    }
                }

                return results;
            }

            private static object SetParametersIndexByKey(ParametersAndTables elementData, int[] tableIds, string[] keys, int[] oneBasedColumnIndexes, object[] values, DateTime?[] timeInfos = null)
            {
                if (!(tableIds.Length == keys.Length
                        && tableIds.Length == oneBasedColumnIndexes.Length
                        && tableIds.Length == values.Length))
                {
                    return 0x80040221L; // Invalid data
                }

                if (timeInfos == null)
                {
                    timeInfos = new DateTime?[5];
                }

                uint[] results = new uint[tableIds.Length];

                for (int i = 0; i < tableIds.Length; i++)
                {
                    var tableModel = elementData.GetTable(tableIds[i]);

                    if (tableModel.SetParameterIndexByKey(keys[i], oneBasedColumnIndexes[i], values[i], timeInfos[i]))
                    {
                        results[i] = (uint)0x0004024AL; // Parameter changed
                    }
                    else
                    {
                        results[i] = (uint)0x800402A4L; // Action not performed
                    }
                }

                return results;
            }
        }
    }
}