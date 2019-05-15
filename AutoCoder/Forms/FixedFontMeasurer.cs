using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Forms
{
  /// <summary>
  /// class used to calc locations within a fixed font presentation space. 
  /// </summary>
 
  public class FixedFontMeasurer
  {
    int mHeadWidth = 0;
    int mTailWidth = 0;
    int mCharWidth = 0;
    int mCharHeight = 0;

    public FixedFontMeasurer(Graphics InGraphics, Font InFont)
    {
      Size xx = TextRenderer.MeasureText(InGraphics, "a", InFont);
      Size yy = TextRenderer.MeasureText(InGraphics, "aa", InFont);

      mCharWidth = yy.Width - xx.Width;
      
      // the additional whitespace before and after the drawn text string. 
      int ws = xx.Width - mCharWidth;
      int rem;
      mHeadWidth = Math.DivRem(ws, 2, out rem);
      mTailWidth = ws - mHeadWidth;

      mCharHeight = xx.Height;
    }

    public int CharHeight
    {
      get { return mCharHeight; }
    }

    public int CharWidth
    {
      get { return mCharWidth; }
    }

    public int HeadWidth
    {
      get { return mHeadWidth; }
    }

    public int TailWidth
    {
      get { return mTailWidth; }
    }

    public Point CalcLocation(int InColNx, int InRowNx)
    {
      int x = mCharWidth * InColNx ;
      int y = mCharHeight * InRowNx;
      Point pt = new Point( x, y ) ;
      return pt;
    }

    public Point CalcCaretLocation(int InColNx, int InRowNx)
    {
      int x = (mCharWidth * InColNx) + mHeadWidth;
      int y = mCharHeight * InRowNx;
      Point pt = new Point(x, y);
      return pt;
    }

    public Rectangle CalcRectangle(FormCharPoint InLoc, Size InSize)
    {
      Rectangle rect = new Rectangle(
        CalcLocation(InLoc.X, InLoc.Y),
        CalcSize(InSize.Width, InSize.Height));
      return rect;
    }

    public Size CalcSize(int InColSx, int InRowSx)
    {
      int x = (InColSx * mCharWidth) + mHeadWidth + mTailWidth;
      int y = (InRowSx * mCharHeight);
      return new Size(x, y);
    }

    public Size CalcPresentationSpaceSize(int InColSx, int InRowSx)
    {
      int x = (InColSx * mCharWidth) + mHeadWidth + mTailWidth;
      int y = (InRowSx * mCharHeight);
      return new Size(x, y);
    }
  }
}
