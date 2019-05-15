using AutoCoder.Ext.System.Text;
using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  /// <summary>
  /// keyboard input that is to be applied to the ScreenContent block.
  /// </summary>
  public class KeyboardInputMessage : ThreadMessageBase
  {
    public ZeroRowCol RowCol
    { get; set; }

    public System.Windows.Input.Key? KeyCode
    { get; set; }

    public bool? ShiftDown
    { get; set; }
    public string Text
    { get; set; }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("KeyboardInputMessage.");
      if (this.RowCol != null)
        sb.SentenceAppend(this.RowCol.ToString());
      if (this.KeyCode != null)
        sb.SentenceAppend(this.KeyCode.Value.ToString());
      if (this.ShiftDown != null)
        sb.SentenceAppend("ShiftDown:" + this.ShiftDown.Value.ToString());
      if (this.Text != null)
        sb.SentenceAppend("Text:" + this.Text);
      return sb.ToString();
    }
  }
}
