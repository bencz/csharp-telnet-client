using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.Vt100
{
  public class TerminalVt100Statement
  {
    public CommandCode? EscapeCmd
    { get; set; }

    /// <summary>
    /// the text that follows the escape char
    /// </summary>
    public string StmtText
    { get; set; }

    public Vt100Command? Command
    { get; set; }

    public TerminalVt100Statement(string StmtText, Vt100Command? Command)
    {
      this.StmtText = StmtText;
      this.EscapeCmd = CommandCode.ESCAPE;
      this.Command = Command;
    }

    public TerminalVt100Statement( 
      CommandCode? EscapeCmd, string StmtText, Vt100Command? Command)
    {
      this.StmtText = StmtText;
      this.EscapeCmd = EscapeCmd;
      this.Command = Command;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("Esc");
      if (StmtText.Length > 0)
      {
        sb.Append(" ");
        sb.Append(StmtText);
      }
      
      if (this.Command != null)
      {
        sb.Append(" ");
        sb.Append(this.Command.Value.ToString());
      }

      return sb.ToString();
    }

    public static Tuple<TerminalVt100Statement,Vt100OutputText> Factory(
      InputByteArray InputArray)
    {
      TerminalVt100Statement stmt = null;
      Vt100OutputText otStmt = null;

      var escapeCmd = InputArray.PeekNextByte().ToTelnetCommandCode();

      if ((escapeCmd != null) && (escapeCmd.Value == CommandCode.ESCAPE)
        && ( InputArray.RemainingLength > 1 ))
      {
        InputArray.AdvanceIndex(1);
        var rv = InputArray.GetBytesUntilCode(new byte[] { 0x1b });
        var buf = rv.Item1;

        var stmtText = buf.ToAscii();

        stmt = Factory_FromStmtText(stmtText);

        // process any output text that follows the stmt text.
        {
          int lx = stmt.StmtText.Length;
          if (stmtText.Length > lx)
          {
            stmtText = stmtText.Substring(lx);
            otStmt = new Vt100OutputText(stmtText);
          }
        }
      }

      return new Tuple<TerminalVt100Statement, Vt100OutputText>(stmt, otStmt);
    }

    private static TerminalVt100Statement Factory_FromStmtText(string StmtText)
    {
      Vt100Command? cmd = null;
      TerminalVt100Statement stmt = null;
      string text1 = "";
      string text2 = "";
      string text3 = "";
      string text4 = "";
      string text20 = "";
      int rx = 0;

      if (StmtText.Length >= 1)
        text1 = StmtText.Substring(0, 1);
      if (StmtText.Length >= 2)
        text2 = StmtText.Substring(0, 2);
      if (StmtText.Length >= 3)
        text3 = StmtText.Substring(0, 3);
      if (StmtText.Length >= 4)
        text4 = StmtText.Substring(0, 4);

      int lx = IntExt.Min(StmtText.Length, 20);
      text20 = StmtText.Substring(0, lx);

      if (text4 == "[?3l")
      {
        cmd = Vt100Command.SetCol80;
        stmt = new TerminalVt100Statement(StmtText.Substring(0,4), cmd);
      }
      else if (text4 == "[?7h")
      {
        cmd = Vt100Command.SetAutoWrap;
        stmt = new TerminalVt100Statement(StmtText.Substring(0,4), cmd);
      }

      else if (text3 == "[0m")
        stmt = new TerminalVt100Statement(text3, Vt100Command.CharAttrOff);

      else if (text3 == "[1m")
        stmt = new TerminalVt100Statement(text3, Vt100Command.BoldModeOn);

      else if (text3 == "[4m")
        stmt = new TerminalVt100Statement(text3, Vt100Command.UnderlineModeOn);

      else if (text3 == "[2J")
        stmt = new TerminalVt100Statement(text3, Vt100Command.ClearScreen);

      else if ((text1 == "[") && (StmtText.Length >= 5)
        && ((rx = StmtText.MatchRegExp(REGEXPCURSORPOSITION)) > 0))
      {
        stmt = new Vt100PosCursor(StmtText.Substring(0,rx));
      }

      else
        stmt = new TerminalVt100Statement(StmtText, null);


      return stmt;
    }

    private static Regex REGEXPCURSORPOSITION = 
      new Regex("\\[\\d*;\\d*[Hf]", RegexOptions.Compiled);

  }

  public static class TerminalVt100StatementExt
  {

    public static int MatchRegExp(this string Text, Regex r)
    {
      if (Text == null || Text.Length < 1)
        return -1;
      Match m = r.Match(Text);
      if (m.Success && m.Index == 0)
      {
        return m.Length;
      }
      else
        return -1;
    }

  }

}
