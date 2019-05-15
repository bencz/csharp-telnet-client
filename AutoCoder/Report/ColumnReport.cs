using AutoCoder.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Report
{
  /// <summary>
  /// a Report that contains a list of column definitions
  /// </summary>
  public class ColumnReport : ReportList
  {
    public List<ReportColDefn> ColDefn { get; set; }

    /// <summary>
    /// number of lines per page. If specified the ColumnReport will auto print
    /// title and column heading on a new page.  Will auto skip to new page
    /// when page overflow.
    /// </summary>
    public int? PageNbrl { get; set; }

    /// <summary>
    /// current page line number. Use to auto overflow to a new page.
    /// </summary>
    private int PageLinn { get; set; }

    private bool IsNewPage { get; set; }

    public string ReportTitle { get; set; }

    public ColumnReport()
      : base()
    {
      this.ColDefn = new List<ReportColDefn>();
    }

    public ColumnReport(string ReportTitle,int PageNbrl)
    : this()
    {
      this.ReportTitle = ReportTitle;
      this.PageNbrl = PageNbrl;
      this.IsNewPage = true;
      this.PageLinn = 0;
    }

    public void AddColDefn(string ColName, int Width = 0, WhichSide WhichSide = WhichSide.Left)
    {
      var colDefn = new ReportColDefn(ColName, Width, WhichSide);
      this.ColDefn.Add(colDefn);
    }

    public void WriteColumnHeading(bool Dashes = false)
    {
      var lb = new BlankFillLineBuilder();
      int bx = 0;

      foreach (var colDefn in this.ColDefn)
      {
        var colName = colDefn.ColName;
        if (colDefn.WhichSide == WhichSide.Right)
          colName = colName.PadLeft(colDefn.Width);

        lb.Put(colName, bx, colDefn.Width);
        bx += (colDefn.Width + 2);
      }

      this.ReportLines.Add(lb.ToString());

      if (Dashes == true)
      {
        var dashesLine = ToDashesLine();
        this.ReportLines.Add(dashesLine);
      }
    }

    public void WriteDetail(string[] ColumnValues)
    {
      var lb = new BlankFillLineBuilder();
      int bx = 0;
      for (int ix = 0; ix < ColumnValues.Length; ++ix)
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

    public override void WriteTextLine(string Text)
    {
      bool isOverflow = false;
      if ( this.PageNbrl != null)
      {
        if ( this.IsNewPage == true )
        {
          this.IsNewPage = false;
          this.PageLinn = 0;
          this.WriteTextLine(this.ReportTitle);
          this.WriteColumnHeading(true);
        }

        this.PageLinn += 1;
        if ( this.PageLinn > this.PageNbrl.Value )
        {
          isOverflow = true;
        }
      }

      base.WriteTextLine(Text);

      // overflow. next line prints on new page.
      if ( isOverflow == true )
      {
        this.IsNewPage = true;
        this.PageLinn = 0;
      }
    }

    private string ToDashesLine()
    {
      var lb = new BlankFillLineBuilder();
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
}
