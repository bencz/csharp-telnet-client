using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using System.Windows;

namespace AutoCoder.Windows.Lines
{
  /// <summary>
  /// the length and direction of a line
  /// </summary>
  public class LineVector
  {

    public LineVector(double Length, WhichDirection Direction)
    {
      this.Length = Length;
      this.Direction = Direction;
    }

    public double Length
    { get; set; }

    public WhichDirection Direction
    { get; set; }

    public Point EndPoint(Point Start)
    {
      double x, y;
      switch (Direction)
      {
        case WhichDirection.Up:
          x = Start.X;
          y = Start.Y - this.Length + 1.00;
          break;
        case WhichDirection.Down:
          x = Start.X;
          y = Start.Y + this.Length - 1.00;
          break;
        case WhichDirection.Left:
          x = Start.X - this.Length + 1.00;
          y = Start.Y;
          break;
        case WhichDirection.Right:
          x = Start.X + this.Length - 1.00;
          y = Start.Y;
          break;
        default:
          throw new ApplicationException("unsupported direction");
      }
      return new Point(x, y);
    }
  }
}
