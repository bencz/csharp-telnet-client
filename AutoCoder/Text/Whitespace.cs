using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Text
{
  /// <summary>
  /// Whitespace characters used in scan operations. 
  /// </summary>
  public class Whitespace
  {
    char[] mWsChars = null;

    public Whitespace(string InChars)
    {
      mWsChars = InChars.ToCharArray();
    }

    public char[] WhitespaceChars
    {
      get { return mWsChars; }
      set { mWsChars = value; }
    }
  }
}
