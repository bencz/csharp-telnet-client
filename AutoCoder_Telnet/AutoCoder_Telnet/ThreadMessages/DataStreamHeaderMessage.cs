using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Header;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class DataStreamHeaderMessage : ThreadMessageBase
  {
    public DataStreamHeader DataStreamHeader
    { get; set; }

    public DataStreamHeaderMessage(DataStreamHeader DataStreamHeader)
    { this.DataStreamHeader = DataStreamHeader; }
  }
}
