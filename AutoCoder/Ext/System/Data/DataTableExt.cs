using AutoCoder.Collections;
using AutoCoder.Ext.System.Reflection.Emit;
using AutoCoder.Html;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace AutoCoder.Ext.System.Data
{
  public static class DataTableExt
  {
    public static DataTable CombineVertically(DataTable report1, DataTable report2)
    {
      var combo = new DataTable();

      // set the columns of the combined reports to the columns of either report1
      // or report 2.
      combo.AddColumns(report1);
      if (combo.Columns.Count == 0)
        combo.AddColumns(report2);

      // combine the report rows of the two input tables.
      combo.AddRows(report1);
      combo.AddRows(report2);

      return combo;
    }

    /// <summary>
    /// add columns of the from table to the end of the to table.
    /// </summary>
    /// <param name="to"></param>
    /// <param name="from"></param>
    public static void AddColumns(this DataTable to, DataTable from)
    {
      for( var ix = 0; ix < from.Columns.Count; ++ix)
      {
        var col = from.Columns[ix];
        var toCol = new DataColumn(col.ColumnName, col.DataType);
        to.Columns.Add(toCol);
      }
    }

    /// <summary>
    /// add rows of the from table to the end of the to table.
    /// </summary>
    /// <param name="to"></param>
    /// <param name="from"></param>
    public static void AddRows(this DataTable to, DataTable from)
    {
      for (var ix = 0; ix < from.Rows.Count; ++ix)
      {
        var row = from.Rows[ix];
        var toRow = to.NewRow();
        for( var jx = 0; jx < from.Columns.Count; ++jx)
        {
          toRow[jx] = row[jx];
        }
        to.Rows.Add(toRow);
      }
    }

    /// <summary>
    /// return the zero based column number of the column name in the data table.
    /// </summary>
    /// <param name="DataTable"></param>
    /// <param name="ColName"></param>
    /// <returns></returns>
    public static int GetColumnNumber(this global::System.Data.DataTable DataTable, string ColName)
    {
      int colNx = -1;
      int cx = 0;
      foreach (DataColumn column in DataTable.Columns)
      {
        if (column.ColumnName == ColName)
        {
          colNx = cx;
          break;
        }
        cx += 1;
      }
      return colNx;
    }

    public static string ToHtmlTableText(this DataTable table)
    {
      var tableElem = new HtmlElem("table");

      // tr for the column heading of each column of the report.
      {
        var trElem = tableElem.AddElem("tr");

        // <th> for each column.
        for( int ix = 0; ix < table.Columns.Count; ++ix)
        {
          var col = table.Columns[ix];
          var thElem = trElem.AddText("th", col.ColumnName);
        }
      }

      // <tr> for each row of the report.
      for( int ix = 0; ix < table.Rows.Count; ++ix)
      {
        var row = table.Rows[ix];
        var trElem = tableElem.AddElem("tr");

        // td for each data item of the row.
        for( int jx = 0; jx < table.Columns.Count; ++jx)
        {
          var tdElem = trElem.AddText("td", row[jx].ToString( ));
        }
      }

      var tableText = tableElem.ToString();
      return tableText;
    }

    public static PropertiedObjectList ToListPropertiedObject(this DataTable dataTable)
    {
      List<object> listObject = new List<object>();

      var tableName = dataTable.TableName;
      if (tableName.IsNullOrEmpty() == true)
        tableName = "PropertiedTable";
      var className = tableName;
      var qualClassName = className + "." + className;

      // create an assembly, a module and then a type with the name of the table.
      AssemblyBuilder ab = null;
      ModuleBuilder mb = null;
      {
        var rv = StartAssembly(tableName);
        ab = rv.Item1;
        mb = rv.Item2;
      }

      // namespace Foo { public class Bar { ... } }
      TypeBuilder tb = mb.DefineType(qualClassName,
        TypeAttributes.Public | TypeAttributes.Class);

      // add each of the table columns as a property of the listObject.
      foreach( DataColumn col in dataTable.Columns)
      {
        var pb = tb.AddStringProperty(col.ColumnName);
      }

      // Seal the lid on this type
      Type t = tb.CreateType();

      // loop for each row of the table.
      foreach( DataRow row in dataTable.Rows)
      {
        var classObject = ab.CreateInstance(qualClassName);
        var itemType = classObject.GetType();

        foreach(DataColumn col in dataTable.Columns)
        {
          var s1 = row[col];
          var propName = col.ColumnName;
          itemType.InvokeMember(propName, BindingFlags.SetProperty, null, classObject,
             new Object[] { s1 });
        }

        // add to list of propertied objects.
        listObject.Add(classObject);
      }

      return new PropertiedObjectList(listObject);
    }

    private static Tuple<AssemblyBuilder, ModuleBuilder> StartAssembly( string tableName)
    {
      // Create a (weak) assembly name
      AssemblyName an = new AssemblyName();
      an.Name = "HelloReflectionEmit";

      // Define a new dynamic assembly (to be written to disk)
      AppDomain ad = AppDomain.CurrentDomain;
      AssemblyBuilder ab =
          ad.DefineDynamicAssembly(an, AssemblyBuilderAccess.RunAndSave);

      // Define a module for this assembly
      string moduleName = tableName + ".dll";
      ModuleBuilder mb = ab.DefineDynamicModule(an.Name, moduleName);

      return new Tuple<AssemblyBuilder, ModuleBuilder>(ab, mb);
    }
  }
}
