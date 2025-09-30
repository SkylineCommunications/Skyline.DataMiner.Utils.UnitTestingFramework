namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    public partial class SLProtocolMock
    {
        internal class NotifyProtocolHelper
        {
            private readonly Dictionary<NotifyType, Func<object, object, object>> notifyToActionMapper = new Dictionary<NotifyType, Func<object, object, object>>();
            private readonly IProtocolCache protocolCache;

            public NotifyProtocolHelper(IProtocolCache protocolCache)
            {
                this.protocolCache = protocolCache;

                notifyToActionMapper = new Dictionary<NotifyType, Func<object, object, object>>
                {
                    { NotifyType.AddRow, (value1, value2) => AddRow(value1, value2) },
                    { NotifyType.GetParameterIndex, (value1, value2) => GetParameterIndex(value1) },

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
                    { NotifyType.NT_GET_TABLE_COLUMNS, (value1, value2) => protocolCache.Tables.GetTableColumns(Convert.ToInt32(value1), (uint[])value2) },
                    { NotifyType.ArrayRowCount, (value1, value2) => protocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                    { NotifyType.NT_GET_KEYS_SLPROTOCOL, (value1, value2) => protocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                    { NotifyType.PutParameterIndex, (value1, value2) => protocolCache.Tables.SetParameterIndex(Convert.ToInt32(((object[])value1)[0]), Convert.ToInt32(((object[])value1)[1]), Convert.ToInt32(((object[])value1)[2]), Convert.ToString(value2)) },
                    { NotifyType.NT_FILL_ARRAY_WITH_COLUMN, (v1, v2) => FillArrayWithColumn(v1, v2) },
                    { NotifyType.DeleteRow, (v1, v2) => DeleteRow(v1, v2) },
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

            internal object AddRow(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.AddRow expects first argument to be of type int, but got {value1?.GetType()} instead.");
                }

                if (!(value2 is string primaryKey))
                {
                    throw new ArgumentException($"NotifyType.AddRow expects second argument to be of type string, but got {value2?.GetType()} instead.");
                }

                return protocolCache.Tables.AddRow(tablePid, primaryKey);
            }

            internal object DeleteRow(object value1, object value2)
            {
                if (!(value1 is int tablePid))
                {
                    throw new ArgumentException();
                }

                if (!protocolCache.TableModels.TryGetValue(tablePid, out var table))
                {
                    throw new ArgumentException();
                }

                if (value2 is string primaryKey)
                {
                    table.RemoveRow(primaryKey);
                    return table.RowCount;
                }
                else if (value2 is string[] primaryKeys)
                {
                    foreach (string pk in primaryKeys)
                    {
                        table.RemoveRow(pk);
                    }
                    return table.RowCount;
                }
                else
                {
                    throw new ArgumentException($"Unsupported NotifyType.DeleteRow overload parameters: v1 - {value1.GetType()} | v2 - {value2.GetType()}");
                }
            }

            internal object FillArrayWithColumn(object value1, object value2)
            {
                if (value1 is object[] columnInfo && value2 is object[] values)
                {
                    if (columnInfo.Length == 2 && values.Length == 2)
                        return protocolCache.Tables.FillArrayWithColumn(Convert.ToInt32(columnInfo[0]), Convert.ToInt32(columnInfo[1]), (object[])values[0], (object[])values[1]);

                    if (columnInfo.Length == values.Length)
                        return protocolCache.Tables.FillArrayWithColumns(columnInfo, values);

                    throw new ArgumentException($"Unsupported NotifyType.NT_FILL_ARRAY_WITH_COLUMN parameters with different lengths between columnInfo ({columnInfo.Length}) and values ({values.Length})");
                }

                throw new ArgumentException($"Unsupported NotifyType.NT_FILL_ARRAY_WITH_COLUMN overload parameters: v1 - {value1.GetType()} | v2 - {value2.GetType()}");
            }

            internal object GetParameterIndex(object value1)
            {
                if (!(value1 is object[] value1AsArray))
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to be of type object[], but got {value1?.GetType()} instead.");

                if (value1AsArray.Length != 3)
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain three objects, but got {value1AsArray.Length} objects instead.");

                if (!(value1AsArray[0] is int tablePid))
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int as first object, but got {value1AsArray[0]?.GetType()} instead.");
                }

                if (!(value1AsArray[2] is int oneBasedColumnIndex))
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int as third object, but got {value1AsArray[2]?.GetType()} instead.");
                }

                int columnIndex = oneBasedColumnIndex - 1;

                if (value1AsArray[1] is int oneBasedRowIndex)
                {
                    return table.GetCell(oneBasedRowIndex - 1, columnIndex, ColumnIndicatorType.Index);
                }
                else if (value1AsArray[1] is string rowPrimaryKey)
                {
                    return table.GetCell(rowPrimaryKey, columnIndex, ColumnIndicatorType.Index);
                }
                else
                {
                    throw new ArgumentException($"NotifyType.GetParameterIndex expects first argument to contain an int or string as second object, but got {value1AsArray[1].GetType()} instead.");
                }
            }
        }
    }
}