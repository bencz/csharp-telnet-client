using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class RepeatToAddressOrder : WtdOrderBase
  {
    public byte RowAddress
    { get; set; }

    public byte ColumnAddress
    { get; set; }

    /// <summary>
    /// row/column address to repeat the text byte up to and including.
    /// </summary>
    public OneRowCol RowCol
    {
      get
      {
        return new OneRowCol(this.RowAddress, this.ColumnAddress);
      }
    }

    public char RepeatChar
    {
      get
      {
        byte[] buf = new byte[] { this.RepeatTextByte };
        System.Text.Encoding encoding =
          System.Text.Encoding.GetEncoding(37); // 37 = ebcdic
        var chBuf = encoding.GetChars(buf);
        return chBuf[0];
      }
    }

    /// <summary>
    /// calc the length of the report order. Calc as the difference between input 
    /// from pos and repeat until location.
    /// </summary>
    /// <param name="FromRowCol"></param>
    /// <returns></returns>
    public int RepeatLength(IRowCol FromRowCol)
    {
      var fromRowCol = FromRowCol.ToOneRowCol();
      var toRowCol = this.RowCol.ToOneRowCol();
      int repeatLength = fromRowCol.DistanceInclusive(toRowCol);
      return repeatLength;
    }

    /// <summary>
    /// the repeat text. The repeat character that is duplicated for the length of 
    /// the repeat length.
    /// </summary>
    /// <param name="FromRowCol"></param>
    /// <returns></returns>
    public string RepeatShowText( IRowCol FromRowCol )
    {
      var lx = RepeatLength(FromRowCol);
      var s1 = new string(this.RepeatShowChar, lx);
      return s1;
    }
    public char RepeatShowChar
    {
      get
      {
        byte[] buf = new byte[] { this.RepeatTextByte };
        if (buf[0] == 0x00)
        {
          buf[0] = 0x40;
        }
        var s1 = buf.FromEbcdic();
        return s1[0];
      }
    }

    public byte RepeatTextByte
    { get; set; }


    /// <summary>
    /// the repeat byte converted from ebcdic to ascii. In the case the value is
    /// unprintable ( hex 00 ) convert to hex external form.
    /// </summary>
    public string RepeatPrintableChar
    {
      get
      {
        byte[] ba = new byte[] { this.RepeatTextByte };
        var s1 = ba.EbcdicBytesToPrintableAscii();
        return s1;
      }
    }

    public RepeatToAddressOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.RepeatToAddress)
    {
      if (InputArray.RemainingLength < 4)
      {
        this.Errmsg = "Repeat to address order. end of stream.";
      }

      if (this.Errmsg == null)
      {

        var buf = InputArray.PeekBytes(4);
        this.RowAddress = buf[1];
        this.ColumnAddress = buf[2];

        // the repeat character.
        this.RepeatTextByte = buf[3];

        this.BytesLength += 3;
        InputArray.AdvanceIndex(this.BytesLength);
      }
    }

    public override IRowCol Advance(IRowCol Current)
    {
      var toRowCol = this.RowCol.ToZeroRowCol();
      var nextRowCol = toRowCol.AdvanceRight();
      return nextRowCol;
    }

    public static byte[] Build(byte RepeatTextByte, OneRowCol ToRowCol)
    {
      var ba = new byte[4];
      ba[0] = (byte)WtdOrder.RepeatToAddress;
      ba[1] = RepeatTextByte;
      ba[2] = (byte)ToRowCol.RowNum;
      ba[3] = (byte)ToRowCol.ColNum;
      return ba;
    }

    /// <summary>
    /// the repeat byte, as received from server, repeated for the specified 
    /// length.
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    public byte[] GetRepeatTextBytes( int Length )
    {
      return this.RepeatTextByte.Repeat(Length);
    }

    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)this.OrderCode.Value);
      ba.Append(this.RowAddress);
      ba.Append(this.ColumnAddress);
      ba.Append(this.RepeatTextByte);
      return ba.ToByteArray();
    }

    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + " order. RowCol:" + this.RowCol.ToText() +
        " Char:" + this.RepeatPrintableChar + " Bytes:" + this.ToBytes().ToHex(' ');
      return s1;
    }

  }
}
