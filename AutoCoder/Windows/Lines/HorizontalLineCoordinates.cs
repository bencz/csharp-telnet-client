using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace AutoCoder.Windows.Lines
{
  public class HorizontalLineCoordinates : LineCoordinates
  {
    public HorizontalLineCoordinates(Point Start, double Length)
      : base(Start, new Point(Start.X + Length - 1.00, Start.Y))
    {
    }

  }
}

