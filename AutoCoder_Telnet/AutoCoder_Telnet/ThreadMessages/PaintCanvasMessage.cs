using AutoCoder.Telnet.IBM5250.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  public class PaintCanvasMessage : ThreadMessageBase
  {
    public ScreenContent ScreenContent
    { get; set;  }

    public PaintCanvasMessage(ScreenContent ScreenContent)
    {
      this.ScreenContent = ScreenContent;
    }
  }
}
