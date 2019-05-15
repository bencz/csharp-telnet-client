using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Header
{
  public class TerminalDataStreamHeader : DataStreamHeader
  {
    public TerminalDataStreamHeader(InputByteArray InputArray)
      : base(InputArray, "TerminalDataStreamHeader")
    {
      // header length should be at least 10 bytes.
      if ((this.Errmsg == null) && (this.HeaderLength < 10))
        this.Errmsg = "Invalid header length";

      if (this.Errmsg == null)
      {
        this.Flags = this.RawBytes.SubArray(7, 2);  // flags 16 bits. see page 3 of rfc 1205

        // data stream opcode. page 4, rfc 1205
        this.OpcodeByte = this.RawBytes[9];
        var termCode = this.OpcodeByte.ToTerminalOpcode();

        if (termCode == null)
          this.Errmsg = "invalid data stream opcode";
        else if (this.VariableLength != 4)
          this.Errmsg = "invalid variable header length";
        else
        {
          this.TerminalOpcode = termCode.Value;
        }
      }
    }
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("DisplayDataStreamHeader. ");
      if (this.Errmsg != null)
        sb.Append(this.Errmsg);
      else
      {
        sb.Append("RcdLgth:" + this.DataStreamLength.ToString() + " ");
        sb.Append("RcdType:" + this.Marker.ToHex() + " ");
        sb.Append("VarHdr length:" + this.VariableLength.ToString() + " ");
        sb.Append("Flags:" + this.Flags.ToHex() + " ");
        sb.Append("Opcode:" + this.OpcodeByte.ToHex() + " " + this.TerminalOpcode.ToString());
      }

      return sb.ToString();
    }


    /// <summary>
    /// report the contents of the ResponseHeader.
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> lines = new List<string>();

      if (Title != null)
        lines.Add(Title);
      else
        lines.Add(this.ReportTitle);

      lines.Add(ToColumnReportHeaderLine());
      lines.Add(ToColumnReportLine());

      return lines;
    }

    public new static string ToColumnReportHeaderLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append("RcdLgth", 7);
      lb.Append("Marker", 6);
      lb.Append("StreamCode", 15);
      lb.Append("VarLgth", 7);
      lb.Append("Flags", 6);
      lb.Append("Opcode", 10);
      return lb.ToString();
    }

    // todo tomorrow:
    // create showLiteralItem from RA order
    // complete the report of save screen response data screen.

    public new string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.DataStreamLength.ToString(), 7);
      lb.Append(this.Marker.ToHex(), 6);
      lb.Append(this.StreamCode.Value.ToString(), 15);
      lb.Append(this.VariableLength.ToString(), 7);
      lb.Append(this.Flags.ToHex(), 6);
      lb.Append(this.OpcodeByte.ToHex() + " " + this.TerminalOpcode.ToString(), 10);

      return lb.ToString();
    }

  }
}
