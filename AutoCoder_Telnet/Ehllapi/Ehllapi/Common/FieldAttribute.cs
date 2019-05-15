using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ehllapi.Enums;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Ehllapi.Common
{
  public class FieldAttribute
  {
    DisplayIntensity mIntensity = DisplayIntensity.None;
    bool mIsField = false;
    bool mIsVisible = false;
    bool mIsProtected = false;
    FieldEntryType mEntryType = FieldEntryType.None;
    bool mMDT = false;

    public FieldAttribute()
    {
    }

    public FieldAttribute(byte InByte)
    {
      Assign(InByte);
    }

    public byte? AttrByte
    {
      get;
      set;
    }

    public FieldEntryType EntryType
    {
      get { return mEntryType; }
      set { mEntryType = value; }
    }

    public DisplayIntensity Intensity
    {
      get { return mIntensity ; }
      set { mIntensity = value ; }
    }

    public bool IsVisible
    {
      get { return mIsVisible; }
      set { mIsVisible = value; }
    }

    public bool IsProtected
    {
      get { return mIsProtected; }
      set { mIsProtected = value; }
    }

    public bool IsField
    {
      get { return mIsField; }
      set { mIsField = value; }
    }

    public bool MDT
    {
      get { return mMDT; }
      set { mMDT = value; }
    }

    public void Assign(byte InByte)
    {
      this.AttrByte = InByte;

      // bit 0 - is a field or literal.
      if ((InByte & 0x80) > 0)
        IsField = true;

      // bit 1 - non display or visible.  
      if ((InByte & 0x40) > 0)
        IsVisible = true;

      // bit 2 - is protected.
      if ((InByte & 0x20) > 0)
        IsProtected = true;

      // bit 3 - high intensity, normal.
      if ((InByte & 0x10) > 0)
        Intensity = DisplayIntensity.High;
      else
        Intensity = DisplayIntensity.Normal;

      // bits 4 thru 6 - field entry type.
      if ((InByte & 0x0e) == 0)
        EntryType = FieldEntryType.AlphaNumeric;
      else if ((InByte & 0x02) == 0x02)
        EntryType = FieldEntryType.AlphaOnly;
      else if ((InByte & 0x04) == 0x04)
        EntryType = FieldEntryType.NumericShift;
      else if ((InByte & 0x06) == 0x06)
        EntryType = FieldEntryType.NumericEdit;
      else if ((InByte & 0x0a) == 0x0a)
        EntryType = FieldEntryType.NumericOnly;
      else if ((InByte & 0x0c) == 0x0c)
        EntryType = FieldEntryType.MagneticStripe;
      else if ((InByte & 0x0e) == 0x0e)
        EntryType = FieldEntryType.SignedNumeric;

      // bit 7 - modified data tag
      if ((InByte & 0x01) > 0)
        MDT = true;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if (IsField == true)
        sb.Append("Field");
      else
        sb.Append("Literal");

      if (IsVisible == true)
        sb.Append(" Visible");
      else
        sb.Append(" NonDisplay");

      sb.Append(" " + Intensity.ToString() +
        " " + EntryType.ToString());

      sb.Append(" MDT:" + MDT.ToString());

      return sb.ToString();
    }
  }

  public static class FieldAttributeExt
  {
    public static XElement ToXElement(this FieldAttribute Attr, XName Name)
    {
      if (Attr == null)
        return new XElement(Name, null);
      else if (Attr.AttrByte != null)
      {
        string hexRep = Attr.AttrByte.Value.ToString("X").PadLeft(2, '0');
        var xe = new XElement(Name, hexRep);
        return xe;
      }
      else
      {
        return new XElement(Name, null);
      }
    }

    public static FieldAttribute FieldAttributeOrDefault(
      this XElement Elem, FieldAttribute Default = null )
    {
      if (Elem == null)
        return Default;
      else
      {
        var s1 = Elem.Value;
        byte attrByte = System.Convert.ToByte(s1, 16);
        return new FieldAttribute(attrByte);
      }
    }

  }
}
