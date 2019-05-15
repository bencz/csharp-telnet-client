using AutoCoder.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  public class ReportColDefn
  {
    public string ColName { get; set; }
    public string[] ColHead { get; set; }

    private int _Width;
    public int Width
    {
      get
      {
        if (_Width == 0)
          return ColName.Length;
        else
          return _Width;
      }
      set { _Width = value; }
    }

    public WhichSide WhichSide { get; set; }

    public ReportColDefn( string ColName, int Width = 0, WhichSide WhichSide = WhichSide.Left )
    {
      this.ColName = ColName;
      this.ColHead = new string[] { this.ColName };
      this.Width = Width;
      this.WhichSide = WhichSide;
    }
  }
}
