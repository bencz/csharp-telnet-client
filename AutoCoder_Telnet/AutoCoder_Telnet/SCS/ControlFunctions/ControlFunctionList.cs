using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.LogFiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.SCS.ControlFunctions
{
  public class ControlFunctionList : List<ControlFunction>, IDataStreamReport
  {
    public string ReportTitle
    {
      get
      {
        var titleText = "SCS Control Function List.";
        return titleText;
      }
    }

    /// <summary>
    /// the list consists of a single null controlFunction, containing a single
    /// null byte. This server sends such a data steam to mark the end of a print
    /// file data stream.
    /// </summary>
    /// <returns></returns>
    public bool IsSingleNullList( )
    {
      bool isEndOfPrintFile = false;
      if ( this.Count == 1 )
      {
        var func = this.First();
        if ( func is NullControlFunction)
        {
          var nullFunction = func as NullControlFunction;
          if (nullFunction.NullBytes.Length == 1)
          {
            isEndOfPrintFile = true;
          }
        }
      }
      return isEndOfPrintFile;
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ReportList();

      var lines = PrintColumnHeading();
      report.WriteTextLines(lines);

      foreach( var control in this)
      {
        var line = control.ToColumnReportLine();
        report.WriteTextLine(line.TrimEndWhitespace( ));
      }
      return report;
    }

    // BgnTemp
    public static InputByteArray PreviousInputArray
    { get; set; }
    // EndTemp

    /// <summary>
    /// parse the byte array as a data stream containing a sequence of SCS control
    /// functions. ( text data is a 
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static Tuple<ControlFunctionList, string> ParseDataStream(InputByteArray InputArray)
    {
      ControlFunctionList funcList = null;
      string errmsg = null;

      while (InputArray.IsEof() == false)
      {
        // check for IAC EOR
        var telCode = InputArray.PeekTelnetCommandCode(CommandCode.EOR);
        if (telCode != null)
        {
          break;
        }

        var func = ControlFunction.Factory(InputArray);
        if ((func == null) || ( func.Errmsg != null))
        {
          errmsg = "invalid control function. Postion:" + InputArray.Index +
            " invalid bytes:" + InputArray.PeekToEnd().Head(16).ToHex(' ');
          break;
        }

        if (funcList == null)
          funcList = new ControlFunctionList();
        funcList.Add(func);
      }

      return new Tuple<ControlFunctionList, string>(funcList, errmsg);
    }

    public static string[] PrintColumnHeading()
    {
      string line1 = null;

      {
        var lb = new BlankFillLineBuilder(2);
        lb.Append("FuncCode", 20);
        lb.Append("Parm1", 20);
        lb.Append("Parm2", 20);
        line1 = lb.ToString();
      }

      return new string[] { line1 };
    }

  }
}
