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