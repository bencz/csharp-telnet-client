using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AutoCoder.Drawing
{
  /// <summary>
  /// The text of a string to draw along with the draw properties and
  /// instructions.
  /// </summary>
  public class DrawString
  {
    string mText = null;
    Nullable<Color> mForeColor = null ;
    Nullable<Color> mBackColor = null;

    public DrawString(string InText)
    {
      mText = InText;
    }

    public Nullable<Color> BackColor
    {
      get { return mBackColor; }
      set { mBackColor = value; }
    }

    public bool BackColorIsAssigned
    {
      get
      {
        if (mBackColor == null)
          return false;
        else
          return true;
      }
    }

    public Nullable<Color> ForeColor
    {
      get { return mForeColor; }
      set { mForeColor = value; }
    }

    public bool ForeColorIsAssigned
    {
      get
      {
        if (mForeColor == null)
          return false;
        else
          return true;
      }
    }

    public string Text
    {
      get { return mText; }
      set { mText = value; }
    }
  }
}
