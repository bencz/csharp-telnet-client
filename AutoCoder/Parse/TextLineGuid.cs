using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Parse
{
  public class TextLineGuid
  {
    string mLineGuid;

    public TextLineGuid()
    {
      mLineGuid = Guid.NewGuid().ToString();
    }

    public TextLineGuid(string InLineGuid)
    {
      mLineGuid = InLineGuid;
    }

    public TextLineGuid(TextLineGuid InLineGuid)
    {
      mLineGuid = InLineGuid.ToString();
    }

    public override string ToString( )
    {
      return mLineGuid ; 
    }
  }
}
