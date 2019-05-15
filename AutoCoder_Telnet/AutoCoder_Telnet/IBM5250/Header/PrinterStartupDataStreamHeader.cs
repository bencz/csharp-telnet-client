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
  class PrinterStartupDataStreamHeader : PrinterDataStreamHeader
  {
    public byte[] PrinterFixedValueBytes
    { get; set; }
    public string ResponseCode
    { get; set; }
    public string SystemName
    { get; set; }
    public string PrinterName
    { get; set; }

    public PrinterStartupDataStreamHeader(InputByteArray InputArray)
      : base(InputArray, "PrinterStartupDataStreamHeader")
    {
      byte[] buf = null;

      // a printer starup message.
      if ((this.Errmsg == null) && (this.DataStreamLength == 73)
        && (this.RawBytes.SubArray(4, 7).CompareEqual(PrinterStartupFixedBytes) == true))
      {
        this.PrinterFixedValueBytes = this.RawBytes.SubArray(4, 7);

        buf = InputArray.GetBytes(this.PayloadLength);

        this.ResponseCode = buf.SubArray(5, 4).EbcdicBytesToString();
        this.SystemName = buf.SubArray(9, 8).EbcdicBytesToString();
        this.PrinterName = buf.SubArray(17, 10).EbcdicBytesToString();
      }

      else if (this.Errmsg == null)
      {
        this.Errmsg = "Bytes do not contain PrinterStartup data stream header";
      }
    }

    public override IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> lines = new List<string>();

      lines.AddRange(base.ToColumnReport(Title));

      lines.Add(" ");
      lines.Add(ToColumnReportHeaderLine());
      lines.Add(ToColumnReportLine());

      return lines;
    }

    public static new string ToColumnReportHeaderLine()
    {
      var lb = new BlankFillLineBuilder();
      lb.Append("Response code", 13);
      lb.Append("System name", 11);
      lb.Append("Printer name", 12);
      return lb.ToString();
    }

    public new string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder();
      lb.Append(this.ResponseCode, 13);
      lb.Append(this.SystemName, 11);
      lb.Append(this.PrinterName, 12);

      return lb.ToString();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("PrinterStartupDataStreamHeader. ");
      if (this.Errmsg != null)
        sb.Append(this.Errmsg);
      else 
      {
        sb.Append("RcdLgth:" + this.DataStreamLength.ToString());
        sb.Append(" RcdType:" + this.Marker.ToHex());
        sb.Append(" Response code:" + this.ResponseCode);
        sb.Append(" SystemName:" + this.SystemName);
        sb.Append(" PrinterName:" + this.PrinterName);
      }
      return sb.ToString();
    }
  }
}
