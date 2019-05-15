using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Systm;
using AutoCoder.Telnet.Commands;
using AutoCoder.Ext.System;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Telnet.IBM5250.Response;
using AutoCoder.Telnet.IBM5250.Common;

namespace AutoCoder.Telnet.IBM5250.Response
{
  public class Query5250Response 
  {
    public string Errmsg
    { get; set;  }

    public byte[] ResponseCode
    { get; set; }

    public int Length
    { get; set;  }

    public byte[] RawBytes
    { get; set; }

    public Query5250Response(InputByteArray InputArray) 
    {
      InputByteArray buf = null;

      if (InputArray.RemainingLength < 58)
        this.Errmsg = "insufficient remaining length.";

      // isolate length and raw bytes. 
      if (this.Errmsg == null )
      {
        this.Length = InputArray.PeekBigEndianShort(0);
        if (this.Length > InputArray.RemainingLength)
          this.Errmsg = "query 5250 response length exceeds byte stream length";
        else
        {
          this.RawBytes = InputArray.PeekBytes(this.Length);
          buf = new InputByteArray(this.RawBytes);
        }
      }

      // isolate components of the response stream.
      if ( this.Errmsg == null )
      {
        buf.GetBigEndianShort();
        this.ResponseCode = buf.SubArray(2, 3);
      }

      // isolate components of the response stream.
      if (this.Errmsg == null)
      {
        this.ResponseCode = buf.SubArray(2, 3);
      }

      ra.Append(new byte[] { 0xd9, 0x70, 0x80 }); // 5250 query response
      ra.Append(new byte[] { 0x06, 0x00 });  // control unit code.
      ra.Append(new byte[] { 0x01, 0x03, 0x00 });   // code release level.
      ra.Append(byteZero.Repeat(16));    // 16 bytes of null
      ra.Append(0x01); // 01 - display station
      ra.Append("3180002".ToEbcdicBytes());     // machine type and model.
      ra.Append(0x02);                      // keyboard id
      ra.Append(0x00);                      // extended keyboard id
      ra.Append(0x00);                      // reserve
      ra.Append(new byte[] { 0x00, 0x61, 0x50, 0x00 });   // device serial number.
      ra.AppendBigEndianShort(256);         // max number input fields.
      ra.Append(0x00);                      // control unit customization
      ra.Append(new byte[] { 0x00, 0x00 }); // reserved
      ra.Append(new byte[] { 0x18, 0x11, 0x00, 0x00, 0x00 });  // 
      ra.Append(byteZero.Repeat(7));        // 7 bytes of null.

    }

    /// <summary>
    /// build the entire 5250 query response stream. Starting from data stream header
    /// and concluding with IAC EOR.
    /// </summary>
    /// <returns></returns>
    public static byte[] BuildQuery5250Response()
    {
      var ra = new ByteArrayBuilder();

      // data stream header.
      {
        var buf = DataStreamHeader.Build(50, DataStreamOpcode.Noop, 0, 0);
        ra.Append(buf);
      }

      // response header.  send the special AID code - 0x88.
      {
        var buf = ResponseHeader.Build(0, 0, AidKey.Query5250Reply);
        ra.Append(buf);
      }

      // build the 5250 query response.
      {
        byte byteZero = 0x00;
        ra.AppendBigEndianShort(58);   // LL. total length of structured field.
        ra.Append(new byte[] { 0xd9, 0x70, 0x80 }); // 5250 query response
        ra.Append(new byte[] { 0x06, 0x00 });  // control unit code.
        ra.Append(new byte[] { 0x01, 0x03, 0x00 });   // code release level.
        ra.Append(byteZero.Repeat(16));    // 16 bytes of null
        ra.Append(0x01); // 01 - display station
        ra.Append("3180002".ToEbcdicBytes());     // machine type and model.
        ra.Append(0x02);                      // keyboard id
        ra.Append(0x00);                      // extended keyboard id
        ra.Append(0x00);                      // reserve
        ra.Append(new byte[] { 0x00, 0x61, 0x50, 0x00 });   // device serial number.
        ra.AppendBigEndianShort(256);         // max number input fields.
        ra.Append(0x00);                      // control unit customization
        ra.Append(new byte[] { 0x00, 0x00 }); // reserved
        ra.Append(new byte[] { 0x18, 0x11, 0x00, 0x00, 0x00 });  // 
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

  }
}


