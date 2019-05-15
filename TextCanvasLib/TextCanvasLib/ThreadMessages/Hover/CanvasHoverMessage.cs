using AutoCoder.Telnet.ThreadMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextCanvasLib.Canvas;

namespace TextCanvasLib.ThreadMessages.Hover
{
  public class CanvasHoverMessage : HoverMessageBase
  {
    public ItemCanvas ItemCanvas
    { get; set; }
    public Point HoverPos
    { get; set; }

    public CanvasHoverMessage(
      ItemCanvas ItemCanvas, Point HoverPos)
    {
      this.ItemCanvas = ItemCanvas;
      this.HoverPos = HoverPos;
    }
  }
}
