namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
    using Moq;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using static Skyline.DataMiner.Scripting.NotifyProtocol;

    public partial class SLProtocolMock
    {
        internal static class ProtocolMockSetupHelper
        {
            public static void Setup(SLProtocolMock mock)
            {
                SetupParameterGets(mock, mock.protocolCache);
                SetupParameterSets(mock, mock.protocolCache);

                SetupTableAddsAndExists(mock, mock.protocolCache.Tables);
                SetupTableDeleteAndClearKeys(mock, mock.protocolCache.Tables);
                SetupParametersIndexByKeys(mock, mock.protocolCache.Tables);
                SetupParametersIndexByCoordinates(mock, mock.protocolCache.Tables);
                SetupGetRowsAndKeyPosition(mock, mock.protocolCache.Tables);
                SetupSetRows(mock, mock.protocolCache.Tables);
                SetupFillArray(mock, mock.protocolCache.Tables);
                SetupCounts(mock, mock.protocolCache.Tables);

                mock.Setup(p => p.NotifyProtocol(It.IsAny<int>(), It.IsAny<object>(), It.IsAny<object>()))
                    .Returns(
                    (int notifyType, object value1, object value2) =>
                    {
                        return mock.notifyProtocolHelper.Execute(notifyType, value1, value2);
                    });
            }

            private static void SetupParameterGets(Mock<SLProtocol> mock, ProtocolCache protocolCache)
            {
                mock.Setup(p => p.GetParameter(It.IsAny<int>()))
                    .Returns(
                    (int pid) =>
                    {
                        protocolCache.Parameters.TryGetParameter(pid, out object value);
                        return value;
                    });

                mock.Setup(p => p.GetParameterByName(It.IsAny<string>()))
                    .Returns(
                    (string parameterName) =>
                    {
                        protocolCache.Parameters.TryGetParameterByName(parameterName, out object value);
                        return value;
                    });

                mock.Setup(p => p.GetParameters(It.IsAny<object>()))
                    .Returns(
                    (uint[] parameters) =>
                    {
                        if (parameters.GetType() == typeof(uint[]))
                        {
                            return protocolCache.Parameters.GetParameters(parameters);
                        }
                        else
                        {
                            throw new ArgumentException("Argument should be of type uint[]");
                        }
                    });

                mock.Setup(p => p.IsEmpty(It.IsAny<int>()))
                    .Returns(
                    (int pid) =>
                    {
                        return protocolCache.Parameters.IsEmpty(pid, mock);
                    });
            }

            private static void SetupParameterSets(Mock<SLProtocol> mock, ProtocolCache protocolCache)
            {
                mock.Setup(p => p.SetParameter(It.IsAny<int>(), It.IsAny<object>()))
                    .Returns(
                    (int pid, object value) =>
                    {
                        return protocolCache.Parameters.SetParameter(pid, value);
                    });

                mock.Setup(p => p.SetParameter(It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                    .Returns(
                    (int pid, object value, DateTime timestamp) =>
                    {
                        return protocolCache.Parameters.SetParameter(pid, value, timestamp);
                    });

                mock.Setup(p => p.SetParameterByName(It.IsAny<string>(), It.IsAny<object>()))
                    .Returns(
                    (string name, object value) =>
                    {
                        return protocolCache.Parameters.SetParameterByName(name, value);
                    });

                mock.Setup(p => p.SetParametersByName(It.IsAny<string[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (string[] names, object[] values) =>
                    {
                        return protocolCache.Parameters.SetParametersByName(names, values);
                    });

                mock.Setup(p => p.SetParameters(It.IsAny<int[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (int[] parameterIDs, object[] values) =>
                    {
                        return protocolCache.Parameters.SetParameters(parameterIDs, values);
                    });

                mock.Setup(p => p.SetParameters(It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                    .Returns(
                    (int[] parameterIDs, object[] values, DateTime[] timestamp) =>
                    {
                        return protocolCache.Parameters.SetParameters(parameterIDs, values, timestamp);
                    });
            }

            private static void SetupTableAddsAndExists(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<bool[]>()))
                    .Callback(
                    (int tableId, object[] row, bool[] keyMask) =>
                    {
                        tablesCache.AddRow(tableId, row);
                    });

                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] row) =>
                    {
                        return tablesCache.AddRow(tableId, row);
                    });

                mock.Setup(p => p.AddRow(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(
                    (int tableId, string primaryKey) =>
                    {
                        return tablesCache.AddRow(tableId, primaryKey);
                    });

                mock.Setup(p => p.AddRowReturnKey(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] row) =>
                    {
                        return tablesCache.AddRowReturnKey(tableId, row);
                    });

                mock.Setup(p => p.AddRowReturnKey(It.IsAny<int>()))
                    .Returns(
                    (int tableId) =>
                    {
                        return tablesCache.AddRowReturnKey(tableId);
                    });

                mock.Setup(p => p.Exists(It.IsAny<int>(), It.IsAny<string>()))
                    .Returns(
                    (int tableId, string primaryKey) =>
                    {
                        return tablesCache.Exists(tableId, primaryKey);
                    });
            }

            private static void SetupTableDeleteAndClearKeys(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<string[]>()))
                   .Returns(
                   (int tableId, string[] primaryKeys) =>
                   {
                       return tablesCache.DeleteRow(tableId, primaryKeys);
                   });

                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int tableId, int rowIndex) =>
                   {
                       return tablesCache.DeleteRow(tableId, rowIndex);
                   });

                mock.Setup(p => p.DeleteRow(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       return tablesCache.DeleteRow(tableId, primaryKey);
                   });

                mock.Setup(p => p.ClearAllKeys(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       return tablesCache.ClearAllKeys(tableId);
                   });
            }

            private static void SetupGetRowsAndKeyPosition(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.GetKeyPosition(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       return tablesCache.GetKeyPosition(tableId, primaryKey);
                   });

                mock.Setup(p => p.GetRow(It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int tableId, int rowIndex) =>
                   {
                       return tablesCache.GetRow(tableId, rowIndex);
                   });

                mock.Setup(p => p.GetRow(It.IsAny<int>(), It.IsAny<string>()))
                   .Returns(
                   (int tableId, string primaryKey) =>
                   {
                       return tablesCache.GetRow(tableId, primaryKey);
                   });
                mock.Setup(p => p.GetKeys(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       return tablesCache.GetKeys(tableId);
                   });
            }

            private static void SetupSetRows(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, DateTime timestamp, bool? enableCellActions) =>
                   {
                       return tablesCache.SetRow(tableId, rowIndex, rowData, timestamp, enableCellActions);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData) =>
                   {
                       return tablesCache.SetRow(tableId, rowIndex, rowData);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, bool? enableCellActions) =>
                   {
                       return tablesCache.SetRow(tableId, rowIndex, rowData, null, enableCellActions);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, int rowIndex, object rowData, DateTime timestamp) =>
                   {
                       return tablesCache.SetRow(tableId, rowIndex, rowData, timestamp);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, DateTime timestamp, bool? enableCellActions) =>
                   {
                       return tablesCache.SetRow(tableId, primaryKey, rowData, timestamp, enableCellActions);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData) =>
                   {
                       return tablesCache.SetRow(tableId, primaryKey, rowData);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<bool>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, bool? enableCellActions) =>
                   {
                       return tablesCache.SetRow(tableId, primaryKey, rowData, null, enableCellActions);
                   });

                mock.Setup(p => p.SetRow(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int tableId, string primaryKey, object rowData, DateTime timestamp) =>
                   {
                       return tablesCache.SetRow(tableId, primaryKey, rowData, timestamp);
                   });
            }

            private static void SetupFillArray(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<SaveOption>(), It.IsAny<DateTime?>()))
                   .Returns(
                   (int tableId, List<object[]> rows, SaveOption option, DateTime? timeInfo) =>
                   {
                       return tablesCache.FillArray(tableId, rows, option, timeInfo);
                   });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<SaveOption>()))
                    .Returns(
                    (int tableId, List<object[]> rows, SaveOption option) =>
                    {
                        return tablesCache.FillArray(tableId, rows, option);
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>()))
                    .Returns(
                    (int tableId, List<object[]> columns) =>
                    {
                        return tablesCache.FillArray(tableId, columns);
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, List<object[]> columns, DateTime? timeInfo) =>
                    {
                        return tablesCache.FillArray(tableId, columns, timeInfo);
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] columns) =>
                    {
                        return tablesCache.FillArray(tableId, columns);
                    });

                mock.Setup(p => p.FillArray(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, object[] columns, DateTime? timeInfo) =>
                    {
                        return tablesCache.FillArray(tableId, columns, timeInfo);
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<List<object[]>>()))
                    .Returns(
                    (int tableId, List<object[]> columns) =>
                    {
                        return tablesCache.FillArrayNoDelete(tableId, columns);
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<List<object[]>>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, List<object[]> columns, DateTime? timeInfo) =>
                    {
                        return tablesCache.FillArrayNoDelete(tableId, columns, timeInfo);
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, object[] columns) =>
                    {
                        return tablesCache.FillArrayNoDelete(tableId, columns);
                    });

                mock.Setup(p => p.FillArrayNoDelete(It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, object[] columns, DateTime? timeInfo) =>
                    {
                        return tablesCache.FillArrayNoDelete(tableId, columns, timeInfo);
                    });

                mock.Setup(p => p.FillArrayWithColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<object[]>(), It.IsAny<DateTime?>()))
                    .Returns(
                    (int tableId, int columnPid, object[] keys, object[] values, DateTime? timeInfo) =>
                    {
                        tablesCache.FillArrayWithColumn(tableId, columnPid, Array.ConvertAll(keys, Convert.ToString), values, timeInfo);
                        return null; // Irrelevant return value.
                    });

                mock.Setup(p => p.FillArrayWithColumn(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object[]>(), It.IsAny<object[]>()))
                    .Returns(
                    (int tableId, int columnPid, object[] keys, object[] values) =>
                    {
                        tablesCache.FillArrayWithColumn(tableId, columnPid, Array.ConvertAll(keys, Convert.ToString), values);
                        return null; // Irrelevant return value.
                    });
            }

            private static void SetupParametersIndexByKeys(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.GetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()))
                    .Returns(
                    (int iPID, string key, int iY) =>
                    {
                        return tablesCache.GetParameterIndexByKey(iPID, key, iY);
                    });

                mock.Setup(p => p.SetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int iID, string key, int iY, object value, DateTime timeInfo) =>
                   {
                       return tablesCache.SetParameterIndexByKey(iID, key, iY, value, timeInfo);
                   });

                mock.Setup(p => p.SetParameterIndexByKey(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                   (int iID, string key, int iY, object value) =>
                   {
                       return tablesCache.SetParameterIndexByKey(iID, key, iY, value);
                   });

                mock.Setup(p => p.SetParametersIndexByKey(It.IsAny<int[]>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                   .Returns(
                   (int[] iIDs, string[] keys, int[] iYs, object[] values, DateTime?[] timeInfos) =>
                   {
                       return tablesCache.SetParametersIndexByKey(iIDs, keys, iYs, values, timeInfos);
                   });

                mock.Setup(p => p.SetParametersIndexByKey(It.IsAny<int[]>(), It.IsAny<string[]>(), It.IsAny<int[]>(), It.IsAny<object[]>()))
                   .Returns(
                   (int[] iIDs, string[] keys, int[] iYs, object[] values) =>
                   {
                       return tablesCache.SetParametersIndexByKey(iIDs, keys, iYs, values);
                   });
            }

            private static void SetupParametersIndexByCoordinates(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.GetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                   .Returns(
                   (int iID, int iX, int iY) =>
                   {
                       return tablesCache.GetParameterIndex(iID, iX, iY);
                   });

                mock.Setup(p => p.SetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>(), It.IsAny<DateTime>()))
                   .Returns(
                   (int iID, int iX, int iY, object value, DateTime timeInfo) =>
                   {
                       return tablesCache.SetParameterIndex(iID, iX, iY, value, timeInfo);
                   });

                mock.Setup(p => p.SetParameterIndex(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<object>()))
                   .Returns(
                   (int iID, int iX, int iY, object value) =>
                   {
                       return tablesCache.SetParameterIndex(iID, iX, iY, value);
                   });

                mock.Setup(p => p.SetParametersIndex(It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<object[]>(), It.IsAny<DateTime[]>()))
                   .Returns(
                   (int[] iIDs, int[] iXs, int[] iYs, object[] values, DateTime?[] timeInfos) =>
                   {
                       return tablesCache.SetParametersIndex(iIDs, iXs, iYs, values, timeInfos);
                   });

                mock.Setup(p => p.SetParametersIndex(It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<int[]>(), It.IsAny<object[]>()))
                   .Returns(
                   (int[] iIDs, int[] iXs, int[] iYs, object[] values) =>
                   {
                       return tablesCache.SetParametersIndex(iIDs, iXs, iYs, values);
                   });
            }

            private static void SetupCounts(Mock<SLProtocol> mock, TablesCache tablesCache)
            {
                mock.Setup(p => p.RowCount(It.IsAny<int>()))
                   .Returns(
                   (int tableId) =>
                   {
                       return tablesCache.RowCount(tableId);
                   });
            }
        }
    }
}