using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Enums.IBM5250;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.WorkstationCommands
{
  /// <summary>
  /// 04 F4. write single structure field. Used to set keyboard buffering.
  /// </summary>
  public class WriteSingleStructuredFieldCommand : WorkstationCommandBase
  {
    public int llLength
    { get; set; }
    public byte cField
    { get; set; }
    public byte tField
    { get; set; }
    public byte f1Field
    { get; set; }
    public byte f2Field
    { get; set; }
    public byte? minor_ll
    { get; set; }
    public byte? minor_t
    { get; set; }
    public byte? minor_f
    { get; set; }

    public WriteSingleStructuredFieldCommand(InputByteArray InputArray)
      : base(InputArray, WorkstationCode.WriteSingleStructuredField)
    {
      byte[] buf = null;

      // advance past the 0x04 and  0xf4.
      InputArray.AdvanceIndex(2);

      // first 2 bytes are the LL length bytes.
      this.llLength = InputArray.PeekBigEndianShort(0);

      // make sure enough length remaining in the input array
      if (this.llLength > InputArray.RemainingLength)
        this.Errmsg = "LL length " + this.llLength.ToString()
          + " exceeds remaining length of input array.";

      // setup c field and t field 
      if (this.Errmsg == null)
      {
        buf = InputArray.GetBytes(this.llLength);
        this.cField = buf[2];
        this.tField = buf[3];
        this.f1Field = buf[4];
        this.f2Field = buf[5];

        // command contains minor structure.
        if ( this.llLength >8)
        {
          this.minor_ll = buf[6];
          this.minor_t = buf[7];
          this.minor_f = buf[8];
        }
      }

      if (this.Errmsg == null)
      {
        this.BytesLength += this.llLength;
      }
    }

    public override int GetDataStreamLength()
    {
      return 2 + this.llLength;
    }

    public override string ToString()
    {
      var s1 = this.CmdCode.ToString() + " LL length:" + this.llLength +
        " cField:" + this.cField.ToHex() +
        " tField:" + this.tField.ToHex() +
        " f2Field:" + this.f2Field.ToHex();

      if ( this.llLength > 8 )
      {
        s1 += " minor_ll:" + this.minor_ll.Value +
          " minor_t:" + this.minor_t.Value.ToHex() +
          " minor_f:" + this.minor_f.Value.ToHex();
      }

      return s1;
    }
  }
}
