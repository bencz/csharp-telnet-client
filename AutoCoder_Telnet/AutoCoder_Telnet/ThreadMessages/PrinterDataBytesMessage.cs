using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class PrinterDataBytesMessage : ThreadMessageBase
  {
    public byte[] DataBytes
    { get; set; }

    public PrinterDataBytesMessage(byte[] DataBytes)
    {
      this.DataBytes = DataBytes;
    }
  }
}
