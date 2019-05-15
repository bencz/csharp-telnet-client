using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScreenDefnLib.Enums
{
  /// <summary>
  /// enum used in section defn. Indicates the usage of the section.
  /// </summary>
  public enum ScreenPurposeCode
  {
    Header,
    Footer,
    ReportColHead,  // subfile column heading
    ReportDetail,     // subfile data
    Body,
    none
  }

  public static class ScreenPurposeCodeExt
  {
    public static XElement ToXElement(this ScreenPurposeCode? PurposeCode, XName Name)
    {
      if (PurposeCode == null)
        return new XElement(Name, null);
      else
      {
        var purposeCode = PurposeCode.Value;
        return new XElement(Name, purposeCode.ToString());
      }
    }

    public static ScreenPurposeCode? TryParseScreenPurposeCode(this string Text)
    {
      var rv = TryParse(Text);
      return rv;
    }
    public static ScreenPurposeCode? TryParse(string Text)
    {
      if (Text == null)
        return null;
      else
      {
        var lcText = Text.ToLower();
        if (lcText == "header")
          return ScreenPurposeCode.Header;
        else if (lcText == "footer")
          return ScreenPurposeCode.Footer;
        else if (lcText == "body")
          return ScreenPurposeCode.Body;
        else if (lcText == "reportcolhead")
          return ScreenPurposeCode.ReportColHead;
        else if (lcText == "reportdetail")
          return ScreenPurposeCode.ReportDetail;
        else if (lcText == "none")
          return ScreenPurposeCode.none;
        else
          return null;
      }
    }
  }
}
