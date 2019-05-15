using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.ScreenDm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders.StructuredFields
{
  public class CreateWindowStructuredField : WriteStructuredFieldOrder
  {
    public byte FlagByte1
    { get; set; }
    public byte FlagByte2
    { get; set; }
    public int NumRow
    { get; set; }
    public int NumCol
    { get; set; }

    public CreateWindowStructuredField(InputByteArray InputArray)
      : base(InputArray)
    {
      this.FlagByte1 = this.FieldBytes[5];
      this.FlagByte2 = this.FieldBytes[6];
      this.NumRow = (int)this.FieldBytes[8];
      this.NumCol = (int)this.FieldBytes[9];
    }
    public ScreenDim WindowDim
    {
      get { return new ScreenDim(NumRow, NumCol); }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(OrderCode.Value.ToString() + " order.");

      sb.Append(" Major lgth:" + this.MajorLength);
      sb.Append(" Class:" + this.ClassByte.ToHex());
      sb.Append(" Type:" + this.TypeByte.ToHex());
      sb.Append(" NumRow:" + this.NumRow + " NumCol:" + this.NumCol);

      return sb.ToString();
    }
    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)this.OrderCode.Value);
      ba.AppendBigEndianShort(this.MajorLength);
      ba.Append(this.ClassByte);
      ba.Append(this.TypeByte);

      return ba.ToByteArray();
    }
  }
}
