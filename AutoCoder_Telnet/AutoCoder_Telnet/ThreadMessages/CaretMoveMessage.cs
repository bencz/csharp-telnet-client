using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.ThreadMessages
{
  /// <summary>
  /// message sent to Master thread. Caret moved to location on the canvas.
  /// </summary>
  public class CaretMoveMessage : ThreadMessageBase
  {
    public ZeroRowCol RowCol
    { get; set; }

    public override string ToString()
    {
      if (this.RowCol == null)
        return "CaretMove. " + "RowCol:Null";
      else
        return "CaretMove. " + this.RowCol.ToString();
    }
  }
}
