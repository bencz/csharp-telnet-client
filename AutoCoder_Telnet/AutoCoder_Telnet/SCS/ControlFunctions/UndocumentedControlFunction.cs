using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.SCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.SCS.ControlFunctions
{
  public class UndocumentedControlFunction : ControlFunction
  {
    public byte[] ParmBytes
    { get; set; }

    public UndocumentedControlFunction(
      InputByteArray InputArray, ControlFunctionCode FunctionCode, int ParmLength)
      : base(InputArray, FunctionCode)
    {
      InputArray.AdvanceIndex(1);

      // isolate the parm bytes.
      if ( ParmLength > 0)
      {
        this.ParmBytes = InputArray.GetBytes(ParmLength);
      }
    }
  }
}
