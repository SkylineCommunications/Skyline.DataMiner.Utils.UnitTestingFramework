namespace Skyline.DataMiner.Utils.UnitTestingFramework.Protocol
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// SLProtocol fake.
    /// </summary>
    public class SLProtocolMock : Mock<SLProtocol>
    {
        private readonly Dictionary<NotifyType, Func<SLProtocol, object, object, object>> notifyToActionMapper = new Dictionary<NotifyType, Func<SLProtocol, object, object, object>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SLProtocolMock"/> class.
        /// </summary>
        /// <param name="protocolModel">The protocol model.</param>
        /// <exception cref="ArgumentNullException"><paramref name="protocolModel"/> is <see langword="null"/>.</exception>
        public SLProtocolMock(IProtocolModelExt protocolModel)
        {
            if (protocolModel == null)
            {
                throw new ArgumentNullException(nameof(protocolModel));
            }

            ProtocolCache = new ProtocolCache();
            protocolModel.LoadParameterValues(ProtocolCache);

            notifyToActionMapper = new Dictionary<NotifyType, Func<SLProtocol, object, object, object>>
            {
                { NotifyType.GetParameter, (p, value1, value2) => ProtocolCache.Parameters.GetParameter(Convert.ToInt32(value1)) },
                { NotifyType.GetParameterByName, (p, value1, value2) => ProtocolCache.Parameters.GetParameterByName(Convert.ToString(value1)) },
                { NotifyType.SetParameter, (p, value1, value2) => ProtocolCache.Parameters.SetParameter(Convert.ToInt32(((uint[])value1)[2]), value2) },
                { NotifyType.SetParameterByName, (p, value1, value2) => ProtocolCache.Parameters.SetParameterByName(Convert.ToString(value1), value2) },
                { NotifyType.AddRow, (p, value1, value2) => ProtocolCache.Tables.AddRow(Convert.ToInt32(value1), Convert.ToString(value2)) },
                { NotifyType.NT_ADD_ROW_RETURN_KEY, (p, value1, value2) => ProtocolCache.Tables.AddRowReturnKey(Convert.ToInt32(value1), Convert.ToString(value2)) },
                { NotifyType.DeleteRow, (p, value1, value2) => ProtocolCache.Tables.DeleteRow(Convert.ToInt32(value1), Convert.ToString(value2)) },
                { NotifyType.NT_EXISTS_ROW, (p, value1, value2) => ProtocolCache.Tables.Exists(Convert.ToInt32(value1), Convert.ToString(value2)) },
                { NotifyType.GetKeyPosition, (p, value1, value2) => ProtocolCache.Tables.GetKeyPosition(Convert.ToInt32(value1), Convert.ToString(value2)) },
                { NotifyType.NT_GET_ROW, (p, value1, value2) => ProtocolCache.Tables.GetRow(Convert.ToInt32(((object[])value1)[0]), Convert.ToString(((object[])value1)[1])) },
                { NotifyType.NT_SET_ROW, (p, value1, value2) => ProtocolCache.Tables.SetRow(Convert.ToInt32(((object[])value1)[0]), Convert.ToString(((object[])value1)[1]), value2) },
                { NotifyType.FillArray, (p, value1, value2) => ProtocolCache.Tables.FillArray(Convert.ToInt32(value1), (object[])value2) },
                { NotifyType.FillArrayNoDelete, (p, value1, value2) => ProtocolCache.Tables.FillArrayNoDelete(Convert.ToInt32(value1), (object[])value2) },
                { NotifyType.NT_FILL_ARRAY_WITH_COLUMN, (p, value1, value2) => ProtocolCache.Tables.FillArrayWithColumn(Convert.ToInt32(((object[])value1)[0]), Convert.ToInt32(((object[])value1)[1]), (object[])((object[])value2)[0], (object[])((object[])value2)[1]) },
                { NotifyType.NT_GET_TABLE_COLUMNS, (p, value1, value2) => ProtocolCache.Tables.GetTableColumns(Convert.ToInt32(value1), (uint[])value2) },
                { NotifyType.ArrayRowCount, (p, value1, value2) => ProtocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                { NotifyType.NT_GET_KEYS_SLPROTOCOL, (p, value1, value2) => ProtocolCache.Tables.RowCount(Convert.ToInt32(value1)) },
                { NotifyType.GetParameterIndex, (p, value1, value2) => ProtocolCache.Tables.GetParameterIndex(Convert.ToInt32(((object[])value1)[0]), Convert.ToInt32(((object[])value1)[1]), Convert.ToInt32(((object[])value1)[2])) },
                { NotifyType.PutParameterIndex, (p, value1, value2) => ProtocolCache.Tables.SetParameterIndex(Convert.ToInt32(((object[])value1)[0]), Convert.ToInt32(((object[])value1)[1]), Convert.ToInt32(((object[])value1)[2]), Convert.ToString(value2)) },
            };

            LoadSetups();
        }

        /// <summary>
        /// Gets the protocol cache.
        /// </summary>
        /// <value>
        /// The protocol cache.
        /// </value>
        public IProtocolCache ProtocolCache { get; }

        /// <summary>
        /// Asserts this instance.
        /// </summary>
        /// <returns><see cref="IAssert"/> interface.</returns>
        public IAssert Assert()
        {
            return new AssertHandler(ProtocolCache);
        }

        private void LoadSetups()
        {
            ProtocolCache.Parameters.LoadSetups(this);
            ProtocolCache.Tables.LoadSetups(this);
            Setup(p => p.NotifyProtocol(It.IsAny<int>(), It.IsAny<object>(), It.IsAny<object>()))
                .Returns(
                (int notifyType, object value1, object value2) =>
                {
                    return NotifyProtocol(notifyType, value1, value2);
                });
        }

        /// <summary>
        /// Notifies SLProtocol.
        /// </summary>
        /// <param name="notifyType">Type of the notify.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns>Result.</returns>
        /// <exception cref="ArgumentException">The specified Notify type is not available.</exception>
        private object NotifyProtocol(int notifyType, object value1, object value2)
        {
            NotifyType castedNotifyType = (NotifyType)notifyType;

            if (notifyToActionMapper.TryGetValue(castedNotifyType, out Func<SLProtocol, object, object, object> toExecute))
            {
                return toExecute.Invoke(Object, value1, value2);
            }
            else
            {
                throw new ArgumentException($"Notify type '{castedNotifyType} ({notifyType})' is unavailable.");
            }
        }
    }
}