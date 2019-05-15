using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class ClearUnitAlternateCommand : WorkstationCommandBase, IDataStreamReport
  {
    /// <summary>
    /// request byte parameter.  0x00 - set screen size to 27x132.
    /// 0x80 - something to do with fax applications.
    /// </summary>
    public byte RequestByte
    { get; set; }

    public ClearUnitAlternateCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.ClearUnitAlternate)
    {
      if (InputArray.RemainingLength < 3)
        this.Errmsg = "too few bytes in input stream";

      if ( this.Errmsg == null)
      {
        InputArray.AdvanceIndex(2);

        this.RequestByte = InputArray.GetByte();
      }
    }
    public ClearUnitAlternateCommand(byte RequestByte = 0x00)
      : base(WorkstationCode.ClearUnitAlternate)
    {
      this.RequestByte = RequestByte;
    }
    public string ReportTitle
    {
      get
      {
        var titleText = "ClearUnitAlternate command.";
        return titleText;
      }
    }

    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      var buf = base.ToBytes();
      ba.Append(buf);
      ba.Append(this.RequestByte);
      return ba.ToByteArray();
    }

    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new List<string>();
      if (Title != null)
        report.Add(Title);
      else
        report.Add(this.ReportTitle);
      return report;
    }
  }
}
