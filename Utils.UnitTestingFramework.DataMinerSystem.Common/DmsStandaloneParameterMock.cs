namespace Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common
{
    using System;

    using Moq;

    using Skyline.DataMiner.Core.DataMinerSystem.Common;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Common.Model.Standalone;

    /// <summary>
    /// Mock of an <see cref="IDmsStandaloneParameter{T}"/> that is backed by an <see cref="IParameterModel"/>,
    /// so that values that are set are stored and can be retrieved again.
    /// </summary>
    /// <typeparam name="T">The type of the parameter value.</typeparam>
    internal class DmsStandaloneParameterMock<T> : Mock<IDmsStandaloneParameter<T>>
    {
        private readonly IParameterModel parameterModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="DmsStandaloneParameterMock{T}"/> class.
        /// </summary>
        /// <param name="parameterModel">The parameter model that holds the value.</param>
        /// <param name="element">The element this parameter belongs to.</param>
        /// <exception cref="ArgumentNullException"><paramref name="parameterModel"/> is <see langword="null"/>.</exception>
        public DmsStandaloneParameterMock(IParameterModel parameterModel, IDmsElement element)
        {
            this.parameterModel = parameterModel ?? throw new ArgumentNullException(nameof(parameterModel));

            Setup(p => p.Id).Returns(parameterModel.Definition.Pid);
            Setup(p => p.Element).Returns(element);

            Setup(p => p.GetValue()).Returns(() => ValueConverter.Convert<T>(this.parameterModel.Value));

            Setup(p => p.SetValue(It.IsAny<T>()))
                .Callback((T value) => this.parameterModel.Update(value));

            Setup(p => p.SetValue(It.IsAny<T>(), It.IsAny<TimeSpan>(), It.IsAny<Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges>()))
                .Callback((T value, TimeSpan _, Skyline.DataMiner.Core.DataMinerSystem.Common.Subscription.Waiters.ExpectedChanges __) => this.parameterModel.Update(value));
        }
    }
}
