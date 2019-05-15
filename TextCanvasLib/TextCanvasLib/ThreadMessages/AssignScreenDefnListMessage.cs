using AutoCoder.Telnet.ThreadMessages;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TextCanvasLib.Canvas;

namespace TextCanvasLib.ThreadMessages
{
  /// <summary>
  /// assign the ScreenDefnList to the match thread. When the match thread
  /// matches a screen content to the screen defn it will search this list.
  /// </summary>
  public class AssignScreenDefnListMessage : ThreadMessageBase
  {
    public ScreenDefnList ScreenDefnList
    { get; set; }

    public AssignScreenDefnListMessage(ScreenDefnList ScreenDefnList)
    {
      this.ScreenDefnList = ScreenDefnList;
    }
  }
}
