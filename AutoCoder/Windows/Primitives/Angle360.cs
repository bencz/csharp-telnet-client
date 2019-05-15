using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Windows.Primitives
{
  /// <summary>
  /// an angle that is expressed in terms of 360 degrees
  /// </summary>
  public struct Angle360
  {
    double _Value;
    public double Value
    { 
      get { return _Value ; } 
    }

    public Angle360(double Value)
    {
      _Value = Value;
    }

    /// <summary>
    /// the value of the angle expressed as radians.
    /// </summary>
    public AngleRadians Radians
    {
      get
      {
        var rads = this.Value * (Math.PI / 180);
        return new AngleRadians(rads);
      }
    }

    /// <summary>
    /// subtract Angle1 from Angle2 
    /// </summary>
    /// <param name="Angle1"></param>
    /// <param name="Angle2"></param>
    /// <returns></returns>
    public static Angle360 Subtract(Angle360 Angle1, Angle360 Angle2)
    {
      double diff ;
      if (Angle1.Value < Angle2.Value)
        diff = Angle2.Value - Angle1.Value;
      else
        diff = (360 + Angle2.Value) - Angle1.Value;
      return new Angle360(diff);
    }

    public override string ToString()
    {
      return _Value.ToString();
    }
  }
}
