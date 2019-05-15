using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.ScreenDm;
using System.Windows;
using TextCanvasLib.Common;

namespace TextCanvasLib.Canvas
{
  public class CanvasDefn
  {
    // the width and height dimensions of the box in which a character is displayed.
    // This value includes the spacing above and to the right of the character.
    public Size CharBoxDim
    {
      get; private set;
    }
    public Size KernDim
    { get; private set; }

    public FontDefn FontDefn { get; set; }
    public ScreenDim ScreenDim
    { get; set; }

    public CanvasDefn( ScreenDim ScreenDim, double PointSize )
    {
      this.ScreenDim = ScreenDim;
      this.FontDefn = new FontDefn(PointSize);
      ChangeFontSize(PointSize);
    }

    private void CalcCharDim( )
    {
      var dim1 = this.FontDefn.MeasureString("M");

      double ht = dim1.Height * 1.26;
      this.CharBoxDim = new Size(dim1.Width, ht);
      this.KernDim = new Size(CharBoxDim.Width * 0.92, CharBoxDim.Height * 0.92);
    }

    public void ChangeFontSize(double PointSize)
    {
      this.FontDefn.PointSize = PointSize;
      CalcCharDim();
    }
  }
}
