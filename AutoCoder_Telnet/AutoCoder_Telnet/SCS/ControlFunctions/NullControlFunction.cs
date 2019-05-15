using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.SCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.SCS.ControlFunctions
{
  public class NullControlFunction : ControlFunction
  {
    public byte[] NullBytes
    { get; set; }

    public NullControlFunction(InputByteArray InputArray)
      : base(InputArray, ControlFunctionCode.Null)
    {
      var ba = new ByteArrayBuilder();

      // isolate the series of null bytes.
      while (InputArray.IsEof() == false)
      {
        var b1 = InputArray.PeekByte(0);
        if ( b1 != 0x00)
          break;

        // accum to array of null bytes.
        ba.Append(b1);
        InputArray.AdvanceIndex(1);
      }

      this.NullBytes = ba.ToByteArray();
    }

    public override string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.ControlCode.ToString(), 20);
      lb.Append(this.NullBytes.Length.ToString( ), 20);
      lb.Append(this.NullBytes.ToHex(' ').Head(40), 40);

      return lb.ToString();
    }
  }
}
