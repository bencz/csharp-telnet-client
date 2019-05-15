using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder
{
  public class AcNamedValue
  {
    string mName;
    string mValue;

    public AcNamedValue()
    {
    }

    public AcNamedValue(string InName, string InValue)
    {
      mName = InName;
      mValue = InValue;
    }

    public string Name
    {
      get { return mName; }
      set { mName = value; }
    }

    public string Value
    {
      get { return mValue; }
      set { mValue = value; }
    }
  }
}
