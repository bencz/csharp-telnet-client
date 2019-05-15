using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.IBM5250.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  /// <summary>
  /// class that encapsulates a SBA order followed by TextDataOrder.  These are the
  /// pair of orders seen in response data streams.
  /// </summary>
  public class LocatedTextDataOrderPair : ParseStreamBase
  {
    public SetBufferAddressOrder SbaOrder
    { get; set; }

    public TextDataOrder TextOrder
    { get; set; }

    /// <summary>
    /// construct the SBA followed by TextData order pair. 
    /// </summary>
    /// <param name="InputArray"></param>
    public LocatedTextDataOrderPair( InputByteArray InputArray)
      : base(InputArray, "LocatedTextDataOrderPair")
    {
      // parse the SBA order.
      this.SbaOrder = new SetBufferAddressOrder(InputArray);
      if (this.SbaOrder.Errmsg != null)
        this.Errmsg = "SetBufferAddress order error.";
      else
      {
        this.BytesLength += this.SbaOrder.GetDataStreamLength();
      }

      // text data order. ( this may be empty. )
      if ( this.Errmsg == null )
      {
        this.TextOrder = new TextDataOrder(InputArray);
        if (this.TextOrder.Errmsg != null)
          this.Errmsg = "Text data order error";
        else
        {
          this.BytesLength += this.TextOrder.GetDataStreamLength();
        }
      }
    }

    public override string ToString()
    {
      var textName = "SBA order / TextDataOrder pair.";
      if (this.Errmsg != null)
        return textName + " " + this.Errmsg;
      else
        return textName;
    }

    /// <summary>
    /// return array of text lines which list the contents of the order pair.
    /// </summary>
    /// <returns></returns>
    public override string[] ToReportLines( )
    {
      List<string> lines = new List<string>();
      lines.Add(this.ToString());
      lines.Add(this.ToHexString());

      if (this.SbaOrder != null)
        lines.AddRange(this.SbaOrder.ToReportLines());

      if (this.TextOrder != null)
        lines.AddRange(this.TextOrder.ToReportLines());

      return lines.ToArray();
    }

    public static string ToColumnReportHeaderLine( )
    {
      var lb = new BlankFillLineBuilder();
      lb.Append("RowNum", 8);
      lb.Append("ColNum", 8);
      lb.Append("AttrByte", 10);
      lb.Append("Length", 8);
      lb.Append("Text data", 30);
      return lb.ToString();
    }
    public string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder();
      lb.Append("  " + SbaOrder.RowNum.ToString(), 8);
      lb.Append("  " + SbaOrder.ColNum.ToString(), 8);

      {
        string s1 = null;
        if (TextOrder.AttrByte == null)
        {
          s1 = "";
        }
        else
          s1 = TextOrder.AttrByte.Value.ToHex();
        lb.Append(s1, 10);
      }

      lb.Append(TextOrder.TextLength.ToString(), 8);
      lb.Append(TextOrder.PrintableText, 30);
      return lb.ToString();
    }
  }
}
