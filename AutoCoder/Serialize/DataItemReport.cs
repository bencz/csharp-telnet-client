using AutoCoder.Html;
using AutoCoder.Systm.Data;
using AutoCoder.Xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace AutoCoder.Serialize
{
  /// <summary>
  /// report of data items.
  /// Consists of an array of columnDefn. The defn of each column of the report.
  /// Then a list of DataItemList. Where each DataItemList is the contents of a
  /// row of the report.
  /// </summary>
  public class DataItemReport
  {
    public DataItemReport( )
    {
      this.Columns = new List<ColumnDefn>();
      this.Rows = new List<DataItemList>();
    }

    public List<ColumnDefn> Columns { get; set; }
    public List<DataItemList> Rows { get; set; }

    public void AddColumns(DataItemReport from)
    {
      this.Columns.AddRange(from.Columns);
    }
    public void AddEmptyRow( )
    {
      var row = new DataItemList();
      for( var ix = 0; ix < this.Columns.Count; ++ix)
      {
        var item = new DataItem(this.Columns[ix].ColName, "");
        row.Add(item);
      }
      this.Rows.Add(row);
    }
    public void AddRows(DataItemReport from)
    {
      this.Rows.AddRange(from.Rows);
    }

    /// <summary>
    /// add the rows of the input report to the rows of this report.
    /// </summary>
    /// <param name="report"></param>
    public void Append(DataItemReport report)
    {
      // if this report does not have columns, copy the columns of the append report.
      if (this.Columns.Count == 0)
        this.Columns = report.Columns.ToList() ;

      // combine the report columns of the two input reports.
      this.Rows.AddRange(report.Rows);
    }

    public EnhancedDataTable ToDataTable()
    {
      var table = new EnhancedDataTable();

      // add columns to the table.
      foreach( var reportColumn in this.Columns)
      {
        table.Columns.Add(reportColumn.ToDataColumn());
      }

      // add the rows to the table.
      foreach( var reportRow in this.Rows)
      {
        var dataRow = table.NewRow();
        int ix = 0;
        foreach( var item in reportRow)
        {
          dataRow[ix] = item.Value;
          ix += 1;
        }
        table.Rows.Add(dataRow);
      }

      return table;
    }

    public static DataItemReport CombineHorizontally(DataItemReport report1, DataItemReport report2)
    {
      var combo = new DataItemReport();

      if (report1.IsEmpty())
        combo = report2.Copy();

      else if (report2.IsEmpty())
        combo = report1.Copy();

      else
      {

        // report has columns but no rows. add a row with empty items.
        if (report1.Rows.Count == 0)
        {
          var dummy = report1.Copy();
          dummy.AddEmptyRow();
          report1 = dummy;
        }

        if (report2.Rows.Count == 0)
        {
          var dummy = report2.Copy();
          dummy.AddEmptyRow();
          report2 = dummy;
        }

        // combine the colummns.
        combo.AddColumns(report1);
        combo.AddColumns(report2);

        // combine rows.
        foreach (var row1 in report1.Rows)
        {
          foreach (var row2 in report2.Rows)
          {
            var comboRow = new DataItemList();
            comboRow.AddRange(row1);
            comboRow.AddRange(row2);

            combo.Rows.Add(comboRow);
          }
        }
      }

      return combo;
    }

    public static DataItemReport CombineVertically(DataItemReport report1, DataItemReport report2)
    {
      var combo = new DataItemReport();

      // set the columns of the combined reports to the columns of either report1
      // or report 2.
      combo.AddColumns(report1);
      if (combo.Columns.Count == 0)
        combo.AddColumns(report2);

      // combine the report columns of the two input reports.
      combo.AddRows(report1);
      combo.AddRows(report2);

      return combo;
    }

    /// <summary>
    /// return a copy of this report.
    /// </summary>
    /// <returns></returns>
    public DataItemReport Copy( )
    {
      var report = new DataItemReport();

      // copy the columns.
      foreach( var col in this.Columns)
      {
        report.Columns.Add(col);
      }

      // copy the rows
      foreach( var row in this.Rows)
      {
        report.Rows.Add(row);
      }

      return report;
    }

    public bool IsEmpty( )
    {
      if ((this.Columns.Count == 0) && (this.Rows.Count == 0))
        return true;
      else
        return false;
    }

    public string ToHtmlTableText( )
    {
      var tableElem = new HtmlElem("table");

      // tr for the column heading of each column of the report.
      {
        var trElem = tableElem.AddElem("tr");

        // <th> for each column.
        foreach (var column in this.Columns)
        {
          var thElem = trElem.AddText("th", column.ColName);
        }
      }

      // <tr> for each row of the report.
      foreach ( var row in this.Rows)
      {
        var trElem = tableElem.AddElem("tr");

        // td for each data item of the row.
        foreach( var dataItem in row)
        {
          var tdElem = trElem.AddText("td", dataItem.Value);
        }
      }

      var tableText = tableElem.ToString();
      return tableText;
    }

    public override string ToString()
    {
      return "DataItemReport. " + this.Columns.Count + " columns. " + this.Rows.Count + " rows.";
    }
  }
}
