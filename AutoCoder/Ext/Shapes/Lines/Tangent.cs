using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.Shapes.LineClasses
{
  /// <summary>
  /// the tangent value of the adjacent side angle of a right triangle.
  /// </summary>
  public struct Tangent
  {
    public double Value;

    public Tangent(double Value)
    {
      this.Value = Value;
    }

    public double CalcAdjacentLgth(double OppLgth)
    {
      double adjLgth ;
      adjLgth = OppLgth / this.Value;
      return adjLgth;
    }

    public double CalcOppositeLgth(double AdjLgth)
    {
      var oppLgth = AdjLgth * this.Value;
      return oppLgth;
    }

    public static Tangent CalcTangent(double AdjLgth, double OppLgth)
    {
      var tan = OppLgth / AdjLgth;
      return new Tangent(tan);
    }

    public static Tangent FromAngle(double Angle)
    {
      var radians = Angle * (Math.PI / 180);
      var tan = Math.Tan(radians);
      return new Tangent(tan);
    }

    public double ToAngle()
    {
      var radians = Math.Atan(this.Value);
      double angle = radians / (Math.PI / 180);
      return angle;
    }

    public override string ToString()
    {
      return this.Value.ToString();
    }
  }
}
