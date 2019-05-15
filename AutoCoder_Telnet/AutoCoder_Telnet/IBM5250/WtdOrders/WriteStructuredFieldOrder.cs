using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.WtdOrders.StructuredFields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public abstract class WriteStructuredFieldOrder : WtdOrderBase
  {
    public short MajorLength
    { get; set; }
    public byte ClassByte
    { get; set; }
    public byte TypeByte
    { get; set; }
    public byte[] FieldBytes
    { get; set; }

    public WriteStructuredFieldOrder(InputByteArray InputArray)
      : base(InputArray, WtdOrder.WriteStructuredField)
    {
      if (InputArray.RemainingLength < 10)
      {
        this.Errmsg = "Start field order. end of stream.";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(10);

        var wtdOrder = buf[0].ToWtdOrder();
        this.MajorLength = buf.BigEndianBytesToShort(1);

        this.FieldBytes = InputArray.GetBytes(this.MajorLength + 1);

        this.ClassByte = buf[3];
        this.TypeByte = buf[4];

        this.BytesLength += this.MajorLength;
//          InputArray.AdvanceIndex(this.MajorLength + 1);
      }
    }

    public static new WriteStructuredFieldOrder Factory(InputByteArray InputArray)
    {
      WriteStructuredFieldOrder fieldOrder = null;

      var buf = InputArray.PeekBytes(10);

      var wtdOrder = buf[0].ToWtdOrder();
      var majorLength = buf.BigEndianBytesToShort(1);

      var fieldBytes = InputArray.PeekBytes(majorLength + 1);

      var classByte = buf[3];
      var typeByte = buf[4];

      if (classByte == 0xd9)
      {
        if ( typeByte== 0x51)
        {
          fieldOrder = new CreateWindowStructuredField(InputArray);
        }
        else if ( typeByte == 0x60)
        {
          fieldOrder = new DrawGridLinesStructuredField(InputArray);
        }
        else
        {
          throw new Exception("unexpected typeByte");
        }
      }
      return fieldOrder;
    }
  }
}

