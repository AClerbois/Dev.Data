﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Apps72.Dev.Data.Internal
{
    /// <summary>
    /// Internal DataTable filled with all data from the Server.
    /// </summary>
    internal class DataTable
    {
        /// <summary>
        /// Initialize a new instance of DataTable
        /// </summary>
        public DataTable()
        {
            this.Columns = null;
            this.Rows = null;
            this.IsColumnDefined = false;
        }

        /// <summary>
        /// Load and fill all data (Rows and Columns) from the DbDataReader.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="firstRowOnly"></param>
        public void Load(DbDataReader reader, bool firstRowOnly)
        {
            List<DataRow> data = new List<DataRow>();
            int fieldCount = 0;

            // Read Columns definition
            if (reader.Read())
            {
                fieldCount = this.FillColumnsProperties(reader);
            }

            // Read data
            if (fieldCount > 0)
            {
                // Only first row
                if (firstRowOnly)
                {
                    object[] row = new object[fieldCount];
                    int result = reader.GetValues(row);
                    data.Add(new DataRow(this, row));
                }

                // All rows
                else
                {
                    do
                    {
                        object[] row = new object[fieldCount];
                        int result = reader.GetValues(row);
                        data.Add(new DataRow(this, row));

                        if (firstRowOnly)
                            continue;
                    }
                    while (reader.Read());
                }
            }

            this.Rows = data.ToArray();
        }
        
        public bool IsColumnDefined { get; set; }

        /// <summary>
        /// Fill all columns properties
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private int FillColumnsProperties(DbDataReader reader)
        {
            int fieldCount = reader.FieldCount;

            var columns = new DataColumn[fieldCount];

            for (int i = 0; i < fieldCount; i++)
            {
                columns[i] = new DataColumn()
                {
                    Ordinal = i,
                    ColumnName = reader.GetName(i),
                    ColumnType = reader.GetFieldType(i),
                    IsNullable = reader.IsDBNull(i)
                };
            }

            this.Columns = columns;
            this.IsColumnDefined = true;

            return fieldCount;
        }

        /// <summary>
        /// Gets the Columns properties
        /// </summary>
        public DataColumn[] Columns { get; private set; }

        /// <summary>
        /// Gets all Rows values
        /// </summary>
        public DataRow[] Rows { get; private set; }

        /// <summary>
        /// Creates a new instance of T type and sets all row values to the new T properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public T[] ConvertTo<T>()
        {
            T[] results = new T[this.Rows.Count()];

            // If is Primitive type (string, int, ...)
            if (Convertor.TypeExtension.IsPrimitive(typeof(T)))
            {
                int i = 0;
                foreach (DataRow row in this.Rows)
                {
                    object scalar = row[0];
                    if (scalar == null || scalar == DBNull.Value)
                        results[i] = default(T);
                    else
                        results[i] = (T)scalar;
                    i++;
                }
            }

            // If is Complex type (class)
            else
            {
                int i = 0;
                foreach (DataRow row in this.Rows)
                {
                    results[i] = row.ConvertTo<T>();
                    i++;
                }
            }

            return results;
        }

#if NET451
        public System.Data.DataTable ConvertToSystemDataTable()
        {
            System.Data.DataTable table = new System.Data.DataTable();
            table.TableName = "DataTable";

            // Columns
            table.Columns.AddRange(this.Columns.Select(c => 
                                            new System.Data.DataColumn()
                                            {
                                                ColumnName = c.ColumnName,
                                                AllowDBNull = c.IsNullable,
                                                DataType = c.ColumnType
                                            }).ToArray());

            table.NewRow();

            return table;
        }
#endif
    }

}
