namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    public partial class SLProtocolMock<T> where T : class, SLProtocol
    {
        internal class NotifyProtocolHelper
        {
            private static readonly IReadOnlyCollection<NotifyType> RelevantNotYetSupportedNotifyTypes = new List<NotifyType>
            {
                NotifyType.GetData, // gets raw data for a parameter
                NotifyType.GetName, // gets the name tag for a parameter
                NotifyType.GetParameterDisplayValue, // gets the display value for a parameter
                NotifyType.GetDisplayForPK, // gets the display key for a primary key
                NotifyType.GetPKForDisplay, // gets the primary key for a display key
                NotifyType.SetParameterWithWait, // sets a parameter and waits for the operation to complete
                NotifyType.GetIndexes, // gets the primary keys and display keys of a table
                NotifyType.SetBinaryData, // sets binary data for a parameter
                NotifyType.GetKeysForIndex, // gets the primary keys of all rows that have the specified value for the specified column.
                NotifyType.NT_REBUILD_INDEX, // rebuilds the index of the specified column
                NotifyType.NT_SET_PARAMETER_WITH_HISTORY, // sets a parameter with the provided timestamp
                NotifyType.NT_INCREMENT_ROW, // Adds the specified numeric values to the current values in the row.
                NotifyType.NT_GET_COLUMN, // ?
                NotifyType.NT_GET_PARAMETER_BY_OID, // gets a parameter value by its SNMP OID
                NotifyType.NT_FILL_ARRAY_WITH_COLUMN_ONLY_UPDATES, // Sets or updates one or more table columns with the provided values
                NotifyType.NT_SET_PARAMETER_BY_ID, // ?
                NotifyType.NT_SET_PARAMETER_CHECK_CONDITIONS, // ?
                NotifyType.NT_GET_KEYS_SLPROTOCOL, // Retrieves the primary keys of a table from the SLProtocol process without interacting with the SLElement process.
                NotifyType.NT_GET_KEYS_FOR_INDEX_CASED, // Gets the primary keys of all rows that have the specified value (case sensitive) for the specified column
                NotifyType.NT_GET_TABLE_PARAMETER_VALUE_BY_INDEX, // ?
            };

            private readonly Dictionary<NotifyType, Func<object, object, object>> notifyToActionMapper = new Dictionary<NotifyType, Func<object, object, object>>();
            private readonly ParametersAndTables parametersAndTables;

            public NotifyProtocolHelper(ParametersAndTables parametersAndTables)
            {
                this.parametersAndTables = parametersAndTables ?? throw new ArgumentNullException(nameof(parametersAndTables));

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
                NotifyType castedNotifyType;

                try
                {
                    castedNotifyType = (NotifyType)notifyType;
                }
                catch
                {
                    // Ignore irrelevant notify types.
                    return null; // Irrelevant return value
                }

                if (notifyToActionMapper.TryGetValue(castedNotifyType, out var functionToExecute))
                {
                    return functionToExecute.Invoke(value1, value2);
                }
                else if (RelevantNotYetSupportedNotifyTypes.Contains(castedNotifyType))
                {
                    throw new NotImplementedException($"Notify type '{castedNotifyType} ({notifyType})' is recognized but not yet implemented in this mock. Please create a feature request.");
                }
                else
                {
                    // Ignore irrelevant notify types.
                    return null; // Irrelevant return value
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

                parametersAndTables.GetTable(tablePid).SetParameterIndex(oneBasedRowIndex, oneBasedColumnIndex, value2);
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

                var table = parametersAndTables.GetTable(tablePid);

                return table.GetRow(primaryKey) ?? new object[table.Schema.ColumnDefinitions.Count];
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

                parametersAndTables.GetParameter((int)value1AsArray[2]).Update(value2);
                return null; // Irrelevant return value.
            }

            internal object SetParameterByName(object value1, object value2)
            {
                if (!(value1 is string parameterName))
                {
                    throw new ArgumentException($"NotifyType.SetParameterByName expects first argument to be of type string, but got {value1?.GetType()} instead.");
                }

                parametersAndTables.GetParameter(parameterName).Update(value2);
                return null; // Irrelevant return value.
            }

            internal object GetParameterByName(object value1, object value2)
            {
                if (!(value1 is string parameterName))
                {
                    throw new ArgumentException($"NotifyType.GetParameterByName expects first argument to be of type string, but got {value1?.GetType()} instead.");
                }

                return parametersAndTables.GetParameter(parameterName).Value;
            }

            internal object GetParameter(object value1, object value2)
            {
                if (!(value1 is int parameterId))
                {
                    throw new ArgumentException($"NotifyType.GetParameter expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                return parametersAndTables.GetParameter(parameterId).Value;
            }

            internal object RowCount(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.RowCount expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                return parametersAndTables.GetTable(tablePid).RowCount;
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

                return parametersAndTables.GetTable(tablePid).GetRowIndex(primaryKey) + 1;
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

                return parametersAndTables.GetTable(tablePid).RowExists(primaryKey);
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

                return parametersAndTables.GetTable(tablePid).AddRowReturnKey(primaryKey);
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

                parametersAndTables.GetTable(tablePid).SetRowReturnChanges(primaryKey, rowValues, timestamp, useClearAndLeave);

                return new object[rowValues.Length];
            }

            internal object FillArrayNoDelete(object value1, object value2)
            {
                return FillArrayInternal(value1, value2, TableModelExtensionsForProtocol.FillArrayNoDelete);
            }
            
            internal object FillArray(object value1, object value2)
            {
                return FillArrayInternal(value1, value2, TableModelExtensionsForProtocol.FillArray);
            }

            private object FillArrayInternal(object value1, object value2, Action<ITableModel, object[][], DateTime?, bool> fillArrayMethod)
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

                fillArrayMethod.Invoke(parametersAndTables.GetTable(tablePid), arrayOfObjectArrays, timestamp, useClearAndLeave);

                return null; // Irrelevant return value.
            }

            public static TCast[] CastItemsOrThrow<TCast>(object[] arrayToConvert)
            {
                if (arrayToConvert is null)
                {
                    throw new ArgumentNullException(nameof(arrayToConvert));
                }

                var arrayOfType = arrayToConvert.OfType<TCast>().ToArray();

                if (arrayToConvert.Length != arrayOfType.Length)
                {
                    throw new ArgumentException($"Expected all items to be of type {typeof(TCast).Name}");
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

                return parametersAndTables.GetTable(tablePid).GetTableColumns(columnIndices);
            }

            internal object AddRow(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.AddRow expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (value2 is string primaryKey)
                {
                    return parametersAndTables.GetTable(tablePid).SetRowReturnOneBasedIndex(primaryKey);
                }
                else if(value2 is object[] objectArray)
                {    
                    if (objectArray[0] is object[] rowData && objectArray[1] is DateTime timestamp && objectArray.Length == 2)
                    {
                        return parametersAndTables.GetTable(tablePid).SetRowReturnOneBasedIndex(rowData, timestamp);
                    }
                    else
                    {
                        return parametersAndTables.GetTable(tablePid).SetRowReturnOneBasedIndex(objectArray);
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
                    var table = parametersAndTables.GetTable(tablePid);
                    table.RemoveRows(primaryKey);
                    return table.RowCount;
                }
                else if (value2 is string[] primaryKeys)
                {
                    var table = parametersAndTables.GetTable(tablePid);

                    table.RemoveRows(primaryKeys);

                    return table.RowCount;
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

                    parametersAndTables.GetTable(tablePid).FillArrayWithColumn(columnPid, primaryKeys, columnValues.Single(), timestamp, useClearAndLeave);
                }
                else
                {
                    var columnPids = Array.ConvertAll(columnInfo.Skip(1).Take(columnsToSetCount).ToArray(), Convert.ToInt32);

                    var columnPidsToValues = columnPids.ToDictionary(pid => pid, pid => columnValues[Array.IndexOf(columnPids, pid)]);

                    parametersAndTables.GetTable(tablePid).FillArrayWithColumns(primaryKeys, columnPidsToValues, timestamp, useClearAndLeave);
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
                    return parametersAndTables.GetTable(tablePid).GetParameterIndex(oneBasedRowIndex, oneBasedColumnIndex);
                }
                else if (value1AsArray[1] is string rowPrimaryKey)
                {
                    return parametersAndTables.GetTable(tablePid).GetParameterIndexByKey(rowPrimaryKey, oneBasedColumnIndex);
                }
                else
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int or string as second object, but got {value1AsArray[1].GetType()} instead.");
                }
            }
        }
    }
}