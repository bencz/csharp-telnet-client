using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi.Common;

namespace AutoCoder.Ehllapi.Presentation
{
  /// <summary>
  /// the lower level contents of the presentation space. The char and attr bytes
  /// returned by ehllapi. And then the PresentationSpacePixels constructed from
  /// those byte arrays.
  /// </summary>
  public class LowPresentationSpace
  {
    byte[] mCharBuf = null;
    byte[] mColorBuf = null;
    PresentationSpaceDim mDim = null;
    DisplayLocation mCursorLocation = null;
    
    // number of bytes per presentation space character.
    int mPixelSx = 1;

    LinkedList<PresentationSpacePixel> mPixels;

    public LowPresentationSpace(
      DisplaySession InSess, byte[] InCharBuf, byte[] InColorBuf, int InPixelSx)
    {
      this.Dim = InSess.Dim;
      mCharBuf = InCharBuf;
      mColorBuf = InColorBuf;
      mPixelSx = InPixelSx;
      LoadPixels( );
      mCursorLocation = InSess.CursorLocation;
    }

    public DisplayLocation CursorLocation
    {
      get { return mCursorLocation; }
    }

    public PresentationSpaceDim Dim
    {
      get { return mDim; }
      set { mDim = value; }
    }

    public LinkedList<PresentationSpacePixel> Pixels
    {
      get
      {
        if (mPixels == null)
          throw new EhllapiExcp("Pixels of presentation space not loaded");
        return mPixels;
      }
    }

    private void LoadPixels()
    {
      if (mCharBuf == null)
        throw new EhllapiExcp("LoadPixels failed. PresentationSpace buffer is empty.");

      mPixels = new LinkedList<PresentationSpacePixel>();
      DisplayLocation loc = new DisplayLocation(1, 1);

      // loop for every character in the character buffer.
      for (int ix = 0; ix < mCharBuf.Length; ix = ix + mPixelSx)
      {

        // create a pixel from the char and color byte buffers.
        PresentationSpacePixel pixel = null;
        if (mPixelSx == 2)
          pixel =
            new PresentationSpacePixel(
            mCharBuf[ix],
            new CharAttrByte(mCharBuf[ix + 1]),
            new ColorAttrByte(mColorBuf[ix + 1]));
        else
          pixel = new PresentationSpacePixel(mCharBuf[ix]);

        pixel.DisplayLocation = loc;
        mPixels.AddLast(pixel);

        // advance to the next display location in the presentation space.
        loc = Dim.IncDisplayLocation(loc);
      }
    }


  }
}
