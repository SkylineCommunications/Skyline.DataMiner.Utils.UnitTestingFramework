# AssertHandler
(namespace **UnitTestingFramework.Protocol**)

The `AssertHandler` class is called in the `Assert()` method in the `SLProtocolMock` class and
accesses the `ProtocolCache` to retrieve the specified values.
The values to retrieve can belong to Parameters or Tables and should be accessed in the following way, respectively:

#### Parameters
```csharp
mock.Assert().Parameter(parameterId).Value.Should().Be(expectedValue);
```

#### Tables
```csharp
mock.Assert().Table(tableId).Row(rowIndex).Should().Equal(expectedRow);
```
or

```csharp
mock.Assert().Table(tableId).Row(primaryKey).Should().Equal(expectedRow);
```

or

```csharp
mock.Assert().Table(tableId).Row(rowIndex).Should().Contain(expectedValue).And.Contain(expectedValue);
```

or

```csharp
mock.Assert().Table(tableId).Column(columnId).Should().Equal(expectedColumn);
```

or

```csharp
mock.Assert().Table(tableId).Column(columnId).Should().Contain(expectedValue).And.Contain(expectedValue);
```

## Use Cases 
```csharp
//Arrange
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);

//Act
mock.Object.SetParameter(1000, 20);

//Assert
mock.Assert().Parameter(1000).Value.Should().Be(20);
```

In the example above, the `Parameter(1000)` returns a `ParameterModel` from the cache's `ParametersToValues` whose
value is accessed with `Value`. [Fluent Assertions](https://fluentassertions.com/) are then used to check if the value is the expected one. 


```csharp
//Arrange
var protocolModel = new ProtocolModel();
var mock = new SLProtocolMock(protocolModel);

//Act
mock.Object.SetParameter(1000, 20);

//Assert
Action act = () => mock.Assert().Parameter(1005);
act.Should().Throw<ArgumentException>();
```
In this second example, an exception is thrown, because it doesn't exist a Parameter with ID 1005, in the protocol.

In the example below, after adding two rows to the Table, `mock.Object.AddRow(900, row1)` and `mock.Object.AddRow(900, row1)`,
we're targeting our assert action to the Table with ID 900 by fetching the rows with Primary Keys "one1" and "two1". 
Using [Fluent Assertions](https://fluentassertions.com/), we're able to validate whether the returned
rows are as expected or not.

```csharp
//Arrange
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);

var row1 = new object[] { "one1", "one2", "one3", "one4", "one5" };
var row2 = new object[] { "two1", "two2", "two3, "two4", "two5" };

//Act
mock.Object.AddRow(900, row1);
mock.Object.AddRow(900, row2);

//Assert
mock.Assert().Table(900).Row("one1").Should().Equal(row1);
mock.Assert().Table(900).Row("two1").Should().Equal(row2);
```

Instead of checking the Table's rows, we could have checked the Table's columns:

```csharp
//Arrange
var protocolModel = new ProtocolModelExt(path);
var mock = new SLProtocolMock(protocolModel);

var row1 = new object[] { "one1", "one2", "one3", "one4", "one5" };
var row2 = new object[] { "two1", "two2", "two3", "two4", "two5" };

//Act
mock.Object.AddRow(900, row1);
mock.Object.AddRow(900, row2);

//Assert
var expectedColumn1 = new object[] { "one1", "two1" };
var expectedColumn2 = new object[] { "one2", "two2" };
var expectedColumn3 = new object[] { "one3", "two3" };
var expectedColumn4 = new object[] { "one4", "two4" };
var expectedColumn5 = new object[] { "one5", "two5" };

mock.Assert().Table(900).Column(901).Should().Equal(expectedColumn1);
mock.Assert().Table(900).Column(902).Should().Equal(expectedColumn2);
mock.Assert().Table(900).Column(903).Should().Equal(expectedColumn3);
mock.Assert().Table(900).Column(904).Should().Equal(expectedColumn4);
mock.Assert().Table(900).Column(905).Should().Equal(expectedColumn5);
```
