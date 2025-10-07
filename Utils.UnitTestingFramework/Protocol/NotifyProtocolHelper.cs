namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
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
                    { NotifyType.FillArray, FillArray },
                    { NotifyType.FillArrayNoDelete, FillArrayNoDelete },
                    { NotifyType.NT_SET_ROW, SetRow },
                    { NotifyType.NT_ADD_ROW_RETURN_KEY, AddRowReturnKey },
                    { NotifyType.NT_EXISTS_ROW, Exists },
                    { NotifyType.GetKeyPosition, GetKeyPosition },
                    { NotifyType.ArrayRowCount, RowCount },
                    { NotifyType.NT_GET_KEYS_SLPROTOCOL, RowCount },
                    { NotifyType.GetParameter, GetParameter },
                    { NotifyType.GetParameterByName, GetParameterByName },
                    { NotifyType.SetParameterByName, SetParameterByName },
                    { NotifyType.SetParameter, SetParameter },
                    { NotifyType.NT_GET_ROW, GetRow },
                    { NotifyType.PutParameterIndex, SetParameterIndex },
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
                    throw new ArgumentException($"Notify type '{castedNotifyType} ({notifyType})' is not supported.");
                }
            }

            internal object SetParameterIndex(object value1, object value2)
            {
                if (!(value1 is object[] value1AsArray))
                {
                    throw new ArgumentException($"NotifyType.SetParameterIndex expects first argument to be of type object[], but got {value1?.GetType()} instead.");
                }

                if (value1AsArray.Length != 3)
                {
                    throw new ArgumentException($"NotifyType.SetParameterIndex expects first argument to contain three objects, but got {value1AsArray.Length} objects instead.");
                }

                if (!(value1AsArray[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.SetParameterIndex expects first argument to contain an int as first object, but got {value1AsArray[0]?.GetType()} instead.");
                }

                if (!(value1AsArray[1] is int oneBasedRowIndex))
                {
                    throw new ArgumentException($"NotifyType.SetParameterIndex expects first argument to contain an int as second object, but got {value1AsArray[1]?.GetType()} instead.");
                }

                if (!(value1AsArray[2] is int oneBasedColumnIndex))
                {
                    throw new ArgumentException($"NotifyType.SetParameterIndex expects first argument to contain an int as third object, but got {value1AsArray[2]?.GetType()} instead.");
                }

                protocolCache.Tables.SetParameterIndex(tablePid, oneBasedRowIndex, oneBasedColumnIndex, value2);
                return null; // Irrelevant return value.
            }

            internal object GetRow(object value1, object value2)
            {
                if (!(value1 is object[] rowInfo))
                {
                    throw new ArgumentException($"NotifyType.GetRow expects first argument to be of type object[], but got {value1?.GetType()} instead.");
                }

                if (rowInfo.Length < 2)
                {
                    throw new ArgumentException($"NotifyType.GetRow expects first argument to contain at least two objects, but got {rowInfo.Length} objects instead.");
                }

                if (!(rowInfo[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.GetRow expects first argument to contain an int as first object, but got {rowInfo[0]?.GetType()} instead.");
                }

                if (!(rowInfo[1] is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.GetRow expects first argument to contain a string as second object, but got {rowInfo[1]?.GetType()} instead.");
                }

                return protocolCache.Tables.GetRow(tablePid, primaryKey);
            }

            internal object SetParameter(object value1, object value2)
            {
                if (!(value1 is uint[] value1AsArray))
                {
                    throw new ArgumentException($"NotifyType.SetParameter expects first argument to be of type uint[], but got {value1?.GetType()} instead.");
                }

                if (value1AsArray.Length != 3)
                {
                    throw new ArgumentException($"NotifyType.SetParameter expects first argument to contain three uint values, but got {value1AsArray.Length} values instead.");
                }

                protocolCache.Parameters.SetParameter((int)value1AsArray[2], value2);
                return null; // Irrelevant return value.
            }

            internal object SetParameterByName(object value1, object value2)
            {
                if (!(value1 is string parameterName))
                {
                    throw new ArgumentException($"NotifyType.SetParameterByName expects first argument to be of type string, but got {value1?.GetType()} instead.");
                }

                protocolCache.Parameters.SetParameterByName(parameterName, value2);
                return null; // Irrelevant return value.
            }

            internal object GetParameterByName(object value1, object value2)
            {
                if (!(value1 is string parameterName))
                {
                    throw new ArgumentException($"NotifyType.GetParameterByName expects first argument to be of type string, but got {value1?.GetType()} instead.");
                }

                return protocolCache.Parameters.GetParameterByName(parameterName);
            }

            internal object GetParameter(object value1, object value2)
            {
                if (!(value1 is int parameterId))
                {
                    throw new ArgumentException($"NotifyType.GetParameter expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                return protocolCache.Parameters.GetParameter(parameterId);
            }

            internal object RowCount(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.RowCount expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                return protocolCache.Tables.RowCount(tablePid);
            }

            internal object GetKeyPosition(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.GetKeyPosition expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.GetKeyPosition expects second argument to be of type string, but got {value2?.GetType()} instead.");
                }

                return protocolCache.Tables.GetOneBasedRowIndex(tablePid, primaryKey);
            }

            internal object Exists(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.Exists expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.Exists expects second argument to be of type string, but got {value2?.GetType()} instead.");
                }

                return protocolCache.Tables.Exists(tablePid, primaryKey);
            }

            internal string AddRowReturnKey(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.AddRowReturnKey expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.AddRowReturnKey expects second argument to be of type string, but got {value2?.GetType()} instead.");
                }

                return protocolCache.Tables.AddRowReturnKey(tablePid, primaryKey);
            }

            internal object SetRow(object value1, object value2)
            {
                if (!(value1 is object[] rowInfo))
                {
                    throw new ArgumentException($"NotifyType.SetRow expects first argument to be of type object[], but got {value1?.GetType()} instead.");
                }

                if (rowInfo.Length < 2)
                {
                    throw new ArgumentException($"NotifyType.SetRow expects first argument to contain at least two objects, but got {rowInfo.Length} objects instead.");
                }

                if (!(rowInfo[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.SetRow expects first argument to contain an int as first object, but got {rowInfo[0]?.GetType()} instead.");
                }

                if (!(rowInfo[1] is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.SetRow expects first argument to contain a string as second object, but got {rowInfo[1]?.GetType()} instead.");
                }

                bool useClearAndLeave = false;
                DateTime? timestamp = null;

                if (rowInfo.Length >= 3)
                {
                    if (rowInfo[2] is DateTime timestampPartOfArray)
                    {
                        timestamp = timestampPartOfArray;
                    }
                    else
                    {
                        throw new ArgumentException($"NotifyType.SetRow expects first argument to contain a DateTime as third object, but got {rowInfo[2]?.GetType()} instead.");
                    }
                }

                if (rowInfo.Length >= 4)
                {
                    if (rowInfo[3] is bool useClearAndLeavePartOfArray)
                    {
                        useClearAndLeave = useClearAndLeavePartOfArray;
                    }
                    else
                    {
                        throw new ArgumentException($"NotifyType.SetRow expects first argument to contain a bool as fourth object, but got {rowInfo[3]?.GetType()} instead.");
                    }
                }

                if (!(value2 is object[] rowValues))
                {
                    throw new ArgumentException($"NotifyType.SetRow expects second argument to be of type object[], but got {value2?.GetType()} instead.");
                }

                protocolCache.Tables.SetRow(tablePid, primaryKey, rowValues, timestamp, useClearAndLeave);

                return new object[rowValues.Length];
            }

            internal object FillArrayNoDelete(object value1, object value2)
            {
                return FillArrayInternal(value1, value2, protocolCache.Tables.FillArrayNoDelete);
            }
            
            internal object FillArray(object value1, object value2)
            {
                return FillArrayInternal(value1, value2, protocolCache.Tables.FillArray);
            }

            private object FillArrayInternal(object value1, object value2, Action<int, object[][], DateTime?, bool> fillArrayMethod)
            {
                int tablePid;
                bool useClearAndLeave = false;
                DateTime? timestamp = null;

                if (value1 is int tablePidAsSingleValue)
                {
                    tablePid = tablePidAsSingleValue;
                }
                else if (value1 is object[] tableInfo)
                {
                    if (tableInfo.Length >= 2 && tableInfo[0] is int tablePidPartOfArray && tableInfo[1] is bool clearAndLeaveFlagPartOfArray)
                    {
                        useClearAndLeave = clearAndLeaveFlagPartOfArray;
                        tablePid = tablePidPartOfArray;
                    }
                    else
                    {
                        throw new ArgumentException($"NotifyType.FillArray expects first argument to be of type int or an object[] containing an int, a bool and an optional DateTime.");
                    }

                    if (tableInfo.Length == 3 && tableInfo[2] is DateTime timestampPartOfArray)
                    {
                        timestamp = timestampPartOfArray;
                    }
                    else
                    {
                        throw new ArgumentException($"NotifyType.FillArray expects first argument to be of type int or an object[] containing an int, a bool and an optional DateTime.");
                    }
                }
                else
                {
                    throw new ArgumentException($"NotifyType.FillArray expects first argument to be of type int or an object[] containing an int and a bool, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is object[] arrayOfColumnValues))
                {
                    throw new ArgumentException($"NotifyType.FillArray expects second argument to be of type object[], but got {value2?.GetType()} instead.");
                }

                var arrayOfObjectArrays = CastItemsOrThrow<object[]>(arrayOfColumnValues);

                fillArrayMethod.Invoke(tablePid, arrayOfObjectArrays, timestamp, useClearAndLeave);

                return null; // Irrelevant return value.
            }

            public static T[] CastItemsOrThrow<T>(object[] arrayToConvert)
            {
                if (arrayToConvert is null)
                {
                    throw new ArgumentNullException(nameof(arrayToConvert));
                }

                var arrayOfType = arrayToConvert.OfType<T>().ToArray();

                if (arrayToConvert.Length != arrayOfType.Length)
                {
                    throw new ArgumentException($"Expected all items to be of type {typeof(T).Name}");
                }

                return arrayOfType;
            }

            internal object[][] GetTableColumns(object value1, object value2)
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