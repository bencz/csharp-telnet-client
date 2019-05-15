using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Systm;
using System;
using System.Text;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.CodedBytes;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  /// <summary>
  /// ibm 5250 start of field order.
  /// See 15.6.12 of 5494 functions reference manual.
  /// </summary>
  public class StartFieldOrder : WtdOrderBase
  {
    public ContinuedFieldSegmentCode? ContinuedFieldSegmentCode
    {
      get
      {
        return this.FCW_Bytes.ToContinuedFieldSegmentCode();
      }
    }

    public byte[] FFW_Bytes
    { get; set; }
    public bool IsBypass
    {
      get
      {
        return FieldFormatWord.IsBypass(this.FFW_Bytes);
      }
    }

    /// <summary>
    /// translate to uppercase. bin 10 of FFW is set to '1'.
    /// </summary>
    public bool IsMonocase
    {
      get
      {
        return FieldFormatWord.IsMonocase(FFW_Bytes);
      }
    }

    public bool IsNonDisplay
    {
      get
      {
        if ((this.AttrByte & 0x07) == 0x07)
        {
          return true;
        }
        else
          return false;
      }
    }

    public byte[] FCW_Bytes
    { get; set; }

    public byte AttrByte
    { get; set; }

    public short LL_Length
    { get; set; }

    private string DebugText
    { get; set; }

    public StartFieldOrder( byte FFW1, byte FFW2, byte AttrByte, int Length )
      : base(WtdOrder.StartField)
    {
      this.FFW_Bytes = new byte[] { FFW1, FFW2 };
      this.AttrByte = AttrByte;
      this.LL_Length = (short) Length;
    }

    public StartFieldOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.StartField)
    {
      if (InputArray.RemainingLength < 8)
      {
        this.Errmsg = "Start field order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(8);

        var wtdOrder = buf[0].ToWtdOrder();

        this.FFW_Bytes = buf.SubArray(1, 2);
        this.FCW_Bytes = null;

        // fcw may or may not be present.
        if (buf[3] >= 0x80)
        {
          this.FCW_Bytes = buf.SubArray(3, 2);
          this.AttrByte = buf[5];
          this.LL_Length = buf.BigEndianBytesToShort(6);
          this.BytesLength += 7;
          InputArray.AdvanceIndex(8);
        }
        else
        {
          this.AttrByte = buf[3];
          this.LL_Length = buf.BigEndianBytesToShort(4);
          this.BytesLength += 5;
          InputArray.AdvanceIndex(6);
        }
      }
    }

    public override IRowCol Advance(IRowCol Current)
    {
      var nextRowCol = Current.AdvanceRight();
      return nextRowCol;
    }

    /// <summary>
    /// build a byte array containing a start of field order.
    /// </summary>
    /// <param name="FFW1"></param>
    /// <param name="FFW2"></param>
    /// <param name="AttrByte"></param>
    /// <param name="Length"></param>
    /// <returns></returns>
    public static byte[] Build(byte FFW1, byte FFW2, byte AttrByte, int Length)
    {
      var sof = new StartFieldOrder(FFW1, FFW2, AttrByte, Length);
      var buf = sof.ToBytes();
      return buf;
    }

    /// <summary>
    /// calc and return the show rowCol of this item.
    /// </summary>
    /// <param name="ItemRowCol"></param>
    /// <returns></returns>
    public IRowCol ShowRowCol(IRowCol ItemRowCol)
    {
      IRowCol showRowCol = null;
      showRowCol = ItemRowCol.AdvanceRight();
      return showRowCol;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(OrderCode.Value.ToString() + " order.");

      sb.Append(" FFW:" + FieldFormatWord.ToString(this.FFW_Bytes)) ;

      if (this.FCW_Bytes == null)
        sb.Append(" FCW: null");
      else
        sb.Append(" FCW:" + this.FCW_Bytes.ToHex());

      sb.Append( " AttrByte:" + this.AttrByte.ToHex( ) +
        " LL:" + this.LL_Length.ToString()) ;

      sb.Append(" Bytes:" + this.ToBytes().ToHex(' '));

      if (this.DebugText != null)
        sb.Append(Environment.NewLine + "DebugText:" + this.DebugText);

      return sb.ToString();
    }

    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)this.OrderCode.Value);
      ba.Append(this.FFW_Bytes);
      if (this.FCW_Bytes != null)
        ba.Append(this.FCW_Bytes);
      ba.Append(this.AttrByte);
      ba.AppendBigEndianShort(this.LL_Length);

      return ba.ToByteArray();
    }
  }
}
