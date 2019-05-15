using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.DTE
{
  public class ButtonText
  {
    string mText = null;

    public ButtonText(string InText)
    {
      mText = InText;
    }

    public string Text
    {
      get { return mText; }
      set { mText = value; }
    }

    public override string ToString()
    {
      return mText;
    }
  }
}

