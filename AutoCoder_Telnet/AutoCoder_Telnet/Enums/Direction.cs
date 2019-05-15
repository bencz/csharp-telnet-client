using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  public enum Direction
  {
    Read,
    Write,
    none
  }

  public static class DirectionExt
  {
    public static Direction? TryParseDirection(this string Text)
    {
      var lcText = Text.ToLower();
      if (lcText == "read")
        return Direction.Read;
      else if (lcText == "write")
        return Direction.Write;
      else if (lcText == "none")
        return Direction.none;
      else
        return null;
    }
  }
}

