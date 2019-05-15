using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.WebControls
{

  /// <summary>
  /// a css class name.
  /// </summary>
  public class CssStyle
  {
    string mStyleText = null;

    public CssStyle(string InStyleText)
    {
      mStyleText = InStyleText;
    }

    public string StyleText
    {
      get { return mStyleText; }
      set { mStyleText = value; }
    }
  }
}
