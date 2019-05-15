using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  /// <summary>
  /// x and y size in character units.
  /// </summary>
  public class CharSize
  {
    public int X
    { get; set; }

    public int Y
    { get; set; }

    public CharSize()
    {
      this.X = 0;
      this.Y = 0;
    }

    public CharSize(int X, int Y)
    {
      this.X = X;
      this.Y = Y;
    }
  }
}
