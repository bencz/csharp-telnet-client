using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.SCS;
using AutoCoder.Telnet.IBM5250.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.SCS.ControlFunctions
{
  public class ControlFunction : ParseStreamBase
  {
    public ControlFunctionCode ControlCode
    { get; set; }

    public ControlFunction(InputByteArray InputArray, ControlFunctionCode ControlCode)
      : base(InputArray)
    {
      this.ControlCode = ControlCode;
    }

    static ControlFunctionCode[] SimpleFunctionCodes = 
      {
      ControlFunctionCode.Backspace, ControlFunctionCode.Bell,
      ControlFunctionCode.CarriageReturn, ControlFunctionCode.FormFeed,
      ControlFunctionCode.HorizontalTab, ControlFunctionCode.LineFeed,
      ControlFunctionCode.NewLine, ControlFunctionCode.Null
    };

    public static ControlFunction Factory(InputByteArray InputArray)
    {
      ControlFunction func = null;

      // isolate the function code.
      ControlFunctionCode? nullableFuncCode = null;
      int length = 0;
      {
        var buf = InputArray.PeekBytesLenient(5);
        var rv = buf.ToControlFunctionCode();
        nullableFuncCode = rv.Item1;
        length = rv.Item2;
      }

      if (nullableFuncCode != null)
      {
        var funcCode = nullableFuncCode.Value;
        if ( funcCode == ControlFunctionCode.Null)
        {
          func = new NullControlFunction(InputArray);
        }
        else if (Array.IndexOf(SimpleFunctionCodes, funcCode) != -1)
        {
          func = new SingleByteControlFunction(InputArray, funcCode);
        }
        else if ( funcCode == ControlFunctionCode.Text)
        {
          func = new TextControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.PresentationPosition)
        {
          func = new PresentationPositionControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.SetPrint)
        {
          func = new SetPrintControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.SetUndocumented)
        {
          func = new SetUndocumentedControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.SetTranslation)
        {
          func = new SetTranslationControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.SetGraphicError)
        {
          func = new SetGraphicErrorControlFunction(InputArray);
        }
        else if (funcCode == ControlFunctionCode.Undocumented1)
        {
          func = new UndocumentedControlFunction(
            InputArray,ControlFunctionCode.Undocumented1, 1);
        }
        else if (funcCode == ControlFunctionCode.Undocumented2)
        {
          func = new UndocumentedControlFunction(
            InputArray, ControlFunctionCode.Undocumented2, 0);
        }
        else if (funcCode == ControlFunctionCode.Undocumented3)
        {
          func = new UndocumentedControlFunction(
            InputArray, ControlFunctionCode.Undocumented3, 0);
        }
      }

      return func;
    }

    public virtual string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.ControlCode.ToString(), 20);

      return lb.ToString();
    }
  }
}

