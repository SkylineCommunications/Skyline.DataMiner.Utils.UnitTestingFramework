# SLProtocolMock
(namespace **UnitTestingFramework.Protocol**)

This is the main class with the properties:
- ProtocolCache (IProtocolCache);
- ProtocolModel (IProtocolModelExt).

The ProtocolModel is required in the class's constructor, so that the Parameters in the _xml_ protocol file can de loaded to ProtocolCache.


## Assert
Provides the assertion mechanism for the protocols in ProtocolCache.

### Parameters
 - None.

### Return Value
- (IAssert): The handler responsible for the assertion mechanism.


## Verify
Provides the verification mechanism for the protocols in ProtocolCache.

### Parameters
 - None.

### Return Value
- (IVerify): The handler responsible for the verification mechanism.
 

## LoadSetups
Defines what function from the framework should be called when a mocked SLProtcol method is invoked in a Unit Test.

### Parameters
 - None.

### Return Value
- (void).