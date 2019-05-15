using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{
  [Flags] public enum WhichSide
  {
    Left = 1, 
    Right = 2, 
    Top = 4, 
    Bottom = 8
  }

  public static class WhichSideExt
  {
    public static WhichDirection ToDirection(this WhichSide Side)
    {
      switch (Side)
      {
        case WhichSide.Bottom:
          return WhichDirection.Down;
        case WhichSide.Top:
          return WhichDirection.Up;
        case WhichSide.Left:
          return WhichDirection.Left;
        case WhichSide.Right:
          return WhichDirection.Right;
        default:
          throw new ApplicationException("unexpected side");
      }
    }
  }
}
