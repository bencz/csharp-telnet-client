using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;

namespace AutoCoder.Ext
{
  public static class DrawingContextExt
  {

    public static void DrawBorder(this DrawingContext InContext,
      Point InStart, Size InSize,
      double InThickness)
    {
      Brush fillBrush = null;
      Pen borderPen = new Pen(Brushes.Black, InThickness);
      Rect rect = new Rect(InStart, InSize);
      InContext.DrawRectangle(fillBrush, borderPen, rect);
    }

    public static void DrawBorder(this DrawingContext InContext,
      Point InStart, Size InSize,
      double InThickness,
      Brush InBorderColor)
    {
      Brush fillBrush = null;
      Rect rect = new Rect(InStart, InSize);
      Pen borderPen = new Pen(InBorderColor, InThickness);
      InContext.DrawRectangle(fillBrush, borderPen, rect);
    }


  }
}
