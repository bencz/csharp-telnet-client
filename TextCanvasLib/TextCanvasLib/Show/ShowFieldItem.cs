using AutoCoder.Telnet.CodedBytes;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Enums;
using System;
using System.Text;
using TextCanvasLib.Ext;
using TextCanvasLib.Visual;

namespace TextCanvasLib.xml
{
  public class ShowFieldItem : ShowItemBase, IVisualItem
  {
    public string Name { get; set; }

    public ShowUsage Usage { get; set; }

    public string UsageText 
    { 
      set
      {
        if ( value != null )
        {
          var rv = ShowUsageExt.TryParse(value);
          if (rv == null)
            this.Errmsg = "Invalid show usage " + value;
          else
            this.Usage = rv.Value;
        }
      }
    }

    public string DataDefn
    {
      set
      {
        if ( value != null )
        {
          var rv = ParseDataDefn(value);
          this.Dtyp = rv.Item1;
          this.Dlen = rv.Item2;
          this.Dprc = rv.Item3;
        }
      }
    }

    public ShowDtyp Dtyp
    {
      get;
      set; 
    }

    /// <summary>
    /// data length of the field.
    /// </summary>
    public int Dlen
    { get; set; }

    /// <summary>
    /// number of decimal places of the field
    /// </summary>
    public int Dprc { get; set; }

    public DsplyAttr[] DsplyAttr
    { get; set; }

    public bool IsBypass
    {
      get
      {
        return FieldFormatWord.IsBypass(this.sfo_FFW);
      }
    }

    public bool IsMonocase
    { get; set; }
    public bool IsField
    {
      get { return true; }
    }

    public bool IsInputItem
    {
      get
      {
        return this.Usage.IsInput();
      }
    }
    public bool IsTabToItem
    {
      get { return true; }
    }

    public byte[] sfo_FFW
    { get; set; }

    public byte[] sfo_FCW
    { get; set; }

    public short sfo_Length
    { get; set; }

    public int ShowLength
    {
      get
      {
        return this.Dlen; 
      }
    }

    public string ShowText
    {
      get { return new string(' ', this.ShowLength); }
      set { var s1 = value; }
    }

    public ShowFieldItem()
      : base( ) 
    {
      this.ItemType = ShowItemType.Field;
    }

    public ShowFieldItem( 
      ZeroRowCol RowCol, byte? AttrByte, ShowUsage Usage, ShowDtyp Dtyp, int Dlen )
      : base( RowCol, ShowItemType.Field, AttrByte )
    {
      this.Usage = Usage;
      this.Dtyp = Dtyp;
      this.Dlen = Dlen;
      this.Value = "";
    }

    Tuple<ShowDtyp,int,int,string> ParseDataDefn(string Text )
    {
      ShowDtyp dtyp = ShowDtyp.Char;
      int dlen = 0;
      int dprc = 0;
      string delim = null ;
      string errmsg = null;

      int ix = 0;

      // text up to the "(" is data type.
      int fx = Text.IndexOf('(', ix);
      int lx = fx - ix;
      if ((ix == -1) || ( lx == 0 ))
        errmsg = "Data defn bad form. " + Text;
      if ( errmsg == null )
      {
        var chDtyp = Text.Substring(ix, lx).Trim();
        var rv = ShowDtypExt.TryParse(chDtyp);
        if (rv == null)
          errmsg = "Invalid data type. " + chDtyp;
        else
        {
          dtyp = rv.Value;
          ix = fx + 1;
        }
      }

      // data length until "," or close paren.
      if ( errmsg == null )
      {
        if (ix >= Text.Length)
          fx = -1;
        else
        {
          fx = Text.IndexOfAny(new char[] { ',', ')' }, ix);
          lx = fx - ix;
        }
        if ((fx == -1) || (lx == 0))
          errmsg = "length part of data defn missing. " + Text;
        else
        {
          delim = Text.Substring(fx,1) ;
          var rv = Text.Substring(ix, lx).TryParseInt32();
          if (rv == null)
            errmsg = "length part of data defn not numeric. " + Text;
          else
          {
            dlen = rv.Value;
            ix = fx + 1;
          }
        }
      }

      // decimal precision
      if (( errmsg == null ) && ( delim != null ) && (delim == ","))
      {
        if (ix >= Text.Length)
          fx = -1;
        else
          fx = Text.IndexOf(')', ix);
        lx = fx - ix;
        if ((fx == -1) || (lx == 0))
          errmsg = "decimal precision part of data defn missing. " + Text;
        else
        {
          delim = Text.Substring(fx, 1);
          var rv = Text.Substring(ix, lx).TryParseInt32();
          if (rv == null)
            errmsg = "decimal precision part of data defn not numeric. " + Text;
          else
          {
            dprc = rv.Value;
            ix = fx + 1;
          }
        }
      }

      return new Tuple<ShowDtyp, int, int, string>(dtyp, dlen, dprc, errmsg);
    }

    public override string ToString()
    {
      var sb = new StringBuilder();
      sb.Append(base.ToString());
      sb.Append(" Usage:" + this.Usage.ToString());
      sb.Append(" Dtyp:" + this.Dtyp.ToString());
      sb.Append(" Dlen:" + this.Dlen);

      return sb.ToString();
    }
  }
}
