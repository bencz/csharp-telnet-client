using AutoCoder.Ext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AutoCoder.Telnet.Enums
{
  public enum DsplyAttr
  {
    ND = 1,
    UL = 2,
    CS = 3,
    BL = 4,
    RI = 5,
    HI = 6,
    NU = 7  // NU = neutral  0x20.
  }

  public static class DsplyAttrExt
  {
    /// <summary>
    /// test that the attr byte array contains 1 item and the item matches Value.
    /// </summary>
    /// <param name="attrArray"></param>
    /// <param name="Value"></param>
    /// <returns></returns>
    public static bool IsEqual(this DsplyAttr[] attrArray, DsplyAttr Value)
    {
      if (attrArray == null)
        return false;
      else if (attrArray.Length != 1)
        return false;
      else
        return (attrArray[0] == Value);
    }
    public static bool IsNullOrEmpty(this DsplyAttr[] attrArray)
    {
      if ((attrArray == null) || (attrArray.Length == 0))
        return true;
      else
        return false;
    }

    public static DsplyAttr[] ParseArray(string Text)
    {
      var items = Text.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
      var cx = items.Length;
      var attrList = new List<DsplyAttr>();
      for (int ix = 0; ix < cx; ++ix)
      {
        var da = items[ix].TryParseDsplyAttr();
        if (da != null)
          attrList.Add(da.Value);
      }
      var attrArray = attrList.ToArray();
      return attrArray;
    }

    public static DsplyAttr[] ToDsplyAttr(this byte? AttrByte)
    {
      List<DsplyAttr> da = new List<DsplyAttr>();
      if ( AttrByte != null )
      {
        var ab = AttrByte.Value;

        if (ab == 0x20)
          da.Add(DsplyAttr.NU);

        if ((ab & 0x21) == 0x21)
          da.Add(DsplyAttr.RI);
        if ((ab & 0x22) == 0x22)
          da.Add(DsplyAttr.HI);
        if ((ab & 0x24) == 0x24)
          da.Add(DsplyAttr.UL);
        if ((ab & 0x27) == 0x27)
          da.Add(DsplyAttr.ND);
      }
      return da.ToArray();
    }
    public static DsplyAttr[] ToDsplyAttr(this XElement Elem, XNamespace Namespace)
    {
      DsplyAttr[] dsplyAttr = null;
      if (Elem != null)
      {
        var cx = Elem.Elements().Count();
        dsplyAttr = new DsplyAttr[cx];
        int ix = 0;
        foreach( var item in Elem.Elements( ))
        {
          var da = item.StringOrDefault().TryParseDsplyAttr().Value;
          dsplyAttr[ix] = da;
          ix += 1;
        }
      }
      else
      {
        dsplyAttr = new DsplyAttr[0];
      }
      return dsplyAttr;
    }

    public static string ToDsplyAttrText(this DsplyAttr[] Array)
    {
      var sb = new StringBuilder();
      if (Array != null)
      {
        foreach (var da in Array)
        {
          if (sb.Length > 0)
            sb.Append(' ');
          sb.Append(da.ToString());
        }
      }
      return sb.ToString();
    }

    public static XElement ToXElement(this DsplyAttr[] DsplyAttr, XName Name)
    {
      if (DsplyAttr == null)
        return new XElement(Name, null);
      else
      {
        XElement xe = new XElement(Name);
        foreach( var attr in DsplyAttr)
        {
          xe.Add(new XElement("attr", attr.ToString()));
        }
        return xe;
      }
    }
    public static XElement ToXElement(this DsplyAttr[] DsplyAttr)
    {
      var xe = DsplyAttr.ToXElement("DsplyAttr");
      return xe;
    }
    public static DsplyAttr? TryParse(string Text)
    {
      var lcText = Text.ToLower();
      if (lcText == "nd")
        return DsplyAttr.ND;
      else if (lcText == "ul")
        return DsplyAttr.UL;
      else if (lcText == "cs")
        return DsplyAttr.CS;
      else if (lcText == "bl")
        return DsplyAttr.BL;
      else if (lcText == "ri")
        return DsplyAttr.RI;
      else if (lcText == "hi")
        return DsplyAttr.HI;
      else if (lcText == "nu")
        return DsplyAttr.NU;
      else
        return null;
    }
    public static DsplyAttr? TryParseDsplyAttr(this string Text)
    {
      var rv = TryParse(Text);
      return rv;
    }
  }
}
