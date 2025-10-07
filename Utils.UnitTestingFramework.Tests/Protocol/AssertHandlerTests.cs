namespace Skyline.DataMiner.Utils.UnitTestingFramework.Tests.Protocol
{
    using System.Collections.Generic;

    using FluentAssertions;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol;

    [TestClass]
    [DeploymentItem("TestFiles/Model/Data/protocol.xml")]
    public class AssertTests
    {
        private readonly string path = "protocol.xml";

        [TestMethod]
        public void ParameterAssertion_ValidInput_SetParameter()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            mock.Object.SetParameter(100, 20);

            // Assert
            mock.Assert().Parameter(100).Value.Should().Be(20);
        }

        [TestMethod]
        public void ParameterAssertion_ValidInput_SetParameterByName()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            mock.Object.SetParameterByName("NumericParameter", 20);

            // Assert
            mock.Assert().Parameter("NumericParameter").Value.Should().Be(20);
        }

        [TestMethod]
        public void ParameterAssertion_InvalidInput_SetParameter()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            // Act
            mock.Object.SetParameter(100, 20);

            // Assert
            mock.Assert().Parameter(10).Should().BeNull();
        }

        [TestMethod]
        public void ParameterAssertion_InvalidInput_SetParameters()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIds = { 4, 5, 6 };
            object[] values = { 40, 50, 60 };

            // Act
            mock.Object.SetParameters(parameterIds, values);

            // Assert
            mock.Assert().Parameter(7).Should().BeNull();
        }

        [TestMethod]
        public void ParameterAssertion_ValidInput_GetAndSetParameters()
        {
            // Arrange

            var mock = new SLProtocolMock(path);
            int[] parameterIds = { 1000, 1001, 1002 };
            object[] values = { 40, 50, 60 };
            uint[] uintParameterIds = { 1000, 1001, 1002 };

            object[] expected = { 40, 50, 60 };

            // Act
            mock.Object.SetParameters(parameterIds, values);
            object[] paramValue = (object[])mock.Object.GetParameters(uintParameterIds);

            // Assert
            paramValue.Should().Equal(expected);
            mock.Assert().Parameter(1000).Value.Should().BeEquivalentTo(40);
            mock.Assert().Parameter(1001).Value.Should().BeEquivalentTo(50);
            mock.Assert().Parameter(1002).Value.Should().BeEquivalentTo(60);
        }

        [TestMethod]
        public void TableAssertionAddRow_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndone", 3, 4, 5 };
            object[] row2 = new object[] { "two", "2ndtwo", 6, 7, 8 };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Assert
            mock.Assert().Table(900).Row("one").Should().Equal(row1);
            mock.Assert().Table(900).Row("two").Should().Equal(row2);
        }

        [TestMethod]
        public void TableAssertionAddRow_InexistentRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Assert
            mock.Assert().Table(900).Row(2).Should().BeNull();
        }

        [TestMethod]
        public void TableAssertionFillArray_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndone", 3, 4, 5 };
            object[] row2 = new object[] { "two", "2ndtwo", 6, 7, 8 };

            List<object[]> listOfRows = new List<object[]>
            {
                row1,
                row2,
            };

            // Act
            mock.Object.FillArray(900, listOfRows, Skyline.DataMiner.Scripting.NotifyProtocol.SaveOption.Partial);

            // Assert
            mock.Assert().Table(900).Row("one").Should().Equal(row1);
            mock.Assert().Table(900).Row("two").Should().Equal(row2);
        }

        [TestMethod]
        public void TableAssertionAddRowReturnKey_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "one", "2ndone", 3, 4, 5 };
            object[] row2 = new object[] { "two", "2ndtwo", 6, 7, 8 };

            // Act
            mock.Object.AddRowReturnKey(900, row1);
            mock.Object.AddRowReturnKey(900, row2);

            // Assert
            mock.Assert().Table(900).Row("one").Should().Equal(row1);
            mock.Assert().Table(900).Row(0).Should().Equal(row1);
            mock.Assert().Table(900).Row("two").Should().Equal(row2);
            mock.Assert().Table(900).Row(1).Should().Equal(row2);
        }

        [TestMethod]
        public void TableAssertionDeleteRowPK_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };

            // Act
            mock.Object.AddRowReturnKey(900, row1);
            mock.Object.AddRowReturnKey(900, row2);
            mock.Object.AddRowReturnKey(900, row3);
            mock.Object.AddRowReturnKey(900, row4);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.DeleteRow(900, "skyline2");

            // Assert
            mock.Assert().Table(900).Row(0).Should().Equal(row1);
            mock.Assert().Table(900).Row(1).Should().Equal(row5);
            mock.Assert().Table(900).Row(2).Should().Equal(row3);
            mock.Assert().Table(900).Row(3).Should().Equal(row4);
        }

        [TestMethod]
        public void TableAssertionDeleteRowIndex_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };

            // Act
            mock.Object.AddRowReturnKey(900, row1);
            mock.Object.AddRowReturnKey(900, row2);
            mock.Object.AddRowReturnKey(900, row3);
            mock.Object.AddRowReturnKey(900, row4);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.DeleteRow(900, 1);

            // Assert
            mock.Assert().Table(900).Row(0).Should().Equal(row1);
            mock.Assert().Table(900).Row(1).Should().Equal(row5);
            mock.Assert().Table(900).Row(2).Should().Equal(row3);
            mock.Assert().Table(900).Row(3).Should().Equal(row4);
        }

        [TestMethod]
        public void TableAssertionDeleteRowsPK_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
            string[] rowsToDelete = new string[] { "skyline1", "skyline2" };

            // Act
            mock.Object.AddRowReturnKey(900, row1);
            mock.Object.AddRowReturnKey(900, row2);
            mock.Object.AddRowReturnKey(900, row3);
            mock.Object.AddRowReturnKey(900, row4);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.DeleteRow(900, rowsToDelete);

            // Assert
            mock.Assert().Table(900).Row(0).Should().Equal(row5);
            mock.Assert().Table(900).Row(1).Should().Equal(row4);
            mock.Assert().Table(900).Row(2).Should().Equal(row3);
        }

        [TestMethod]
        public void TableAssertionClearAllKeys_EqualRow()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
            object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
            object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
            object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };

            // Act
            mock.Object.AddRowReturnKey(900, row1);
            mock.Object.AddRowReturnKey(900, row2);
            mock.Object.AddRowReturnKey(900, row3);
            mock.Object.AddRowReturnKey(900, row4);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.AddRowReturnKey(900, row5);
            mock.Object.ClearAllKeys(900);

            // Assert
            mock.Assert().Table(900).Row(0).Should().BeNull();
        }

        [TestMethod]
        public void TableAssertionAddRow_EqualColumn()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Assert
            mock.Assert().Table(900).Column(901).Should().Contain("skyline1").And.Contain("skyline2");
            mock.Assert().Table(900).Column(902).Should().Contain("2ndColumnSkyline1").And.Contain("2ndColumnSkyline2");
            mock.Assert().Table(900).Column(903).Should().Contain(1).And.Contain(10);
            mock.Assert().Table(900).Column(904).Should().Contain(2).And.Contain(11);
            mock.Assert().Table(900).Column(905).Should().Contain(3).And.Contain(12);
        }

        [TestMethod]
        public void TableAssertionAddRow_InexistentColumn()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            // Assert
            mock.Assert().Table(900).Column(906).Should().BeNull();
        }

        [TestMethod]
        public void TableAssertionFillArrayNoDelete_EqualColumns()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", "4", "5", "6" };

            object[] col1 = new object[] { "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { "7", "10" };
            object[] col4 = new object[] { "8", "11" };
            object[] col5 = new object[] { "9", "12" };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArrayNoDelete(900, listOfCols);

            // Assert
            string[] expectedCol1 = { "skyline1", "skyline2", "skyline3", "skyline4" };
            string[] expectedCol2 = { "2ndColumnSkyline1", "2ndColumnSkyline2", "2ndSkyline3", "2ndSkyline4" };
            string[] expectedCol3 = { "1", "4", "7", "10" };
            string[] expectedCol4 = { "2", "5", "8", "11" };
            string[] expectedCol5 = { "3", "6", "9", "12" };

            mock.Assert().Table(900).Column(901).Should().Equal(expectedCol1);
            mock.Assert().Table(900).Column(902).Should().Equal(expectedCol2);
            mock.Assert().Table(900).Column(903).Should().Equal(expectedCol3);
            mock.Assert().Table(900).Column(904).Should().Equal(expectedCol4);
            mock.Assert().Table(900).Column(905).Should().Equal(expectedCol5);
        }

        [TestMethod]
        public void TableAssertionFillArrayWithDeleteAfterAddRow_EqualColumns()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };
            object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", "10", "11", "12" };

            object[] col1 = new object[] { "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { "7", "10" };
            object[] col4 = new object[] { "8", "11" };
            object[] col5 = new object[] { "9", "12" };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.AddRow(900, row1);
            mock.Object.AddRow(900, row2);

            mock.Object.FillArray(900, listOfCols);

            // Assert
            object[] expectedColumn1 = new object[] { "skyline4", "skyline3" };
            object[] expectedColumn2 = new object[] { "2ndSkyline4", "2ndSkyline3" };
            object[] expectedColumn3 = new object[] { "10", "7" };
            object[] expectedColumn4 = new object[] { "11", "8" };
            object[] expectedColumn5 = new object[] { "12", "9" };
            mock.Assert().Table(900).Column(901).Should().Equal(expectedColumn1);
            mock.Assert().Table(900).Column(902).Should().Equal(expectedColumn2);
            mock.Assert().Table(900).Column(903).Should().Equal(expectedColumn3);
            mock.Assert().Table(900).Column(904).Should().Equal(expectedColumn4);
            mock.Assert().Table(900).Column(905).Should().Equal(expectedColumn5);
        }

        [TestMethod]
        public void TableAssertionFillArrayWithDelete_EqualColumns()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] col1 = new object[] { "skyline3", "skyline4" };
            object[] col2 = new object[] { "2ndSkyline3", "2ndSkyline4" };
            object[] col3 = new object[] { "7", "10" };
            object[] col4 = new object[] { "8", "11" };
            object[] col5 = new object[] { "9", "12" };

            List<object[]> listOfCols = new List<object[]>
            {
                col1,
                col2,
                col3,
                col4,
                col5,
            };

            // Act
            mock.Object.FillArray(900, listOfCols);

            // Assert
            mock.Assert().Table(900).Column(901).Should().Equal(col1);
            mock.Assert().Table(900).Column(902).Should().Equal(col2);
            mock.Assert().Table(900).Column(903).Should().Equal(col3);
            mock.Assert().Table(900).Column(904).Should().Equal(col4);
            mock.Assert().Table(900).Column(905).Should().Equal(col5);
        }

        [TestMethod]
        public void TableAssertionFillArrayWithColumn_EqualColumns()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] pk = new object[] { "skyline1", "skyline2", "skyline3", "skyline4", "skyline5" };
            object[] values = new object[] { "value1", "value2", "value3", "value4", "value5" };

            // Act
            mock.Object.FillArrayWithColumn(900, 902, pk, values);

            // Assert
            mock.Assert().Table(900).Column(901).Should().Contain("skyline1").And.Contain("skyline2").And.Contain("skyline3").And.Contain("skyline4").And.Contain("skyline5");
            mock.Assert().Table(900).Column(902).Should().Contain("value1").And.Contain("value2").And.Contain("value3").And.Contain("value4").And.Contain("value5");
        }

        [TestMethod]
        public void TableAssertionNotConsecutiveRows_GetCorrectRows()
        {
            // Arrange

            var mock = new SLProtocolMock(path);

            object[] pk = new object[] { "skyline1", "skyline2" };
            object[] values = new object[] { "3rdColValue1", "3rdColValue2" };

            // Act
            mock.Object.ClearAllKeys(900);

            mock.Object.FillArrayWithColumn(900, 903, pk, values);

            mock.Object.GetRow(900, "skyline1");
            mock.Object.GetRow(900, "skyline2");

            // Assert
            mock.Assert().Table(900).Column(901).Should().Contain("skyline1").And.Contain("skyline2");

            object[] colNull = { null, null };

            mock.Assert().Table(900).Column(902).Should().Equal(colNull);

            mock.Assert().Table(900).Column(903).Should().Contain("3rdColValue1").And.Contain("3rdColValue2");

            mock.Assert().Table(900).Column(904).Should().Equal(colNull);

            mock.Assert().Table(900).Column(905).Should().Equal(colNull);
        }
    }
}