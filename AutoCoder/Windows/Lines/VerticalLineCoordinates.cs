using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace AutoCoder.Windows.Lines
{
  public class VerticalLineCoordinates : LineCoordinates
  {
    public VerticalLineCoordinates(Point Start, double Length)
      : base(Start, new Point(Start.X, CalcEndVert(Start.Y, Length)))
    {
    }

    private static double CalcEndVert(double StartVert, double Length)
    {
      double endVert ;
      if (Length < 0)
        endVert = StartVert + Length + 1.00;
      else
        endVert = StartVert + Length - 1.00;
      return endVert;
    }

  }
}
