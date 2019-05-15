using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.SCS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.SCS.ControlFunctions 
{
  /// <summary>
  /// a 1 bytes SCS control function. carriage return, new line, ...
  /// </summary>
  public class SingleByteControlFunction : ControlFunction
  {
    public SingleByteControlFunction(
      InputByteArray InputArray, ControlFunctionCode FunctionCode)
      : base(InputArray, FunctionCode )
    {
      InputArray.AdvanceIndex(1);
    }
  }
}
