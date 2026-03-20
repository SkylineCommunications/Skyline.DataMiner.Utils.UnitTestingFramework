namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Table;

    /// <summary>
    /// Parameter model.
    /// </summary>
    /// <seealso cref="IParameterModel" />
    public class ParameterModel : IParameterModel
    {
        private readonly object syncRoot = new object();
        private object value;
        private DateTime timestamp;
        private int suspendNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModel"/> class.
        /// </summary>
        /// <param name="parameterDefinition"></param>
        /// <param name="value">The value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public ParameterModel(ParameterDefinition parameterDefinition, object value, DateTime? timestamp = null)
        {
            this.value = value;
            this.timestamp = timestamp ?? DateTime.Now;
            Definition = parameterDefinition ?? throw new ArgumentNullException(nameof(parameterDefinition));
        }

        /// <summary>
        /// Occurs when the parameter value or timestamp changes.
        /// </summary>
        public event EventHandler<ParameterModelChangedEventArgs> Changed;

        public ParameterDefinition Definition { get; }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <value>
        /// The parameter value.
        /// </value>
        public object Value
        {
            get
            {
                lock (syncRoot)
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>
        /// The timestamp.
        /// </value>
        public DateTime Timestamp
        {
            get
            {
                lock (syncRoot)
                {
                    return timestamp;
                }
            }
        }

        /// <summary>
        /// Updates the parameter value and timestamp.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public bool Update(object value, DateTime? timestamp = null)
        {
            var updatedTimestamp = timestamp ?? DateTime.Now;
            EventHandler<ParameterModelChangedEventArgs> handler;
            var oldValue = this.value;
            var oldTimestamp = this.timestamp;

            lock (syncRoot)
            {
                if (Equals(this.value, value) && this.timestamp == updatedTimestamp)
                {
                    return false;
                }

                this.value = value;
                this.timestamp = updatedTimestamp;
                handler = Changed;
            }

            if (handler != null)
            {
                handler(this, new ParameterModelChangedEventArgs(Definition, oldValue, value, oldTimestamp, updatedTimestamp));
            }

            return true;
        }

        /// <inheritdoc/>
        public IDisposable SuspendNotifications()
        {
            Interlocked.Increment(ref suspendNotifications);
            return new NotificationScope(this);
        }

        private sealed class NotificationScope : IDisposable
        {
            private readonly ParameterModel parameter;

            public NotificationScope(ParameterModel parameterModel)
            {
                parameter = parameterModel ?? throw new ArgumentNullException(nameof(parameterModel));
            }

            public void Dispose()
            {
                Interlocked.Decrement(ref parameter.suspendNotifications);
            }
        }
    }
}