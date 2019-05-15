using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250.WtdOrders
{
  public class StartHeaderOrder : WtdOrderBase
  {
    public StartHeaderOrder()
      : base(WtdOrder.StartHeader)
    {
    }
  }
}
