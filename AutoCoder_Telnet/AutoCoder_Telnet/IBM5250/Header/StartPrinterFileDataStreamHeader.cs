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
  public class StartPrinterFileDataStreamHeader : PrinterDataStreamHeader
  {
    public StartPrinterFileDataStreamHeader(InputByteArray InputArray)
      : base(InputArray, "StartPrinterFileDataStreamHeader")
    {
      byte[] buf = null;
      buf = InputArray.PeekBytesLenient(20);

      if (InputArray.RemainingLength < 3)
        this.Errmsg = "Not enough bytes for data stream header";

      // a start printer file message.
      if ((this.Errmsg == null) 
        && (buf.SubArray(0, 3).CompareEqual(StartPrinterFileMarkerBytes) == true))
      {
//        buf = InputArray.GetBytes(this.PayloadLength);
      }

      else if (this.Errmsg == null)
      {
        this.Errmsg = "Bytes do not contain StartPrinterFile data stream header";
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("StartPrinterFileDataStreamHeader. ");
      if (this.Errmsg != null)
        sb.Append(this.Errmsg);
      else
      {
        sb.Append("RcdLgth:" + this.DataStreamLength.ToString() + " ");
        sb.Append("RcdType:" + this.Marker.ToHex() + " ");
      }

      return sb.ToString();
    }

  }
}

