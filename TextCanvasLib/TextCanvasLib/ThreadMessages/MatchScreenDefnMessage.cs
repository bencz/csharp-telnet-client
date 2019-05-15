using AutoCoder.Telnet.IBM5250.Content;
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
  /// message sent to paint thread. Contains the screenDefn that was matched
  /// to the screen content of the current screen. Paint thread applies the
  /// screen defn to the item canvas.
  /// </summary>
  public class MatchScreenDefnMessage : ThreadMessageBase
  {
    public IScreenDefn ScreenDefn
    { get; set; }

    public ScreenContent ScreenContent
    { get; set;  }

    public MatchScreenDefnMessage(IScreenDefn ScreenDefn, ScreenContent ScreenContent)
    {
      this.ScreenDefn = ScreenDefn;
      this.ScreenContent = ScreenContent;
    }
  }
}
