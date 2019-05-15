using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Telnet.Common;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Ext.System;
using AutoCoder.Report;
using AutoCoder.Telnet.IBM5250.Common;

namespace AutoCoder.Telnet.IBM5250.Header
{
  // Data stream header follows telnet handshake commands and preceeds the 5250 data
  // stream.  See RFC1205 for description of data stream header.

  public abstract class DataStreamHeader : ParseStreamBase, IDataStreamReport
  {
    /// <summary>
    /// bytes 0 - 1. Length of the entire data stream. header + the payload that 
    /// follows. Most of the time the length from the start of the DS header up
    /// to the telnet EOR that marks the end of the byte stream sent by the 
    /// client or server.
    /// </summary>
    public int DataStreamLength
    { get; set; }

    /// <summary>
    /// bytes 2 - 3. Data stream header marker. Always 12A0.
    /// </summary>
    public byte[] Marker
    { get; set; }

    /// <summary>
    /// bytes 4 - 5. 
    /// </summary>
    public byte[] StreamCodeBytes
    { get; set; }

    public DataStreamCode? StreamCode
    { get; set; }
   
    /// <summary>
    /// byte 6. Length of bytes of the data stream header itself. Header bytes
    /// include this VariableLength byte. Data stream payload starts at byte 6 + 
    /// VariableLength.
    /// </summary>
    public byte VariableLength
    { get; set; }

    public byte[] Flags
    { get; set; }

    public TerminalOpcode TerminalOpcode
    { get; set; }
    public byte OpcodeByte
    { get; set; }
    public byte[] RawBytes
    { get; set; }
    public int HeaderLength
    {
      get
      {
        return 6 + this.VariableLength;
      }
    }

    /// <summary>
    /// payload is the data bytes that follow the header bytes.
    /// </summary>
    public int PayloadLength
    {
      get
      {
        return this.DataStreamLength - this.HeaderLength;
      }
    }
  
    public string ReportTitle
    {
      get { return ItemName; }
    }
    protected static byte[] PrinterStartupFixedBytes =
      new byte[] { 0x90, 0x00, 0x05, 0x60, 0x06, 0x00, 0x20 };
    protected static byte[] StartPrinterFileMarkerBytes =
      new byte[] { 0x34, 0xc4, 0x01 };

    protected DataStreamHeader(InputByteArray InputArray, string ItemName)
      : base(InputArray, ItemName)
    {
      byte[] buf = null;

      // must be at least 7 bytes in the input stream.
      if (InputArray.RemainingLength < 7)
        this.Errmsg = "Not enough bytes for data stream header";

      // isolate the next 7 bytes.  
      if ( this.Errmsg == null)
      {
        buf = InputArray.PeekBytesLenient(7);

        // the first 6 bytes are logical record length, 2 byte marker and 2 bytes
        // DataStreamCode.
        this.DataStreamLength = buf.BigEndianBytesToShort(0);
        this.Marker = buf.SubArray(2, 2);

        this.StreamCodeBytes = buf.SubArray(4, 2);
        this.StreamCode = StreamCodeBytes.ToDataStreamCode();

        this.VariableLength = buf[6];

        if ((this.Marker[0] != 0x12) || (this.Marker[1] != 0xa0))
          this.Errmsg = "Invalid record type";
        else if (this.DataStreamLength < 7)
          this.Errmsg = "Invalid record length";
        else if (this.VariableLength == 0)
          this.Errmsg = "Invalid variable length";
      }

      if (this.Errmsg == null)
      {
        this.RawBytes = InputArray.GetBytes(this.HeaderLength);
        this.BytesLength = this.HeaderLength;
      }
    }

    public static Tuple<DataStreamHeader,string> Factory( InputByteArray InputArray)
    {
      DataStreamHeader dsh = null;
      string errmsg = null;
      int rcdLgth = 0;
      byte[] buf = null;
      DataStreamCode dataStreamCode = DataStreamCode.Terminal;

      if (InputArray.RemainingLength < 10)
        errmsg = "Not enough bytes for data stream header";

      if (errmsg == null)
      {
        buf = InputArray.PeekBytesLenient(20);

        // the first 6 bytes are logical record length, 2 byte marker and 2 bytes
        // DataStreamCode.
        rcdLgth = buf.BigEndianBytesToShort(0);
        var rcdType = buf.SubArray(2, 2);

        if ((rcdType[0] != 0x12) || (rcdType[1] != 0xa0))
          errmsg = "Invalid record type";
        else if (rcdLgth < 10)
          errmsg = "Invalid record length";
      }

      // isolate the data stream code.
      if (errmsg == null)
      {
        var nullableCode = buf.SubArray(4, 2).ToDataStreamCode();
        if (nullableCode == null)
          errmsg = "not valid data stream code";
        else
          dataStreamCode = nullableCode.Value;
      }

      if ( errmsg == null)
      { 
        if ( dataStreamCode == DataStreamCode.Terminal)
        {
          dsh = new TerminalDataStreamHeader(InputArray);
        }
        else if ( dataStreamCode == DataStreamCode.PrinterStartup)
        {
          dsh = new PrinterStartupDataStreamHeader(InputArray);
        }

        else if ( dataStreamCode == DataStreamCode.PrinterPrint)
        {
          var payloadBx = InputArray.PeekDataStreamHeaderLength();
          var payloadBuf = InputArray.PeekBytesLenient(payloadBx, 3);
          if ( payloadBuf.CompareEqual(StartPrinterFileMarkerBytes) == true )
          {
            dsh = new StartPrinterFileDataStreamHeader(InputArray);
          }
          else
          {
            dsh = new PrinterDataStreamHeader(InputArray, "Printer");
          }
        }
        else if ( dataStreamCode == DataStreamCode.PrinterResponse)
        {
          dsh = new PrinterDataStreamHeader(InputArray, "PrintResponse");
        }
      }

      return new Tuple<DataStreamHeader, string>(dsh, errmsg);
    }

