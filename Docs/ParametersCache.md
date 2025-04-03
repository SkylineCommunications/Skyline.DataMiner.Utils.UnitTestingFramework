# <span style="color:#3caff2">ParametersCache</span>

(namespace **UnitTestingFramework.Protocol.Data**)

This class has the methods described below. Optional arguments are underlined.

## <span style="color:#3caff2">LoadSetups</span>
Auxiliary method to setup SLProtocol single parameter methods.

### Parameters
- mock (Mock\<SLProtocol>): The row data to set. 

### Return Value
- (void).

### Remarks
The return array can also have value 2 (the value to set is equal to the previous value in the row).

## <span style="color:#3caff2">LoadGetSetups</span>
Auxiliary method to setup get-related SLProtocol single parameter methods.

### Parameters
- mock (Mock\<SLProtocol>): The row data to set. 

### Return Value
- (void).

### Remarks
The return array can also have value 2 (the value to set is equal to the previous value in the row).

## <span style="color:#3caff2">LoadSetSetups</span>
Auxiliary method to setup set-related SLProtocol single parameter methods.

### Parameters
- mock(Mock\<SLProtocol>): SLProtocol mocked class.

### Return Value
- (void).

### Remarks
The return array can also have value 2 (the value to set is equal to the previous value in the row).

## <span style="color:#3caff2">LoadParameterName</span>
This method is used to load to cache the names of the parameters in the protocol.

### Parameters
- parameterName (string): The name of the parameter to set. 
- parameterId (int): The ID of the parameter to set.

### Return Value
- (void).

### Remarks
This method is used by the method **LoadParameterValues** (ProtocolModelExt)

## <span style="color:#3caff2">GetParameterModel</span>
Gets the specified parameter from cache.
  
### Parameters
- parameterId (int): The ID of the parameter to set.

### Return Value
- (IParameterModel): The specified ParameterModel.

### Remarks
This method is used in Unit Tests' assertions.

## <span style="color:#3caff2">GetParameter</span>
Gets the value of the parameter with the specified ID.
  
### Parameters
- parameterId (int): The ID of the parameter to retrieve.

### Return Value
- (object): The parameter value. If the parameterId doesn't exist in cache, a null reference is returned.

## <span style="color:#3caff2">GetParameterByName</span>
Gets the value of the parameter with the specified name.
  
### Parameters
- parameterName (string): The name of the parameter to retrieve.

### Return Value
- (object): The parameter value. If the parameterId doesn't exist in cache, a null reference is returned.

## <span style="color:#3caff2">GetParameters</span>
Gets the values of the specified parameters.
  
### Parameters
- parameterIDs(uint[]): The IDs of the parameters to retrieve.

### Return Value
- (object): The values of the retrieved parameters. In one parameterId doesn't exist in cache, a null reference is returned.

## <span style="color:#3caff2">SetParameter</span>
Sets the parameter with the specified ID to the specified value.
  
### Parameters
- parameterID(int): The ID of the parameter to set.
- value (object): The value to set.
- <ins> timestamp (DateTime): The timestamp. If provided, it will be stored in cache with the value. </ins>
- <ins> checkIfExists (bool): Indicates if the parameterID is already in cache or not. The value is true by default. </ins>

### Return Value
- (int): The value 0 is returned.

### Remarks
The checkIfExists argument is true by default, so that when the method is used, only parameters with ids existent in the protocol are set. It is only false, when the parameters are being loaded from the protocol for the first time, in the method **LoadParameters** (ProtocolModelExt).

## <span style="color:#3caff2">SetParameterByName</span>
Sets the parameter with the specified name to the specified value.
  
### Parameters
- parameterName (string): The name of the parameter to set.
- value (object): The value to set.

### Return Value
- (int): The value 0 is returned.

## <span style="color:#3caff2">SetParameters</span>
Sets the parameters with the specified IDs to the specified values.
  
### Parameters
- parameterID(int[]): The IDs of the parameters to set.
- values (object[]): The values to set.
- <ins> timestamp (DateTime): The timestamp. If provided, it will be stored in cache with the value. </ins>

### Return Value
- (int): If the parameterId exists in cache, the value 0 is returned. Otherwise, the Constants.HRESULT_FAIL_IDINEXISTENT (-2147220959) is returned.


### Remarks
The checkIfExists argument is true by default, so that when the method is used, only parameters with ids existent in the protocol are set. It is only false, when the parameters are being loaded from the protocol for the first time, in the method **LoadParameters** (ProtocolModelExt).

