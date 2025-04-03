# <span style="color:#3caff2">TablesCache</span>

(namespace **UnitTestingFramework.Protocol.Data**)

This class has the methods described below. Optional arguments are underlined.

## <span style="color:#3caff2">AddModel</span>
This method is used when loading a parameter of type array to cache as a table.

### Parameters
- tableModel (ITableModel): The TableModel to set.

### Return Value
- (void).

### Remarks
The **ArrayHandler** class firstly creates a **TableModel** from the parameters in the protocol and then adds the TableModel to cache using this method.

## <span style="color:#3caff2">GetTableModel</span>
This method returns the table with the specified ID.

### Parameters
- tableId (int): The ID of the table to retrieve.

### Return Value
- (ITableModel).

## <span style="color:#3caff2">LoadSetups</span>
Defines what function from the framework should be called when a mocked SLProtocol method is invoked in a Unit Test.

### Parameters
- mock(Mock\<SLProtocol>): SLProtocol mocked class.

### Return Value
- (void).

## <span style="color:#3caff2">LoadSetups</span>
Defines what function from the framework should be called when a mocked SLProtocol method is invoked in a Unit Test.

### Parameters
- mock(Mock\<SLProtocol>): SLProtocol mocked class.

### Return Value
- (void).

## <span style="color:#3caff2">AddRow</span>
Adds a row with the specified data to the table.

### Parameters
- tableId (int): The ID of the table parameter.
- rowData (object[]): The row data.

### Return Value
- (int): The 1-based internal position of the row in the table.
 
## <span style="color:#3caff2">AddRow</span>
Adds a row to the table having the specified primary key.

### Parameters
- tableId: The ID of the table parameter.
- primaryKey (string): The primary key of the row.

### Return Value
- (int): The 1-based internal position of the row in the table.

## <span style="color:#3caff2">AddRowReturnKey</span>
Adds a row to the specified table and returns the primary key.

### Parameters
- tableID (int): The ID of the table parameter.
- row (object[]): The row data.

### Return Value
- (string) The primary key of the added row.

## <span style="color:#3caff2">AddRowReturnKey</span>
Adds a row to the specified table and returns the primary key.

### Parameters
- tableID (int): The ID of the table parameter.
- (string) The primary key of the row to add.

### Return Value
- (string) The primary key of the added row.

## <span style="color:#3caff2">DeleteRow</span>
Removes the specified row from the specified table.

### Parameters
- tableID (int): The ID of the table parameter.
- rowIndex (int): The index of the row.

### Return Value
- (int) Number of remaining rows in the table.

## <span style="color:#3caff2">DeleteRow</span>
Removes the specified row from the specified table.

### Parameters
- tableID (int): The ID of the table parameter.
- primaryKey (string): The primary key of the row to remove.

### Return Value
- (int) Number of remaining rows in the table.


## <span style="color:#3caff2">DeleteRow</span>
Removes the specified rows from the specified table.

### Parameters
- tableID (int): The ID of the table parameter.
- primaryKeys: The primary keys of the rows to remove.

### Return Value
- (int) Number of remaining rows in the table.

## <span style="color:#3caff2">ClearAllKeys</span>
Removes all rows from the specified table.
### Parameters
- tableID (int): The ID of the table parameter.

### Return Value
 - (object) Thevalue 0. In case the method has been invoked specifying an empty table, -1 is returned.

## <span style="color:#3caff2">Exists</span>
Determines whether a row with the specified primary key exists in the specified table.

### Parameters
- tableID (int): ID of the table parameter
- primaryKey (string): The primary key of the row.

### Return Value
- (bool) Indication of whether the table contains a row with the specified primary key.

## <span style="color:#3caff2">GetKeyPosition</span>
Gets the 1-based position of the row with the specified primary key in the table with the specified ID.

### Parameters
 - tableID (int): The ID of the table parameter.
- primaryKey (string): The primary key of the row for which the position has to be determined.

### Return Value
- (int): The 1-based position of the row in the table. If the table does not contain a row with the specified primary key, 0 is returned.

