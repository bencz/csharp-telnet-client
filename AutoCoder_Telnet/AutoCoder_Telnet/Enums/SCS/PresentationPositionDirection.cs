using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums.SCS
{
  public enum PresentationPositionDirection
  {
    Vertical = 0xc4,
    Horizontal = 0xc0
  };

  public static class PresentationPositionDirectionExt
  {
    public static PresentationPositionDirection? ToPresentationPositionDirection(this byte Value)
    {
      if (Value == 0xc4)
        return PresentationPositionDirection.Vertical;
      else if (Value == 0xc0)
        return PresentationPositionDirection.Horizontal;
      else
        return null;
    }
  }
}
