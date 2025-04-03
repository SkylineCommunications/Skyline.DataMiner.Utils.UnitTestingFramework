# ITableModel
(namespace **UnitTestingFramework.Protocol.Model**)

This interface has the methods described below. Optional arguments are underlined.

## SetColumn
Sets the data of the specified column to the specified values.

### Parameters
 - columnIdx (int): The Idx of the column.
 - keys (string[]): The primary keys to set. 
 - values (object[]): The column data to set. In order to preserve the value of a cell, provide a null reference for that cell.
  - <ins>timeInfo (DateTime): The timestamp to be set together with the values.</ins>

### Return Value
- (void)


## SetRow
Sets the data of the specified row to the specified values.

### Parameters
- rowData (object[]): The row data to set. 
- <ins>timestamp (ValueType): The timestamp to be set together with the values.</ins>

### Return Value
- (object): An object array with value 0 (No Change) or 1 (Change) to indicate the change state of the cell in the row.


## SetExistingRow
Updates the data of the specified row to the specified values.

### Parameters
- rowData (object[]): The row data to set. 
- rowIndex (int): The index of the row.
- <ins>timestamp (ValueType): The timestamp to be set together with the values.</ins>

### Return Value
- (object): An object array with value 0 (No Change) or 1 (Change) to indicate the change state of the cell in the row.

### Remarks
The return array can also have value 2 (the value to set is equal to the previous value in the row).


## RemoveRow
Removes the specified row from the table.

### Parameters
- rowIndex (int): The index of the row.

### Return Value
- (void)

### Remarks
 To simulate the deletion behavior in SLProtocol, this method replaces the deleted line with data from the last line of the table.            
