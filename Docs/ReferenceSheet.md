### AddRow
```csharp
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);
    
object[] row1 = new object[] { "one", "2ndone", 3, 4, 5 };
object[] row2 = new object[] { "two", "2ndtwo", 6, 7, 8 };
    
mock.Object.AddRow(900, row1);
mock.Object.AddRow(900, row2);
    
mock.Assert().Table(900).Row("one").Should().BeEquivalentTo(row1);
```

### AddRowReturnKey
```csharp
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);
    
object[] row1 = new object[] { "one", "2ndone", 3, 4, 5 };
object[] row2 = new object[] { "two", "2ndtwo", 6, 7, 8 };
    
//Act
mock.Object.AddRowReturnKey(900, row1);
mock.Object.AddRowReturnKey(900, row2);
    
//Assert
mock.Assert().Table(900).Row("one").Should().BeEquivalentTo(row1);
mock.Assert().Table(900).Row(0).Should().BeEquivalentTo(row1);
mock.Assert().Table(900).Row("two").Should().BeEquivalentTo(row2);
mock.Assert().Table(900).Row(1).Should().BeEquivalentTo(row2);
```

### DeleteRow
```csharp
//Arrange
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);
    
object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", 1, 2, 3 };
object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", 10, 11, 12 };
object[] row3 = new object[] { "skyline3", "2ndColumnSkyline3", 13, 14, 15 };
object[] row4 = new object[] { "skyline4", "2ndColumnSkyline4", 16, 17, 18 };
object[] row5 = new object[] { "skyline5", "2ndColumnSkyline5", 19, 20, 21 };
    
//Act
mock.Object.AddRowReturnKey(900, row1);
mock.Object.AddRowReturnKey(900, row2);
mock.Object.AddRowReturnKey(900, row3);
mock.Object.AddRowReturnKey(900, row4);
mock.Object.AddRowReturnKey(900, row5);
mock.Object.AddRowReturnKey(900, row5);
mock.Object.DeleteRow(900, "skyline2");
    
//Assert
mock.Assert().Table(900).Row(0).Should().BeEquivalentTo(row1);
mock.Assert().Table(900).Row(1).Should().BeEquivalentTo(row5);
mock.Assert().Table(900).Row(2).Should().BeEquivalentTo(row3);
mock.Assert().Table(900).Row(3).Should().BeEquivalentTo(row4);
```

### FillArrayNoDelete
```csharp
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);
    
object[] row1 = new object[] { "skyline1", "2ndColumnSkyline1", "1", "2", "3" };
object[] row2 = new object[] { "skyline2", "2ndColumnSkyline2", "4", "5", "6" };
    
object[] col1 = new object[] { "skyline3", "skyline4" };
object[] col2 = new object[] { "2ndSkyline3", "2ndSkyline4" };
object[] col3 = new object[] { "7", "10" };
object[] col4 = new object[] { "8", "11" };
object[] col5 = new object[] { "9", "12" };
    
List<object[]> listOfCols = new List<object[]> {
    col1,
    col2,
    col3,
    col4,
    col5
};
    
mock.Object.AddRow(900, row1);
mock.Object.AddRow(900, row2);
    
mock.Object.FillArrayNoDelete(900, listOfCols);
    
string[] expectedCol1 = { "skyline1", "skyline2", "skyline3", "skyline4" };
string[] expectedCol2 = { "2ndColumnSkyline1", "2ndColumnSkyline2", "2ndSkyline3", "2ndSkyline4" };
string[] expectedCol3 = { "1", "4", "7", "10" };
string[] expectedCol4 = { "2", "5", "8", "11" };
string[] expectedCol5 = { "3", "6", "9", "12" };
    
mock.Assert().Table(900).Column(901).Should().BeEquivalentTo(expectedCol1);
mock.Assert().Table(900).Column(902).Should().BeEquivalentTo(expectedCol2);
mock.Assert().Table(900).Column(903).Should().BeEquivalentTo(expectedCol3);
mock.Assert().Table(900).Column(904).Should().BeEquivalentTo(expectedCol4);
mock.Assert().Table(900).Column(905).Should().BeEquivalentTo(expectedCol5);
```




