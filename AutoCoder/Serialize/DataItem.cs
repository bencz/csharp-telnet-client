using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AutoCoder.Serialize
{
  /// <summary>
  /// data element value along with the name, data defn and prompt text of the
  /// element.
  /// </summary>
  public class DataItem
  {
    public DataItem(string ItemName, string Value)
    {
      this.ItemName = ItemName;
      this.Value = Value.TrimEndWhitespace();
    }
    public string Value { get; set; }
    public string ItemName { get; set; }
    public DataDefn DataDefn { get; set; }
    public string TextDesc { get; set; }
    public string[] ColHead { get; set; }

    public ColumnDefn ToColumnDefn( )
    {
      var colDefn = new ColumnDefn();
      colDefn.ColName = this.ItemName;
      colDefn.ColHead = this.ColHead;
      colDefn.DataDefn = this.DataDefn;
      return colDefn;
    }
    public DataColumn ToDataColumn( )
    {
      var colDefn = new DataColumn();
      colDefn.ColumnName = this.ItemName;
      return colDefn;
    }

    public override string ToString()
    {
      return "DataItem. " + this.Value;
    }
  }
}
