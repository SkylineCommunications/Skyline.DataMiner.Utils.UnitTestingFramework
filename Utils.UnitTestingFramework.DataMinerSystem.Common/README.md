# Skyline.DataMiner.Utils.UnitTestingFramework.DataMinerSystem.Common

## About

The Unit Testing Framework simplifies unit testing code that uses the DataMiner System interfaces from the `Skyline.DataMiner.Core.DataMinerSystem.Common` package.

It provides in-memory mocks for a DataMiner System, agents, elements, services, views, protocols, standalone parameters, tables, an SLNet connection, and DataMiner Object Models (DOM). Test code can arrange a realistic DataMiner System and pass the exposed `.Object` instances to the code under test.

The framework stores the resulting state instead of requiring tests to verify individual Moq calls. This allows tests to assert the outcome of an operation and keeps them less dependent on its internal implementation.

## Key capabilities

- Build an entire DataMiner System through one fluent `DmsBuilder`, including protocols, DMAs, elements, views, initial parameter and table values, and DOM objects.
- Define protocols either from an existing `protocol.xml` file or entirely in code through `IDmsProtocolMockBuilder`.
- Reuse a protocol's parameter and table definitions across multiple elements while keeping the runtime values of every element independent.
- Interact with the mocks through the real `IDms`, `IDmsElement`, `IDmsStandaloneParameter<T>`, `IDmsTable`, and `IConnection` interfaces.
- Generate `ParameterChangeEventMessage` and `ParameterTableUpdateEventMessage` notifications when standalone parameters, table rows, or table cells change.
- Apply registered `IConnection` subscription filters so listeners only receive changes for the requested elements.
- Register handlers for custom `DMSMessage` request types without replacing the built-in DOM message handling.
- Use the real `DomHelper` API against the in-memory DMS connection.
- Keep DOM data isolated per module and support CRUD operations for DOM instances, DOM definitions, section definitions, and behavior definitions.
- Publish `DomInstancesChangedEventMessage` notifications for DOM instance create, update, and delete operations.

### When each capability is useful

| Capability | Use it when | Main benefit |
| --- | --- | --- |
| Protocol XML | The test should represent an existing complete protocol | Parameter and table definitions are obtained from the real protocol model |
| Programmatic protocol | The test only needs a small, focused protocol | No XML fixture is required and the test declares only the relevant definitions |
| Multiple protocol versions | Different elements must represent different versions of the same protocol | Each element resolves the definitions belonging to its selected name and version |
| `DmsBuilder` | A test needs multiple related DMS objects | The complete DMS, DMA, element, view, protocol, connection, and DOM graph is created consistently |
| Connection subscriptions | The code under test reacts to DataMiner events | Only changes matching the registered filters reach the listener |
| Custom message handlers | The client sends application-specific SLNet requests | A test can define deterministic responses without modifying the framework |
| DOM support | The code under test uses `DomHelper` | The normal DOM API can run against an isolated in-memory module store |

## Requirements and dependencies

The package targets .NET Framework 4.8 and uses the DataMiner System Common interfaces.

DOM support itself is part of this package and works with the DOM types available through the DataMiner dependencies. The examples below use `DomDefinitionBuilder`, `DomInstanceBuilder`, `SectionDefinitionBuilder`, and `DomBehaviorDefinitionBuilder` from `Skyline.DataMiner.Utils.DOM`. Add a compatible version of that package to the test project when those optional builders are used.

## Getting started

### Building a DataMiner System in code

The following example defines a protocol without a `protocol.xml`, creates a view and a DMA, and places an element with an initial parameter value under that view.

```csharp
var dmsMock = new DmsBuilder()
    .WithProtocol("ExampleProtocol", protocol => protocol
        .AddParameterDefinition(
            new StandaloneParameterDefinition("Status", typeof(string), 100)))
    .WithView(500, "Main view")
    .WithDma(1, dma => dma
        .WithElement(
            id: 10,
            name: "Example element",
            protocolName: "ExampleProtocol",
            configure: element => element
                .UnderView(500)
                .WithParameter(100, "Ready")))
    .Build();

IDms dms = dmsMock.Object;
IDmsElement element = dms.GetElement("Example element");

Assert.AreEqual("Ready", element.GetStandaloneParameter<string>(100).GetValue());
Assert.AreSame(element, dms.GetView(500).Elements.Single());
```

`DmsBuilder.Build()` creates one connected object graph. Agents, elements, protocols, views, the connection, and DOM therefore belong to the same simulated DataMiner System.

### Building objects directly

`DmsBuilder` is the recommended option when a test needs a complete related object graph. Individual mocks can also be created directly for smaller arrangements or features that are not exposed by the fluent builder, such as services.

```csharp
var dmsMock = new IDmsMock();
var dmaMock = dmsMock.CreateAgent(agentId: 1, name: "Agent 1");
var viewMock = dmsMock.CreateView(viewId: 500, name: "Main view");
var serviceMock = dmaMock.CreateService(serviceId: 10, name: "Example service");

serviceMock.AddView(viewId: 500);
```

Objects created this way still use the same internal DMS cache and are available through `dmsMock.Object`.

