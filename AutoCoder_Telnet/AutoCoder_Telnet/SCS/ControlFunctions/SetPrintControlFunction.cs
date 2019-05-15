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
  public class SetPrintControlFunction : VariableLengthControlFunction
  {

    public SetPrintControlFunction(InputByteArray InputArray)
      : base(InputArray, ControlFunctionCode.SetPrint)
    {
    }
  }
}
