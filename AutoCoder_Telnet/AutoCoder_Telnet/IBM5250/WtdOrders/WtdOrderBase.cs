using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public abstract class WtdOrderBase : ParseStreamBase
  {
    /// <summary>
    /// write to display order enum. ( can be null because TextDataOrder does not start
    /// with an explicit order code. )
    /// </summary>
    public WtdOrder? OrderCode
    { get; set; }

    /// <summary>
    /// the bytes from the input stream from which the order was constructed.
    /// </summary>
    public byte[] OrderBytes
    {
      get
      {
        if (this.InputBytes == null)
          return null;
        else if ((this.BytesStart == 0) || (this.BytesLength == 0))
          return new byte[] { };
        else
        {
          // adjust length for now if it exceeds input bytes.
          int lx = this.BytesLength;
          if (this.InputBytes.Length < (this.BytesStart + lx))
            lx = this.InputBytes.Length - this.BytesStart;
          return this.InputBytes.SubArray(this.BytesStart, lx);
        }
      }
    }

    /// <summary>
    /// advance the rowcol caret to the rowCol which follows this order.
    /// </summary>
    /// <param name="Current"></param>
    /// <returns></returns>
    public virtual IRowCol Advance(IRowCol Current)
    {
      return Current;
    }

    /// <summary>
    /// return byte array containing bytes that immediately follow the bytes of this
    /// order in the input byte stream.
    /// </summary>
    /// <param name="Length"></param>
    /// <returns></returns>
    private byte[] GetFollowingBytes(int Length)
    {
      int bx = this.BytesStart + this.BytesLength;
      return this.InputBytes.SubArrayLenient(bx, Length);
    }

    public WtdOrderBase(WtdOrder? OrderCode)
      : base( )
    {
      this.OrderCode = OrderCode;
    }
    public WtdOrderBase(InputByteArray InputArray, WtdOrder? OrderCode)
      : base(InputArray, OrderCode == null ? "" : OrderCode.Value.ToItemName( ))
    {
      this.OrderCode = OrderCode;
      if ( this.OrderCode != null )
        this.BytesLength = 1;
    }

    public WtdOrderBase(InputByteArray InputArray, string ItemName )
      : base(InputArray, ItemName )
    {
      this.OrderCode = null;
    }

    public WtdOrderBase( )
      : base( )
    {
    }

    public static WtdOrderBase Factory(InputByteArray InputArray)
    {
      WtdOrderBase orderBase = null;
      var b1 = InputArray.PeekByte(0);
      var wtdOrder = b1.ToWtdOrder();
      if (wtdOrder == null)
        return null;

      if (wtdOrder.Value == WtdOrder.StartHeader)
        orderBase = new StartHeaderOrder(InputArray);

      else if (wtdOrder.Value == WtdOrder.SetBufferAddress)
      {
        orderBase = new SetBufferAddressOrder(InputArray);
      }

      else if (wtdOrder.Value == WtdOrder.RepeatToAddress)
      {
        orderBase = new RepeatToAddressOrder(InputArray);
      }

      else if (wtdOrder.Value == WtdOrder.TransparentData)
      {
        orderBase = new TransparentDataOrder(InputArray);
      }

      else if (wtdOrder.Value == WtdOrder.StartField)
      {
        orderBase = new StartFieldOrder(InputArray);
      }

      else if (wtdOrder.Value == WtdOrder.InsertCursor)
      {
        orderBase = new InsertCursorOrder(InputArray);
      }
      else if (wtdOrder.Value == WtdOrder.WriteStructuredField)
      {
        orderBase = WriteStructuredFieldOrder.Factory(InputArray);
      }
      else if(wtdOrder.Value == WtdOrder.EraseToAddress)
      {
        orderBase = new EraseToAddressOrder(InputArray);
      }

      return orderBase;
    }

    public int GetDataStreamLength()
    {
      return this.BytesLength;
    }
    public abstract override string ToString();

    public virtual string ToString(IRowCol StartRowCol )
    {
      return StartRowCol.ToText() + " " + this.ToString();
    }

    public virtual byte[] ToBytes()
    {
      if (this.OrderBytes != null)
        return this.OrderBytes;
      else
      {
        var ba = new ByteArrayBuilder();
        ba.Append((byte)this.OrderCode.Value);
        return ba.ToByteArray();
      }
    }

  }
}
