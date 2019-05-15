using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class TelnetDeviceAttrMessage : ThreadMessageBase
  {
    public TypeTelnetDevice TypeDevice
    { get; set; }

    public TelnetDeviceAttrMessage(TypeTelnetDevice TypeDevice )
    {
      this.TypeDevice = TypeDevice;
    }
  }
}
