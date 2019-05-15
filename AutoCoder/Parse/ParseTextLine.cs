using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Parse
{
  public class ParseTextLine
  {
    string mLineText;
    TextLineGuid mLineGuid;

    public ParseTextLine(string InLineText, TextLineGuid InLineGuid)
    {
      mLineText = InLineText;
      mLineGuid = InLineGuid;
    }

    public TextLineGuid LineGuid
    {
      get { return mLineGuid; }
      set { mLineGuid = value; }
    }

    public string LineText
    {
      get { return mLineText; }
      set { mLineText = value; }
    }
  }
}
