using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ehllapi;
using System.Xml.Linq;
using AutoCoder.Ehllapi.Common;
using AutoCoder.Ext;

namespace AutoCoder.Ehllapi.Presentation
{
  public static class PresentationSpaceFieldExt
  {
    public static bool TextContains(
      this PresentationSpaceField Field, string Text)
    {
      if (Field == null)
        return false;
      else if (Field.Text.Contains(Text))
        return true;
      else
        return false;
    }

    public static XElement ToXElement(this PresentationSpaceField Field, XName Name)
    {
      if (Field == null)
        return new XElement(Name, null);
      else
      {
        string fldText = "";
        int textLx = 0;
        if (Field.Text != null)
        {
          textLx = Field.Text.Length;
          fldText = Field.Text.TrimEnd(new char[] { ' ' });
        }

        XElement xe = new XElement(Name,
          Field.Location.ToXElement("Location"),
          new XElement("Text", fldText),
          new XElement("Lx", textLx),
          Field.FieldAttribute.ToXElement("FldAttr")
          );

        return xe;
      }
    }

    public static PresentationSpaceField PresentationSpaceFieldOrDefault(
      this XElement Elem, XNamespace ns, PresentationSpaceField Default = null)
    {
      if (Elem == null)
        return Default;
      else
      {
        var loc = Elem.Element(ns + "Location").DisplayLocationOrDefault(ns, null);
        var fldText = Elem.Element(ns + "Text").StringOrDefault();
        var lx = Elem.Element(ns + "Lx").IntOrDefault(0).Value;
        var fldAttr = Elem.Element(ns + "FldAttr").FieldAttributeOrDefault(null);

        // pad the text to its original size.
        fldText.PadRight(lx, ' ');

        return new PresentationSpaceField(fldAttr, loc, fldText);
      }
    }
  }
}
