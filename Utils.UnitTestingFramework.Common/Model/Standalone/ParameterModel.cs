namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone
{
    using System;
    using System.Threading;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model;

    /// <summary>
    /// Parameter model.
    /// </summary>
    /// <seealso cref="IParameterModel" />
    public class ParameterModel : ParameterModelBase<ParameterDefinition>, IParameterModel
    {
        private int suspendNotifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterModel"/> class.
        /// </summary>
        /// <param name="parameterDefinition"></param>
        /// <param name="value">The value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public ParameterModel(ParameterDefinition parameterDefinition, object value, DateTime? timestamp = null)
            : base(parameterDefinition, value, timestamp)
        {

        }

        /// <summary>
        /// Occurs when the parameter value or timestamp changes.
        /// </summary>
        public event EventHandler<ParameterModelChangedEventArgs> Changed;

        /// <summary>
        /// Updates the parameter value and timestamp.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public override bool Update(object value, DateTime? timestamp = null)
        {
            var oldValue = Value;
            var oldTimestamp = Timestamp;

            bool changed = base.Update(value, timestamp);

            if (changed && suspendNotifications == 0)
            {
                Changed?.Invoke(this, new ParameterModelChangedEventArgs(Definition, oldValue, Value, oldTimestamp, Timestamp));
            }

            return changed;
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