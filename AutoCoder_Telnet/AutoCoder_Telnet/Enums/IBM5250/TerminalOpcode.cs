using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.IBM5250
{
  // data stream codes documented in RFC1205. Page 4.


    /// <summary>
    /// terminal data stream opcode.
    /// </summary>
  public enum TerminalOpcode : byte
  {
    Noop = 0,
    Invite = 1,
    OutputOnly = 2,
    PutGet = 3,
    SaveScreen = 4,
    RestoreScreen = 5,
    ReadImmediate = 6,
    Reserved1 = 7,
    ReadScreen = 8,
    Reserved2 = 9,
    CancelInvite = 10,
    MessageLightOn = 11,
    MessageLightOff = 12
  }

  public static class TerminalOpcodeExt
  {
    /// <summary>
    /// Return the TelnetCommandCode enum represented by the InByte argument.
    /// </summary>
    /// <param name="InByte"></param>
    /// <returns></returns>
    public static TerminalOpcode? ToTerminalOpcode(this byte Value)
    {
      if (Value == 0)
        return TerminalOpcode.Noop ;
      else if ( Value == 1 )
        return TerminalOpcode.Invite ;
      else if ( Value == 2 )
        return TerminalOpcode.OutputOnly ;
      else if ( Value == 3 )
        return TerminalOpcode.PutGet ;
      else if ( Value == 4 )
        return TerminalOpcode.SaveScreen ;
      else if ( Value == 5 )
        return TerminalOpcode.RestoreScreen ;
      else if ( Value == 6 )
        return TerminalOpcode.ReadImmediate ;
      else if ( Value == 7 )
        return TerminalOpcode.Reserved1 ;
      else if ( Value == 8 )
        return TerminalOpcode.ReadScreen ;
      else if ( Value == 9 )
        return TerminalOpcode.Reserved2 ;
      else if ( Value == 10 )
        return TerminalOpcode.CancelInvite ;
      else if ( Value == 11 )
        return TerminalOpcode.MessageLightOn ;
      else if ( Value == 12 )
        return TerminalOpcode.MessageLightOff ;
      else
        return null;
    }
  }

}
