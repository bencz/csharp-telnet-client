using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.Vt100
{
  public class Vt100OutputText : TerminalVt100Statement
  {
    public string OutputText
    { get; set; }

    public Vt100OutputText(string StmtText)
      : base(StmtText, Vt100Command.OutputText)
    {
      this.OutputText = StmtText;
    }

    public override string ToString()
    {
      var s1 = this.Command.ToString() + " " + this.OutputText;
      return s1;
    }
  }
}

