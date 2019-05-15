using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using AutoCoder.Ext;

namespace AutoCoder.Collections
{
  public class RecentUsedItem : IEquatable<RecentUsedItem>
  {
    public string ItemText
    { get; set; }

    public DateTime LastUsedDate
    { get; set; }

    public RecentUsedItem( )
    {
      this.ItemText = null;
      this.LastUsedDate = DateTime.Now;
    }

    public RecentUsedItem(string ItemText)
    {
      this.ItemText = ItemText;
      this.LastUsedDate = DateTime.Now;
    }

    public RecentUsedItem(string ItemText, DateTime LastUsedDate  )
    {
      this.ItemText = ItemText;
      this.LastUsedDate = LastUsedDate;
    }

    public bool Equals(RecentUsedItem other)
    {
      if (other == null)
        return false;
      else
        return (this.ItemText == other.ItemText);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.LastUsedDate.ToString("yyyy-MM-dd hh:mm:ss tt"));
      sb.Append(" " + this.ItemText);
      return sb.ToString();
    }

    public XElement ToXElement(XName Name)
    {
      XElement xe = new XElement(Name,
        new XElement("ItemText", this.ItemText),
        new XElement("LastUsedDate", this.LastUsedDate)) ;
      return xe;
    }
  }

  public static class RecentUsedItemExt
  {
    public static RecentUsedItem ToRecentUsedItem(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return null;
      else
      {
        var itemText = Elem.Element(Namespace + "ItemText").StringOrDefault();
        var lastUsedDate = 
          Elem.Element(Namespace + "LastUsedDate").DateTimeOrDefault(DateTime.Now).Value ;

        RecentUsedItem item = new RecentUsedItem()
        {
          ItemText = itemText,
          LastUsedDate = lastUsedDate
        };
        return item;
      }
    }
  }
}
