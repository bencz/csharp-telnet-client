using AutoCoder.Core.Enums;
using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
#if skip
  /// <summary>
  /// a Report that contains a list of column definitions
  /// </summary>
  public class ColumnReport : Report
  {
    public List<ReportColDefn> ColDefn { get; set; }

    public ColumnReport()
      : base( ) 
    {
      this.ColDefn = new List<ReportColDefn>();
    }

    public void AddColDefn(string ColName, int Width = 0, WhichSide WhichSide = WhichSide.Left)
    {
      var colDefn = new ReportColDefn(ColName, Width, WhichSide);
      this.ColDefn.Add(colDefn);
    }

    public void WriteColumnHeading( bool Dashes = false )
    {
      var lb = new TextLineBuilder();
      int bx = 0;

      foreach( var colDefn in this.ColDefn)
      {
        var colName = colDefn.ColName;
        if (colDefn.WhichSide == WhichSide.Right)
          colName = colName.PadLeft(colDefn.Width);

        lb.Put(colName, bx, colDefn.Width);
        bx += (colDefn.Width + 2);
      }

      this.ReportLines.Add(lb.ToString());

      if ( Dashes == true )
      {
        var dashesLine = ToDashesLine();
        this.ReportLines.Add(dashesLine);
      }
    }

    public void WriteDetail(string[] ColumnValues)
    {
      var lb = new TextLineBuilder();
      int bx = 0;
      for(int ix = 0; ix < ColumnValues.Length; ++ix)
      {
        var colDefn = this.ColDefn[ix];
        var colText = ColumnValues[ix];
        if (colDefn.WhichSide == WhichSide.Right)
          colText = colText.PadLeft(colDefn.Width);

        lb.Put(colText, bx, colDefn.Width);
        bx += (colDefn.Width + 2);
      }

      this.WriteTextLine(lb.ToString());
    }
    private string ToDashesLine( )
    {
      var lb = new TextLineBuilder();
      int bx = 0;

      foreach (var colDefn in this.ColDefn)
      {
        string dashText = new string('-', colDefn.Width);
        lb.Put(dashText, bx, colDefn.Width);
        bx += (colDefn.Width + 2);
      }

      return lb.ToString();
    }
  }
#endif
}
