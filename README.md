
# Protocol Unit Testing Framework

The Protocol Unit Testing Framework aims to simplify writing unit tests when developing a protocol.
This is achieved by allowing
1. Easier arranging of data needed for testing SLProtocol calls.
1. Enabling assertion on the result of SLProtocol calls instead of verifying if specific SLProtocol calls were made with specific arguments.

## SLProtocolMock Class

The framework provides the `SLProtocolMock` class, which extends the `Mock<SLProtocol>` class from the [Moq library](https://www.nuget.org/packages/Moq).

An instance of `SLProtocolMock` exposes an instance of `SLProtocol` through its `Object` property.
It will store any data set using SLProtocol calls such as `SetParameter`, `AddRow`, and others, in internal storage structures.
As a result, it is possible to

1. Arrange data by using SLProtocol calls
1. Assert on that stored data instead of verifying if specific SLProtocol calls were made with specific arguments.

> [!NOTE]
> Because `SLProtocolMock` only provides an instance of `SLProtocol`, the Unit Testing Framework currently does not support `SLProtocolExt`.

Initializing an instance of the `SLProtocolMock` class is as simple as calling its parameterless constructor.
This constructor will find the protocol.xml file, parse it and create internal storage for standalone parameters and tables.

Consider the following use case, where we want to unit test a `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` method.
As the method name indicates, it will parse the string and store the parsed values in a table.

Below is an example of how this unit test would be written **without** using the Unit Testing Framework:

```csharp
[TestMethod]
public void ResponseParserTest()
{
    // Arrange
    var expectedRows = new List<object[]>
    {
        new MediainstancesusagetableQActionRow
        {
            Mediainstancesusagetableid_2401 = "test",
            Mediainstancesusagetablemedianame_2402 = "Media Name",
            Mediainstancesusagetablemediatype_2403 = "Media Type",
            Mediainstancesusagetablemediasetname_2404 = "Set Name",
            Mediainstancesusagetableavailability_2405 = 100,
            Mediainstancesusagetableearliestusagelist_2406 = "Usage List",
            Mediainstancesusagetableearliestusagelistuid_2407 = "1234",
            Mediainstancesusagetableearliestusagetime_2408 = 0.0d
        }.ToObjectArray(),
    };

    string response = File.ReadAllText("response_content.xml");
    var protocolMock = new Mock<SLProtocol>();

    // Act
    ResponseParser.ParseAndStoreInTable(protocolMock.Object, response);
    
    // Assert
    protocolMock.Verify(p => p.FillArray(Parameter.Mediainstancesusagetable.tablePid, expectedRows, NotifyProtocol.SaveOption.Full));
}
```

By using this approach we can see multiple problems:

1. If in the future the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` uses a different way if populating the table instead of a `FillArray` call, the unit test will fail.
2. The format of the arguments used in the `FillArray` of the `Verify` need to be exactly the same as the format of the arguments of the `FillArray` call inside the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)`.
3. If the developer wishes to just check the row key or the table row length, the `Verify` will require the developer to build exactly the same arguments, forcing the developer to spend way more time than needed.

Basically, the unit test is tightly coupled to the implementation of the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` and the assertion is not very flexible.

By using the `SLProtocolMock` class from the Unit Testing Framework, we can achieve the same goal but just simply doing the following:

```csharp
[TestMethod]
public void ResponseParserTest()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    string responseContent = File.ReadAllText("response_content.xml");

    // Act
    ResponseParser.ParseAndStoreInTable(protocolMock.Object, response);
    
    // Assert
    protocolMock.Assert()
        .Table(Parameter.Mediainstancesusagetable.tablePid)
        .Column(Parameter.Mediainstancesusagetable.Pid.mediainstancesusagetableid_2401)
        .Should()
        .Contain("test");
}
```

Note the difference in the Assert step. Instead of verifying that a specific SLProtocol call was made with specific arguments, we're now asserting on the content of the table.

Compared to the previous approach, this new approach has the following advantages:
1. The unit test is not tightly coupled to the implementation of the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)`. If the implementation changes but the end result is the same, the unit test will still pass.
1. Assertion is more flexible, it can check only the parts of the data that are relevant for the unit test.

## How To Use The Framework

### Arranging Data

The `SLProtocolMock` class exposes an instance of `SLProtocol` through its `Object` property.
This means that any method available in the `SLProtocol` interface can be called using `SLProtocolMock.Object`.

