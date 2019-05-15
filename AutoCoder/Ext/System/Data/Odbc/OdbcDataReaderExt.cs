using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.Data.Odbc
{
  public static class OdbcDataReaderExt
  {
    public static DataTable ToDataTable(this OdbcDataReader reader)
    {
      var schema = reader.GetSchemaTable();
      var table = SchemaTableToDataTable(schema);

      object[] row = new object[reader.FieldCount];

      while(true)
      {
        var rc = reader.Read();
        if (rc == false)
          break;
        var cx = reader.GetValues(row);
        var dataRow = table.NewRow();

        for(int ix = 0; ix < reader.FieldCount; ++ix)
        {
          dataRow[ix] = reader[ix];
        }
        table.Rows.Add(dataRow);
      }

      return table;
    }

    static DataTable SchemaTableToDataTable(DataTable schema)
    {
      var table = new DataTable();
      foreach (DataRow row in schema.Rows)
      {

        string colName = row["ColumnName"] as string;
        int columnSize = (int)row["ColumnSize"];
        int numericPrecision = (short)row["NumericPrecision"];
        int numericScale = (short)row["NumericScale"];
        var dataType = row["DataType"] as Type;
        int ordinal = (int)row["ColumnOrdinal"];

        var column = new DataColumn(colName, dataType);
        table.Columns.Add(column);
      }

      return table;
    }
  }
}
