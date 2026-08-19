# Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common

## About

The Unit Testing Framework was developed to simplify how Unit Tests are made when developing Connectors. The purpose is to allow developers to focus on actual tests without being tied to the code implementation of the method that's being tested.

This package provides a pre-arranged mock of `IDmsElement` (from the `Skyline.DataMiner.Core.DataMinerSystem.Common` NuGet package). It re-uses a protocol.xml file to determine which standalone parameters and tables an element of that protocol would contain, and keeps track of the values that are set on and retrieved from them.

```csharp
var elementMock = new IDmsElementMock("path/to/protocol.xml");

elementMock.Object.GetStandaloneParameter<string>(123).SetValue("new value");
var parameterValue = elementMock.Object.GetStandaloneParameter<string>(123).GetValue(); // "new value"

elementMock.Object.GetTable(1000).GetColumn<string>(1001).SetValue("PK", "cell value");
var cellValue = elementMock.Object.GetTable(1000).GetColumn<string>(1001).GetValue("PK"); // "cell value"
```

### Arranging and asserting data

The `IDmsElement` mock is passed to the code under test through `IDmsElementMock.Object`. To arrange (set up) the data before the test and to assert (verify) the data afterwards, you do not need to go through `.Object`: `IDmsElementMock` exposes `GetStandaloneParameter<T>` and `GetTable` directly. Both the direct methods and `.Object` are backed by the same store, so values written through one are visible through the other. Because the direct methods bypass `.Object`, arranging data through them is not recorded as an invocation on the mock, so it does not interfere with a later `IDmsElementMock.Verify(...)`.

```csharp
var elementMock = new IDmsElementMock("path/to/protocol.xml");

// Arrange: set up the data the code under test will read.
elementMock.GetStandaloneParameter<string>(123).SetValue("initial value");
elementMock.GetTable(1000).AddRow(new object[] { "PK", "description" });

// Act: run the code under test, passing in elementMock.Object.
SystemUnderTest.DoSomething(elementMock.Object);

// Assert: verify the data the code under test wrote.
Assert.AreEqual("expected value", elementMock.GetStandaloneParameter<string>(123).GetValue());
Assert.AreEqual("expected cell", elementMock.GetTable(1000).GetColumn<string>(1001).GetValue("PK"));
```

Only the four types supported by the DataMiner System interfaces (`int?`, `double?`, `DateTime?` and `string`) can be used as the generic argument of `GetStandaloneParameter<T>` and `GetColumn<T>`. Any other type results in a `NotSupportedException`.

Value monitors are supported as well: `StartValueMonitor`/`StopValueMonitor` on standalone parameters, tables and columns invoke the registered callback whenever the underlying value changes.

### About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exist. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as "protocols" or "drivers").

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

### About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