Below is an example of how to arrange data using the `SLProtocolMock` class.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    protocolMock.Object.SetParameter(1000, 30);

    object[] row = new object[] { "one", "two", "three", "four", "five" };
    protocolMock.Object.AddRow(2000, row);

    var eventRow = new EventstableQActionRow
	{
        // Only values for the columns that are relevant for the test need to be set
		Eventstableid_2101 = "15007.123",
		Eventstableplaylistid_2103 = "15007",
		Eventstablereconcilekey_2110 = "1835695279327",
	};

	protocolMock.Object.AddRow(3000, eventRow.ToObjectArray());

    // Act
    ... some code that uses protocolMock.Object ...
    
    // Assert
    ... some assertions ...
}
```

### Asserting Data

There are two ways of asserting on the data stored in the internal storage structures of the `SLProtocolMock` instance.

#### SLProtocol Interface

As SLProtocolMock exposes exposes an instance of `SLProtocol` through its `Object` property, it is possible to retrieve the data using SLProtocol calls and then assert on that data.
Below is an example of how to assert data using the `SLProtocol` methods.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    // Act
    ... some code that uses protocolMock.Object ...
    
    // Assert
    Assert.AreEqual(30, protocolMock.Object.GetParameter(1000));

    Assert.AreEqual("expected cell value", protocolMock.Object.GetRow(2000, "primary key")[0]);
}
```

#### IAsserter Interface

Alternatively, the `SLProtocolMock` class exposes an `Assert()` method that returns an instance of the `IAsserter` interface. This class provides more flexible methods to assert on the stored data.

Useful features of this interface are

1. Getting all rows in a table using `SLProtocolMock.Assert().Table([tableId]).AllRows()`
1. Getting a specific row in a table as `QActionTableRow` by using `SLProtocolMock.Assert().Table([tableId]).Row<[QActionTableRowType]>([primary key or index])`

Below is an example of how to assert data using the `AssertHandler` class.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    // Act
    ... some code that uses protocolMock.Object ...
    
    // Assert
    Assert.AreEqual(30, protocolMock.Assert().Parameter(1000).Value);

    Assert.AreEqual("expected cell value", protocolMock.Assert().Table(2000).Row("primary key")[0]);
    Assert.AreEqual("expected cell value", protocolMock.Assert().Table(2000).Row<QActionTableRow>("primary key").TableId_2001);

    Assert.IsTrue(protocolMock.Assert().Table(2000).AllRows().Any(r => r[0].ToString() == "expected cell value"));
}
```

The `IAsserter` interface has been designed with [Fluent Assertions](https://www.nuget.org/packages/FluentAssertions) in mind, making it easy to write readable assertions.

Below is an example of how to assert data using the `IAsserter` interface combined with Fluent Assertions.
```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    // Act
    ... some code that uses protocolMock.Object ...
    
    // Assert
    var expectedLiveStreamOrderRow = new LivestreamordersQActionRow
    {
        // Only values for the columns that are relevant for the test need to be set
        Livestreamordersareenaid_1001 = "1-62831376",
        Livestreamordersdescriptionmainfin_1002 = "Robot Framework -testi: luodaan 4h urheilulive, joka alkaa nyt. Tällä matkitaan hirviliveä. Liveen liitetään 2 chattia: fin ja swe. Live on liitetty myös sarjaan. Livelle on laitettu myös suomenkielinen herovideo. / 16.6.-22",
        Livestreamordersdescriptionmainswe_1003 = "Robot Framework -test på svenska: Robot Framework -testi: luodaan 4h urheilulive, joka alkaa nyt. / 16.6.-22",
        Livestreamordersdescriptionshortfin_1004 = "-1",
    };

    protocolMock.Assert().Table(1000).RowCount.Should().Be(2);

    protocolMock.Assert().Table(1000).Row<LivestreamordersQActionRow>("1-62831376").Should().BeEquivalentTo(expectedLiveStreamOrderRow, options => options
    .ExcludeMissingMembers() // Exclude properties that are not set in expectedLiveStreamOrderRow
    .Excluding(row => row.Livestreamorderslastupdatedtimestamp) // Exclude non-deterministic values (e.g.: timestamps set to DateTime.Now)
    .Excluding(row => row.Livestreamorderslastupdatedtimestamp_1091) // Exclude non-deterministic values
    .Excluding(row => row.Columns)); // Exclude irrelevant properties

    protocolMock.Assert().Table(1000).AllRows().Should().ContainKeys("1-62831376_1", "1-62831376_2");
}
```

> [!WARNING]
> As of version 8.0.0, using the Fluent Assertions library requires a license for commercial use. Use version 7.2.0 or earlier for free commercial use.
