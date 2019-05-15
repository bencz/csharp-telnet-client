using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Content;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class SetBufferAddressOrder : WtdOrderBase
  {
    public byte RowNum
    { get; set; }

    public byte ColNum
    { get; set; }

    public OneRowCol GetRowCol( ScreenDim ScreenDim)
    {
      return new OneRowCol(RowNum, ColNum, ScreenDim);
    }

#if skip
    public SetBufferAddressOrder( OneRowCol RowCol)
      : base( WtdOrder.SetBufferAddress)
    {
      this.RowAddress = (byte)RowCol.RowNum;
      this.ColumnAddress = (byte)RowCol.ColNum;
    }
#endif

    public SetBufferAddressOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.SetBufferAddress)
    {
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "SBA order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(3);
        this.RowNum = buf[1];
        this.ColNum = buf[2];

        this.BytesLength += 2;
        InputArray.AdvanceIndex(3);
      }
    }

    public override IRowCol Advance(IRowCol Current)
    {
      return this.GetRowCol(Current.Dim).ToZeroRowCol();
    }

    /// <summary>
    /// build a byte array containing a SBA order.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    /// <returns></returns>
    public static byte[] Build( OneRowCol RowCol )
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)WtdOrder.SetBufferAddress);
      ba.Append(RowCol.RowNum);
      ba.Append(RowCol.ColNum);

      return ba.ToByteArray();
    }

    /// <summary>
    /// check that the current bytes in the input stream contain an SBA order.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static string CheckOrder( InputByteArray InputArray)
    {
      string errmsg = null;
      byte[] buf = null;

      if (InputArray.RemainingLength < 3)
        errmsg = "not enough bytes for SBA order";

      if (errmsg == null)
      {
        buf = InputArray.PeekBytes(3);
        if (buf[0] != (byte)WtdOrder.SetBufferAddress)
        {
          errmsg = "first byte does not contain SBA order code.";
        }
      }

      return errmsg;
    }

    public override byte[] ToBytes()
    {
      if (this.OrderBytes != null)
        return this.OrderBytes;
      else
      {
        var ba = new ByteArrayBuilder();
        ba.Append((byte)this.OrderCode.Value);
        ba.Append(this.RowNum);
        ba.Append(this.ColNum);
        return ba.ToByteArray();
      }
    }

    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + " order. Row:" + this.RowNum 
        + " Col:" + this.ColNum 
        + " Bytes:" + this.ToBytes().ToHex(' ');
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
      lines.Add("SBA Order");
      lines.Add(this.ToHexString());
      lines.Add(this.ToString());

      return lines.ToArray();
    }
  }
}
