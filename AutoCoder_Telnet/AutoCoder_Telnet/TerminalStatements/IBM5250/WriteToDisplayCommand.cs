using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.TerminalStatements.IBM5250.WtdOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250
{
  public class WriteToDisplayCommand : IBM5250DataStreamCommand
  {
    public byte[] ControlChars
    { get; set; }

    public List<WtdOrderBase> OrderList
    { get ; set ; }

    public WriteToDisplayCommand( InputByteArray InputArray, byte[] ControlChars )
      : base(CommandCode.WTD)
    {
      this.ControlChars = ControlChars;
      this.OrderList = new List<WtdOrderBase>();
    }

    public static WriteToDisplayCommand Factory(InputByteArray InputArray)
    {
      WriteToDisplayCommand wtdCmd = null;

      if (InputArray.RemainingLength >= 4)
      {
        var buf = InputArray.PeekBytes(4);
        if ((buf[0] == 0x04) && (buf[1] == 0x11))
        {
          byte[] controlChars = new byte[2];
          Array.Copy(buf, 3, controlChars, 0, 2);

          wtdCmd = new WriteToDisplayCommand(InputArray, controlChars);
          InputArray.AdvanceIndex(4);

          // gather WTD orders and display characters.
          while (true)
          {
            if (InputArray.RemainingLength == 0)
              break;

            var b1 = InputArray.PeekByte(0);
            var wtdOrder = b1.ToWtdOrder();
            if (wtdOrder != null)
            {

            }

            else if (TextDataOrder.IsTextDataChar(b1))
            {
              var tdOrder = TextDataOrder.Factory(InputArray);
              wtdCmd.OrderList.Add(tdOrder);
            }
            else
              break;
          }

        }
      }

      return wtdCmd;
    }
  }
}