### Building a DataMiner System from protocol XML

An existing protocol XML file can be used instead of declaring the parameter and table definitions manually.

```csharp
var dmsMock = new DmsBuilder()
    .WithProtocol("TestFiles/protocol.xml")
    .WithDma(1, dma => dma
        .WithElement(
            id: 10,
            name: "Example element",
            protocolName: "NameFromProtocolXml"))
    .Build();
```

The XML file must be available in the test output directory. For MSTest projects, this can for example be arranged with `DeploymentItem`.

### Selecting protocol versions

Both XML and programmatic protocols have a name and referenced version:

- For XML protocols, the name and version are read from `protocol.xml`.
- For programmatic protocols, the caller supplies the name and can supply the version.
- If a programmatic version is omitted, `IDmsProtocolMock.DefaultVersion` (`1.0.0.1`) is used.
- An element selects its protocol by both name and version.

This makes it possible to use different versions of the same protocol in one simulated DMS.

```csharp
var dmsMock = new DmsBuilder()
    .WithProtocol(
        "VersionedProtocol",
        protocol => protocol.AddParameterDefinition(
            new StandaloneParameterDefinition("Old status", typeof(string), 100)),
        version: "1.0.0.1")
    .WithProtocol(
        "VersionedProtocol",
        protocol => protocol.AddParameterDefinition(
            new StandaloneParameterDefinition("New status", typeof(string), 200)),
        version: "2.0.0.0")
    .WithDma(1, dma => dma
        .WithElement(10, "Old element", "VersionedProtocol", "1.0.0.1")
        .WithElement(20, "New element", "VersionedProtocol", "2.0.0.0"))
    .Build();
```

The old element exposes parameter `100`, while the new element exposes parameter `200`. The protocol definitions are not mixed even though both protocols have the same name.

### Defining a table without protocol XML

Programmatic protocol definitions are useful for focused tests that only need a small part of a protocol.

```csharp
var keyColumn = new ColumnDefinition("Key", typeof(string), pid: 201, idx: 0, allowNull: false);
var valueColumn = new ColumnDefinition("Value", typeof(string), pid: 202, idx: 1);
var tableDefinition = new TableDefinition(new[] { keyColumn, valueColumn }, keyColumn);

var dmsMock = new DmsBuilder()
    .WithProtocol("ExampleProtocol", protocol => protocol
        .AddTableDefinition(200, tableDefinition))
    .WithDma(1, dma => dma
        .WithElement(
            id: 10,
            name: "Example element",
            protocolName: "ExampleProtocol",
            configure: element => element.WithTable(
                200,
                new[] { new object[] { "row-1", "Initial value" } })))
    .Build();
```

The definitions belong to `IDmsProtocolMock`. When an element is created, it receives its own parameter and table models based on those definitions. Consequently, changing a value on one element does not change the value on another element that uses the same protocol.

## Arranging and asserting parameter and table data

The object passed to the code under test is available through `IDmsElementMock.Object`. The mock and the exposed object use the same underlying parameter and table models.

```csharp
var elementMock = dmsMock.GetElementMock("Example element");

// Arrange through the mock.
elementMock.GetStandaloneParameterMock<string>(100).UpdateValue("Running");
elementMock.GetDmsTableMock(200).SetCell("row-1", 202, "Updated value");

// The code under test uses the real interfaces.
IDmsElement element = elementMock.Object;

// Assert the resulting state.
Assert.AreEqual("Running", element.GetStandaloneParameter<string>(100).GetValue());
Assert.AreEqual("Updated value", element.GetTable(200).GetColumn<string>(202).GetValue("row-1"));
```

The generic parameter types supported by the DataMiner System interfaces are `int?`, `double?`, `DateTime?`, and `string`. Unsupported generic types result in a `NotSupportedException`.

Value monitors are supported for standalone parameters, tables, and columns. Their callbacks are invoked when the corresponding underlying value changes.

## Connection subscriptions and events

Every `IDmsMock` exposes one shared `IConnectionMock` through its `Connection` property. Elements use this connection to publish parameter and table changes.

The mock supports the following subscription operations:

- `Subscribe(SubscriptionFilter[])`
- `Unsubscribe`
- `AddSubscription`
- `RemoveSubscription`
- `ReplaceSubscription`
- `ClearSubscriptions`

`Subscribe(SubscriptionFilter[])` creates an unnamed subscription. The remaining methods manage named subscription sets by subscription ID.

For parameter and table changes, the registered filters are evaluated before `IConnection.OnNewMessage` is raised. The filter checks the event-message type, DMA ID, and element ID. A negative DMA or element ID acts as a wildcard. A listener tracking one element therefore does not receive matching parameter changes from other elements.

```csharp
IConnection connection = dmsMock.Connection.Object;
var elementId = elementMock.Object.DmsElementId;

connection.OnNewMessage += (sender, args) =>
{
    if (args.Message is ParameterChangeEventMessage message)
    {
        // React to the parameter change.
    }
};

connection.AddSubscription(
    "Parameter changes",
    new SubscriptionFilter[]
    {
        new SubscriptionFilterElement(
            typeof(ParameterChangeEventMessage),
            elementId.AgentId,
            elementId.ElementId),
    });
```

