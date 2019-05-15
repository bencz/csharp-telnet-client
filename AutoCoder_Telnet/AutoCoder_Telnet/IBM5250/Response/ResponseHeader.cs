using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using System;
using System.Collections.Generic;

namespace AutoCoder.Telnet.IBM5250.Response
{
  public class ResponseHeader : ParseStreamBase, IDataStreamReport
  {
    public byte RowNum
    { get; set; }

    public byte ColNum
    { get; set; }

    public byte AidByte
    { get; set; }

    public AidKey? AidKey
    { get; set; }
    public string ReportTitle
    {
      get { return "ReponseHeader"; }
    }

    public ResponseHeader(InputByteArray InputArray)
      : base(InputArray, "ResponseHeader")
    {
      if (InputArray.RemainingLength < 3)
      {
        this.Errmsg = "input array shortage";
      }

      if (this.Errmsg == null)
      {
        var buf = InputArray.PeekBytes(3);

        this.RowNum = buf[0];
        this.ColNum = buf[1];
        this.AidByte = buf[2];
        this.AidKey = buf[2].ToAidKey();
        this.BytesLength = 3;
        InputArray.AdvanceIndex(3);
      }
    }

    public static Tuple<bool,string> IsResponseHeader( InputByteArray InputArray)
    {
      bool isHeader = true;
      string errmsg = null;

      if (InputArray.RemainingLength < 3)
      {
        errmsg = "input array shortage";
        isHeader = false;
      }

      if (errmsg == null)
      {
        var buf = InputArray.PeekBytes(3);

        byte rowNum = buf[0];
        byte colNum = buf[1];
        var aidKey = buf[2].ToAidKey(); ;

        if (rowNum > 27 )
        {
          isHeader = false;
          errmsg = "invalid row number";
        }
        else if (colNum > 132)
        {
          isHeader = false;
          errmsg = "invalid column number";
        }
        else if (aidKey == null)
        {
          isHeader = false;
          errmsg = "invalid aid key byte";
        }
      }
      return new Tuple<bool, string>(isHeader, errmsg);
    }

    public override string ToString()
    {
      string aidText = null;
      if (this.AidKey != null)
        aidText = this.AidKey.Value.ToString();
      else
        aidText = "Invalid aid byte " + this.AidByte.ToHex();

      return "Row:" + RowNum.ToString() + " Column:" + ColNum.ToString()
        + " AID: " + aidText;
    }

    /// <summary>
    /// report the contents of the ResponseHeader.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> lines = new List<string>();

      if (Title != null)
        lines.Add(Title);

      {
        var lb = new BlankFillLineBuilder();
        lb.Append("Cursor row", 12) ;
        lb.Append("Cursor col", 12);
        lb.Append("AID key", 9);
        lines.Add(lb.ToString());
      }

      {
        var lb = new BlankFillLineBuilder();
        lb.Append(this.RowNum.ToString(), 12);
        lb.Append(this.ColNum.ToString(), 12);
        lb.Append(this.AidKey.ToString(), 9);
        lines.Add(lb.ToString());
      }

      return lines;
    }

    public static byte[] Build( OneRowCol RowCol, AidKey AidKey )
    {
      var ba = new ByteArrayBuilder();
      ba.Append((byte)RowCol.RowNum);
      ba.Append((byte)RowCol.ColNum);
      ba.Append((byte)AidKey);

      return ba.ToByteArray();
    }
  }
}

