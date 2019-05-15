using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class TransparentDataOrder : WtdOrderBase
  {
    public byte[] LLBytes
    { get; set; }

    public string TransparentData
    { get; set; }

    public TransparentDataOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.TransparentData)
    {
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "Transparent data order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(3);
        this.LLBytes = buf.SubArray(1, 2);

        // this is all wrong. was a bug workaround. should instead process the 
        // LLBytes as a big endian length value.
        //          throw new Exception("TransparentDataOrder order does not handle LL length correctly");

        // advance past order code and LL bytes.
        this.BytesLength += 2;
        InputArray.AdvanceIndex(3);

        // bytes after the LL byte pair are bytes to show on the display ??
        var rv = Common5250.ScanNonTextDataByte(InputArray);
        int textLx = rv.Item2;
        this.BytesLength += textLx;
        this.TransparentData = InputArray.GetEbcdicBytes(textLx);
      }
    }

    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + "order. llBytes:" + this.LLBytes.ToHex( ) +
        " TransparentData:" + this.TransparentData;
      return s1;
    }

  }
}
