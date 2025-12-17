
# Protocol Unit Testing Framework

The Protocol Unit Testing Framework aims to simplify writing unit tests when developing a protocol and make those tests more future-proof, more resilient.
It allows us to
1. Arrange the data for testing code involving SLProtocol calls more easily and faster.
1. Assert on the result of SLProtocol calls instead of verifying if specific SLProtocol calls were made with specific arguments. In other words, test the output, not the implementation.

The framework achieves this by providing a mock implementation of the `SLProtocol` and `SLProtocolExt` interface, which stores data in internal storage structures.

## Mocking SLProtocol

The framework provides the `SLProtocolMock` class, which extends the `Mock<T>` class from the [Moq library](https://www.nuget.org/packages/Moq).

An instance of `SLProtocolMock` exposes an instance of `SLProtocol` through its `Object` property.
It will store any data set using SLProtocol calls such as `SetParameter`, `AddRow`, and others, in internal storage structures.

Initializing an instance of the `SLProtocolMock` class is as simple as calling its parameterless constructor.
This constructor will find the protocol.xml file, parse it and create internal storage for standalone parameters and tables.

Consider the following use case, where we want to unit test a `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)` method.
As the method name indicates, it will parse the string and store the parsed values in a table.

By using the `SLProtocolMock` class from the Unit Testing Framework, the test method can look as simple as:

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

Note the clean Assert step. Instead of verifying that a specific SLProtocol call was made with specific arguments, we're asserting on the content of the table.

This approach has the following advantages:
1. The unit test is not tightly coupled to the implementation of the method `ResponseParser.ParseAndStoreInTable(SLProtocol protocol, string response)`. If the implementation changes but the end result is the same, the unit test will still pass.
1. Assertion is more flexible, it can check only the parts of the data that are relevant for the unit test.

## Mocking SLProtocolExt

The framework provides the `SLProtocolMock<T>` class, which extends the `Mock<T>` class from the [Moq library](https://www.nuget.org/packages/Moq) and where `T` has the constraint that it needs to implement the `SLProtocol` interface.

This class allows a user of the framework to create a mock of the `SLProtocolExt` interface by creating an instance of `SLProtocolMock<ConcreteSLProtocolExt>`,
which exposes an instance of `SLProtocolExt` through its `Object` property.

Note that `ConcreteSLProtocolExt` is part of the DIS-generated QAction_Helper project in your protocol solution.

Similar to `SLProtocolMock`, it will store any data set in internal storage structures.

Initializing an instance of the `SLProtocolMock<ConcreteSLProtocolExt>` class is as simple as calling its parameterless constructor.
This constructor will find the protocol.xml file, parse it and create internal storage for standalone parameters and tables.

Consider the following use case, where we want to unit test a `ResponseParser.ParseAndStoreInTable(SLProtocolExt protocolExt, string response)` method.
As the method name indicates, it will parse the string and store the parsed values in a table.

By using the `SLProtocolMock<T>` class from the Unit Testing Framework, the test method can look as simple as:

```csharp
[TestMethod]
public void ResponseParserTest()
{
    // Arrange
    var protocolExtMock = new SLProtocolMock<ConcreteSLProtocolExt>();    

    string responseContent = File.ReadAllText("response_content.xml");

    // Act
    ResponseParser.ParseAndStoreInTable(protocolExtMock.Object, response);
    
    // Assert
    protocolExtMock.Assert()
        .Table(1000)
        .Column(1001)
        .Should()
        .Contain("PK1");
}
```

## How To Use The Framework

### Arranging Data

The `SLProtocolMock` class exposes an instance of `SLProtocol` through its `Object` property.
This means that methods available in the `SLProtocol` interface can all be called using `SLProtocolMock.Object`.

Below is an example of how to arrange data using the `SLProtocolMock` class.

A similar approach can be used for `SLProtocolMock<ConcreteSLProtocolExt>`.

```csharp
[TestMethod]
public void TestMethod()
{
    // Arrange
    var protocolMock = new SLProtocolMock();    

    protocolMock.Object.SetParameter(150, 30);

    var firstRow = new object[] { "PK1", "First Stream Name", "10.12.80.124", "8080" };
    protocolMock.Object.AddRow(1000, firstRow);

    var secondRow = new StreamsQActionRow
	{
        // Only values for the columns that are relevant for the test need to be set
		Streamsid_1001 = "PK2",
		Streamsname_1002 = "Second Stream Name",
	};

	protocolMock.Object.AddRow(1000, secondRow.ToObjectArray());

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

A similar approach can be used for `SLProtocolMock<ConcreteSLProtocolExt>`.

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
