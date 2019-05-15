using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.LogFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  /// <summary>
  /// ibm 5250 start of header order.
  /// See 15.6.9 of 5494 functions reference manual.
  /// </summary>
  public class StartHeaderOrder : WtdOrderBase
  {
    public byte LengthByte
    { get; set; }

    public byte FlagByte
    { get; set; }

    public byte ReservedByte
    { get; set; }

    public byte FirstResponseFieldNumber
    { get; set; }

    public byte ErrRow
    { get; set; }

    public byte[] CmdKeySwitches
    { get; set; }

    public StartHeaderOrder( 
      byte Flag, byte FirstResponseFieldNumber, byte ErrRow, byte[] CmdKeySwitches)
      : base(WtdOrder.StartHeader)
    {
      this.LengthByte = 7;
      this.FlagByte = Flag;
      this.ReservedByte = 0x00;       
      this.FirstResponseFieldNumber = FirstResponseFieldNumber;
      this.ErrRow = ErrRow;
      this.CmdKeySwitches = CmdKeySwitches;
    }
    public StartHeaderOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.StartHeader)
    {
      // at least 3 bytes remaining. ( command code, length byte and length must be from
      // 1 to 7.
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "SOH not long enough";
      }

      if (this.Errmsg == null)
      {
        var buf2 = InputArray.PeekBytes(2);
        this.LengthByte = buf2[1];

        // invalid start of header order length.
        if ((this.LengthByte < 1) || (this.LengthByte > 7))
        {
          this.Errmsg = "SOH length not valid";
        }
      }

      if (this.Errmsg == null)
      {
        int actualOrderLength = CalcActualOrderLength(this.LengthByte);
        var buf = InputArray.PeekBytesPad(this.BytesLength, 9, 0x00);

        this.FlagByte = buf[2];
        this.ReservedByte = buf[3];
        this.FirstResponseFieldNumber = buf[4];
        this.ErrRow = buf[5];
        this.CmdKeySwitches = buf.SubArray(6, 3);

        // advance current pos in input array by the actual length of the order.
        InputArray.AdvanceIndex(this.BytesLength);
      }
    }

    /// <summary>
    /// calc the actual length of the start of header order in the data stream.
    /// Length is the value of the length byte + 1 for the command code byte + 
    /// 1 for the length byte.
    /// </summary>
    /// <param name="LengthByte"></param>
    /// <returns></returns>
    private int CalcActualOrderLength(byte LengthByte)
    {
      this.BytesLength = 2 + LengthByte;
      return this.BytesLength;
    }

    public static byte[] Build(
      byte Flag, byte FirstResponseFieldNumber, byte ErrRow, byte[] CmdKeySwitches)
    {
      var soh = new StartHeaderOrder(Flag, FirstResponseFieldNumber, ErrRow, CmdKeySwitches);
      var buf = soh.ToBytes();
      return buf;
    }

    public override byte[] ToBytes()
    {
      if (this.OrderBytes != null)
        return this.OrderBytes;
      else
      {
        var ba = new ByteArrayBuilder();
        ba.Append((byte)this.OrderCode.Value);
        ba.Append(this.LengthByte);
        ba.Append(this.FlagByte);
        ba.Append(this.ReservedByte);
        ba.Append(this.FirstResponseFieldNumber);
        ba.Append(this.ErrRow);
        ba.Append(this.CmdKeySwitches);
        return ba.ToByteArray();
      }
    }
    public override string ToString()
    {
      var s1 = OrderCode.Value.ToString() + " order. Length:" + this.LengthByte.ToString() +
        " Flag:" + this.FlagByte.ToHex() +
        " FirstResponseFieldNumber:" + this.FirstResponseFieldNumber.ToString() +
        " ErrRow:" + this.ErrRow.ToString() +
        " Command key switches:" + this.CmdKeySwitches.ToHex();
      return s1;
    }

  }
}
