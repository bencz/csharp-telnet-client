using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Enums
{
  [Flags]
  public enum WhichDirection
  {
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
  }

  public static class WhichDirectionExt
  {
    public static IEnumerable<WhichDirection> Directions()
    {
      yield return WhichDirection.Up;
      yield return WhichDirection.Down;
      yield return WhichDirection.Left;
      yield return WhichDirection.Right;
      yield break;
    }

    public static bool Equals(this Nullable<WhichDirection> Value1, WhichDirection Value2)
    {
      if (Value1 == null)
        return false;
      else if (Value1.Value == Value2)
        return true;
      else
        return false;
    }

    public static bool IsHorizontal(this WhichDirection Direction)
    {
      if (Direction == WhichDirection.Left)
        return true;
      else if (Direction == WhichDirection.Right)
        return true;
      else
        return false;
    }

    public static bool IsVertical(this WhichDirection Direction)
    {
      if (Direction == WhichDirection.Up)
        return true;
      else if (Direction == WhichDirection.Down)
        return true;
      else
        return false;
    }

    public static WhichSide ToSide(this WhichDirection Direction)
    {
      switch (Direction)
      {
        case WhichDirection.Down:
          return WhichSide.Bottom;
        case WhichDirection.Up:
          return WhichSide.Top;
        case WhichDirection.Left:
          return WhichSide.Left;
        case WhichDirection.Right:
          return WhichSide.Right;
        default:
          throw new ApplicationException("unexpected direction");
      }
    }
  }
}
