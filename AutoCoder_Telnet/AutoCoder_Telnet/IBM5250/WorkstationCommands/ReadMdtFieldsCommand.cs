using AutoCoder.Core.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Enums.IBM5250;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  public class ReadMdtFieldsCommand : WorkstationCommandBase, IDataStreamReport
  {
    public byte[] ControlChars
    { get; set; }

    public ReadMdtFieldsCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.ReadMdtFields)
    {
      if (InputArray.RemainingLength < 4)
        this.Errmsg = "Byte stream too short. Missing control chars.";

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(4);

        this.ControlChars = buf.SubArray(2, 2);
        this.BytesLength += 2;
        InputArray.AdvanceIndex(this.BytesLength);
      }
    }

    public ReadMdtFieldsCommand( byte ControlChar1 = 0, byte ControlChar2 = 0 )
      : base(WorkstationCode.ReadMdtFields)
    {
      this.ControlChars = new byte[] { ControlChar1, ControlChar2 };
    }

    public override int GetDataStreamLength()
    {
      return 4;
    }
    public override HowReadScreen? GetHowReadScreenCode()
    {
      return HowReadScreen.ReadMdt;
    }
    public override byte[] ToBytes()
    {
      var ba = new ByteArrayBuilder();
      var buf = base.ToBytes();
      ba.Append(buf);
      ba.Append(this.ControlChars);
      return ba.ToByteArray();
    }
    public override string ToString()
    {
      var s1 = base.ToString();
      return s1 + " ControlChars:" + this.ControlChars.ToHex();
    }
    public string ReportTitle
    {
      get
      {
        var titleText = "ReadMdtFields command.";
        return titleText;
      }
    }
    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      var report = new ColumnReport();
      if (Title != null)
        report.WriteTextLine(Title);
      else
        report.WriteTextLine(this.ReportTitle);

      report.AddColDefn("Control Char1", 0, WhichSide.Left);
      report.AddColDefn("Control Char2");

      report.WriteColumnHeading();
      var valueList = new string[]
      {
        this.ControlChars[0].ToHex( ),
        this.ControlChars[1].ToHex( ),
      };
      report.WriteDetail(valueList);
      return report;
    }

  }
}
