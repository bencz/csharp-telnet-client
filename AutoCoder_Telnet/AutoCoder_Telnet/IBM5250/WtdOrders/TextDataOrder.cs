using AutoCoder.Ext.System;
using AutoCoder.Systm;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.IBM5250.Common;
using System;
using System.Text;

namespace AutoCoder.Telnet.IBM5250.WtdOrders
{
  public class TextDataOrder : WtdOrderBase
  {
    public byte? AttrByte
    { get; set; }

    /// <summary>
    /// attribute byte at the end of the text data order. This reduces the length of
    /// textBytes by 1.  ( for now, there is no provision for an attribute byte
    /// bounded by text characters in the textDataOrder.
    /// </summary>
    public byte? TailAttrByte
    { get; set; }

    /// <summary>
    /// byte length of this order in the data stream.
    /// </summary>
    public int DataStreamLength
    { get; set; }

    string _DisplayText;
    public string DisplayText
    {
      get
      {
        if (_DisplayText != null)
          return _DisplayText;
        else if (this.TextBytes == null)
          return null;
        else
        {
          var s1 = this.TextBytes.EbcdicBytesToString().TrimEnd(new char[] { ' ', '\0' });
          return s1;
        }
      }
      set
      {
        _DisplayText = value;
      }
    }
    public IRowCol ItemEndRowCol(IRowCol CurRowCol)
    {
      IRowCol endRowCol = null;
      int lx = 0;
      if (this.AttrByte != null)
        lx = 1;
      if (this.TextBytes != null)
        lx += this.TextBytes.Length;
      if (this.TailAttrByte != null)
        lx += 1;
      if (lx > 0)
        endRowCol = CurRowCol.Advance(lx - 1);
      return endRowCol;
    }

    /// <summary>
    /// calc and return the end pos of the show text of the text data order.
    /// </summary>
    /// <param name="ShowRowCol"></param>
    /// <returns></returns>
    public IRowCol ShowEndRowCol( IRowCol ShowRowCol )
    {
      IRowCol endRowCol = null;
      int lx = 0;
      if (this.TextBytes != null)
        lx = this.TextBytes.Length;
      if ( lx > 0 )
        endRowCol = ShowRowCol.Advance(lx - 1);
      return endRowCol;
    }

    /// <summary>
    /// calc and return the show rowCol of this item.
    /// </summary>
    /// <param name="ItemRowCol"></param>
    /// <returns></returns>
    public IRowCol ShowRowCol( IRowCol ItemRowCol )
    {
      IRowCol showRowCol = null;
      if (this.AttrByte == null)
        showRowCol = ItemRowCol;
      else
      {
        showRowCol = ItemRowCol.AdvanceRight();
      }
      return showRowCol;
    }

