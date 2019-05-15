using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum Vt100Command
  {
    BoldModeOn,
    CharAttrOff,
    ClearScreen,
    OutputText,
    SetCol80,
    SetAutoWrap,
    PosCursor,
    UnderlineModeOn
  }
}
