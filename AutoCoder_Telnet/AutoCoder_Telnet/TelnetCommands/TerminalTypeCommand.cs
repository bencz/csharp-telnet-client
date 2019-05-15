using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  public class TerminalTypeCommand : TelnetCommand
  {
    /// <summary>
    /// IS, SEND, INFO
    /// </summary>
    public TelnetOptionParm? SubOption
    { get; set; }

    public byte[] TerminalNameBytes
    { get; set; }

    /// <summary>
    /// stmt end IAC SE is found.
    /// </summary>
    private bool EndFound
    { get; set; }

    public TerminalTypeCommand(
      CommandCode CmdCode, TelnetOptionParm SubOption)
      : base(CmdCode, TelnetSubject.TerminalType)
    {
      this.SubOption = SubOption;
      this.TerminalNameBytes = null;
    }

    public TerminalTypeCommand(InputByteArray InputArray, CommandCode CmdCode)
      : base(InputArray, CmdCode, TelnetSubject.TerminalType)
    {
      this.SubOption = null;
      this.EndFound = false;

      // statement contains additional parameters.
      if (this.CmdCode == CommandCode.SB)
      {
        var b1 = InputArray.GetNextByte();
        this.SubOption = b1.ToTelnetOptionParm();
        this.RawBytes.Append(b1);

        if ((this.SubOption != null ) 
          && (this.SubOption.Value == TelnetOptionParm.IS))
        {
          var rv = InputArray.GetBytesUntilCode(new byte[] { 0xff });
          this.TerminalNameBytes = rv.Item1;
          this.RawBytes.Append(this.TerminalNameBytes);
        }

        // parse the closing IAC SE
        ParseClosingSE(InputArray);
      }
    }

    public void AssignTerminalName(string TerminalName)
    {
      this.TerminalNameBytes = Encoding.ASCII.GetBytes(TerminalName);
    }

    protected override byte[] BodyToBytes()
    {
      ByteArrayBuilder ab = new ByteArrayBuilder();

      if (this.SubOption != null)
        ab.Append(this.SubOption.Value.ToByte());

      if (this.TerminalNameBytes != null)
        ab.Append(this.TerminalNameBytes);

      return ab.ToByteArray();
    }

    protected override string BodyToString()
    {
      var sb = new StringBuilder();
      if (this.SubOption != null)
      {
        sb.Append(this.SubOption.Value.ToString());
      }

      if (this.TerminalNameBytes != null)
      {
        var s1 = Encoding.ASCII.GetString(this.TerminalNameBytes);
        if (sb.Length > 0)
          sb.Append(" ");
        sb.Append(s1);
      }

      return sb.ToString();
    }
  }
}


