using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Connect;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TelnetCommands
{
  /// <summary>
  /// sequence of bytes that start with the telnet IAC code.
  /// form of telnet command is: 
  /// IAC 
  /// request(DO WILL WONT SB) 
  /// subject - NEW_ENVIRON, TERMINAL_TYPE, END_OF_RECORD, TRANSMIT_BINARY
  /// action/qualifier - SEND IS
  /// command parms in form of KWD followed by data bytes. As in USERVAR DEVNAME or
  /// VALUE value bytes.
  /// </summary>
  public class TelnetCommand
  {
    /// <summary>
    /// the IAC command that starts this statement.
    /// </summary>
    public CommandCode EscapeCode
    { get; set; }

    public CommandCode CmdCode
    { get; set; }

    /// <summary>
    /// telnet option code. NEW_ENVIRON, TERMINAL_TYPE, ...
    /// </summary>
    public TelnetSubject? Subject
    { get; set; }

    public int Start
    { get; set; }

    public ByteArrayBuilder RawBytes
    {
      get;
      set;
    }

    /// <summary>
    /// the closing IAC SE is included in the bytes of this telent command.
    /// </summary>
    public bool GotClosingSE
    { get; set; }

    public TelnetCommand()
    {
      this.Subject = null;
      this.RawBytes = null;
    }

    public TelnetCommand(InputByteArray InputArray, CommandCode CmdCode)
    {
      this.Start = InputArray.Index;
      this.RawBytes = new ByteArrayBuilder();
      this.RawBytes.Append(InputArray.GetBytes(2));
      this.EscapeCode = CommandCode.IAC;
      this.CmdCode = CmdCode;
    }
    public TelnetCommand(InputByteArray InputArray, CommandCode CmdCode, TelnetSubject Subject)
      : this(InputArray, CmdCode )
    {
      this.Subject = Subject;
      this.RawBytes.Append(InputArray.GetByte());
    }

    public TelnetCommand(CommandCode Command)
    {
      this.EscapeCode = CommandCode.IAC;
      this.CmdCode = Command;
      this.Subject = null;
    }

    public TelnetCommand(CommandCode CmdCode, TelnetSubject Subject)
      : this(CmdCode)
    {
      this.Subject = Subject;
    }

    /// <summary>
    /// reply WONT to this received statement.
    /// </summary>
    /// <param name="NetStream"></param>
    public virtual TelnetCommand BuildWontReply()
    {
      var replyStmt =
        new TelnetCommand(CommandCode.WONT, this.Subject.Value);
      return replyStmt;
    }

    /// <summary>
    /// reply DO to this received statement.
    /// </summary>
    /// <param name="NetStream"></param>
    public virtual TelnetCommand BuildDoReply( )
    {
      var replyStmt =
        new TelnetCommand(CommandCode.DO, this.Subject.Value);
      return replyStmt;
    }

    /// <summary>
    /// reply to the DO command with a WILL or WONT command as per NegotiateSettings.
    /// </summary>
    /// <param name="Settings"></param>
    /// <returns></returns>
    public TelnetCommand BuildReply( NegotiateSettings Settings )
    {
      TelnetCommand reply = null;
      var option = this.Subject.Value;
      if ( Settings.OptionDict.ContainsKey(option) == true )
      {
        var cmdCode = Settings.OptionDict[option];
        reply = new TelnetCommand(cmdCode, this.Subject.Value);
      }
      else
      {
        reply = new TelnetCommand(CommandCode.WONT, this.Subject.Value);
      }
      return reply;
    }

    /// <summary>
    /// reply WILL to this received statement.
    /// </summary>
    /// <param name="NetStream"></param>
    public virtual TelnetCommand BuildWillReply( )
    {
      var replyStmt =
        new TelnetCommand(CommandCode.WILL, this.Subject.Value);
      return replyStmt;
    }

    public static TelnetCommand Factory(InputByteArray InputArray, CommandCode CmdCode)
    {
      TelnetCommand telnetCmd = null;

      TelnetSubject? subject = null;
      if (InputArray.RemainingLength > 2)
        subject = InputArray.PeekTelnetSubject(2);

      if (CmdCode == CommandCode.SE)
      {
        telnetCmd = new EndSubNegCommand(InputArray);
      }
      else if (CmdCode == CommandCode.EOR)
      {
        telnetCmd = new EOR_Command(InputArray);
      }

      else if (subject == null)
      {
        telnetCmd = new TelnetCommand(InputArray, CmdCode);
      }

      else if (subject.Value == TelnetSubject.NewEnviron)
      {
        telnetCmd = new NewEnvironCommand(InputArray, CmdCode);
      }

      else if (subject.Value == TelnetSubject.TerminalType)
      {
        telnetCmd = new TerminalTypeCommand(InputArray, CmdCode);
      }

      else if (subject.Value == TelnetSubject.END_OF_RECORD)
      {
        telnetCmd = new EndOfRecordStatement(InputArray, CmdCode);
      }

      else if (subject.Value == TelnetSubject.TRANSMIT_BINARY)
      {
        telnetCmd = new TransmitBinaryCommand(InputArray, CmdCode);
      }
      else if (subject.Value == TelnetSubject.NAWS)
      {
        telnetCmd = new NawsStatement(InputArray, CmdCode);
      }
      else if (subject.Value == TelnetSubject.ECHO)
      {
        throw new Exception("echo subject of " + CmdCode.ToString() + " command.");
      }
      else
      {
        throw new Exception("Unexpect telnet command");
      }

      return telnetCmd;
    }

    public static TelnetCommand Factory(InputByteArray InputArray)
    {
      TelnetCommand cmd = null;

      var cmdCode = InputArray.PeekTelnetCommandCode();
      if (cmdCode != null )
      {
        cmd = TelnetCommand.Factory(InputArray, cmdCode.Value);
      }

      return cmd;
    }

    /// <summary>
    /// expecting closing IAC SE command code. If exists at current position of the
    /// input array, process the IAC SE command.
    /// </summary>
    /// <param name="InputArray"></param>
    protected void ParseClosingSE(InputByteArray InputArray)
    {
      // parse the closing IAC SE
      var seCode = InputArray.PeekTelnetCommandCode(CommandCode.SE);
      if (seCode != null)
      {
        this.GotClosingSE = true;
        this.RawBytes.Append(InputArray.GetBytes(2));
      }
    }

    public virtual byte[] ToBytes()
    {
      ByteArrayBuilder ab = new ByteArrayBuilder();
      ab.Append((byte)this.EscapeCode);
      ab.Append((byte)this.CmdCode);
      if (this.Subject != null)
        ab.Append((byte)this.Subject.Value);

      {
        var bodyBytes = BodyToBytes();
        if ( bodyBytes != null )
        {
          ab.Append(bodyBytes);
        }
      }

      if ( this.GotClosingSE == true )
      {
        ab.Append((byte)CommandCode.IAC);
        ab.Append((byte)CommandCode.SE);
      }
      return ab.ToByteArray();
    }
    public string ToHexString()
    {
      byte[] byteStream = null;
      if (this.RawBytes != null)
        byteStream = this.RawBytes.ToByteArray();
      else
        byteStream = ToBytes();

      var s1 = "Lgth: " + byteStream.Length + " Bytes: "
          + byteStream.ToHex(' ');
      return s1;
    }

    public virtual string[] ToReportLines( )
    {
      var lines = new List<string>();
      lines.Add(this.ToString());
      lines.Add(this.ToHexString());
      return lines.ToArray();
    }
    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(this.EscapeCode.ToString() + " " + this.CmdCode.ToString());
      if (this.Subject != null)
        sb.Append(" " + this.Subject.Value.ToString());

      var s1 = BodyToString();
      if ( s1 != null )
      {
        sb.Append(" " + s1);
      }

      if (this.GotClosingSE == true)
        sb.Append(" IAC SE");
      return sb.ToString();
    }
    protected virtual string BodyToString( )
    {
      return null;
    }
    protected virtual byte[] BodyToBytes()
    {
      return null;
    }

    public void Write(NetworkStream NetStream)
    {
      var buf = this.ToBytes();
      NetStream.Write(buf, 0, buf.Length);
    }
  }

  public static class TelnetCommandExt
  {

    /// <summary>
    /// read next Telnet command byte sequence from the input byte array.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static TelnetCommand NextTelnetCommand(
      this InputByteArray InputArray)
    {
      TelnetCommand cmd = null;
      var code = InputArray.PeekTelnetCommandCode( );
      if ( code != null )
      {
        cmd = TelnetCommand.Factory(InputArray, code.Value);
      }
      return cmd;
    }
  }
}
