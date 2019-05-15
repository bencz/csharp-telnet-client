using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm.Data
{
  public class EnhancedDataTable : DataTable
  {
    /// <summary>
    /// row in the datatable that is selected for processing.
    /// </summary>
    public DataRow SelectedRow
    {
      get
      {
        if (this.SelectedRowNum == null)
          return null;
        else if (this.Rows.Count == 0)
          return null;
        else if (this.SelectedRowNum.Value > (this.Rows.Count + 1))
          return null;
        else
          return this.Rows[this.SelectedRowNum.Value];
      }
    }
    private int? SelectedRowNum
    { get; set; }

    public EnhancedDataTable( )
      : base( )
    {
    }

    public EnhancedDataTable(string TableName )
      : base(TableName)
    {
    }

    public EnhancedDataTable(string TableName, string tableNamespace )
      : base(TableName, tableNamespace)
    {
    }

    public void MarkSelectedRow( int RowNum)
    {
      this.SelectedRowNum = RowNum;
    }
  }
}
