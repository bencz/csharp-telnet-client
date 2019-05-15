using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class SendDataMessage : ThreadMessageBase
  {
    /// <summary>
    /// raw data bytes to send to the server.
    /// </summary>
    public byte[] DataBytes
    { get; set; }
  
    public SendDataMessage(byte[] DataBytes)
    {
      this.DataBytes = DataBytes;
    }

    public override string ToString()
    {
      return "SendDataMessage. DataBytes:" + this.DataBytes.ToHex(' ');
    }
  }

}
