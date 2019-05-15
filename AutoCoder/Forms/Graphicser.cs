using System;
using System.Collections.Generic;
using System.Drawing ;
using System.Windows.Forms;
using System.Text;

namespace AutoCoder.Forms
{
  public enum AlignPoint { Left, Right, Center, Top, Bottom, None } ;

  public class Graphicser
  {
    public static Rectangle DrawVerticalLine(
      Pen InPen, Graphics InGraphics, Point InFrom, int InLength)
    {
      Point to = new Point(InFrom.X, InFrom.Y + InLength - 1);
      InGraphics.DrawLine(InPen, InFrom, to);
      return new Rectangle(
        InFrom.X, InFrom.Y, (int)InPen.Width, InLength);
    }

    public static Rectangle DrawHorizontalLine(
      Pen InPen, Graphics InGraphics, Point InFrom, int InLength)
    {
      Point to = new Point(InFrom.X + InLength - 1, InFrom.Y);
      InGraphics.DrawLine(InPen, InFrom, to);
      return new Rectangle(
        InFrom.X, InFrom.Y, InLength, (int)InPen.Width);
    }

    public static Rectangle DrawHorizontalText(
      Graphics InGraphics, string InText, Font InFont, Color InColor,
      Point InFrom, int InLength)
    {
      // the rect in which to draw the text.
      Rectangle r1 = new Rectangle(
        InFrom.X, InFrom.Y, InLength, InFont.Height);

      TextRenderer.DrawText(InGraphics, InText, InFont, r1, InColor);
      return r1;
    }

    public static void DrawSolidRectangle(
      Graphics InGraphics, Brush InBrush, Rectangle InRect )
    {
      InGraphics.FillRectangle( InBrush, InRect ) ;
    }

    public static Point SetRelativePoint(
      Nullable<Rectangle> InHrzBase, AlignPoint InHrzAlign, int InHrzPos,
      Nullable<Rectangle> InVrtBase, AlignPoint InVrtAlign, int InVrtPos)
    {
      int x = 0;
      int y = 0;

      if (InHrzAlign == AlignPoint.Left)
        x = InHrzBase.Value.Left + InHrzPos;
      else if (InHrzAlign == AlignPoint.Right)
        x = InHrzBase.Value.Right + InHrzPos;
      else
        x = InHrzPos;

      if (InVrtAlign == AlignPoint.Top)
        y = InVrtBase.Value.Top + InVrtPos;
      else if (InVrtAlign == AlignPoint.Bottom)
        y = InVrtBase.Value.Bottom + InVrtPos;
      else
        y = InVrtPos;

      return new Point(x, y);
    }

    public static Rectangle CalcRelativeHorizontalLine(
      Rectangle InBaseRect,
      AlignPoint InHrzAlign, int InHrzPos,
      AlignPoint InVrtAlign, int InVrtPos,
      int InLineHeight, int? InLineLength)
    {
      int x = 0;
      int y = 0;
      int Lx = 0;

      if (InHrzAlign == AlignPoint.Left)
        x = InBaseRect.Left + InHrzPos;
      else if (InHrzAlign == AlignPoint.Right)
        x = InBaseRect.Right + InHrzPos;
      else
        x = InHrzPos;

      if (InVrtAlign == AlignPoint.Top)
        y = InBaseRect.Top + InVrtPos;
      else if (InVrtAlign == AlignPoint.Bottom)
        y = InBaseRect.Bottom + InVrtPos;
      else
        y = InVrtPos;

      // line length either the width of the baserect or the spcfd value.
      if (InLineLength == null)
        Lx = InBaseRect.Width;
      else
        Lx = InLineLength.Value;

      Point loc = new Point(x, y);
      Size sx = new Size(Lx, InLineHeight);
      return new Rectangle(loc, sx);
    }
  }
}
