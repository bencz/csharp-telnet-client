using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace AutoCoder.Ehllapi.Api.Structs
{
  [StructLayout(LayoutKind.Explicit)]
  public struct SessName
  {
    [FieldOffset(0)]
    public byte shortName;

    [FieldOffset(0)]
    public uint hSess;
  }
}
