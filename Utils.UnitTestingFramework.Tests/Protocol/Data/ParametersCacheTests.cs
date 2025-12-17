namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol.Data
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    public class ParametersCacheTestInstance : ParametersCache
    {
        public void LoadParameter(int pid, object value)
        {
            ParametersToValues[pid] = new ParameterModel(value);
        }
    }

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class ParametersCacheTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void GetParameterTest_ValidInput_AreEqual()
        {
            // Arrange
            var cache = new ParametersCacheTestInstance();
            cache.LoadParameter(100, 20);

            // Act
            var paramValue = cache.GetParameter(100);

            // Assert
            Assert.AreEqual(20, paramValue);
        }

        [TestMethod]
        public void GetParameterTest_InexistentKeyInput_IsNull()
        {
            // Arrange
            var cache = new ParametersCacheTestInstance();
            cache.LoadParameter(100, 20);

            // Act
            cache.TryGetParameter(101, out object paramValue);

            // Assert
            Assert.IsNull(paramValue);
        }

        [TestMethod]
        public void GetParametersTest_ValidInput_AreEqual()
        {
            // Arrange
            uint[] input = { 1, 2, 3 };
            var cache = new ParametersCacheTestInstance();
            cache.LoadParameter(1, 1);
            cache.LoadParameter(2, 2);
            cache.LoadParameter(3, 3);

            // Act
            object[] paramValue = (object[])cache.GetParameters(input);

            // Assert
            Assert.AreEqual(1, paramValue.GetValue(0));
            Assert.AreEqual(2, paramValue.GetValue(1));
            Assert.AreEqual(3, paramValue.GetValue(2));
        }

        [TestMethod]
        public void GetParametersTest_InexistentKeyInput_IsNull()
        {
            // Arrange
            uint[] input = { 1, 2, 3 };
            var cache = new ParametersCacheTestInstance();
            cache.LoadParameter(1, 1);
            cache.LoadParameter(3, 3);

            // Act
            object[] paramValue = (object[])cache.GetParameters(input);

            // Assert
            Assert.AreEqual(1, paramValue.GetValue(0));
            Assert.IsNull(paramValue.GetValue(1));
            Assert.AreEqual(3, paramValue.GetValue(2));
        }

        [TestMethod]
        public void SetParameterTest_ValidInput_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000 };
            object[] valuesArray = { 10 };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            cache.SetParameter(1000, 20);
            var value = cache.GetParameter(1000);

            // Assert
            Assert.AreEqual(20, value);
        }

        [TestMethod]
        public void SetParameterTest_AlreadyExistingKey_ChangesValue()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000 };
            object[] valuesArray = { 10 };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var outputFirstSet = cache.SetParameter(1000, 20);
            var outputSecondSet = cache.SetParameter(1000, 30);
            var outputGet = cache.GetParameter(1000);

            // Assert
            Assert.AreEqual((int)Constants.HRESULT_SUCCESS, outputFirstSet);
            Assert.AreEqual((int)Constants.HRESULT_SUCCESS, outputSecondSet);
            Assert.AreEqual(30, outputGet);
        }

        [TestMethod]
        public void SetParameterTest_InexistentKey_EmptyGet()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000 };
            object[] valuesArray = { 10 };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var outputFirstSet = cache.SetParameter(1030, 20);
            var outputSecondSet = cache.SetParameter(1030, 30);
            cache.TryGetParameter(1030, out object outputGet);

            // Assert
            Assert.AreEqual(0, outputFirstSet);
            Assert.AreEqual(0, outputSecondSet);
            Assert.IsNull(outputGet);
        }

        [TestMethod]
        public void SetParameterTest_InvalidValue_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();

            // Act
            cache.SetParameter(1, null);
            cache.TryGetParameter(1, out object value);

            // Assert
            Assert.IsNull(value);
        }

        [TestMethod]
        public void SetParameterTest_ValidInputWithTimestamp_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            DateTime date = new DateTime(2022, 7, 14);

            int[] parameterIdsArray = { 1000 };
            object[] valuesArray = { 10 };
            DateTime[] timestampsArray = { new DateTime(2022, 7, 14) };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            List<DateTime> timestamps = new List<DateTime>(timestampsArray);
            cache.LoadParameters(parameterIds, values, timestamps);

            // Act
            var outputSet = cache.SetParameter(1000, 10, date);
            var outputGet = cache.GetParameter(1000);

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.AreEqual(10, outputGet);
        }

        [TestMethod]
        public void SetParameterTest_AlreadyExistingKeyWithTimestamp_IsEqualToPreviousValue()
        {
            // Arrange
            var cache = new ParametersCache();
            DateTime date = new DateTime(2022, 7, 14);

            int[] parameterIdsArray = { 1000 };
            object[] valuesArray = { 10 };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var outputFirstSet = cache.SetParameter(1000, 20, date);
            var outputSecondSet = cache.SetParameter(1000, 10, date);
            var outputGet = cache.GetParameter(1000);

            // Assert
            Assert.AreEqual((int)Constants.HRESULT_SUCCESS, outputFirstSet);
            Assert.AreEqual((int)Constants.HRESULT_SUCCESS, outputSecondSet);
            Assert.AreEqual(10, outputGet);
        }

        [TestMethod]
        public void SetParametersTest_ValidInput_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 10, "15", 0x0A };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var valueArray = (int[])cache.SetParameters(parameterIdsArray, valuesArray);
            var value1 = cache.GetParameter(1000);
            var value2 = cache.GetParameter(1001);
            var value3 = cache.GetParameter(1002);

            // Assert
            Assert.AreEqual(10, value1);
            Assert.AreEqual("15", value2);
            Assert.AreEqual(0x0A, value3);

            Assert.AreEqual(0, valueArray[0]);
            Assert.AreEqual(0, valueArray[1]);
            Assert.AreEqual(0, valueArray[2]);
        }

        [TestMethod]
        public void SetParameterTest_ValidInput_DefaultValue()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 10, "15", 0x0A };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var outputSet = cache.SetParameter(1000, 35);
            var outputGet = cache.GetParameter(1000);

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.AreEqual(35, outputGet);
        }

        [TestMethod]
        public void SetParameterTest_ValidInput_FixedValue()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 10, "15", 0x0A };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var outputSet = cache.SetParameter(1002, 0x0C);
            var outputGet = cache.GetParameter(1002);

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.AreEqual(0x0C, outputGet);
        }

        [TestMethod]
        public void SetParametersTest_DifferentLenghts_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIds = { 1, 2 };
            object[] values = { 10, 20, 30 };

            // Act
            var output = cache.SetParameters(parameterIds, values);

            // Assert
            Assert.AreEqual(Constants.HRESULT_FAIL_DIFFLEN, output);
        }

        [TestMethod]
        public void SetParametersTest_ValidInput_OutputArrayAreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1, 2, 3 };
            object[] valuesArray = { 10, 20, 30 };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            int[] output = (int[])cache.SetParameters(parameterIdsArray, valuesArray);

            // Assert
            Assert.AreEqual(0, output[0]);
            Assert.AreEqual(0, output[1]);
            Assert.AreEqual(0, output[2]);
        }

        [TestMethod]
        public void SetParametersTest_ValidInputWithTimestamp_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1, 2, 3 };
            object[] valuesArray = { 10, 20, 30 };
            DateTime[] timestampsArray = { new DateTime(2022, 7, 14), new DateTime(2022, 7, 15), new DateTime(2022, 7, 16) };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            List<DateTime> timestamps = new List<DateTime>(timestampsArray);
            cache.LoadParameters(parameterIds, values, timestamps);

            // Act
            var outputSet = (int[])cache.SetParameters(parameterIdsArray, valuesArray, timestampsArray);
            var value1 = cache.GetParameter(1);
            var value2 = cache.GetParameter(2);
            var value3 = cache.GetParameter(3);

            // Assert
            Assert.AreEqual(10, value1);
            Assert.AreEqual(20, value2);
            Assert.AreEqual(30, value3);

            Assert.AreEqual(0, outputSet[0]);
            Assert.AreEqual(0, outputSet[1]);
            Assert.AreEqual(0, outputSet[2]);
        }

        [TestMethod]
        public void SetParametersTest_DifferentLengthsWithTimestamp_AreEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIds = { 1, 2 };
            object[] values = { 10, 20, 30 };
            DateTime[] timestamps = { new DateTime(2022, 7, 14), new DateTime(2022, 7, 15), new DateTime(2022, 7, 16) };

            // Act
            var output = cache.SetParameters(parameterIds, values, timestamps);

            // Assert
            Assert.AreEqual(Constants.HRESULT_FAIL_DIFFLEN, output);
        }

        [TestMethod]
        public void SetParameterByNameTest_ValidInput_IsEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002, 1003 };
            object[] valuesArray = { 10, "15", 0x0A, "20" };

            cache.LoadParameterName("NumericParameter", 1000);
            cache.LoadParameterName("StringParameter", 1001);
            cache.LoadParameterName("NumericParameterFixed", 1002);
            cache.LoadParameterName("StringParameterFixed", 1003);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var setOutput = cache.SetParameterByName("NumericParameter", 111);
            var getOutput = cache.GetParameterByName("NumericParameter");

            // Assert
            Assert.AreEqual(0, setOutput);
            Assert.AreEqual(111, getOutput);
        }

        [TestMethod]
        public void SetParameterByNameTest_WrongParameterName_IsEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002, 1003 };
            object[] valuesArray = { 10, "15", 0x0A, "20" };

            cache.LoadParameterName("NumericParameter", 1000);
            cache.LoadParameterName("StringParameter", 1001);
            cache.LoadParameterName("NumericParameterFixed", 1002);
            cache.LoadParameterName("StringParameterFixed", 1003);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values = new List<object>(valuesArray);
            cache.LoadParameters(parameterIds, values);

            // Act
            var setOutput = cache.SetParameterByName("NumericParameter2", 111);
            var getOutput = cache.GetParameterByName("NumericParameter");

            // Assert
            Assert.AreEqual(0, setOutput);
            Assert.AreEqual(10, getOutput);
        }

        [TestMethod]
        public void SetParametersByNameTest_ValidInput_IsEqual()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001, 1002, 1003 };
            object[] valuesArray = { 10, "15", 0x0A, "20" };

            cache.LoadParameterName("NumericParameter", 1000);
            cache.LoadParameterName("StringParameter", 1001);
            cache.LoadParameterName("NumericParameterFixed", 1002);
            cache.LoadParameterName("StringParameterFixed", 1003);

            string[] names = { "NumericParameter", "StringParameter" };
            object[] values1 = { 222, "333" };

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values2 = new List<object>(valuesArray);

            cache.LoadParameters(parameterIds, values2);

            // Act
            var setOutput = (uint[])cache.SetParametersByName(names, values1);
            var getOutput1 = cache.GetParameterByName("NumericParameter");
            var getOutput2 = cache.GetParameterByName("StringParameter");

            // Assert
            Assert.AreEqual(0, (int)setOutput[0]);
            Assert.AreEqual(0, (int)setOutput[1]);
            Assert.AreEqual(222, getOutput1);
            Assert.AreEqual("333", getOutput2);
        }

        [TestMethod]
        public void IsEmptyTest_Invalid_ParameterID()
        {
            // Arrange
            var cache = new ParametersCacheTestInstance();

            var mock = new SLProtocolMock(path);

            int pid = 101;

            // Act
            bool actual = cache.IsEmpty(pid, mock.Object);

            // Assert
            Assert.IsTrue(actual);
            mock.Verify(p => p.Log($"NT_GET_DATA for '{pid}' failed. 0x80040239"), Times.Once);
        }

        [TestMethod]
        public void IsEmptyTest_ParameterUninitialized()
        {
            // Arrange
            var cache = new ParametersCacheTestInstance();
            int pid = 100;
            cache.LoadParameter(pid, null);

            var mock = new SLProtocolMock(path);

            // Act
            bool actual = cache.IsEmpty(pid, mock.Object);

            // Assert
            Assert.IsTrue(actual);
            mock.Verify(p => p.Log($"NT_GET_DATA for {pid} failed. 0x80040239"), Times.Never);
        }

        [TestMethod]
        public void IsEmptyTest_ParameterInitialized()
        {
            // Arrange
            var cache = new ParametersCacheTestInstance();
            cache.LoadParameter(100, 20);

            var mock = new SLProtocolMock(path);

            // Act
            bool actual = cache.IsEmpty(100, mock.Object);

            // Assert
            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void TryGetParameterNameByPIDTest_CorrectOutput()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001 };
            object[] valuesArray = { 10, "15" };

            cache.LoadParameterName("NumericParameter", 1000);
            cache.LoadParameterName("StringParameter", 1001);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values2 = new List<object>(valuesArray);

            cache.LoadParameters(parameterIds, values2);

            // Act
            var getOutput1 = cache.TryGetParameterNameByPID(1000, out string paramName_1000);
            var getOutput2 = cache.TryGetParameterNameByPID(1001, out string paramName_1001);

            // Assert
            Assert.AreEqual("NumericParameter", paramName_1000);
            Assert.AreEqual("StringParameter", paramName_1001);
            Assert.IsTrue(getOutput1);
            Assert.IsTrue(getOutput2);
        }

        [TestMethod]
        public void TryGetParameterNameByPIDTest_NoNameWasLoaded()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001 };
            object[] valuesArray = { 10, "15" };

            cache.LoadParameterName("NumericParameter", 1000);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values2 = new List<object>(valuesArray);

            cache.LoadParameters(parameterIds, values2);

            // Act
            var getOutput1 = cache.TryGetParameterNameByPID(1000, out string paramName_1000);
            var getOutput2 = cache.TryGetParameterNameByPID(1001, out string paramName_1001);

            // Assert
            Assert.AreEqual("NumericParameter", paramName_1000);
            Assert.IsNull(paramName_1001);
            Assert.IsTrue(getOutput1);
            Assert.IsFalse(getOutput2);
        }

        [TestMethod]
        public void GetParametersNamesByPIDTest_CorrectOutput()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001 };
            object[] valuesArray = { 10, "15" };

            cache.LoadParameterName("NumericParameter", 1000);
            cache.LoadParameterName("StringParameter", 1001);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values2 = new List<object>(valuesArray);

            cache.LoadParameters(parameterIds, values2);

            string[] expected = new string[] { "NumericParameter", "StringParameter" };

            // Act
            string[] actual = cache.GetParametersNamesByPID(new int[] { 1000, 1001 });

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetParametersNamesByPIDTest_NoNameWasLoaded()
        {
            // Arrange
            var cache = new ParametersCache();
            int[] parameterIdsArray = { 1000, 1001 };
            object[] valuesArray = { 10, "15" };

            cache.LoadParameterName("NumericParameter", 1000);

            List<int> parameterIds = new List<int>(parameterIdsArray);
            List<object> values2 = new List<object>(valuesArray);

            cache.LoadParameters(parameterIds, values2);

            string[] expected = new string[] { "NumericParameter", null };

            // Act
            string[] actual = cache.GetParametersNamesByPID(new int[] { 1000, 1001 });

            // Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}