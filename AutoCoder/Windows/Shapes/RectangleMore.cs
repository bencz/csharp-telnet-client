using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using AutoCoder.Windows.Lines;
using System.Windows;
using AutoCoder.Core.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.Shapes;

namespace AutoCoder.Windows.Shapes
{
  /// <summary>
  /// more info that represents a rectangle shape.
  /// </summary>
  public class RectangleMore : ShapeMore
  {
    public RectangleMore(Rectangle ActualRectangle)
      : base(ActualRectangle)
    {
      this.ActualRectangle = ActualRectangle;
    }

    public Rectangle ActualRectangle
    { get; set; }

    Rect? _BoundedRect;
    public Rect? BoundedRect
    {
      get
      {
        if (_BoundedRect == null)
          _BoundedRect = this.ActualRectangle.GetBoundedRect();
        return _BoundedRect;
      }
    }

    public override LineCoordinates DrawLineToOrbitCorner(
      Point Start, WhichDirection Direction)
    {
      double lgthToOrbit = 30;
      LineCoordinates lineCoor = null;

      switch (Direction)
      {
        case WhichDirection.Left:
          {
            var rect = this.BoundedRect.Value;
            var toX = rect.Left - lgthToOrbit + 1.00;
            var orbitPoint = new Point(toX, Start.Y);
            lineCoor = new LineCoordinates(orbitPoint, Start);
            break;
          }

        case WhichDirection.Right:
          {
            var rect = this.BoundedRect.Value;
            var toX = rect.Right + lgthToOrbit - 1.00;
            var orbitPoint = new Point(toX, Start.Y);
            lineCoor = new LineCoordinates(orbitPoint, Start);
            break;
          }

        case WhichDirection.Up:
          {
            var rect = this.BoundedRect.Value;
            var toY = rect.Top - lgthToOrbit + 1.00;
            var orbitPoint = new Point(Start.X, toY);
            lineCoor = new LineCoordinates(orbitPoint, Start);
            break;
          }

        case WhichDirection.Down:
          {
            var rect = this.BoundedRect.Value;
            var toY = rect.Bottom + lgthToOrbit - 1.00;
            var orbitPoint = new Point(Start.X, toY);
            lineCoor = new LineCoordinates(orbitPoint, Start);
            break;
          }

        default:
          throw new ApplicationException("unhandled direction");
      }

      return lineCoor;
    }
  }
}
