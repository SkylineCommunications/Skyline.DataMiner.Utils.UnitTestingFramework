namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;
    using System.Collections.Generic;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Selectors;
    using Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Monitors;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    /// <summary>
    /// Mock of an <see cref="IDmsStandaloneParameter{T}"/> that is backed by an <see cref="IParameterModel"/>,
    /// so that values that are set are stored and can be retrieved again.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value.</typeparam>
    internal class DmsStandaloneParameterMock<T> : Mock<IDmsStandaloneParameter<T>>
    {
        private readonly IParameterModel parameterModel;
        private readonly IDmsElement element;
        private readonly Dictionary<string, EventHandler<ParameterModelChangedEventArgs>> valueMonitors =
            new Dictionary<string, EventHandler<ParameterModelChangedEventArgs>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsStandaloneParameterMock{T}"/> class.
        /// </summary>
        /// <param name="parameterModel">The parameter model that holds the value.</param>
        /// <param name="element">The element this parameter belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterModel"/> is <see langword="null"/>.</exception>
        public DmsStandaloneParameterMock(IParameterModel parameterModel, IDmsElement element)
        {
            this.parameterModel = parameterModel ?? throw new ArgumentNullException(nameof(parameterModel));
            this.element = element;

            Setup(p => p.Id).Returns(parameterModel.Definition.Pid);
            Setup(p => p.Element).Returns(element);

            Setup(p => p.GetValue()).Returns(() => ValueConverter.Convert<T>(this.parameterModel.Value));

            Setup(p => p.SetValue(It.IsAny<T>()))
                .Callback((T value) => this.parameterModel.Update(value));

            Setup(p => p.SetValue(It.IsAny<T>(), It.IsAny<TimeSpan>(), It.IsAny<Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges>()))
                .Callback((T value, TimeSpan _, Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges __) => this.parameterModel.Update(value));

            SetupValueMonitors();
        }

        private void SetupValueMonitors()
        {
            Setup(p => p.StartValueMonitor(It.IsAny<string>(), It.IsAny<Action<ParamValueChange<T>>>(), It.IsAny<bool>()))
                .Callback((string sourceId, Action<ParamValueChange<T>> action, bool _) => StartValueMonitor(sourceId, action));

            Setup(p => p.StartValueMonitor(It.IsAny<string>(), It.IsAny<Action<ParamValueChange<T>>>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, Action<ParamValueChange<T>> action, TimeSpan _, bool __) => StartValueMonitor(sourceId, action));

            Setup(p => p.StopValueMonitor(It.IsAny<string>(), It.IsAny<bool>()))
                .Callback((string sourceId, bool _) => StopValueMonitor(sourceId));

            Setup(p => p.StopValueMonitor(It.IsAny<string>(), It.IsAny<TimeSpan>(), It.IsAny<bool>()))
                .Callback((string sourceId, TimeSpan _, bool __) => StopValueMonitor(sourceId));
        }

        private void StartValueMonitor(string sourceId, Action<ParamValueChange<T>> action)
        {
            if (sourceId == null)
            {
                throw new ArgumentNullException(nameof(sourceId));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            // Replace any existing monitor with the same source ID.
            StopValueMonitor(sourceId);

            void Handler(object sender, ParameterModelChangedEventArgs e)
            {
                var value = ValueConverter.Convert<T>(e.NewValue);
                var param = new Param(element?.AgentId ?? 0, element?.Id ?? 0, parameterModel.Definition.Pid);

                action(new ParamValueChange<T>(param, value, sourceId, null));
            }

            valueMonitors[sourceId] = Handler;
            parameterModel.Changed += Handler;
        }

        private void StopValueMonitor(string sourceId)
        {
            if (sourceId != null && valueMonitors.TryGetValue(sourceId, out var handler))
            {
                parameterModel.Changed -= handler;
                valueMonitors.Remove(sourceId);
            }
        }
    }
}
