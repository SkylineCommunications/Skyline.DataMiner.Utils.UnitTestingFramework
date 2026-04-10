namespace Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model
{
    using System;

    internal class ParameterModelBase<TDefinition> where TDefinition : ParameterDefinition
    {
        private readonly object syncRoot = new object();
        private object value;
        private DateTime timestamp;

        public ParameterModelBase(TDefinition definition, object value, DateTime? timestamp = null)
        {
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));

            Definition.Validate(value);

            this.value = value;
            this.timestamp = timestamp ?? DateTime.Now;
        }

        public TDefinition Definition { get; }

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
        /// Updates the parameter value and timestamp, returning a value indicating whether the value was actually updated.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="timestamp">The timestamp.</param>
        public virtual bool Update(object value, DateTime? timestamp = null)
        {
            lock (syncRoot)
            {
                Definition.Validate(value);

                var updatedTimestamp = timestamp ?? DateTime.Now;

                if (Equals(this.value, value) && this.timestamp == updatedTimestamp)
                {
                    return false;
                }

                this.value = value;
                this.timestamp = updatedTimestamp;
            }

            return true;
        }
    }
}