using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCoder.Telnet.Enums
{
  // variable literal and static literal account for the difference where a text
  // data order is actually the value of an output field or an actual, never
  // changing literal.  A DDS output field shows in the 5250 data stream as a
  // TextData order. Not a StartField order.  
  public enum ShowItemType
  {
    none = 0,
    Literal,
    VarLit,    // variable literal.
    Static,
    StaticLit,  // static literal.
    Field,
    Section,     // a section of the screen. contains its own collection of items.
    ScreenDefn
  }

  public static class ShowItemTypeExt
  {
    public static bool IsScreenAtomic(this ShowItemType ItemType)
    {
      if ((ItemType == ShowItemType.Section) || (ItemType == ShowItemType.ScreenDefn))
        return false;
      else
        return true;
    }
    public static ShowItemType? TryParseShowItemType(this string Text)
    {
      var rv = TryParse(Text);
      return rv;
    }
    public static ShowItemType? TryParse(string Text)
    {
      var lcText = Text.ToLower();
      if (lcText == "literal")
        return ShowItemType.Literal;
      else if (lcText == "field")
        return ShowItemType.Field;
      else if (lcText == "varlit")
        return ShowItemType.VarLit;
      else if (lcText == "staticlit")
        return ShowItemType.StaticLit;
      else if (lcText == "static")
        return ShowItemType.Static;
      else if (lcText == "section")
        return ShowItemType.Section;
      else
        return null;
    }
  }
}

