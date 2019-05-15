using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AutoCoder.Serialize
{
  /// <summary>
  /// the column name, column heading, data defn, width of a column of a report.
  /// </summary>
  public class ColumnDefn
  {
    public string ColName { get; set; }
    public DataDefn DataDefn { get; set; }
    public string[] ColHead { get; set; }

    /// <summary>
    /// The width of the column. Optional, otherwise calc width from data defn
    /// length.
    /// </summary>
    public int? Width { get; set; }

    public override string ToString()
    {
      return "ColumnDefn. " + this.ColName;
    }
    public DataColumn ToDataColumn()
    {
      var colDefn = new DataColumn();
      colDefn.ColumnName = this.ColName;
      return colDefn;
    }
  }
}
