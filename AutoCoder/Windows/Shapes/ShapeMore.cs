using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Windows.Lines;
using System.Windows;
using AutoCoder.Core.Enums;

namespace AutoCoder.Windows.Shapes
{
  /// <summary>
  /// object that stores more info about a shape.
  /// </summary>
  public class ShapeMore
  {
    public ShapeMore(Shape ActualShape)
    {
      this.ActualShape = ActualShape;
    }

    public Shape ActualShape
    { get; set; }

    public static ShapeMore Construct(Shape ActualShape)
    {
      if (ActualShape is Line)
      {
        var sm = new ShapeMore(ActualShape);
        return sm;
      }
      else if (ActualShape is Rectangle)
      {
        var sm = new RectangleMore(ActualShape as Rectangle);
        return sm;
      }
      else
      {
        throw new ApplicationException("not a supported shape");
      }
    }

    public virtual LineCoordinates DrawLineToOrbitCorner(
      Point Start, WhichDirection Direction)
    {
      throw new ApplicationException(
        "abstract method. should be implemented by derived class.");
    }
  }
}
