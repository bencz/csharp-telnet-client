using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCoder.Telnet.Common.ScreenDm
{
  /// <summary>
  /// row and column dimensions of the text screen.
  /// </summary>
  public class ScreenDim : IScreenDim
  {
    public int Height
    { get; set; }

    public int Width
    { get; set; }

    public ScreenDim()
    {
      this.Height = 0;
      this.Width = 0;
    }

    public ScreenDim(int Height, int Width)
    {
      this.Height = Height;
      this.Width = Width;
    }

#if skip
    /// <summary>
    /// screen dimensions are either wide screen, 27x132 or not 24x80.
    /// </summary>
    public bool IsWideScreen
    {
      get { return ((this.Height == 27) && (this.Width == 132)); }

      set
      {
        if ( value == true )
        {
          this.Height = 27;
          this.Width = 132;
        }
        else
        {
          this.Height = 24;
          this.Width = 80;
        }
      }
    }
#endif

#if skip
    public Size ToCanvasDim(Size charBoxDim)
    {
      var x = charBoxDim.Width * this.Width;
      var y = charBoxDim.Height * this.Height;
      return new Size(x, y);
    }
#endif

    public override string ToString()
    {
      return "ScreenDim. Height:" + this.Height + " Width:" + this.Width;
    }
  }
}