    /// <summary>
    /// the text byte converted to ebcdic, with the 0x00 and attrbyte bytes 
    /// replaced with blanks.
    /// </summary>
    public string ShowText
    {
      get
      {
        if (this.TextBytes == null)
          return "";
        else
        {
        var buf = this.TextBytes.SubArray(0);
        for( int ix = 0; ix < buf.Length; ++ix )
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
    }

    public string PrintableText
    {
      get
      {
        if (_DisplayText != null)
          return _DisplayText;
        else if (this.TextBytes == null)
          return null;
        else
        {
          var s1 = this.TextBytes.EbcdicBytesToPrintableAscii( ).TrimEnd(
            new char[] { ' ' });
          return s1;
        }
      }
    }

    /// <summary>
    /// alternate to DisplayText. The actual bytes of the text data order. ( 
    /// TextBytes are stripped of the leading and tail attr bytes. )
    /// </summary>
    public byte[] TextBytes
    { get; set; }

    public int TextLength
    {
      get
      {
        if (this.TextBytes != null)
          return this.TextBytes.Length;
        else if (this.DisplayText != null)
          return this.DisplayText.Length;
        else
          return 0;
      }
    }

    public byte[] RawBytes
    { get; set; }

    public TextDataOrder( string DisplayText, byte AttrByte )
      : base()
    {
      this.ItemName = "TextData";
      this.AttrByte = AttrByte;
      this.TailAttrByte = null;
      this.DisplayText = DisplayText;
    }
    public TextDataOrder(byte[] TextBytes, byte? AttrByte = null, byte? TailAttrByte = null)
      : base()
    {
      this.ItemName = "TextData";
      this.AttrByte = AttrByte;
      this.TextBytes = TextBytes;
      this.TailAttrByte = TailAttrByte;
      this.DisplayText = null;
    }
    public TextDataOrder( InputByteArray InputArray )
      : base( InputArray, "TextData" )
    {
      var b1 = InputArray.PeekByte(0);
      if (Common5250.IsTextDataChar(b1) == false)
      {
        this.Errmsg = "Invalid text data order. Order must begin with data character.";
      }

      if ( this.Errmsg == null )
      {
        // scan forward in the input array for a non text character.
        var rv = Common5250.ScanNonTextDataByte(InputArray);
        this.DataStreamLength = rv.Item2;

        // the actual data stream bytes.
        var rawBytes = InputArray.PeekBytes(this.DataStreamLength);
        int rawLx = rawBytes.Length;

        // first byte is the attribute byte.
        if (Common5250.IsAttributeByte(b1))
        {
          this.AttrByte = b1;
        }

        // last text byte is an attribute byte.
        if ((rawLx > 1 ) && (rawBytes[rawLx-1].IsAttributeByte( ) == true))
        {
          this.TailAttrByte = rawBytes[rawLx - 1];
        }

        // bytes the attrByte and tailAttrByte are textBytes.
        {
          int fx = 0;
          int lx = rawLx;
          if (this.AttrByte != null)
          {
            fx += 1;
            lx -= 1;
          }
          if (this.TailAttrByte != null)
          {
            lx -= 1;
          }
          if (lx > 0)
          {
            this.TextBytes = rawBytes.SubArray(fx, lx);
          }
        }

        this.RawBytes = rawBytes;
        this.BytesLength = rawLx;
        InputArray.AdvanceIndex(rawLx);
      }
    }

    /// <summary>
    /// advance the RowCol cursor based on the attribute bytes and show text of the
    /// text data order.
    /// </summary>
    /// <param name="Current"></param>
    /// <returns></returns>
    public override IRowCol Advance( IRowCol Current)
    {
      var nextRowCol = Current.Advance(this.ItemLength());
      return nextRowCol;
    }

    /// <summary>
    /// build a byte array containing a TextData order.
    /// </summary>
    /// <param name="RowNum"></param>
    /// <param name="ColNum"></param>
    /// <returns></returns>
    public static byte[] Build(
      string TextData, byte? AttrByte = null, byte? TailAttrByte = null)
    {
      var ba = new ByteArrayBuilder();

      string textData = TextData;
      if ((textData.Length == 0) && (AttrByte == null) && (TailAttrByte == null ))
        textData = " ";

      if (AttrByte != null)
        ba.Append(AttrByte.Value);
      ba.Append(textData.ToEbcdicBytes());
      if (TailAttrByte != null)
        ba.Append(TailAttrByte.Value);

      return ba.ToByteArray();
    }

    /// <summary>
    /// TextDataOrder contains no content. ( not sure if ever true. )
    /// </summary>
    /// <returns></returns>
    public bool IsEmpty( )
    {
      if ((this.AttrByte != null)
        || ((this.TextBytes != null) && (this.TextBytes.Length > 0))
        || (this.TailAttrByte != null))
        return false;
      else
        throw new Exception("TextDataOrder is empty");
    }
    public int ItemLength( )
    {
      int lx = 0;
      if (this.AttrByte != null)
        lx += 1;
      if (this.TailAttrByte != null)
        lx += 1;
      if (this.TextBytes != null)
        lx += this.TextBytes.Length;
      return lx;
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append("TextData order. Attr byte:");
      if (this.AttrByte == null)
        sb.Append("[none] ");
      else
        sb.Append(this.AttrByte.Value.ToHex() + " ");

      if ( this.TextBytes != null )
      {
        sb.Append("Length:" + this.TextBytes.Length + " Text:");
        sb.Append(this.TextBytes.EbcdicBytesToPrintableAscii());
        sb.Append(" Bytes:" + this.ToBytes().ToHex(' ')) ;
      }

      else if (this.DisplayText == null)
        sb.Append("[Display text is null]");
      else
      {
        sb.Append("Lgth:" + this.DisplayText.Length.ToString() + " ");
        bool lastHex = false;
        foreach( var ch1 in this.DisplayText)
        {
          if ( char.IsControl(ch1))
          {
            var b1 = (byte)ch1;
            sb.Append(" " + b1.ToHex());
            lastHex = true;
          }
          else
          {
            if (lastHex == true)
              sb.Append(' ');
            sb.Append(ch1);
            lastHex = false;
          }
        }
        sb.Append(this.DisplayText);
      }

      return sb.ToString();
    }

    public override byte[] ToBytes()
    {
      if (this.RawBytes != null)
        return this.RawBytes;
      else
      {
      var ba = new ByteArrayBuilder();

        if ( this.AttrByte != null)
          ba.Append(this.AttrByte.Value);

        if ( this.TextBytes != null )
        {
          ba.Append(this.TextBytes);
        }
        else if ( this.DisplayText != null )
        {
          ba.Append(this.DisplayText.ToEbcdicBytes());
        }
        return ba.ToByteArray();
      }
    }
  }
}