## <span style="color:#3caff2">GetRow</span>
Gets the row data of the specified row in the specified table.

### Parameters
 - tableID (int): The ID of the table parameter.
 - rowIndex (int): The 0-based index of the row.

### Return Value
- (object): The row data.


## <span style="color:#3caff2">GetRow</span>
Gets the row data of the specified row in the specified table.

### Parameters
 - tableID (int): The ID of the table parameter.
 - primaryKey (string): The primary key of the row.
 
### Return Value
- (object): The row data.


## <span style="color:#3caff2">SetRow</span>
Sets the data of the specified row to the specified values.

### Parameters
- tableID (int): The ID of the table parameter.
- rowIndex (int): The 0-based index of the row.
- rowData (object): The row data. 
- <ins> timestamp (DateTime): Time stamp. </ins>
- <ins> enableCellActions (bool): When set to true, protocol.Clear and protocol.Leave can be used as cell values, which will clear or preserve the cell content, respectively. </ins> TODO
 
### Return Value
- (object) Array with value 0 (No Change) or 1(Change) to indicate the change state of the cell in the row.


## <span style="color:#3caff2">SetRow</span>
Sets the data of the specified row to the specified values.

### Parameters
- tableID (int): The ID of the table parameter.
- primaryKey (string): The primary key of the row.
- rowData (object): The row data. 
- <ins> timestamp (DateTime): Time stamp. </ins>
- <ins> enableCellActions (bool): When set to true, protocol.Clear and protocol.Leave can be used as cell values, which will clear or preserve the cell content, respectively. </ins> TODO
 
### Return Value
- (object) Array with value 0 (No Change) or 1(Change) to indicate the change state of the cell in the row.

## <span style="color:#3caff2">FillArray</span>
Sets the content of the table to the provided content.

### Parameters
- tableID (int): The ID of the table parameter.
- columns (List<object[]>): The columns of the table.
- saveOption (NotifyProtocol.SaveOption): SaveOption.Full = unspecified primary keys are removed; SaveOption .Partial = rows with unspecified primary keys are preserved.
- <ins> timeInfo: Time stamp. </ins>
 
### Return Value
- (bool) The value true.


## <span style="color:#3caff2">FillArray</span>
Sets the content of the table to the provided content.

### Parameters
- tableID (int): The ID of the table parameter.
- columns (List<object[]>): The columns of the table.
- <ins> timeInfo (DateTime): Time stamp. </ins>
 
### Return Value
- (bool) The value true.


## <span style="color:#3caff2">FillArray</span>
Sets the content of the table to the provided content.

### Parameters
- tableID (int): The ID of the table parameter.
- columns (object[]): The columns of the table.
- <ins> timeInfo (DateTime): Time stamp. </ins>
 
### Return Value
- (bool) The value true.


## <span style="color:#3caff2">FillArrayNoDelete</span>
Adds the provided rows to the specified table.

### Parameters
- tableID (int): The ID of the table parameter.
- columns (List<object[]>): The columns of the table.
- <ins> timeInfo (DateTime): Time stamp. </ins>
 
### Return Value
- (bool) The value true.


## <span style="color:#3caff2">FillArrayNoDelete</span>
Adds the provided rows to the specified table.

### Parameters
- tableID (int): The ID of the table parameter.
- columns (object[]): The columns of the table.
- <ins> timeInfo (DateTime): Time stamp. </ins>
 
### Return Value
- (bool) The value true.


## <span style="color:#3caff2">FillArrayWithColumn</span>
Sets the specified cells of a column with the provided values.

### Parameters
- tableID (int): The ID of the table parameter.
- columnID (int): The ID of the column parameter.
- primaryKeys (object[]): The primary keys of the rows for which the column has to be updated.
- values (object[]): The values to set.
- <ins> timeInfo (DateTime): Time stamp. </ins>
 
### Return Value
- (bool) The value true.

### Remarks
- If the length of "primaryKeys" is not equal to the length of "values" and the length of the values array does not equal 1, an ArgumentException is thrown.