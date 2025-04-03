# ProtocolModelExt
(namespace **UnitTestingFramework.Protocol.Model**)

There are two options to choose the path of the _protocol.xml_ to be loaded:
- Either the user defines the path when calling the _ProtocolModelExt_
    ```csharp
    var protocolModel = new ProtocolModelExt(path);
    var mock = new SLProtocolMock(protocolModel);
    ```
- Or the user allows the framework to look for the _protocol.xml_ file in a parent directory of the current directory
    ```csharp
    var protocolModel = new ProtocolModelExt();
    var mock = new SLProtocolMock(protocolModel);
    ```
This class has the methods described below. Optional arguments are underlined.


## LoadParameterValues 
This method loads into cache the parameters present in the _xml_ protocol file.

### Parameters
 - cache (IProtocolCache): The cache with protocols.

### Return Value
- (void)

### Remarks
- The parameters (single parameters and table parameters) are all loaded according to their types (read, fixed, write or array) specifications


## LoadParameters 
This method loads into cache the parameters present in the _xml_ protocol file.

### Parameters
 - parametersIds (List\<int>): The IDs of the parameters to load.
 - values (List\<object>): The values of the parameters to load.
 - parametersCache (ParametersCache): The cache which contains single parameters.
 - <ins> timestamps (List\<DateTime>): The timestamp to be loaded together with the parameters.</ins>

### Return Value
- (void)

### Remarks
- This method is used in the Unit Tests to load single parameters' values to cache.
