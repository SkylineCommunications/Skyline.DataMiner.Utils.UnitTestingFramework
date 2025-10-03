namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;

    public partial class SLProtocolMock
    {
        internal class NotifyProtocolHelper
        {
            private readonly Dictionary<NotifyType, Func<object, object, object>> notifyToActionMapper = new Dictionary<NotifyType, Func<object, object, object>>();
            private readonly ProtocolCache protocolCache;

            public NotifyProtocolHelper(ProtocolCache protocolCache)
            {
                this.protocolCache = protocolCache;

                notifyToActionMapper = new Dictionary<NotifyType, Func<object, object, object>>
                {
                    { NotifyType.DeleteRow, DeleteRow },
                    { NotifyType.GetParameterIndex, GetParameterIndex },
                    { NotifyType.AddRow, AddRow },
                    { NotifyType.NT_GET_TABLE_COLUMNS, GetTableColumns },
                    { NotifyType.NT_FILL_ARRAY_WITH_COLUMN, FillArrayWithColumn },

                    { NotifyType.GetParameter, (value1, value2) => protocolCache.Parameters.GetParameter(Convert.ToInt32(value1)) },
                    { NotifyType.GetParameterByName, (value1, value2) => protocolCache.Parameters.GetParameterByName(Convert.ToString(value1)) },
                    { NotifyType.SetParameter, (value1, value2) => protocolCache.Parameters.SetParameter(Convert.ToInt32(((uint[])value1)[2]), value2) },
                    { NotifyType.SetParameterByName, (value1, value2) => protocolCache.Parameters.SetParameterByName(Convert.ToString(value1), value2) },
                    { NotifyType.NT_ADD_ROW_RETURN_KEY, (value1, value2) => protocolCache.Tables.AddRowReturnKey(Convert.ToInt32(value1), Convert.ToString(value2)) },
                    { NotifyType.NT_EXISTS_ROW, (value1, value2) => protocolCache.Tables.Exists(Convert.ToInt32(value1), Convert.ToString(value2)) },
                    { NotifyType.GetKeyPosition, (value1, value2) => protocolCache.Tables.GetKeyPosition(Convert.ToInt32(value1), Convert.ToString(value2)) },
                    { NotifyType.NT_GET_ROW, (value1, value2) => protocolCache.Tables.GetRow(Convert.ToInt32(((object[])value1)[0]), Convert.ToString(((object[])value1)[1])) },
                    { NotifyType.NT_SET_ROW, (value1, value2) => protocolCache.Tables.SetRow(Convert.ToInt32(((object[])value1)[0]), Convert.ToString(((object[])value1)[1]), value2) },
                    { NotifyType.FillArray, (value1, value2) => protocolCache.Tables.FillArray(Convert.ToInt32(value1), (object[])value2) },
                    { NotifyType.FillArrayNoDelete, (value1, value2) => protocolCache.Tables.FillArrayNoDelete(Convert.ToInt32(value1), (object[])value2) },
                    { NotifyType.ArrayRowCount, (value1, value2) => protocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                    { NotifyType.NT_GET_KEYS_SLPROTOCOL, (value1, value2) => protocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                    { NotifyType.PutParameterIndex, (value1, value2) => protocolCache.Tables.SetParameterIndex(Convert.ToInt32(((object[])value1)[0]), Convert.ToInt32(((object[])value1)[1]), Convert.ToInt32(((object[])value1)[2]), Convert.ToString(value2)) },
                };
            }

            public object Execute(int notifyType, object value1, object value2)
            {
                NotifyType castedNotifyType = (NotifyType)notifyType;

                if (notifyToActionMapper.TryGetValue(castedNotifyType, out var functionToExecute))
                {
                    return functionToExecute.Invoke(value1, value2);
                }
                else
                {
                    throw new ArgumentException($"Notify type '{castedNotifyType} ({notifyType})' is unavailable.");
                }
            }

            internal object GetTableColumns(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.GetTableColumns expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is uint[] columnIndices))
                {
                    throw new ArgumentException($"NotifyType.GetTableColumns expects second argument to be of type uint[], but got {value2?.GetType()} instead.");
                }

                return protocolCache.Tables.GetTableColumns(tablePid, columnIndices);
            }

            internal object AddRow(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.AddRow expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (value2 is string primaryKey)
                {
                    return protocolCache.Tables.AddRow(tablePid, primaryKey);
                }
                else if(value2 is object[] objectArray)
                {    
                    if (objectArray[0] is object[] rowData && objectArray[1] is DateTime timestamp && objectArray.Length == 2)
                    {
                        return protocolCache.Tables.AddRow(tablePid, rowData, timestamp);
                    }
                    else
                    {
                        return protocolCache.Tables.AddRow(tablePid, objectArray);
                    }
                }
                else
                {
                    throw new ArgumentException($"NotifyType.AddRow expects second argument to be of type string or object[], but got {value2?.GetType()} instead.");
                }
            }

            internal object DeleteRow(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.DeleteRow expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (value2 is string primaryKey)
                {
                    return protocolCache.Tables.DeleteRow(tablePid, primaryKey);
                }
                else if (value2 is string[] primaryKeys)
                {
                    foreach (string pk in primaryKeys)
                    {
                        protocolCache.Tables.DeleteRow(tablePid, pk);
                    }

                    return protocolCache.Tables.RowCount(tablePid);
                }
                else
                {
                    throw new ArgumentException($"NotifyType.DeleteRow expects second argument to be of type string or string[], but got {value2?.GetType()} instead.");
                }
            }

            internal object FillArrayWithColumn(object value1, object value2)
            {
                if (!(value1 is object[] columnInfo))
                {
                    throw new ArgumentException($"NotifyType.NT_FILL_ARRAY_WITH_COLUMN expects first argument to be of type object[], but got {value1?.GetType()} instead.");
                }

                if (columnInfo.Length < 2)
                {
                    throw new ArgumentException($"NotifyType.NT_FILL_ARRAY_WITH_COLUMN expects first argument to contain at least two objects, but got {columnInfo.Length} objects instead.");
                }

                if (!(columnInfo[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.NT_FILL_ARRAY_WITH_COLUMN expects first argument to contain an int as first object, but got {columnInfo[0]?.GetType()} instead.");
                }

                if (!(value2 is object[] values))
                {
                    throw new ArgumentException($"NotifyType.NT_FILL_ARRAY_WITH_COLUMN expects first argument to be of type object[], but got {value2?.GetType()} instead.");
                }

                if (!(values[0] is object[] primaryKeysAsObjects))
                {
                    throw new ArgumentException($"NotifyType.NT_FILL_ARRAY_WITH_COLUMN expects second argument to contain an object[] as first object, but got {values[0]?.GetType()} instead.");
                }

                var primaryKeys = Array.ConvertAll(primaryKeysAsObjects, Convert.ToString);
                var columnValues = values.Skip(1).Cast<object[]>().ToArray();

                bool oneOrMoreCellValuesAreArrays = columnValues.Any(cv => cv.Any(cellValue => cellValue is object[]));
                if (oneOrMoreCellValuesAreArrays)
                {
                    // Adding timestamps to individual column values is not supported.
                    throw new ArgumentException("This mock implementation of NotifyType.NT_FILL_ARRAY_WITH_COLUMN does not support adding timestamps to individual column values.");
                }

                int columnsToSetCount = values.Length - 1; // First item is always the primary keys.

                bool useClearAndLeave = false;
                var timestamp = DateTime.Now;

                bool columnInfoHasOptions = columnInfo.Length == columnsToSetCount + 2;
                if (columnInfoHasOptions) 
                {
                    var optionsItem = columnInfo.Last();

                    if (optionsItem is bool lastColumnInfoItemAsBoolean)
                    {
                        useClearAndLeave = lastColumnInfoItemAsBoolean;
                    }
                    else if (optionsItem is object[] lastColumInfoItemAsArray)
                    {
                        if (lastColumInfoItemAsArray.Length > 0 && lastColumInfoItemAsArray[0] is bool lastColumInfoItemAsArrayFirstItem)
                        {
                            useClearAndLeave = lastColumInfoItemAsArrayFirstItem;
                        }

                        if (lastColumInfoItemAsArray.Length == 2 && lastColumInfoItemAsArray[1] is DateTime lastColumInfoItemAsArraySecondItem)
                        {
                            timestamp = lastColumInfoItemAsArraySecondItem;
                        }
                    }
                }

                if (columnsToSetCount == 1)
                {
                    int columnPid = Convert.ToInt32(columnInfo[1]);

                    protocolCache.Tables.FillArrayWithColumn(tablePid, columnPid, primaryKeys, columnValues.Single(), timestamp, useClearAndLeave);
                }
                else
                {
                    var columnPids = Array.ConvertAll(columnInfo.Skip(1).Take(columnsToSetCount).ToArray(), Convert.ToInt32);

                    var columnPidsToValues = columnPids.ToDictionary(pid => pid, pid => columnValues[Array.IndexOf(columnPids, pid)]);

                    protocolCache.Tables.FillArrayWithColumns(tablePid, primaryKeys, columnPidsToValues, timestamp, useClearAndLeave);
                }

                return null; // Irrelevant return value.
            }

            internal object GetParameterIndex(object value1, object value2)
            {
                if (!(value1 is object[] value1AsArray))
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to be of type object[], but got {value1?.GetType()} instead.");
                }

                if (value1AsArray.Length != 3)
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain three objects, but got {value1AsArray.Length} objects instead.");
                }

                if (!(value1AsArray[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int as first object, but got {value1AsArray[0]?.GetType()} instead.");
                }

                if (!(value1AsArray[2] is int oneBasedColumnIndex))
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int as third object, but got {value1AsArray[2]?.GetType()} instead.");
                }

                if (value1AsArray[1] is int oneBasedRowIndex)
                {
                    return protocolCache.Tables.GetParameterIndex(tablePid, oneBasedRowIndex, oneBasedColumnIndex);
                }
                else if (value1AsArray[1] is string rowPrimaryKey)
                {
                    return protocolCache.Tables.GetParameterIndexByKey(tablePid, rowPrimaryKey, oneBasedColumnIndex);
                }
                else
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int or string as second object, but got {value1AsArray[1].GetType()} instead.");
                }
            }
        }
    }
}