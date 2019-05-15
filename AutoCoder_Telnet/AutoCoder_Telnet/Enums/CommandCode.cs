using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  // see RFC 854 for the telnet command codes.
  // see RFC 885 for the EOR command code.
  public enum CommandCode : byte
  {
    ESCAPE = 27, // 1B
    EOR = 239,   // EF end of record. See RFC 885.
    SE = 240,    // F0 end of subnegotiation parameters
    NOP = 241,   // F1 noop
    DM = 242,    // F2 data mark
    BRK = 243,   // F3 Break. Indicates that break or attention key was pressed.
    IP = 244,    // F4 interrupt, suspend or abort the process
    AO = 245,    // F5 abort output.
    AYT = 246,   // F6 are you there?
    EC = 247,    // F7 erase character. receiver should delete the preceeding char rcvd.
    EL = 248,    // F8 erase line
    GA = 249,    // F9 go ahead. tells the other end it can transmit.
    SB = 250,    // FA sub negotiation of the indicated option follows
    WILL = 251,  // FB
    WONT = 252,  // FC
    DO = 253,    // FD 
    DONT = 254,  // FE
    IAC = 255    // FF
  }

  public static class CommandCodeExt
  {

    /// <summary>
    /// get the next byte from the input array if the command has an option.
    /// </summary>
    /// <param name="Code"></param>
    /// <param name="ByteArray"></param>
    /// <returns></returns>
    public static SubjectByte GetOptionByte( 
      this CommandCode Code, InputByteArray ByteArray)
    {
      if (ByteArray.RemainingLength == 0)
        return null;
      else if ((Code == CommandCode.SB) || (Code == CommandCode.WILL) ||
        (Code == CommandCode.WONT) || (Code == CommandCode.DO) ||
        (Code == CommandCode.DONT))
      {
        var b1 = ByteArray.GetNextByte();
        return new SubjectByte(b1);
      }
      else
        return null;
    }

    /// <summary>
    /// peek at the next 2 bytes of input array. Return CommandCode if they bytes
    /// contain IAC escape followed by IAC command code.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static CommandCode? PeekTelnetCommandCode(
      this InputByteArray InputArray)
    {
      CommandCode? code = null;

      if (InputArray.RemainingLength >= 2)
      {
        var buf = InputArray.PeekBytes(2);
        code = buf.ParseTelnetCommandCode();
      }

      return code;
    }

    public static CommandCode? ParseTelnetCommandCode(this byte[] Bytes)
    {
      CommandCode? code = null;
      if ( Bytes.Length >= 2 )
      {
        if (Bytes[0] == (byte)CommandCode.IAC)
        {
          code = Bytes[1].ToTelnetCommandCode();
          if ((code != null) && (code.Value == CommandCode.IAC))
            code = null;
        }
      }
      return code;
    }

    /// <summary>
    /// peek at current bytes of input array for a specific telnet command code.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <param name="CmdCode"></param>
    /// <returns></returns>
    public static CommandCode? PeekTelnetCommandCode(
      this InputByteArray InputArray, CommandCode CmdCode )
    {
      var cmdCode = InputArray.PeekTelnetCommandCode();
      if ((cmdCode != null) && (cmdCode.Value != CmdCode))
        cmdCode = null;
      return cmdCode;
    }

    public static string ToString(this CommandCode Code)
    {
      switch (Code)
      {
        case CommandCode.WILL:
          return "WILL";
        case CommandCode.WONT:
          return "WONT";
        case CommandCode.DO:
          return "DO";
        case CommandCode.DONT:
          return "DONT";
        case CommandCode.IAC:
          return "IAC";
        case CommandCode.SE:
          return "SE";
        case CommandCode.NOP:
          return "NOP";
        case CommandCode.DM:
          return "DM";
        case CommandCode.BRK:
          return "BRK";
        case CommandCode.IP:
          return "IP";
        case CommandCode.AO:
          return "AO";
        case CommandCode.AYT:
          return "AYT";
        case CommandCode.EC:
          return "EC";
        case CommandCode.EL:
          return "EL";
        case CommandCode.GA:
          return "GA";
        case CommandCode.SB:
          return "SB";
        case CommandCode.EOR:
          return "EOR";
        default:
          return "";
      }
    }

    /// <summary>
    /// Return the TelnetCommandCode enum represented by the InByte argument.
    /// </summary>
    /// <param name="InByte"></param>
    /// <returns></returns>
    public static CommandCode? ToTelnetCommandCode(this byte Value)
    {
      if (Value == 0x1b)
        return CommandCode.ESCAPE;
      else if (Value == 251)
        return CommandCode.WILL;
      else if (Value == 252)
        return CommandCode.WONT;
      else if (Value == 253)
        return CommandCode.DO;
      else if (Value == 254)
        return CommandCode.DONT;
      else if (Value == 255)
        return CommandCode.IAC;
      else if (Value == 240)
        return CommandCode.SE;
      else if (Value == 241)
        return CommandCode.NOP;
      else if (Value == 242)
        return CommandCode.DM;
      else if (Value == 243)
        return CommandCode.BRK;
      else if (Value == 244)
        return CommandCode.IP;
      else if (Value == 245)
        return CommandCode.AO;
      else if (Value == 246)
        return CommandCode.AYT;
      else if (Value == 247)
        return CommandCode.EC;
      else if (Value == 248)
        return CommandCode.EL;
      else if (Value == 249)
        return CommandCode.GA;
      else if (Value == 250)
        return CommandCode.SB;
      else if (Value == 239)
        return CommandCode.EOR;
      else
        return null;
    }
  }
}
