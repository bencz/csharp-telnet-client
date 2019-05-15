using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public interface IScreenField : IScreenAtomic
  {
    ShowUsage Usage { get; set; }
  }

  public static class IScreenFieldExt
  {
  }
}