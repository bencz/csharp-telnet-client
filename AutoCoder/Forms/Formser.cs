using System;
using System.Collections.Generic;
using System.Drawing ;
using System.Text;
using System.Windows.Forms ;

namespace AutoCoder.Forms
{

  /// <summary>
  /// Windows.Forms namespace helper methods
  /// </summary>
  public class Formser
  {

    /// <summary>
    /// Calc the lower left point location of a control
    /// </summary>
    /// <param name="InControl"></param>
    /// <returns></returns>
    public static Point LowerLeftLocation( Control InControl )
    {
      Point lowerLeft = new Point( ) ;
      lowerLeft.X = InControl.Location.X ;
      lowerLeft.Y = InControl.Location.Y + InControl.Height - 1 ;
      return lowerLeft ;
    }

    public static Size CalcRemainingClientSize(
      Size InClientSize, int InUsedHeight, int InUsedWidth)
    {
      Size rem = InClientSize;
      rem.Height -= InUsedHeight;
      rem.Width -= InUsedWidth;
      return rem;
    }
  }
}
