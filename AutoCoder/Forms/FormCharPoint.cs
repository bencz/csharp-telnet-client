using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Forms
{
  /// <summary>
  /// zero based x,y location of a character
  /// </summary>
  public class FormCharPoint
  {
    int mx;
    int my;

    public FormCharPoint(int InX, int InY)
    {
      mx = InX;
      my = InY;
    }

    public int X
    {
      get { return mx; }
      set { mx = value; }
    }

    public int Y
    {
      get { return my; }
      set { my = value; }
    }
  }
}
