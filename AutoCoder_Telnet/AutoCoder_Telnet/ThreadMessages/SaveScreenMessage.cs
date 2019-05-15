using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class SaveScreenMessage : ThreadMessageBase
  {
    public ScreenContent ScreenContent
    { get; set; }

    public SaveScreenMessage( ScreenContent ScreenContent)
    {
      this.ScreenContent = ScreenContent;
    }

    public override string ToString()
    {
      return "SaveScreenMessage. " + this.ScreenContent.ToString();
    }
  }
}
