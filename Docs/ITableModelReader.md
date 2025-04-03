# ITableModelReader
(namespace **UnitTestingFramework.Protocol.Model**)

This interface has the methods described below.

## Column
Gets the specified column from the table.  
    
### Parameters
 - columnId (int): The ID of the column.

### Return Value
- (object[]): An object array containing the column data.


## Row
Gets the specified row from the table.  
    
### Parameters
 - rowIndex (int): The index of the row.

### Return Value
- (object[]): An object array containing the row data.


## Row
Gets the specified row from the table.  
    
### Parameters
 - primaryKey(string): The primary key of the row.

### Return Value
- (object[]): An object array containing the row data.


## Row\<TRow>
Gets the specified row from the table.  
    
### Parameters
 - rowIndex (int): The index of the row.

### Return Value
- (TRow): A QActionTableRow, available in the class library.

## Row\<TRow>
Gets the specified row from the table.  
    
### Parameters
 - primaryKey(string): The primary key of the row.

### Return Value
- (TRow): A QActionTableRow, available in the class library.
