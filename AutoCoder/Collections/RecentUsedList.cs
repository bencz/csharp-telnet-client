using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Linq;

namespace AutoCoder.Collections
{
  public class RecentUsedList : ObservableCollection<RecentUsedItem>
//    , IEnumerable<RecentUsedItem>
  {
    public int? RetentionLimit
    {
      get;
      set;
    }

    public void ApplyJustUsed(string ItemText)
    {
      // look for the itemText in the list.
      var item =
        this.FirstOrDefault(c => c.ItemText == ItemText);
      if (item != null)
      {
        item.LastUsedDate = DateTime.Now;
      }

      else
      {
        item = new RecentUsedItem(ItemText, DateTime.Now);
        this.Add(item);

        // remove old entries from the list.
        while ((this.RetentionLimit != null)
          && (this.Count > this.RetentionLimit.Value))
        {
          RemoveOldest();
        }
      }
    }

    public RecentUsedItem MostRecent
    {
      get
      {
        var items = from c in this
                    orderby c.LastUsedDate descending
                    select c;
        var item = items.FirstOrDefault();
        return item;
      }
    }

    public RecentUsedItem RemoveOldest()
    {
      var items = from c in this
                  orderby c.LastUsedDate
                  select c;
      var item = items.FirstOrDefault();
      if (item != null)
        this.Remove(item);
      return item;
    }

    public XElement ToXElement(XName Name)
    {
      return new XElement(Name,
        from c in this
        select c.ToXElement("RecentUsedItem")
        );
    }

#if skip
    IEnumerator<RecentUsedItem> IEnumerable<RecentUsedItem>.GetEnumerator()
    {
      var items = from c in this
                  orderby c.LastUsedDate descending
                  select c;

      foreach (var item in items)
        yield return item;
      yield break;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      var items = from c in this
                  orderby c.LastUsedDate descending
                  select c;

      foreach (var item in items)
        yield return item;
      yield break;
    }
#endif

  }

  public static class RecentUsedListExt
  {
    public static RecentUsedList ToRecentUsedList(
      this XElement Elem, XNamespace Namespace)
    {
      if (Elem == null)
        return new RecentUsedList();
      else
      {
        var sl = from c in Elem.Elements(Namespace + "RecentUsedItem")
                 select c.ToRecentUsedItem(Namespace);

        RecentUsedList recentList = new RecentUsedList();
        foreach (var sf in sl)
        {
          recentList.Add(sf);
        }

        return recentList;
      }
    }
  }
}
