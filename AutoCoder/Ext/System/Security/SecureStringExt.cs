using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.Runtime.InteropServices;

namespace AutoCoder.Ext.System.Security
{
  public static class SecureStringExt
  {

    /// <summary>
    /// Get the actual text contents of the SecureString.
    /// </summary>
    /// <param name="SecText"></param>
    /// <returns></returns>
    public static String GetText(this SecureString SecText)
    {
      string text = null;
      IntPtr ptr = SecureStringToBSTR(SecText);
      text = PtrToStringBSTR(ptr);
      return text;
    }

    private static IntPtr SecureStringToBSTR(SecureString ss)
    {
      IntPtr ptr = new IntPtr();
      ptr = Marshal.SecureStringToBSTR(ss);
      return ptr;
    }

    private static string PtrToStringBSTR(IntPtr ptr)
    {
      string s = Marshal.PtrToStringBSTR(ptr);
      Marshal.ZeroFreeBSTR(ptr);
      return s;
    }  

  }
}
