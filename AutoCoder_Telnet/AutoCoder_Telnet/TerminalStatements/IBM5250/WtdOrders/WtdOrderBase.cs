using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250.WtdOrders
{
  public abstract class WtdOrderBase
  {
    public WtdOrder? OrderCode
    { get; set; }

    public WtdOrderBase(WtdOrder? OrderCode)
    {
      this.OrderCode = OrderCode;
    }

    public static WtdOrderBase Factory(InputByteArray InputArray)
    {
      WtdOrderBase orderBase = null;
      var b1 = InputArray.PeekByte(0);
      var wtdOrder = b1.ToWtdOrder();
      if (wtdOrder == null)
        return null;
      else if (wtdOrder.Value == WtdOrder.StartHeader)
      {
        orderBase = new StartHeaderOrder();
      }

      return orderBase;
    }
  }
}
