using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Systm
{
  public class CharPoint
  {
    public int X;
    public int Y;

    public CharPoint( )
    {
      X = 0;
      Y = 0;
    }

    public CharPoint(int x, int y)
    {
      X = x;
      Y = y;
    }

    public override string ToString()
    {
      return "CharPoint " + X + "," + Y;
    }
  }
}
