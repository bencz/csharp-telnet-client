using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public abstract class ContentItemBase
  {
    public ZeroRowCol RowCol
    { get; set; }

    public byte? AttrByte
    { get; set; }

    public virtual int FieldNum
    {
      get { return 0; }
    }

    public abstract byte[] GetTextBytes(ScreenContent ScreenContent);

    public byte? GetAttrByte(ScreenContent ScreenContent)
    {
      var ix = ScreenContent.ToContentIndex(this.RowCol);
      var b1 = ScreenContent.ContentArray[ix];
      if (b1.IsAttrByte() == true)
        return b1;
      else
        return null;
    }

    public abstract int GetItemLength(ScreenContent ScreenContent);

    public string GetShowText( ScreenContent ScreenContent)
    {
      var buf = this.GetTextBytes(ScreenContent);
      if (buf == null)
        return "";
      else
      {
        for (int ix = 0; ix < buf.Length; ++ix)
        {
          var b1 = buf[ix];
          if (b1 == 0x00)
            buf[ix] = 0x40;
          else if ((b1 >= 0x20) && (b1 <= 0x3f))
            buf[ix] = 0x40;
        }
        return buf.EbcdicBytesToString();
      }
    }

    public virtual string GetFieldShowText(ScreenContent ScreenContent)
    {
      return this.GetShowText(ScreenContent);
    }

    public virtual string ToString(ScreenContent ScreenContent)
    {
      return this.ToString();
    }
  }
}
