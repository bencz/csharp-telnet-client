using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCanvasLib.Canvas;

namespace TextCanvasLib.Ext
{
  public static class IRowColExt
  {
    public static CanvasPositionCursor LocalPosToParentPos(
      this IRowCol AdjustRowCol, CanvasPositionCursor LocalPos)
    {
      CanvasPositionCursor parentPos = null;
      if (AdjustRowCol == null)
        parentPos = LocalPos;
      else
        parentPos = LocalPos.Add(AdjustRowCol);
      return parentPos;
    }
  }
}
