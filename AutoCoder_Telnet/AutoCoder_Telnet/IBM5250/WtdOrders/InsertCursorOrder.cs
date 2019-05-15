using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class InsertCursorOrder : WtdOrderBase
  {
    public byte RowAddress
    { get; set; }

    public byte ColumnAddress
    { get; set; }

    public OneRowCol RowCol
    {
      get
      {
        return new OneRowCol(this.RowAddress, this.ColumnAddress);
      }
    }

    public InsertCursorOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.InsertCursor)
    {
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "Insert cursor order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(3);
        this.RowAddress = buf[1];
        this.ColumnAddress = buf[2];

        this.BytesLength += 2;
        InputArray.AdvanceIndex(3);
      }
    }

    /// <summary>
    /// build a byte array containing an InsertCursor order.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    /// <returns></returns>
    public static byte[] Build(OneRowCol RowCol)
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)WtdOrder.InsertCursor);
      ba.Append(RowCol.RowNum);
      ba.Append(RowCol.ColNum);

      return ba.ToByteArray();
    }

    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + " order. Row:" + this.RowAddress.ToString() +
        " Column:" + this.ColumnAddress.ToString();
      return s1;
    }

    /// <summary>
    /// return array of text lines which list the contents of the SetBufferAddress 
    /// order.
    /// </summary>
    /// <returns></returns>
    public override string[] ToReportLines()
    {
      List<string> lines = new List<string>();
      lines.Add("InsertCursor Order");
      lines.Add(this.ToHexString());
      lines.Add(this.ToString());

      return lines.ToArray();
    }
  }
}
