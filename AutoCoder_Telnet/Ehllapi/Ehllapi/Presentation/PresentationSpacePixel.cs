using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text ;
using AutoCoder.Ehllapi.Common;
using AutoCoder.Ext.System;

namespace AutoCoder.Ehllapi.Presentation
{
  public class PresentationSpacePixel
  {
    int mPixelSx;
    byte mByte1;
    CharAttrByte mCharAttr = null ;
    ColorAttrByte mColorAttr;
    DisplayLocation mLoc;

    public PresentationSpacePixel( byte InByte1 )
    {
      mPixelSx = 1 ;
      mByte1 = InByte1 ;
    }

    public PresentationSpacePixel(
      byte InByte1, CharAttrByte InCharAttr, ColorAttrByte InColorAttr)
    {
      mPixelSx = 2;
      mByte1 = InByte1;
      mCharAttr = InCharAttr;
      mColorAttr = InColorAttr ;
    }

    public byte Byte1
    {
      get { return mByte1; }
    }

    public CharAttrByte CharAttrByte
    {
      get { return mCharAttr; }
      set { mCharAttr = value ; }
    }

    public char? CharValue
    {
      get
      {
        if ((mByte1 >= 128) || (mByte1 < 32))
          return null;
        else
        {
          string s1 = Encoding.ASCII.GetString(new byte[] { mByte1 });
          return s1[0];
        }
      }
    }

    public ColorAttrByte ColorAttrByte
    {
      get { return mColorAttr; }
      set { mColorAttr = value; }
    }

    public DisplayLocation DisplayLocation
    {
      get { return mLoc; }
      set { mLoc = value; }
    }

    public bool IsFieldAttribute
    {
      get
      {
        if ((mByte1 & 0x80) != 0)
          return true;
        else
          return false;
      }
    }

    public string ToDumpString()
    {
      string s1 = Encoding.ASCII.GetString(new byte[] { mByte1}) ;
      string s2 = null ;

      if (mPixelSx == 1)
      {
        s2 = s1 + " " + mByte1.ToHex( ) ;
      }
      else
      {
        s2 = s1 + " " +
          mByte1.ToHex( ) + " " +
          mCharAttr.ToHexString();
      }
      return s2;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if (mLoc != null)
        sb.Append(this.DisplayLocation.ToString());
      char? ch1 = this.CharValue;
      if (ch1 != null)
        sb.SentenceAppend(ch1.Value.ToString());
      return sb.ToString();
    }
  }
}
