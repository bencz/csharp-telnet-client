using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace AutoCoder.Enums
{
  public enum CopyPasteCode
  {
    Copy,
    Paste,
    Cut,
    none
  }

  public static class CopyPasteCodeExt
  {
    public static bool CompareEqual(this CopyPasteCode? Code, CopyPasteCode? Value)
    {
      if ((Code == null) && (Value == null))
        return true;
      else if ((Code == null) || (Value == null))
        return false;
      else
        return (Code.Value == Value.Value);
    }

    public static XElement ToXElement(this CopyPasteCode? PurposeCode, XName Name)
    {
      if (PurposeCode == null)
        return new XElement(Name, null);
      else
      {
        var purposeCode = PurposeCode.Value;
        return new XElement(Name, purposeCode.ToString());
      }
    }

    public static CopyPasteCode? TryParseCopyPasteCode(this string Text)
    {
      var rv = TryParse(Text);
      return rv;
    }
    public static CopyPasteCode? TryParse(string Text)
    {
      if (Text == null)
        return null;
      else
      {
        var lcText = Text.ToLower();
        if (lcText == "copy")
          return CopyPasteCode.Copy;
        else if (lcText == "cut")
          return CopyPasteCode.Cut;
        else if (lcText == "paste")
          return CopyPasteCode.Paste;
        else if (lcText == "none")
          return CopyPasteCode.none;
        else
          return null;
      }
    }
  }
}


