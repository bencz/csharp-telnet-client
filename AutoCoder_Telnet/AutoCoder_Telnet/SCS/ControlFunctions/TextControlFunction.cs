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
  public class TextControlFunction : ControlFunction
  {
    public byte[] TextBytes
    { get; set; }

    public TextControlFunction(InputByteArray InputArray)
      : base(InputArray, ControlFunctionCode.Text)
    {
      var ba = new ByteArrayBuilder();

      // isolate the series of printable bytes.
      while (InputArray.IsEof() == false)
      {
        var b1 = InputArray.PeekByte(0);
        if (Array.IndexOf(ControlFunctionCodeExt.TextBytes, b1) == -1)
          break;

        // is a printable byte. accum to array of text bytes.
        ba.Append(b1);
        InputArray.AdvanceIndex(1);
      }

      this.TextBytes = ba.ToByteArray();
    }

    public override string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.ControlCode.ToString(), 20);
      lb.Append( this.TextBytes.Length.ToString( ) + "  " +
        this.TextBytes.EbcdicBytesToString().Head(60), 60);

      return lb.ToString();
    }

    public override string ToString()
    {
      return "TextControlFunction. " + this.TextBytes.EbcdicBytesToString();
    }

  }
}
