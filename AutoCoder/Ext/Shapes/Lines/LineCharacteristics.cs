using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;

namespace AutoCoder.Ext.Shapes.LineClasses
{
  public class LineCharacteristics
  {

    public LineCharacteristics()
    {
      this.Stroke = Brushes.Black;
      this.Fill = Brushes.Black;
      this.StrokeLineJoin = PenLineJoin.Bevel;
      this.StrokeThickness = 2;
    }

    public Brush Fill
    { get; set; }

    public Brush Stroke
    { get; set; }

    public PenLineJoin StrokeLineJoin
    { get; set; }

    public double StrokeThickness
    { get; set; }

    public void demo( )
    {
      Line newLine = new Line();
      newLine.Stroke = Brushes.Black;
      newLine.Fill = Brushes.Black;
      newLine.StrokeLineJoin = PenLineJoin.Bevel;
      newLine.StrokeThickness = 2;
    }
  }
}
