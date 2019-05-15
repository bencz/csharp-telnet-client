using System.Collections.Generic;
using AutoCoder.Systm;
using AutoCoder.Telnet.TelnetCommands;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Common;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Header;

namespace AutoCoder.Telnet.IBM5250.Response
{
  public class Query5250Response 
  {
    public string Errmsg
    { get; set; }

    public byte[] RawBytes
    { get; set;  }

    public int Length
    { get; set; }

    public byte cField
    { get; set; }

    public WSF_RequestCode? RequestCode
    { get; set; }
    public byte ResponseByte
    { get; set; }

    public byte[] ControlUnitCode
    { get; set; }
    public byte[] CodeLevel
    { get; set; }

    /// <summary>
    /// 16 bytes of hex 00 that follow code level.
    /// </summary>
    public byte[] Reserve1
    { get; set; }

    public byte WorkstationByte
    { get; set; }
    public string MachineType
    { get; set; }
    public byte KeyboardId
    { get; set; }
    public byte[] SerialNumber
    { get; set; }
    public int MaxInputFields
    { get; set; }
    public byte[] Reserve2
    { get; set;  }
    public byte[] Capabilities
    { get; set; }

    public Query5250Response(InputByteArray InputArray)
    {
      InputByteArray buf = null;
      if (InputArray.RemainingLength < 58)
        this.Errmsg = "Insufficient bytes in byte stream for 5250 query response.";

      if ( this.Errmsg == null )
      {
        this.Length = InputArray.PeekBigEndianShort(0);
        if (InputArray.RemainingLength < this.Length)
          this.Errmsg = "response length exceeds byte stream.";
      }

      if ( this.Errmsg == null )
      {
        this.RawBytes = InputArray.PeekBytes(this.Length);
        buf = new InputByteArray(this.RawBytes);
        buf.AdvanceIndex(2);  // the length
        this.cField = buf.GetByte();
        var tField = buf.GetByte();
        this.RequestCode = tField.ToRequestCode();
        this.ResponseByte = buf.GetByte();   // 0x80 fixed code

        if ((cField != 0xd9) || (this.RequestCode == null) 
          || (this.ResponseByte != 0x80))
          this.Errmsg = "invalid c Field, t Field or response byte in 5250 " 
            + "query response.";
      }

      // isolate other 5250 query response fields.
      if ( this.Errmsg == null )
      {
        this.ControlUnitCode = buf.GetBytes(2);
        this.CodeLevel = buf.GetBytes(3);
        this.Reserve1 = buf.GetBytes(16);

        this.WorkstationByte = buf.GetByte();
        this.MachineType = buf.GetEbcdicBytes(7);
        this.KeyboardId = buf.GetByte();
        buf.AdvanceIndex(2);
        this.SerialNumber = buf.GetBytes(4);
        this.MaxInputFields = buf.GetBigEndianShort();
        this.Reserve2 = buf.GetBytes(3);
        this.Capabilities = buf.GetBytes(5);
      }

      // is a valid query 5250 response byte stream. Advance index of input bytes.
      if (this.Errmsg == null)
        InputArray.AdvanceIndex(this.Length);
    }

    public static byte[] BuildQuery5250Response()
    {
      var ra = new ByteArrayBuilder();

      // data stream header.
      {
        var buf = DataStreamHeader.Build(50, TerminalOpcode.Noop, 0, 0);
        ra.Append(buf);
      }

      // response header.  send the special AID code - 0x88.
      {
        var buf = ResponseHeader.Build(new OneRowCol(0, 0), AidKey.Query5250Reply);
        ra.Append(buf);
      }

      // build the 5250 query response.
      {
        byte byteZero = 0x00;
        ra.AppendBigEndianShort(58);   // LL. total length of structured field.

        // 5250 query response
        ra.Append(new byte[] { 0xd9, (byte) WSF_RequestCode.Query5250, 0x80 });

        ra.Append(new byte[] { 0x06, 0x00 });  // control unit code.

        // set code release level to 030200.  this matches what client access sends.
        // ra.Append(new byte[] { 0x01, 0x03, 0x00 });   // code release level.
        ra.Append(new byte[] { 0x03, 0x02, 0x00 });   // code release level.

        ra.Append(byteZero.Repeat(16));    // 16 bytes of null
        ra.Append(0x01); // 01 - display station
        // ra.Append("3180002".ToEbcdicBytes());     // machine type and model.
        ra.Append("3179002".ToEbcdicBytes());     // machine type and model.
        ra.Append(0x02);                      // keyboard id
        ra.Append(0x00);                      // extended keyboard id
        ra.Append(0x00);                      // reserve
        ra.Append(new byte[] { 0x00, 0x61, 0x50, 0x00 });   // device serial number.
        ra.AppendBigEndianShort(256);         // max number input fields.
        ra.Append(0x00);                      // control unit customization
        ra.Append(new byte[] { 0x00, 0x00 }); // reserved

        // controller/display capability.
        // ra.Append(new byte[] { 0x18, 0x11, 0x00, 0x00, 0x20 });  // 
        ra.Append(new byte[] { 0x7F, 0x11, 0xD0, 0x00, 0x5F });  // 

        ra.Append(byteZero.Repeat(7));        // 7 bytes of null.
      }

      // update length of response data stream.
      {
        var wk = new ByteArrayBuilder();
        wk.AppendBigEndianShort((short)ra.Length);
        ra.CopyToBuffer(wk.ToByteArray(), 0);
      }

      // IAC EOR
      {
        ra.Append(EOR_Command.Build());
      }

      return ra.ToByteArray();
    }
    public override string ToString()
    {
      var s1 = "Query5250Response. ";
      if (this.Errmsg != null)
        s1 = s1 + " " + this.Errmsg;
      else
        s1 = s1 + " RequestCode:" + this.RequestCode.Value.ToString() +
          " Control unit code:" + this.ControlUnitCode.ToHex(' ') +
          " Code level:" + this.CodeLevel.ToHex(' ') +
          " Reserve:" + this.Reserve1.ToHex(' ') +
          " Wrkstn byte:" + this.WorkstationByte.ToHex() +
          " MachineType:" + this.MachineType +
          " Keyboard id:" + this.KeyboardId.ToHex() +
          " Serial number:" + this.SerialNumber.ToHex(' ') +
          " Max input fields:" + this.MaxInputFields +
          " Reserve:" + this.Reserve2.ToHex(' ') +
          " Capabilities:" + this.Capabilities.ToHex(' ');
      return s1;
    }

  /// <summary>
  /// return array of text lines which list the contents of the SetBufferAddress 
  /// order.
  /// </summary>
  /// <returns></returns>
  public string[] ToReportLines()
    {
      List<string> lines = new List<string>();
      lines.Add(this.ToString());
      if (this.RawBytes != null)
      {
        lines.Add("Byte stream. Length:" + this.RawBytes.Length);
        lines.AddRange(this.RawBytes.ToHexReport(16));
      }

      return lines.ToArray();
    }

  }
}


