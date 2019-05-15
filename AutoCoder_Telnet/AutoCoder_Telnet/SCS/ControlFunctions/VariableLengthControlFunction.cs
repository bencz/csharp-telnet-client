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
  public abstract class VariableLengthControlFunction : ControlFunction
  {
    /// <summary>
    /// the byte count field that follows the function code.
    /// </summary>
    public int ByteCount
    { get; set; }

    public byte[] ParmBytes
    { get; set; }

    public VariableLengthControlFunction(
      InputByteArray InputArray, ControlFunctionCode ControlCode )
      : base(InputArray, ControlCode)
    {
      // first 3 bytes contain control code and then length byte.
      {
        var buf = InputArray.PeekBytes(3);
        this.ByteCount = buf[2];
      }

      // byte count from 1 to 50
      if ((this.ByteCount < 1) || (this.ByteCount > InputArray.RemainingLength)
          || (this.ByteCount > 20))
      {
        this.Errmsg = "invalid byte count";
      }

      // isolate function parameter bytes.
      if (this.Errmsg == null)
      {
        InputArray.AdvanceIndex(3);
        this.ParmBytes = InputArray.GetBytes(this.ByteCount - 1);
      }
    }

    public override string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.ControlCode.ToString(), 20);
      lb.Append(this.ByteCount.ToString(), 20);
      lb.Append(this.ParmBytes.ToHex(' '), 20);

      return lb.ToString();
    }
  }
}
