using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using AutoCoder.Ehllapi.Enums;

namespace AutoCoder.Ehllapi.Api.Structs
{

  [StructLayout(LayoutKind.Sequential)]
  public struct SessInfo
  {
    SessName name;
    uint mStatus;

    public char ShortName
    {
      get
      {
        string s1 = Encoding.ASCII.GetString(new byte[] { name.shortName });
        return s1[0];
      }
    }

    public string SessId
    {
      get
      {
        string s1 = Encoding.ASCII.GetString(new byte[] { name.shortName });
        return s1;
      }
    }

    public SessionStatus Status
    {
      get
      {
        return mStatus.ToSessionStatus();
      }
    }
  }

}
