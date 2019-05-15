using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Common
{
  /// <summary>
  /// class that encapsulates an attribute byte.
  /// </summary>
  public class DisplayAttrByte
  {
    public byte? AttrByte
    { get; set; }

    public DisplayAttrByte(byte Value)
    {
      this.AttrByte = Value;
    }

    public DisplayAttrByte(byte? AttrByte)
    {
      this.AttrByte = AttrByte;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      if (this.AttrByte != null)
      {
        byte attrByte = this.AttrByte.Value;

        if (attrByte == 0x20)
          sb.Append("Plain");
        else if (attrByte == 0x27)
          sb.Append("ND");
        else
        {
          if ((attrByte & 0x10) != 0)
            sb.Append(" CS");
          if ((attrByte & 0x08) != 0)
            sb.Append(" BL");
          if ((attrByte & 0x04) != 0)
            sb.Append(" UL");
          if ((attrByte & 0x02) != 0)
            sb.Append(" HI");
          if ((attrByte & 0x01) != 0)
            sb.Append(" RI");
        }
      }

      return sb.ToString().Trim();
    }
  }
}
