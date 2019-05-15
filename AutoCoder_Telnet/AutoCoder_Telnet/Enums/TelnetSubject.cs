using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  /// <summary>
  /// the subject of the telnet command.
  /// </summary>
  public enum TelnetSubject
  {
    TRANSMIT_BINARY = 0,
    ECHO = 1,
    SUPPRESS_GO_AHEAD = 3,
    TerminalType = 24,  // hex 18
    TERMINAL_TYPE = 24, // hex 18
    END_OF_RECORD = 25, // hex 19
    NAWS = 31,          // negotiate about window size
    NEW_ENVIRON = 39,   // hex 27
    NewEnviron = 39     // hex 27
  }

  public static class TelnetSubjectExt
  {
    public static string ToString(this TelnetSubject Optn)
    {
      switch (Optn)
      {
        case TelnetSubject.SUPPRESS_GO_AHEAD:
          return "SUPPRESS_GO_AHEAD";
        case TelnetSubject.NewEnviron:
          return "NEW_ENVIRON";
        case TelnetSubject.TERMINAL_TYPE:
          return "TERMINAL_TYPE";
        default:
          return "";
      }
    }

    public static TelnetSubject? ToTelnetSubject(this byte Value)
    {
      TelnetSubject? optn;

      if (Value == 0)
        optn = TelnetSubject.TRANSMIT_BINARY;
      else if (Value == 1)
        optn = TelnetSubject.ECHO;
      else if (Value == 3)
        optn = TelnetSubject.SUPPRESS_GO_AHEAD;
      else if (Value == 24)
        optn = TelnetSubject.TerminalType;
      else if (Value == 25)
        optn = TelnetSubject.END_OF_RECORD;
      else if (Value == 31)
        optn = TelnetSubject.NAWS;
      else if (Value == 39)
        optn = TelnetSubject.NEW_ENVIRON;
      else
        optn = null;

      return optn;
    }

    public static TelnetSubject? PeekTelnetSubject( this InputByteArray InputArray, int Offset)
    {
      var b1 = InputArray.PeekByte(Offset);
      var subject = b1.ToTelnetSubject();
      return subject;
    }

    public static bool IsEqual(this TelnetSubject? Fac1, TelnetSubject Fac2 )
    {
      if (Fac1 == null)
        return false;
      else if (Fac1.Value == Fac2)
        return true;
      else
        return false;
    }
  }
}
