﻿using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Commands;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.IBM5250.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  /// <summary>
  /// 04 F3. write structure field. Used as vehicle for the D9 70 5250 query command.
  /// </summary>
  public class WriteStructuredFieldCommand : WorkstationCommandBase
  {
    public int llLength
    { get; set; }

    public byte cField
    { get; set; }

    public byte tField
    { get; set; }

    public byte[] FormatFlags
    { get; set; }

    public WSF_RequestCode? RequestCode
    { get; set; }

    public WriteStructuredFieldCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.WriteStructuredField)
    {
      // advance past the 0x04 and  0xf3.
      InputArray.AdvanceIndex(2);

      // first 2 bytes are the LL length bytes.
      this.llLength = InputArray.PeekBigEndianShort(0);

      // make sure enough length remaining in the input array
      if (this.llLength > InputArray.RemainingLength)
        this.Errmsg = "LL length " + this.llLength.ToString( ) 
          + " exceeds remaining length of input array.";

      if ( this.Errmsg == null )
      {
        this.BytesLength += this.llLength;

        var buf = InputArray.GetBytes(this.llLength);
        this.FormatFlags = null;

        // isolate cField and tField
        this.cField = buf[2];
        this.tField = buf[3];

        // 5250 query. format flags are 1 byte.
        if ( this.cField == 0xd9 )
        {
          if (this.tField == 0x30)
            this.RequestCode = WSF_RequestCode.DefineAuditWindow;
          else if (this.tField == 0x31)
            this.RequestCode = WSF_RequestCode.DefineCmdKeyFunc;
          else if ( this.tField == 0x70 )
          {
            this.FormatFlags = buf.SubArray(4, 1);
            this.RequestCode = WSF_RequestCode.Query5250;
          }
        }
      }
    }

    public override int GetDataStreamLength()
    {
      return 2 + this.llLength;
    }

    public string GetOperationDesc( )
    {
      if (this.RequestCode != null)
        return this.RequestCode.Value.ToString();
      else
        return "c Field:" + this.cField.ToHex() + " t Field:" + this.tField.ToHex();
    }

    public override string ToString()
    {
      var s1 = this.CmdCode.ToString() + " LL length:" + this.llLength + " RequestCode:" +
        this.GetOperationDesc();
      if (this.FormatFlags != null)
        s1 += " Format flags:" + this.FormatFlags.ToHex(' ');
      return s1;
    }
  }
}
