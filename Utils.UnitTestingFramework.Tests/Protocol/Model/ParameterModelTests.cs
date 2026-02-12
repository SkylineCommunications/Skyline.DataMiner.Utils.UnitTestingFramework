namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Model
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    [TestClass]
    public class ParameterModelTests
    {
        [TestMethod]
        public void Update_ChangedEventRaisedWithValues()
        {
            // Arrange
            var initialTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var updatedTimestamp = initialTimestamp.AddMinutes(1);
            var parameter = new ParameterModel("initial", initialTimestamp);

            ParameterModelChangedEventArgs eventArgs = null;
            parameter.Changed += (sender, args) => eventArgs = args;

            // Act
            parameter.Update("updated", updatedTimestamp);

            // Assert parameter and event args
            Assert.IsNotNull(eventArgs);
            Assert.AreEqual("initial", eventArgs.OldValue);
            Assert.AreEqual("updated", eventArgs.NewValue);
            Assert.AreEqual(initialTimestamp, eventArgs.OldTimestamp);
            Assert.AreEqual(updatedTimestamp, eventArgs.NewTimestamp);
            Assert.AreEqual("updated", parameter.Value);
            Assert.AreEqual(updatedTimestamp, parameter.Timestamp);
        }

        [TestMethod]
        public void Update_ThreadSafeAndRaisesEvents()
        {
            // Arrange
            const int iterations = 100;
            var baseTimestamp = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var parameter = new ParameterModel(0, baseTimestamp);

            int eventCount = 0;
            parameter.Changed += (sender, args) => Interlocked.Increment(ref eventCount);

            // Act
            Parallel.For(0, iterations, i =>
            {
                parameter.Update(i, baseTimestamp.AddTicks(i + 1));
                _ = parameter.Value;
                _ = parameter.Timestamp;
            });

            // Assert
            Assert.AreEqual(iterations, eventCount);
            Assert.IsNotNull(parameter.Value);
            Assert.IsTrue(parameter.Timestamp >= baseTimestamp);
        }
    }
}
