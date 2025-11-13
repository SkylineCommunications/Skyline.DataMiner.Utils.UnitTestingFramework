
# Protocol Unit Testing Framework

The Protocol Unit Testing Framework aims to simplify writing unit tests when developing a protocol and make those tests more future-proof, more resilient.
It allows us to
1. Arrange the data for testing code involving SLProtocol calls more easily and faster.
1. Assert on the result of SLProtocol calls instead of verifying if specific SLProtocol calls were made with specific arguments. In other words, test the output, not the implementation.

## SLProtocolMock Class

The framework provides the `SLProtocolMock` class, which extends the `Mock<SLProtocol>` class from the [Moq library](https://www.nuget.org/packages/Moq).

An instance of `SLProtocolMock` exposes an instance of `SLProtocol` through its `Object` property.
It will store any data set using SLProtocol calls such as `SetParameter`, `AddRow`, and others, in internal storage structures.
As a result, it is possible to

1. Arrange data by using SLProtocol calls
1. Assert on that stored data instead of verifying if specific SLProtocol calls were made with specific arguments. Again, testing the output, not the implementation.

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
    var protocolMock = new Mock<SLProtocol>();
    string response = File.ReadAllText("response_content.xml");

    // Act
    ResponseParser.ParseAndStoreInTable(protocolMock.Object, response);
    
    // Assert
    var expectedRows = new List<object[]>
    {
        new StreamsQActionRow
        {
            Streamsid_1001 = "PK1",
            Streamsname_1002 = "Stream Name",
            Streamsaddress_1003 = "10.12.80.124",
            Streamsport_1004 = "8080",
        }.ToObjectArray(),
    };

    protocolMock.Verify(p => p.FillArray(1000, expectedRows, NotifyProtocol.SaveOption.Full));
}
```

By using this approach we can see multiple problems:

1. If in the future the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` uses a different way to populate the table instead of a `FillArray` call, the unit test will be broken.
2. The format of the arguments used in the `FillArray` of the `Verify` needs to be exactly the same as the format of the arguments of the `FillArray` call inside the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)`.
3. If the developer wishes to just check the row key or the table row length, the `Verify` will require the developer to build exactly the same arguments, forcing the developer to spend way more time than needed.

Basically, the unit test is tightly coupled to the implementation of the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` and the assertion is not very flexible.

By using the `SLProtocolMock` class from the Unit Testing Framework, we can achieve the same goal (and more) simply by doing the following:

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
        .Table(1000)
        .Column(1001)
        .Should()
        .Contain("PK1");
}
```

Note the difference in the Assert step. Instead of verifying that a specific SLProtocol call was made with specific arguments, we're now asserting on the content of the table.

Compared to the previous approach, this new approach has the following advantages:
1. The unit test is not tightly coupled to the implementation of the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)`. If the implementation changes but the end result is the same, the unit test will still pass.
1. Assertion is more flexible, it can check only the parts of the data that are relevant for the unit test.

## How To Use The Framework

### Arranging Data

The `SLProtocolMock` class exposes an instance of `SLProtocol` through its `Object` property.
This means that methods available in the `SLProtocol` interface can all be called using `SLProtocolMock.Object`.

Below is an example of how to arrange data using the `SLProtocolMock` class.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    protocolMock.Object.SetParameter(150, 30);

    var firstRow = new object[] { "PK1", "First Stream Name", "10.12.80.124", "8080" };
    protocolMock.Object.AddRow(1000, row);

    var secondRow = new StreamsQActionRow
	{
        // Only values for the columns that are relevant for the test need to be set
		Streamsid_1001 = "PK2",
		Streamsname_1002 = "Second Stream Name",
	};

	protocolMock.Object.AddRow(1000, eventRow.ToObjectArray());

    // Act
    ... some code that uses protocolMock.Object ...
    
    // Assert
    ... some assertions ...
}
```

### Mocking More SLProtocol Calls

Not all SLProtocol methods are implemented in the `SLProtocolMock` class. For example, some less common variations of inputs for `SLProtocol.NotifyProtocol` will throw an exception indicating that they are not supported.
However, it is possible to mock additional SLProtocol calls using the Moq library. This will allow you to extend the behavior of the `SLProtocolMock` instance as needed.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    // Mocking an variation of NotifyProtocol that is not supported out of the box by the framework
    protocolMock.Setup(x => x.NotifyProtocol([notify type integer], It.IsAny<object>(), It.IsAny<object>())).Returns(...);

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
    Assert.AreEqual(30, protocolMock.Object.GetParameter(150));

    Assert.AreEqual("Stream Name", protocolMock.Object.GetRow(1000, "PK1")[1]);
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
    Assert.AreEqual(30, protocolMock.Assert().Parameter(150).Value);

    Assert.IsTrue(protocolMock.Assert().Table(1000).AllRows().ContainsKey("PK1");

    Assert.AreEqual("Stream Name", protocolMock.Assert().Table(1000).Row("PK1")[1]);

    Assert.AreEqual("10.12.80.124", protocolMock.Assert().Table(2000).Row<StreamsQActionRow>("PK1").Streamsaddress_1003);
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
    var expectedStreamRow = new StreamsQActionRow
    {
        // Only values for the columns that are relevant for the test need to be set
        Streamsid_1001 = "PK1",
        Streamsname_1002 = "Stream Name",
    };

    protocolMock.Assert().Table(1000).RowCount.Should().Be(1);

    protocolMock.Assert().Table(1000).AllRows().Should().ContainKeys("PK1");

    protocolMock.Assert()
        .Table(1000).Row<StreamsQActionRow>("PK1").Should().BeEquivalentTo(expectedStreamRow, options => options
            .ExcludeMissingMembers()         // Exclude properties that are not set in expectedStreamRow
            .Excluding(row => row.Columns)); // Exclude irrelevant properties
}
```

> [!WARNING]
> As of version 8.0.0, using the Fluent Assertions library requires a license for commercial use. Use version 7.2.0 or earlier for free commercial use.
