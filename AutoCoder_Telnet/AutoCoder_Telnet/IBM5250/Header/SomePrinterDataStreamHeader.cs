using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Header
{
  public class SomePrinterDataStreamHeader : PrinterDataStreamHeader
  {
    public byte[] ByteData
    { get; set; }

    public SomePrinterDataStreamHeader(InputByteArray InputArray)
      : base(InputArray, "SomePrinterDataStreamHeader")
    {
      // get the remaining bytes of the data stream.
      int remLx = this.DataStreamLength - this.HeaderLength;

      if (remLx <= 0)
        this.Errmsg = "Not enough remaining bytes";

      if (this.Errmsg == null)
      {
        if (InputArray.RemainingLength < remLx)
          this.ByteData = InputArray.GetBytesToEnd();
        else
          this.ByteData = InputArray.GetBytes(remLx);
      }
    }

    public new string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder();
      lb.Append(this.DataStreamLength.ToString(), 9);
      lb.Append(this.Marker.ToHex(), 10);

      return lb.ToString();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("SomePrinterDataStreamHeader. ");
      if (this.Errmsg != null)
        sb.Append(this.Errmsg);
      else
      {
        sb.Append("RcdLgth:" + this.DataStreamLength.ToString());
        sb.Append(" RcdType:" + this.Marker.ToHex());
        sb.Append("Text:" + this.ByteData.EbcdicBytesToPrintableAscii());
      }
      return sb.ToString();
    }
  }
}
