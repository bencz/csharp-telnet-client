using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class GeneralThreadMessage : ThreadMessageBase
  {
    public ThreadMessageCode MessageCode
    { get; set; }

    public ScreenContent ScreenContent
    { get; set; }

    public GeneralThreadMessage( ThreadMessageCode MessageCode )
    {
      this.MessageCode = MessageCode;
    }

    public GeneralThreadMessage(ThreadMessageCode MessageCode, ScreenContent ScreenContent)
    {
      this.MessageCode = MessageCode;
      this.ScreenContent = ScreenContent;
    }
  }
}
