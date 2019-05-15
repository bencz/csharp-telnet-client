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
  public class PresentationPositionControlFunction : ControlFunction
  {
    public PresentationPositionDirection Direction
    { get; set; }

    public byte PositionValue
    { get; set; }

    /// <summary>
    /// the byte count field that follows the function code.
    /// </summary>
    public int ByteCount
    { get; set; }

    public byte[] ParmBytes
    { get; set; }

    public PresentationPositionControlFunction(InputByteArray InputArray)
      : base(InputArray, ControlFunctionCode.PresentationPosition)
    {
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "need 3 bytes";
      }
      else
      {
        var buf = InputArray.PeekBytes(3);

        if (buf[0] == 0x34)
        {
          var dir = buf[1].ToPresentationPositionDirection();
          if ( dir != null)
          {
            this.Direction = dir.Value;
            this.PositionValue = buf[2];
          }
          else
          {
            this.Errmsg = "invalid direction code";
          }
        }
        else
        {
          this.Errmsg = "invalid function code";
        }
      }

      // valid control function. advance in input stream.
      if ( this.Errmsg == null)
      {
        InputArray.AdvanceIndex(3);
      }
    }

    public override string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.ControlCode.ToString(), 15);
      lb.Append(this.Direction.ToString(), 15);
      lb.Append(this.PositionValue.ToString( ), 15);

      return lb.ToString();
    }
  }
}

