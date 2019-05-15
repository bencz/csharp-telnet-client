using AutoCoder.Enums;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.Serialize;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.Enums;
using AutoCoder.Telnet.IBM5250.Content;
using ScreenDefnLib.Common;
using ScreenDefnLib.CopyPaste;
using ScreenDefnLib.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScreenDefnLib.Defn
{
  public interface IScreenItem
  {
    string ItemName { get; set; }
    ShowItemType ItemType { get; set; }
    string ItemGuid { get; set; }
    int MatchNum { get; set; }
    OneScreenLoc ScreenLoc { get; set; }
    CopyPasteCode? CopyPasteCode { get; set; }
    bool IsOptional { get; set; }
    string[] HoverCode { get; set; }
    string[] HoverXaml { get; set; }

    /// <summary>
    /// section header which contains the Items list this IScreenItem is a 
    /// member of.
    /// </summary>
    ISectionHeader SectionHeader { get; set; }

    void ApplyMatch(IScreenItem source);
  }

  public static class IScreenItemExt
  {
    public static void AssignItemGuid(this IScreenItem item)
    {
      item.ItemGuid = Guid.NewGuid().ToString();
    }

    public static Tuple<DataItem, DataItemReport> CaptureReport(
      this IScreenItem item, IScreenLoc Start, ScreenContent Content)
    {
      string captureText = null;
      DataItem dataItem = null;
      DataItemReport itemReport = null;
      ContentItemBase contentItem = null;

      // get the content item at the rowcol location.
      bool rc = true;
      IScreenAtomic atomicItem = null;

      // adjust screen loc of the item by start pos of the section it is contained in.
      var adjRow = Start.RowNum - 1;
      var adjCol = Start.ColNum - 1;
      var loc = new OneScreenLoc(
        item.ScreenLoc.RowNum + adjRow, item.ScreenLoc.ColNum + adjCol);
      var zeroLoc = loc.ToZeroRowCol();

      if (item.ItemType != ShowItemType.Section)
      {
        rc = Content.FieldDict.TryGetValue(zeroLoc, out contentItem);
        atomicItem = item as IScreenAtomic;
      }

      if (rc == false)
      {
      }

      else if (item.ItemType == ShowItemType.Section)
      {
        var sectionItem = item as IScreenSection;
        itemReport = sectionItem.CaptureReport(Content);
      }

      else if (item.ItemType == ShowItemType.Field)
      {
        if ((contentItem is ContentField) == true)
        {
          var contentField = contentItem as ContentField;
          dataItem = new DataItem(item.ItemName, contentField.GetShowText(Content));
          captureText = item.ItemName + "=" + contentField.GetShowText(Content);
        }
      }

      // match fixed literal
      else if (item.ItemType == ShowItemType.Literal)
      {
        if ((contentItem is ContentText) == true)
        {
          var contentText = contentItem as ContentText;
          var itemLit = item as IScreenLiteral;
          var ctValue = contentText.GetShowText(Content).TrimEndWhitespace();
        }
      }

      return new Tuple<DataItem, DataItemReport>(dataItem, itemReport);
    }

    public static ScreenItemInstance FindItem(
      this IScreenItem item, IScreenLoc Start, IScreenLoc FindZeroLoc, 
      ScreenContent Content)
    {
      ScreenItemInstance findItem = null;

      if (item.ItemType == ShowItemType.Section)
      {
        var sectionItem = item as IScreenSection;
        findItem = sectionItem.FindItem(Start, FindZeroLoc, Content);
      }

      else
      {
        // adjust screen loc of the item by start pos of the section it is contained in.
        var adjRow = Start.RowNum - 1;
        var adjCol = Start.ColNum - 1;
        var loc = new OneScreenLoc(
          item.ScreenLoc.RowNum + adjRow, item.ScreenLoc.ColNum + adjCol);
        var zeroLoc = loc.ToZeroRowCol();

        var itemAtomic = item as IScreenAtomic;
        var range = new ScreenLocRange(zeroLoc, itemAtomic.Length, Content.ScreenDim);
        if (range.Contains(FindZeroLoc) == true)
          findItem = new ScreenItemInstance(item, range.From);
      }

      return findItem;
    }

    public static DsplyAttr[] GetDsplyAttr(this IScreenItem Item)
    {
      if (Item is IScreenAtomic)
      {
        var itemAtomic = Item as IScreenAtomic;
        return itemAtomic.DsplyAttr;
      }
      else if (Item is IScreenSection)
        return null;
      else
        throw new Exception("unexpected interface");
    }

    public static string GetLength( this IScreenItem Item)
    {
      if (Item is IScreenLiteral)
        return (Item as IScreenLiteral).Length.ToString();
      else if (Item is IScreenField)
        return (Item as IScreenField).Length.ToString();
      else if (Item is IScreenSection)
      {
        var itemSection = Item as IScreenSection;
        var dim = itemSection.ToRepeatRange().ToDim();
        return dim.b.ToString();
      }
      throw new Exception("unexpected interface");
    }

    public static ScreenLocRange GetLocRange(this IScreenItem item, ScreenDim Dim)
    {
      ScreenLocRange range = null;
      if ( item is IScreenAtomic )
      {
        var atomicItem = item as IScreenAtomic;
        range = new ScreenLocRange(item.ScreenLoc, atomicItem.Length, Dim);
      }
      else if (item is IScreenSection)
      {
        var sectionItem = item as IScreenSection;
        range = sectionItem.ToRepeatRange();
      }
      return range;
    }

    public static string GetValue( this IScreenItem Item)
    {
      if (Item is IScreenLiteral)
        return (Item as IScreenLiteral).Value;
      else
        return "";
    }

    public static ShowUsage? GetUsage(this IScreenItem Item)
    {
      if (Item is IScreenLiteral)
        return ShowUsage.Output;
      else if (Item is IScreenField)
        return (Item as IScreenField).Usage;
      else if (Item is IScreenSection)
        return null;
      else
        throw new Exception("unexpected interface");
    }

    public static bool Match(
      this IScreenItem item, IScreenLoc Start, ScreenContent Content, 
      string DebugInfo)
    {
      bool isMatch = true;
      ContentItemBase contentItem = null;

      // get the content item at the rowcol location.
      bool rc = true;
      IScreenAtomic atomicItem = null;

      // adjust screen loc of the item by start pos of the section it is contained in.
      var adjRow = Start.RowNum - 1;
      var adjCol = Start.ColNum - 1;
      var loc = new OneScreenLoc(
        item.ScreenLoc.RowNum + adjRow, item.ScreenLoc.ColNum + adjCol) ;
      var zeroLoc = loc.ToZeroRowCol();

      // item is not a section. Get the field or literal located at the item loc.
      if (item.ItemType != ShowItemType.Section)
      {
        atomicItem = item as IScreenAtomic;
      }

      if ((rc == false) && ( 1 == 2))
      {
        if ( item.IsOptional == false )
          isMatch = false;
      }

      else if ((isMatch == true) && (item.ItemType == ShowItemType.Section))
      {
        var sectionItem = item as IScreenSection;
        isMatch = sectionItem.Match(Content, DebugInfo);
      }

      else if ((isMatch == true) && (item.ItemType == ShowItemType.Field))
      {
        var fieldItem = item as IScreenField;
        rc = Content.FieldDict.TryGetValue(zeroLoc, out contentItem);
        if (rc == false)
          isMatch = false;
        
        // can match a screen field to a literal, but only if the field is output
        // only.
        else if ((contentItem is ContentText) 
          && (fieldItem.Usage != ShowUsage.Output))
          isMatch = false;

        else if ((contentItem is ContentField) == false)
          isMatch = false;
        else
        {
          var contentField = contentItem as ContentField;
          if (contentField.LL_Length != atomicItem.Length)
            isMatch = false;
        }
      }

      // match screen literal to actual content on the screen. The content can be
      // either a field or a literal. But the content has to match one of the 
      // values of the screen literal.
      else if ((isMatch == true) && (item.ItemType == ShowItemType.Literal))
      {
        // screen literal has a dsply attr. advance ....
        if ( atomicItem.DsplyAttr.IsNullOrEmpty( ) == false )
        {
          zeroLoc = zeroLoc.Advance(1) as ZeroRowCol;
        }

        var buf = Content.GetContentBytes_NonNull(zeroLoc, atomicItem.Length);
        var itemLit = item as IScreenLiteral;
        var contentText = buf.EbcdicBytesToString().TrimEndWhitespace();

        // match the screen defn item to the screen content.
        isMatch = itemLit.MatchValue(contentText);

        // failed the match. But if the item content is blank. And this screen
        // item is optional then that is ok.
        if ((isMatch == false) && (item.IsOptional == true) && (contentText.Length == 0))
          isMatch = true;
      }

      else if (isMatch == true)
      {
        throw new Exception("unrecognized screen item type");
      }

      return isMatch;
    }

    /// <summary>
    /// move the item down in the list it is a member of.
    /// </summary>
    /// <param name="Item"></param>
    public static void MoveDown(this IScreenItem Item)
    {
      // the sectionHeader of the item.
      var sectionHeader = Item.SectionHeader;

      // index of the item in the items list of section header.
      var ix = sectionHeader.ItemIndexOf(Item);
      if (sectionHeader.IsLastItem(ix) == false)
      {
        var cpl = new CopyPasteList(false);
        cpl.AddCut(Item);
        cpl.PasteAfter(sectionHeader, ix + 1);
      }
    }

    /// <summary>
    /// move the item up in the list it is a member of.
    /// </summary>
    /// <param name="Item"></param>
    public static void MoveUp(this IScreenItem Item)
    {
      // the sectionHeader of the item.
      var sectionHeader = Item.SectionHeader;

      // index of the item in the items list of section header.
      var ix = sectionHeader.ItemIndexOf(Item);
      if (ix > 0)
      {
        ScreenDefnGlobal.CopyPasteList.AddCut(Item);
        ScreenDefnGlobal.CopyPasteList.PasteBefore(sectionHeader, ix - 1);
      }
    }

    public static IScreenItem Remove(this IScreenItem Item)
    {
      var sectionHeader = Item.SectionHeader;
      sectionHeader.RemoveItem(Item);
      return Item;
    }

    public static XElement ToXElement(this IScreenItem Item)
    {
      var elem = Item.ToXElement("ScreenItem");
      return elem;
    }
    public static XElement ToXElement(this IScreenItem Item, XName Name)
    {
      XElement xe = null;
      if (Item == null)
        xe = new XElement(Name, null);
      else
      {
        // IScreenItem info.
        {
          var item = Item;
          xe = new XElement(Name,
              new XElement("ItemName", item.ItemName),
              new XElement("ItemType", item.ItemType),
              new XElement("ItemGuid", item.ItemGuid),
              new XElement("MatchNum", item.MatchNum),
              item.HoverCode.ToXElementNew("HoverCode"),
              item.HoverXaml.ToXElementNew("HoverXaml"),
              new XElement("IsOptional", item.IsOptional),
              item.ScreenLoc.ToXElement("ScreenLoc")
              );
        }
        if (Item is IScreenSection)
        {
          var item = Item as IScreenSection;
          xe.Add(new XElement("AssocSectionName", item.AssocSectionName));
          xe.Add(new XElement("RepeatCount", item.RepeatCount));
          xe.Add(new XElement("IsExpanded", item.IsExpanded));
          xe.Add(item.PurposeCode.ToXElement("PurposeCode"));
          xe.Add(item.Items.ToXElement("ItemList"));
        }
        if (Item is IScreenField)
        {
          var item = Item as IScreenField;
          xe.Add(new XElement("Usage", item.Usage));
          xe.Add(new XElement("Length", item.Length));
          xe.Add(item.DsplyAttr.ToXElement());
        }
        if (Item is IScreenLiteral)
        {
          var item = Item as IScreenLiteral;
          xe.Add(new XElement("Length", item.Length));
          xe.Add(item.ListValues.ToXElementNew("ListValues"));
          xe.Add(item.DsplyAttr.ToXElement());
        }
      }
      return xe;
    }

    /// <summary>
    /// create XML from the list of ScreenItem objects.
    /// </summary>
    /// <param name="ItemList"></param>
    /// <param name="ListName"></param>
    /// <returns></returns>
    public static XElement ToXElement(this IEnumerable<IScreenItem> ItemList, XName ListName)
    {
      if (ItemList == null)
        return new XElement(ListName);
      else
      {
        return new XElement(ListName,
          from c in ItemList
          select c.ToXElement("ScreenItem")
          );
      }
    }

    public static IScreenItem ToScreenItem(this XElement Elem, XNamespace Namespace)
    {
      IScreenItem item = null;
      if (Elem != null)
      {
        // first get itemType. Could be field, literal or section.
        var itemType = Elem.Element(Namespace + "ItemType").StringOrDefault("").TryParseShowItemType().Value;
        var itemName = Elem.Element(Namespace + "ItemName").StringOrDefault("");
        var screenLoc = Elem.Element(Namespace + "ScreenLoc").ToScreenLoc(Namespace) as OneScreenLoc;
        var hoverCode = Elem.Element(Namespace + "HoverCode").ToIEnumerableString("Item").ToArray();
        var hoverXaml = Elem.Element(Namespace + "HoverXaml").ToIEnumerableString("Item").ToArray();

        item = ScreenItem.Factory(itemType);

        item.ItemType = itemType;
        item.ItemName = itemName;
        item.MatchNum = Elem.Element(Namespace + "MatchNum").IntOrDefault(0).Value;
        item.ItemGuid = Elem.Element(Namespace + "ItemGuid").StringOrDefault(null);
        item.IsOptional = Elem.Element(Namespace + "IsOptional").BooleanOrDefault(false).Value;
        item.ScreenLoc = screenLoc;
        item.HoverCode = hoverCode;
        item.HoverXaml = hoverXaml;

        if (item is IScreenAtomic)
        {
          var atom = item as ScreenAtomic;
          atom.Length = Elem.Element(Namespace + "Length").IntOrDefault(0).Value;
          atom.DsplyAttr = Elem.Element(Namespace + "DsplyAttr").ToDsplyAttr(Namespace);
        }

        if (item is ScreenLiteral)
        {
          var lit = item as ScreenLiteral;
          lit.ListValues = Elem.Element(Namespace + "ListValues").ListStringOrDefault(Namespace, "Item", null);
        }

        if (item is IScreenField)
        {
          var fld = item as IScreenField;
          fld.Usage = Elem.Element(Namespace + "Usage").StringOrDefault("").TryParseShowUsage().Value;
        }

        if (item is IScreenSection)
        {
          var sect = item as IScreenSection;

          var itemList = Elem.Element(Namespace + "ItemList").ToScreenItemList(Namespace);
          sect.Items = new List<IScreenItem>();
          var sectionHeader = item as ISectionHeader;
          sectionHeader.LoadItems(itemList);

          sect.RepeatCount = Elem.Element(Namespace + "RepeatCount").IntOrDefault(0).Value;
          sect.IsExpanded = Elem.Element(Namespace + "IsExpanded").BooleanOrDefault(false).Value;
          sect.AssocSectionName = Elem.Element(Namespace + "AssocSectionName").StringOrDefault("");
          sect.PurposeCode = Elem.Element(Namespace + "PurposeCode").StringOrDefault("").TryParseScreenPurposeCode();
        }
      }
      return item;
    }

    public static IEnumerable<IScreenItem> ToScreenItemList(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return new List<IScreenItem>();
      else
      {
        var sl = from c in Elem.Elements()
                 select c.ToScreenItem(Namespace);
        return sl;
      }
    }
  }
}
