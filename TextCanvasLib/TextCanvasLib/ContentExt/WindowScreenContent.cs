using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.Threads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TextCanvasLib.Canvas;
using TextCanvasLib.Threads;
using TextCanvasLib.Windows;

namespace TextCanvasLib.ContentExt
{
  public class WindowScreenContent : ScreenContent
  {

    /// <summary>
    /// the upper left pos of the window when draw on the canvas of the parent.
    /// ( this is the pos of the border that surrounds the canvas. To cal pos
    /// of the item canvas of the window have to add the dot width of the 
    /// border.
    /// </summary>
    public Point WindowPos
    { get; set;  }

    public WindowScreenContent( 
      ScreenContent Parent, ScreenDim WindowDim, ZeroRowCol RowCol)
      : base(Parent, WindowDim, Parent.ContentOdom)
    {
      this.StartRowCol = RowCol;
    }

    /// <summary>
    /// create this screenContent from the contents of another ScreenContent block
    /// </summary>
    /// <param name="ToParent"></param>
    /// <param name="FromContent"></param>
    public WindowScreenContent( ScreenContent ToParent, WindowScreenContent FromContent)
      : base(ToParent, FromContent)
    {
      this.StartRowCol = FromContent.StartRowCol;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="PointSize"></param>
    /// <param name="MasterThread"></param>
    /// <returns></returns>
    public Tuple<Border,ItemCanvas> CreateWindowedItemCanvas(
      ItemCanvas ParentItemCanvas, 
      double PointSize, MasterThread MasterThread, PaintThread PaintThread,
       int ContentNum)
    {
      Border border = null;
      System.Windows.Controls.Canvas canvas = null;

      border = new Border();
      border.BorderThickness = new Thickness(3);
      border.Margin = new Thickness(5);
      border.BorderBrush = Brushes.Aquamarine;
      border.CornerRadius = new CornerRadius(3);

      canvas = new System.Windows.Controls.Canvas();
      border.Child = canvas;

      var itemCanvas = new ItemCanvas(
        canvas, this.ScreenDim, PointSize, MasterThread, PaintThread, 
        ContentNum);
//      this.ItemCanvas = itemCanvas;

      // add this itemCanvas to the Children of the parent ItemCanvas.
      ParentItemCanvas.AddChild(itemCanvas);

      // store reference to the Border in the ItemCanvas.  Used in the PaintThread
      // when adding the border and its contained canvas to the Telnet canvas.
      itemCanvas.BorderControl = border;

      var boxDim = itemCanvas.CanvasDefn.CharBoxDim;
      var dim = this.ScreenDim.ToCanvasDim(boxDim);
      canvas.Width = dim.Width;
      canvas.Height = dim.Height;
      canvas.Background = Brushes.Black;

      canvas.Focusable = true;
      canvas.IsEnabled = true;
      canvas.Focus();

      return new Tuple<Border,ItemCanvas>( border, itemCanvas);
    }

    public override ScreenContent Copy( ScreenContent ToParent = null)
    {
      var sc = new WindowScreenContent(ToParent, this);
      return sc;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("WindowScreenContent.");
      sb.Append(" Start:" + this.StartRowCol.ToString());
      sb.Append(" Num fields:" + this.FieldDict.Count);
      return sb.ToString();
    }
  }
}
