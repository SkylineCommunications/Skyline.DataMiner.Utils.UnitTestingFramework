namespace Skyline.DataMiner.Utils.UnitTestingFramework.SnapshotTools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Data;
    using Skyline.DataMiner.Utils.UnitTestingFramework.Protocol.Model;

    /// <summary>
    /// Struct for standalone parameters in snapshot tests.
    /// </summary>
    public struct Showcase
    {
        /// <summary>
        /// Name of the parameter or variable.
        /// </summary>
        public object Name { get; set; }

        /// <summary>
        /// Value of the parameter or variable.
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Struct used for tables in snapshot tests.
    /// </summary>
    public struct TableShowcase
    {
        /// <summary>
        /// Name of the table.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The table id.
        /// </summary>
        public int TableID { get; set; }

        /// <summary>
        /// Name of the column that contains the keys.
        /// </summary>
        public string KeyColumn { get; set; }

        /// <summary>
        /// The index of the key column.
        /// </summary>
        public object KeyColumnIdx { get; set; }

        /// <summary>
        /// The rows of the table.
        /// </summary>
        public object Rows { get; set; }

        /// <summary>
        /// The return value of the method being tested.
        /// </summary>
        public object ReturnValue { get; set; }
    }

    /// <summary>
    /// Struct used for a single row.
    /// </summary>
    public struct RowShowcase
    {
        /// <summary>
        /// Index of the row.
        /// </summary>
        public object RowIndex { get; set; }

        /// <summary>
        /// The content of the row.
        /// </summary>
        public List<RowContent> Content { get; set; }
    }

    /// <summary>
    /// Struct used for a single cell of a row.
    /// </summary>
    public struct RowContent
    {
        /// <summary>
        /// The column that the cell is in.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// The value of the cell.
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Methods for snapshot testing.
    /// </summary>
    public static class SnapshotTools
    {
        /// <summary>
        /// Fills a collection of showcase structs. It fills the
        /// Name prop of the Showcase with integers from 0 to values.Length.
        /// </summary>
        /// <param name="values">Values to fill the showcase.</param>
        /// <returns>The showcase collection.</returns>
        public static List<Showcase> FillShowcase(object[] values)
        {
            List<Showcase> showcase = new List<Showcase>();

            int[] names = Enumerable.Range(0, values.Length).ToArray();

            for (int i = 0; i < values.Length; i++)
            {
                showcase.Add(new Showcase
                {
                    Name = names[i],
                    Value = values[i],
                });
            }

            return showcase;
        }

        /// <summary>
        /// Fills a collection of showcase structs.
        /// </summary>
        /// <param name="names">Names to fill the showcase.</param>
        /// <param name="values">Values to fill the showcase.</param>
        /// <returns>The showcase collection.</returns>
        public static List<Showcase> FillShowcase(string[] names, object[] values)
        {
            if (names.Length != values.Length)
            {
                throw new ArgumentException($"Input arrays have different sizes. {nameof(names)} length '{names.Length}' != {nameof(values)} length '{values.Length}'");
            }

            List<Showcase> showcase = new List<Showcase>();

            for (int i = 0; i < values.Length; i++)
            {
                showcase.Add(new Showcase
                {
                    Name = names[i],
                    Value = values[i],
                });
            }

            return showcase;
        }

        /// <summary>
        /// Fills a collection of showcase structs.
        /// </summary>
        /// <param name="names">Names to fill the showcase.</param>
        /// <param name="values">Values to fill the showcase.</param>
        /// <returns>The showcase collection.</returns>
        public static List<Showcase> FillShowcase(string[] names, uint[] values)
        {
            if (names.Length != values.Length)
            {
                throw new ArgumentException($"Input arrays have different sizes. {nameof(names)} length '{names.Length}' != {nameof(values)} length '{values.Length}'");
            }

            List<Showcase> showcase = new List<Showcase>();

            for (int i = 0; i < values.Length; i++)
            {
                showcase.Add(new Showcase
                {
                    Name = names[i],
                    Value = values[i],
                });
            }

            return showcase;
        }

        /// <summary>
        /// Fills a collection of showcase structs.
        /// </summary>
        /// <param name="ids">Parameter ids to fill the showcase.</param>
        /// <param name="values">Values to fill the showcase.</param>
        /// <returns>The showcase collection.</returns>
        public static List<Showcase> FillShowcase(uint[] ids, object[] values)
        {
            if (ids.Length != values.Length)
            {
                throw new ArgumentException($"Input arrays have different sizes. {nameof(ids)} length '{ids.Length}' != {nameof(values)} length '{values.Length}'");
            }

            List<Showcase> showcase = new List<Showcase>();

            for (int i = 0; i < values.Length; i++)
            {
                showcase.Add(new Showcase
                {
                    Name = ids[i],
                    Value = values[i],
                });
            }

            return showcase;
        }

        /// <summary>
        /// Fills a showcase struct.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Value of the parameter.</param>
        /// <returns>The showcase with name and value of the param.</returns>
        public static Showcase FillShowcase(string name, object value)
        {
            Showcase showcase = new Showcase
            {
                Name = name,
                Value = value,
            };

            return showcase;
        }

        /// <summary>
        /// Creates a TableShowcase for snapshot testing.
        /// </summary>
        /// <param name="cache">The cache with the data.</param>
        /// <param name="tableId">The table id.</param>
        /// <returns>The table in the TableShowcase form, ready to be used with verify.</returns>
        public static TableShowcase ShowTable(ProtocolCache cache, int tableId)
        {
            ITableModelReader tableModel = cache.Tables.GetTableModel(tableId);

            List<RowShowcase> rows = new List<RowShowcase>();
            string[] keys = cache.Tables.GetKeys(tableId);
            cache.Tables.GetColumnsNamesAndPids(cache, tableId, out string[] columnNames, out int[] columnPids);

            for (int i = 0; i < keys.Length; i++)
            {
                object[] row = (object[])cache.Tables.GetRow(tableId, keys[i]);
                List<RowContent> rowContent = new List<RowContent>();

                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] == null)
                    {
                        row[j] = "EMPTY CELL";
                    }

                    rowContent.Add(new RowContent
                    {
                        ColumnName = $"{columnNames[j]}_{columnPids[j]}",
                        Value = row[j],
                    });
                }

                rows.Add(new RowShowcase
                {
                    RowIndex = i,
                    Content = rowContent,
                });
            }

            int keyColumnIdx = tableModel.KeyColumnIdx;
            int keyColumnPid = tableModel.ColumnIndexesToPids[keyColumnIdx];

            cache.Parameters.TryGetParameterNameByPID(tableId, out string tableName);
            cache.Parameters.TryGetParameterNameByPID(keyColumnPid, out string keyColumnName);

            TableShowcase table = new TableShowcase
            {
                Name = tableName,
                TableID = tableId,
                KeyColumn = keyColumnName,
                KeyColumnIdx = keyColumnIdx,
                Rows = rows,
            };

            if (keys.Length == 0)
            {
                table.Rows = "EMPTY TABLE";
            }

            return table;
        }

        /// <summary>
        /// Creates a TableShowcase for snapshot testing.
        /// </summary>
        /// <param name="cache">The cache with the data.</param>
        /// <param name="tableId">The table id.</param>
        /// <param name="returnValue">The return value of the method being tested.</param>
        /// <returns>The table in the TableShowcase form, ready to be used with verify.</returns>
        public static TableShowcase ShowTable(ProtocolCache cache, int tableId, object returnValue)
        {
            ITableModelReader tableModel = cache.Tables.GetTableModel(tableId);

            List<RowShowcase> rows = new List<RowShowcase>();
            string[] keys = cache.Tables.GetKeys(tableId);
            cache.Tables.GetColumnsNamesAndPids(cache, tableId, out string[] columnNames, out int[] columnPids);

            for (int i = 0; i < keys.Length; i++)
            {
                object[] row = (object[])cache.Tables.GetRow(tableId, keys[i]);
                List<RowContent> rowContent = new List<RowContent>();

                for (int j = 0; j < row.Length; j++)
                {
                    if (row[j] == null)
                    {
                        row[j] = "EMPTY CELL";
                    }

                    rowContent.Add(new RowContent
                    {
                        ColumnName = $"{columnNames[j]}_{columnPids[j]}",
                        Value = row[j],
                    });
                }

                rows.Add(new RowShowcase
                {
                    RowIndex = i,
                    Content = rowContent,
                });
            }

            int keyColumnIdx = tableModel.KeyColumnIdx;
            int keyColumnPid = tableModel.ColumnIndexesToPids[keyColumnIdx];

            cache.Parameters.TryGetParameterNameByPID(tableId, out string tableName);
            cache.Parameters.TryGetParameterNameByPID(keyColumnPid, out string keyColumnName);

            TableShowcase table = new TableShowcase
            {
                Name = tableName,
                TableID = tableId,
                KeyColumn = keyColumnName,
                KeyColumnIdx = keyColumnIdx,
                Rows = rows,
                ReturnValue = returnValue,
            };

            if (keys.Length == 0)
            {
                table.Rows = "EMPTY TABLE";
            }

            return table;
        }

        /// <summary>
        /// Creates a collection of TableShowcase for snapshot testing.
        /// </summary>
        /// <param name="cache">The cache with the data.</param>
        /// <param name="tableIds">The table ids.</param>
        /// <returns>The tables in the TableShowcase form, ready to be used with verify.</returns>
        public static List<TableShowcase> ShowTables(ProtocolCache cache, int[] tableIds)
        {
            List<TableShowcase> tables = new List<TableShowcase>();

            foreach (int id in tableIds)
            {
                ITableModelReader tableModel = cache.Tables.GetTableModel(id);

                List<RowShowcase> rows = new List<RowShowcase>();
                string[] keys = cache.Tables.GetKeys(id);
                cache.Tables.GetColumnsNamesAndPids(cache, id, out string[] columnNames, out int[] columnPids);

                for (int i = 0; i < keys.Length; i++)
                {
                    object[] row = (object[])cache.Tables.GetRow(id, keys[i]);
                    List<RowContent> rowContent = new List<RowContent>();

                    for (int j = 0; j < row.Length; j++)
                    {
                        if (row[j] == null)
                        {
                            row[j] = "EMPTY CELL";
                        }

                        rowContent.Add(new RowContent
                        {
                            ColumnName = $"{columnNames[j]}_{columnPids[j]}",
                            Value = row[j],
                        });
                    }

                    rows.Add(new RowShowcase
                    {
                        RowIndex = i,
                        Content = rowContent,
                    });
                }

                int keyColumnIdx = tableModel.KeyColumnIdx;
                int keyColumnPid = tableModel.ColumnIndexesToPids[keyColumnIdx];

                cache.Parameters.TryGetParameterNameByPID(id, out string tableName);
                cache.Parameters.TryGetParameterNameByPID(keyColumnPid, out string keyColumnName);

                TableShowcase table = new TableShowcase
                {
                    Name = tableName,
                    TableID = id,
                    KeyColumn = keyColumnName,
                    KeyColumnIdx = keyColumnIdx,
                    Rows = rows,
                };

                if (keys.Length == 0)
                {
                    table.Rows = "EMPTY TABLE";
                }

                tables.Add(table);
            }

            return tables;
        }
    }
}