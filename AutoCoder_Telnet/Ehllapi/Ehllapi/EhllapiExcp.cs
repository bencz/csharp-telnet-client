using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Ehllapi
{
  public class EhllapiExcp : ApplicationException
  {
    public EhllapiExcp(string InText)
      : base(InText)
    {
    }
    public EhllapiExcp( string InText, Exception InInner)
      : base(InText, InInner)
    {
    }
  }
}