Table notifications include table, row, and column information where applicable. Row deletion notifications are marked as deleted.

### Custom SLNet messages

Tests can register a typed handler when the code under test sends an application-specific `DMSMessage`.

The connection has several distinct message paths:

| Message path | Registered by | Behavior |
| --- | --- | --- |
| Built-in DOM request handler | `IDmsMock` | Handles the supported DOM CRUD request messages automatically |
| Custom request handler | Test or framework client | Handles one client-defined `DMSMessage` type and is evaluated before the built-in DOM handler |
| Parameter and table event publication | `IDmsElementMock` | Creates DataMiner event messages automatically and applies element subscription filters |
| DOM instance event publication | `DomSystemMock` | Publishes created, updated, and deleted DOM instances through the shared connection |
| Custom event publication | Test or framework client | Publishes any `DMSMessage` through `NotifySubscriptions` without applying a type-specific filter |

```csharp
dmsMock.Connection.RegisterMessageHandler<CustomRequestMessage>(request =>
    new CustomResponseMessage
    {
        Value = request.Value,
    });

DMSMessage[] responses = dmsMock.Connection.Object.HandleMessages(
    new DMSMessage[] { new CustomRequestMessage { Value = "request" } });
```

Custom handlers are evaluated before the built-in handler. Unhandled requests continue to the DOM message handler, so custom messages and `DomHelper` can use the same connection.

Handlers are registered per exact request-message type. Different clients can therefore add handlers for different message types. A handler can also be removed again with `UnregisterMessageHandler<TMessage>()`. Registering a handler for one type does not replace the DOM handler or handlers registered for other types. Registering a second handler for the same message type results in an `InvalidOperationException`.

`IConnectionMock.NotifySubscriptions(DMSMessage)` can be used to publish any custom event message to active subscribers without applying parameter or DOM-specific filters.

## DataMiner Object Models

DOM objects can be included in the same DMS arrangement. The framework intentionally works with the DOM object builders provided by `Skyline.DataMiner.Utils.DOM` instead of maintaining a second set of DOM builders.

```csharp
const string moduleId = "example-module";
Guid definitionId = Guid.NewGuid();

var dmsMock = new DmsBuilder()
    .WithDomDefinition(moduleId, () => new DomDefinitionBuilder()
        .WithID(definitionId)
        .WithName("Example definition")
        .Build())
    .Build();

var helper = new DomHelper(dmsMock.Connection.Object.HandleMessages, moduleId);

DomDefinition definition = helper.DomDefinitions
    .Read(DomDefinitionExposers.Id.Equal(definitionId))
    .Single();

Assert.AreEqual("Example definition", definition.Name);
```

`DomHelper` sends its normal SLNet request messages through `IConnection.HandleMessages` on `dmsMock.Connection.Object`. `DomSystemMock` handles those requests and stores the data in a separate in-memory cache for each module ID. Data from one module is therefore not visible through a helper for another module.

The following object types support read, create, update, and delete operations:

- `DomInstance`
- `DomDefinition`
- `SectionDefinition`
- `DomBehaviorDefinition`

Creating, updating, or deleting a `DomInstance` also publishes a `DomInstancesChangedEventMessage` through the shared connection.

## Architecture

See the [class diagram](CLASS_DIAGRAM.md) for the relationship between the builders, DMS cache, protocol definitions, element values, connection subscriptions, and DOM module caches.

## Scope

This package provides deterministic in-memory behavior for unit tests. It does not attempt to reproduce every DataMiner server feature.

The current simulation has the following boundaries:

- User sessions and permission checks are not simulated.
- DOM data is separated per module, but not per user session.
- DOM instance notifications are published through the shared connection without module- or instance-specific subscription filtering.
- Stored DOM objects are not cloned. Mutating the same object reference can therefore affect the in-memory state without a separate update call.
- Only the explicitly supported DOM CRUD request messages are handled.
- The connection is an in-memory message pipeline, not a complete SLNet implementation.

Unsupported DOM SLNet message types result in a `NotSupportedException`, making unsupported behavior explicit instead of silently returning an incorrect result.

## About DataMiner

DataMiner is a transformational platform that provides vendor-independent control and monitoring of devices and services. Out of the box and by design, it addresses key challenges such as security, complexity, multi-cloud, and much more. It has a pronounced open architecture and powerful capabilities enabling users to evolve easily and continuously.

The foundation of DataMiner is its powerful and versatile data acquisition and control layer. With DataMiner, there are no restrictions to what data users can access. Data sources may reside on premises, in the cloud, or in a hybrid setup.

A unique catalog of 7000+ connectors already exists. In addition, you can leverage DataMiner Development Packages to build your own connectors (also known as protocols or drivers).

> **Note**
> See also: [About DataMiner](https://aka.dataminer.services/about-dataminer).

## About Skyline Communications

At Skyline Communications, we deal in world-class solutions that are deployed by leading companies around the globe. Check out [our proven track record](https://aka.dataminer.services/about-skyline) and see how we make our customers' lives easier by empowering them to take their operations to the next level.
