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

The `IDmsElement` mock is passed to the code under test through `IDmsElementMock.Object`. Both arranging (setting up) the data before the test and asserting (verifying) the data afterwards happen through `.Object`, backed by the same store the code under test reads from.

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
