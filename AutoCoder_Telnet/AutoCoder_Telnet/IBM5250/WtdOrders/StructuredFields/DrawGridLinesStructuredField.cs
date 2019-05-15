using AutoCoder.Ext.System;
using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders.StructuredFields
{
  public class DrawGridLinesStructuredField : WriteStructuredFieldOrder
  {
    public byte Partition
    { get; set; }
    public byte[] PreProcessFlags
    { get; set; }
    public byte[] PostProcessFlags
    { get; set; }
    public byte DefaultColor
    { get; set; }
    public byte LineStyle
    { get; set; }

    public DrawGridLinesStructuredField(InputByteArray InputArray)
      : base(InputArray)
    {
      this.Partition = this.FieldBytes[5];
      this.PreProcessFlags = this.FieldBytes.SubArray(6, 2);
      this.PostProcessFlags = this.FieldBytes.SubArray(8, 2);
      if (this.MajorLength <= 9)
      {
        this.DefaultColor = 0xff;
        this.LineStyle = 0xff;
      }
      else
      {
        this.DefaultColor = this.FieldBytes[10];
        this.LineStyle = this.FieldBytes[11];
      }
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("DrawGridLinesStructuredField");

      sb.Append(" Major lgth:" + this.MajorLength);
      sb.Append(" Class:" + this.ClassByte.ToHex());
      sb.Append(" Type:" + this.TypeByte.ToHex());

      return sb.ToString();
    }
    public override byte[] ToBytes()
    {
      return this.FieldBytes;
    }
  }
}

