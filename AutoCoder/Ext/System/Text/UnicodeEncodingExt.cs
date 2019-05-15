using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;

namespace AutoCoder.Ext.System.Text
{
  public static class UnicodeEncodingExt
  {
    public static SecureString GetSecureString(
      this UnicodeEncoding Encoding, byte[] Bytes )
    {
      string s1 = Encoding.GetString(Bytes);
      SecureString secstr = s1.ToSecureString();
      return secstr;
    }
  }
}
