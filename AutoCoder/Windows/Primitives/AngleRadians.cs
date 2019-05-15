using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Windows.Primitives
{
  public struct AngleRadians
  {
    private double _Value;
    public double Value
    {
      get { return _Value; }
    }

    public AngleRadians(double Value)
    {
      _Value = Value;
    }

    public static AngleRadians AngleToRadians(double Angle)
    {
      var rads = Angle * (Math.PI / 180);
      return new AngleRadians(rads);
    }

    public double Angle
    {
      get
      {
        double angle = this.Value / (Math.PI / 180);
        return angle;
      }
    }

    public double Acos
    {
      get
      {
        var acos = Math.Acos(_Value);
        return acos;
      }
    }

    public Cosine Cos
    {
      get
      {
        var cos = Math.Cos(_Value);
        return new Cosine(cos);
      }
    }

    public double Sine
    {
      get
      {
        var sin = Math.Sin(_Value);
        return sin;
      }
    }

    public override string ToString()
    {
      return _Value.ToString();
    }
  }
}
