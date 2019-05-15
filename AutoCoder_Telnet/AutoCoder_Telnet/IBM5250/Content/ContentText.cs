using AutoCoder.Ext.System;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.IBM5250.Content
{
  public class ContentText : ContentItemBase
  {
    private int TextLength
    {
      get { return _TextLength; }
      set
      {
        _TextLength = value;
      }
    }
    int _TextLength;

    public int? TailAttrByteIx
    { get; set;  }

    public ContentText( 
      ZeroRowCol RowCol, byte? AttrByte, int TextLength, 
      int? TailAttrByteIx)
    {
      this.RowCol = RowCol;
      this.AttrByte = AttrByte;
      this.TextLength = TextLength;
      this.TailAttrByteIx = TailAttrByteIx;
    }
    public override int GetItemLength(ScreenContent ScreenContent)
    {
      return this.GetShowText(ScreenContent).Length;
    }

    public byte? GetTailAttrByte(ScreenContent ScreenContent)
    {
      if (this.TailAttrByteIx == null)
        return null;
      else
      {
        int ix = this.TailAttrByteIx.Value;
        var b1 = ScreenContent.ContentArray[ix];
        return b1;
      }
    }

    public override byte[] GetTextBytes(ScreenContent ScreenContent)
    {
      var ix = ScreenContent.ToContentIndex(this.RowCol);
      if (ScreenContent.ContentArray[ix].IsAttrByte() == true)
        ix += 1;
      return ScreenContent.ContentArray.SubArray(ix, this.TextLength);
    }

    public override string ToString( ScreenContent ScreenContent)
    {
      return this.ToString( ) +  
        " " + this.GetShowText(ScreenContent);
    }

    public override string ToString()
    {
      return "ContentText. RowCol:" + this.RowCol.ToString() +
        " AttrByte:" + this.AttrByte.ToColorDsplyAttr().ToText();
    }
  }
}
