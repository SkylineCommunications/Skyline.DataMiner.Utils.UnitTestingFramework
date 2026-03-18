namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using System;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Constants;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class ProtocolMock_ParameterMethods_Tests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetAndSetParameter()
        {
            // Arrange
            var mock = new SLProtocolMock(path);

            // Act
            mock.Object.SetParameter(1000, 20);
            var output = mock.Object.GetParameter(1000);

            // Assert
            Assert.AreEqual(20, output);
        }

        [TestMethod]
        public void SLProtocolMockTest_InvalidTypeStringPidGet_ArgumentException()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            string[] parameterIds = { "100", "200", "300" };

            // Act
            mock.Object.SetParameter(100, 10);
            mock.Object.SetParameter(200, 20);
            mock.Object.SetParameter(300, 30);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetParameters(parameterIds));
        }

        [TestMethod]
        public void SLProtocolMockTest_InvalidTypeObjectPidGet_ArgumentException()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            object[] parameterIds = { "100", "200", "300" };

            // Act
            mock.Object.SetParameter(100, 10);
            mock.Object.SetParameter(200, 20);
            mock.Object.SetParameter(300, 30);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetParameters(parameterIds));
        }

        [TestMethod]
        public void SLProtocolMockTest_InvalidStringGet_GetAndSetParameter()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var outputSet = mock.Object.SetParameter(1000, 20);
            var outputGet = mock.Object.GetParameter(1010);

            // Assert
            Assert.AreEqual(0, outputSet);
            Assert.IsNull(outputGet);
        }

        [TestMethod]
        public void SLProtocolMockTest_DuplicatedSet_SetAndSetParameterValidGet()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var outputSet1 = mock.Object.SetParameter(1000, 20);
            var outputSet2 = mock.Object.SetParameter(1000, 25);
            var outputGet = mock.Object.GetParameter(1000);

            // Assert
            Assert.AreEqual(0, outputSet1);
            Assert.AreEqual(0, outputSet2);
            Assert.AreEqual(25, outputGet);
        }

        [TestMethod]
        public void SLProtocolMockTest_InvalidSetParameters_ArrayWithFailConstant()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            int[] parameterIdsArray = { 1000, 1001, 1002, 1003, 1004 };
            object[] valuesArray = { 10, "15", 0x0A, "parameterValue", 11 };

            // Act
            var value1 = (int[])mock.Object.SetParameters(parameterIdsArray, valuesArray);
            var outputGet1 = mock.Object.GetParameter(1000);
            var outputGet2 = mock.Object.GetParameter(1001);
            var outputGet3 = mock.Object.GetParameter(1002);
            var outputGet4 = mock.Object.GetParameter(1003);
            var outputGet5 = mock.Object.GetParameter(1004);

            // Assert
            Assert.AreEqual(0, value1[0]);
            Assert.AreEqual(0, value1[1]);
            Assert.AreEqual(0, value1[2]);
            Assert.AreEqual(0, value1[3]);
            Assert.AreEqual(Constants.HRESULT_FAIL_IDINEXISTENT, value1[4]);
            Assert.AreEqual(10, outputGet1);
            Assert.AreEqual("15", outputGet2);
            Assert.AreEqual(0x0A, outputGet3);
            Assert.AreEqual("parameterValue", outputGet4);
            Assert.IsNull(outputGet5);
        }

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetParameterAndSetParameters()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 10, "15", 0x0A };

            // Act
            var value1 = (uint[])mock.Object.SetParameters(parameterIdsArray, valuesArray);
            var value2 = mock.Object.GetParameter(1000);
            var value3 = mock.Object.GetParameter(1001);
            var value4 = mock.Object.GetParameter(1002);

            // Assert
            Assert.AreEqual(0, (int)value1[0]);
            Assert.AreEqual(0, (int)value1[1]);
            Assert.AreEqual(0, (int)value1[2]);
            Assert.AreEqual(10, value2);
            Assert.AreEqual("15", value3);
            Assert.AreEqual(0x0A, value4);
        }

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetParametersAndSetParameters()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            uint[] uintParameterIds = { 1000, 1001, 1002 };
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 11, "16", 0x0B };

            // Act
            var outputSet = (int[])mock.Object.SetParameters(parameterIdsArray, valuesArray);
            object[] paramValue = (object[])mock.Object.GetParameters(uintParameterIds);

            // Assert
            Assert.AreEqual(0, outputSet[0]);
            Assert.AreEqual(0, outputSet[1]);
            Assert.AreEqual(0, outputSet[2]);
            Assert.AreEqual(11, paramValue.GetValue(0));
            Assert.AreEqual("16", paramValue.GetValue(1));
            Assert.AreEqual(0x0B, paramValue.GetValue(2));
        }

        [TestMethod]
        public void SLProtocolMockTest_InexistentPidGetParameters_GetIsNull()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIds = { 1, 2, 3 };
            object[] values = { 10, 20, 30 };
            uint[] uintParameterIds = { 4, 5, 6 };

            // Act
            mock.Object.SetParameters(parameterIds, values);
            object[] paramValue = (object[])mock.Object.GetParameters(uintParameterIds);

            // Assert
            Assert.IsNull(paramValue.GetValue(0));
            Assert.IsNull(paramValue.GetValue(1));
            Assert.IsNull(paramValue.GetValue(2));
        }

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetParametersAndSetParametersWithTimestamp()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            uint[] uintParameterIds = { 1000, 1001, 1002 };
            int[] parameterIdsArray = { 1000, 1001, 1002 };
            object[] valuesArray = { 11, "16", 0x0B };
            DateTime[] timestampsArray = { new DateTime(2022, 8, 14), new DateTime(2022, 8, 15), new DateTime(2022, 8, 16) };

            // Act
            var outputSet = (int[])mock.Object.SetParameters(parameterIdsArray, valuesArray, timestampsArray);
            object[] paramValue = (object[])mock.Object.GetParameters(uintParameterIds);

            // Assert
            Assert.AreEqual(0, outputSet[0]);
            Assert.AreEqual(0, outputSet[1]);
            Assert.AreEqual(0, outputSet[2]);
            Assert.AreEqual(11, paramValue.GetValue(0));
            Assert.AreEqual("16", paramValue.GetValue(1));
            Assert.AreEqual(0x0B, paramValue.GetValue(2));
        }

        [TestMethod]
        public void SLProtocolMockTest_DifferentInputLengths_GetParametersAndSetParametersWithTimestamp()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIds = { 4, 5, 6 };
            object[] values = { 40, 50 };
            uint[] uintParameterIds = { 4, 5, 6 };
            DateTime[] timestamps = { new DateTime(2022, 7, 14), new DateTime(2022, 7, 15), new DateTime(2022, 7, 16) };

            // Act
            var outputSet = mock.Object.SetParameters(parameterIds, values, timestamps);
            object[] paramValue = (object[])mock.Object.GetParameters(uintParameterIds);

            // Assert
            Assert.AreEqual(Constants.HRESULT_FAIL_DIFFLEN, outputSet);
            Assert.IsNull(paramValue.GetValue(0));
            Assert.IsNull(paramValue.GetValue(1));
            Assert.IsNull(paramValue.GetValue(2));
        }

        [TestMethod]
        public void SLProtocolMockTest_InvalidGetInput_ThrowsArgumentException()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIds = { 1, 2, 3 };
            object[] values = { 10, 20, 30 };
            int[] intParameterIds = { 1, 2, 3 };

            // Act
            mock.Object.SetParameters(parameterIds, values);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => mock.Object.GetParameters(intParameterIds));
        }

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetParameterByNameNumeric()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value = mock.Object.GetParameterByName("NumericParameter");

            // Assert
            Assert.AreEqual(10.0, value);
        }

        [TestMethod]
        public void SLProtocolMockTest_ValidInput_GetParameterByNameString()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var value = mock.Object.GetParameterByName("StringParameterFixed");

            // Assert
            Assert.AreEqual("parameterValue", value);
        }

        [TestMethod]
        public void SetParameterByNameTest_ValidInput_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var setOutput = mock.Object.SetParameterByName("NumericParameter", 111);
            var getOutput = mock.Object.GetParameterByName("NumericParameter");

            // Assert
            Assert.AreEqual(0, setOutput);
            Assert.AreEqual(111, getOutput);
        }

        [TestMethod]
        public void SetParameterByNameTest_WrongParameterName_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            var setOutput = mock.Object.SetParameterByName("NumericParameter2", 111);
            var getOutput = mock.Object.GetParameterByName("NumericParameter");

            // Assert
            Assert.AreEqual(0, setOutput);
            Assert.AreEqual(10, getOutput);
        }

        [TestMethod]
        public void SetParametersByNameTest_ValidInput_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            string[] names = { "NumericParameter", "StringParameter" };
            object[] values = { 222, "333" };

            // Act
            var setOutput = (uint[])mock.Object.SetParametersByName(names, values);
            var getOutput1 = mock.Object.GetParameterByName("NumericParameter");
            var getOutput2 = mock.Object.GetParameterByName("StringParameter");

            // Assert
            Assert.AreEqual(0, (int)setOutput[0]);
            Assert.AreEqual(0, (int)setOutput[1]);
            Assert.AreEqual(222, getOutput1);
            Assert.AreEqual("333", getOutput2);
        }

        [TestMethod]
        public void SetParametersByNameTest_FixedValueParameter_IsEqual()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            string[] names = { "NumericParameterFixed", "StringParameter" };
            object[] values = { 222, "333" };

            // Act
            var setOutput = (uint[])mock.Object.SetParametersByName(names, values);
            var getOutput1 = mock.Object.GetParameterByName("NumericParameterFixed");
            var getOutput2 = mock.Object.GetParameterByName("StringParameter");

            // Assert
            Assert.AreEqual(0, (int)setOutput[0]);
            Assert.AreEqual(0, (int)setOutput[1]);
            Assert.AreEqual(222, getOutput1);
            Assert.AreEqual("333", getOutput2);
        }
    }
}