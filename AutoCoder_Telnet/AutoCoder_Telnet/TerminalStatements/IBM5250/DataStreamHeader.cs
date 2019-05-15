using AutoCoder.Telnet.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.IBM5250;
using AutoCoder.Ext.System;

namespace AutoCoder.Telnet.TerminalStatements.IBM5250
{
  public class DataStreamHeader
  {

    public int RcdLgth
    { get; set; }

    public byte[] RcdType
    { get; set; }

    public byte[] Reserved
    { get; set; }

    public byte VarHdrLgth
    { get; set; }

    public byte[] Flags
    { get; set; }

    public DataStreamOpcode DataStreamOpcode
    { get; set; }

    public DataStreamHeader(int RcdLgth, byte[] RcdType, byte[] Reserved,
      byte VarHdrLgth, byte[] Flags, DataStreamOpcode DataStreamOpcode )
    {
      this.RcdLgth = RcdLgth;
      this.RcdType = RcdType;
      this.Reserved = Reserved;
      this.VarHdrLgth = VarHdrLgth;
      this.Flags = Flags;
      this.DataStreamOpcode = DataStreamOpcode;
    }

    public static bool IsDataStreamHeader(InputByteArray InputArray)
    {
      if ( InputArray.DataLgth < 6 )
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

    public static DataStreamHeader Factory(InputByteArray InputArray)
    {
      DataStreamHeader dsHeader = null;

      if (InputArray.RemainingLength >= 10)
      {
        var buf = InputArray.PeekBytes(10);
        var rcdLgth = buf.BigEndianBytesToShort(0);

        byte[] rcdType = new byte[2];
        Array.Copy(buf, 2, rcdType, 0, 2);

        byte[] reserved = new byte[2];
        Array.Copy(buf, 4, reserved, 0, 2);

        if ((rcdType[0] == 0x12) && (rcdType[1] == 0xa0)
          && (rcdLgth >= 10))
        {
          byte varHdrLgth = buf[6];
          byte[] flags = new byte[2];
          Array.Copy(buf, 7, flags, 0, 2);
          var dsOpcode = buf[9].ToDataStreamOpcode();

          if ((varHdrLgth == 4) && (dsOpcode != null))
          {
            dsHeader = new DataStreamHeader(
              rcdLgth, rcdType, reserved, varHdrLgth, flags, dsOpcode.Value);

            InputArray.AdvanceIndex(10);
          }
        }
      }

      return dsHeader;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder( ) ;
      sb.Append("DataStreamHeader. " ) ;
      sb.Append("RcdLgth:" + this.RcdLgth.ToString( ) + " ") ;
      sb.Append("RcdType:" + this.RcdType.ToHex()   + " " );
      sb.Append("VarHdr length:" + this.VarHdrLgth.ToHex( ) + " " ) ;
      sb.Append("Flags:" + this.Flags.ToHex( ) + " " ) ;
      sb.Append("Opcode:" + this.DataStreamOpcode.ToString( )) ;

      return sb.ToString( ) ;
    }

  }
}
