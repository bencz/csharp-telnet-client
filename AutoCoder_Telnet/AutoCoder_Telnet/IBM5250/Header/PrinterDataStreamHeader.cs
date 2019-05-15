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
  /// <summary>
  /// base class for printer data stream headers.
  /// </summary>
  public class PrinterDataStreamHeader : DataStreamHeader
  {
    public PrinterDataStreamHeader(InputByteArray InputArray, string ItemName)
      : base(InputArray, ItemName)
    {
    }
  }
}
