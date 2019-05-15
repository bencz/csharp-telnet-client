using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.Vt100
{
  public class Vt100PosCursor : TerminalVt100Statement
  {
    public int CursorRow
    { get; set; }

    public int CursorCol
    { get; set; }

    public Vt100PosCursor(string StmtText)
      : base(StmtText, Vt100Command.PosCursor)
    {
      int ix1 = StmtText.IndexOf(';');
      int ix2 = StmtText.IndexOf('H');
      this.CursorRow = Int32.Parse(StmtText.SubstringStartEnd(1,ix1 - 1).Trim( )) ;
      this.CursorCol = Int32.Parse(StmtText.SubstringStartEnd(ix1 + 1,ix2 - 1).Trim( )) ;
    }

    public override string ToString()
    {
      var s1 = base.ToString();
      return s1 + " " + this.CursorRow.ToString() + "," + this.CursorCol.ToString();
    }
  }
}