    private static object ToDataStreamCode()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// build a data stream header. Return byte array containing the header bytes.
    /// </summary>
    /// <param name="Length"></param>
    /// <param name="Opcode"></param>
    /// <param name="Flag1"></param>
    /// <param name="Flag2"></param>
    /// <returns></returns>
    public static byte[] Build( 
      int Length, TerminalOpcode Opcode, 
      byte Flag1 = 0x00, byte Flag2 =  0x00)
    {
      var ba = new ByteArrayBuilder();

      ba.AppendBigEndianShort((short)Length);
      ba.Append(0x12);
      ba.Append(0xa0);
      ba.Append( new byte[] { 0x00, 0x00 });
      ba.Append(0x04);
      ba.Append( new byte[] { Flag1, Flag2 });
      ba.Append((byte)Opcode);

      return ba.ToByteArray();
    }

    public override string ToHexString()
    {
      byte[] byteStream = null;
      if (this.RawBytes != null)
        byteStream = this.RawBytes;

      if (byteStream == null)
        return "";
      else
      {
        // split raw bytes into header bytes and following bytes.
        var headerBytes = byteStream.SubArrayLenient(0, 6);
        var followingBytes = byteStream.SubArrayLenient(this.VariableLength);

        var s1 = "Lgth: " + this.HeaderLength + " Bytes: "
            + headerBytes.ToHex(' ') + " Following:" + followingBytes.ToHex(' ') ;
        return s1;
      }
    }

    /// <summary>
    /// report the contents of the ResponseHeader.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<string> ToColumnReport(string Title = null)
    {
      List<string> lines = new List<string>();

      if (Title != null)
        lines.Add(Title);
      else
        lines.Add(this.ReportTitle);

      lines.Add(ToColumnReportHeaderLine());
      lines.Add(ToColumnReportLine());

      return lines;
    }

    public static string ToColumnReportHeaderLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append("RcdLgth", 9);
      lb.Append("Marker", 8);
      lb.Append("StreamCode", 15);
      lb.Append("VarLgth", 7);
      return lb.ToString();
    }

    // todo tomorrow:
    // create showLiteralItem from RA order
    // complete the report of save screen response data screen.

    public string ToColumnReportLine()
    {
      var lb = new BlankFillLineBuilder(2);
      lb.Append(this.DataStreamLength.ToString(), 9);
      lb.Append(this.Marker.ToHex(), 8);
      lb.Append(this.StreamCode.Value.ToString(), 15);
      lb.Append(this.VariableLength.ToString(), 7);

      return lb.ToString();
    }

    public override string[] ToReportLines()
    {
      var lines = new List<string>();
      lines.Add(this.ToString());
      lines.Add(this.ToHexString());
      return lines.ToArray();
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder( ) ;
      sb.Append(this.ItemName) ;
      if (this.Errmsg != null)
        sb.Append(this.Errmsg);
      else
      {
        sb.Append("RcdLgth:" + this.DataStreamLength.ToString() + " ");
        sb.Append("RcdType:" + this.Marker.ToHex() + " ");
      }

      return sb.ToString( ) ;
    }
  }

  public static class DataStreamHeaderExt
  {
    public static int? GetDataStreamHeaderLength(this byte[] Bytes)
    {
      int? headerLength = null;

      if ((Bytes.Length > 7) && (Bytes[2] == 0x12) && (Bytes[3] == 0xa0))
      {
        byte varLgth = Bytes[6];
        headerLength = 6 + varLgth;
      }

      return headerLength;
    }

    /// <summary>
    /// peek at the bytes in the InputArray.  return true if bytes 2 - 3 contain
    /// GDS identifier.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static bool IsDataStreamHeader(this InputByteArray InputArray)
    {
      if (InputArray.RemainingLength < 6)
        return false;

      var buf = InputArray.PeekBytes(6);
      var lgth = buf.BigEndianBytesToShort(0);

      if ((buf[2] == 0x12) && (buf[3] == 0xa0))
      {
        if (lgth >= 10)
        {
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// working from InputArray positioned at the start of data stream header,
    /// peek ahead and calc the header length.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static int PeekDataStreamHeaderLength(this InputByteArray InputArray)
    {
      byte varLgth = InputArray.PeekByte(6);
      return 6 + varLgth;
    }

    /// <summary>
    /// return the datastreamcode from the input stream.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static DataStreamCode? PeekDataStreamCode( this InputByteArray InputArray)
    {
      DataStreamCode? code = null;
      var buf = InputArray.PeekBytesLenient(7);
      if ((buf[2] == 0x12) && (buf[3] == 0xa0))
      {
        code = buf.SubArray(4, 2).ToDataStreamCode();
      }
      return code;
    }

    /// <summary>
    /// peek at current location in input array for a 5250 data stream header. If is
    /// a valid header, return a DataStreamHeader. Otherwise, return null.
    /// </summary>
    /// <param name="InputArray"></param>
    /// <returns></returns>
    public static DataStreamHeader PeekDataStreamHeader( this InputByteArray InputArray )
    {
      DataStreamHeader dsh = null;

      if ( InputArray.IsDataStreamHeader( ) == true )
      {
        var buf = new InputByteArray(InputArray.PeekBytesLenient(100));
        var rv = DataStreamHeader.Factory(buf);
        dsh = rv.Item1;
      }

      return dsh;
    }
  }
}
