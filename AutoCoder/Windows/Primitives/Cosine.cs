using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Windows.Primitives
{
  public struct Cosine
  {
    private double _Value;
    public double Value
    {
      get { return _Value; }
    }

    public Cosine(double Value)
    {
      _Value = Value;
    }

    public AngleRadians Radians
    {
      get
      {
        var acos = Math.Acos(_Value);
        return new AngleRadians(acos);
      }
    }
  }
}
