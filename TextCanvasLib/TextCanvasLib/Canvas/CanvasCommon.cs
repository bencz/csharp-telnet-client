using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;

namespace TextCanvasLib.Canvas
{
  public static class CanvasCommon
  {
    public static bool DoesSpanMultipleRows( IRowCol ItemRowCol, byte? AttrByte, int ShowLength )
    {
      bool doesSpan = false;

      var showRowCol = ItemRowCol;
      if (AttrByte != null)
        showRowCol = ItemRowCol.Advance(1);

      var endRowCol = showRowCol;
      if ( ShowLength > 0)
        endRowCol = showRowCol.Advance(ShowLength - 1);

      if (showRowCol.RowNum == endRowCol.RowNum)
        doesSpan = false;
      else
        doesSpan = true;

      return doesSpan;
    }
  }
}
