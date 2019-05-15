using AutoCoder.Collections;
using AutoCoder.Core.Enums;
using AutoCoder.Ext.System;
using AutoCoder.Ext.System.Collections.Generic;
using AutoCoder.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Core.Interfaces
{
  // implementing classes:  AutoCoder.Telnet.Common.TelnetLogList
  public interface ILogList
  {
    string ListName { get; }
    string TextDesc { get; }

    void AddItem(ILogItem item);
    int Count { get; }

    IEnumerator<ILogItem> GetEnumerator();
    ILogItem GetItem(int index);
    ILogList NewList(string ListName = "");

    IEnumerable<ILogItem> LogItems();
  }

  public static class ILogListExt
  {
    public static string GetListTitle(this ILogList List)
    {
      var sb = new StringBuilder();
      if (List.ListName.IsNullOrEmpty() == false)
        sb.Append(List.ListName);

      if (List.TextDesc.IsNullOrEmpty() == false)
      {
        if (sb.Length > 0)
          sb.Append(" - ");
        sb.Append(List.TextDesc);
      }

      return sb.ToString();
    }
    public static ILogList MergeOld(this ILogList List1, ILogList List2, ILogList List3 = null)
    {
      ILogList merge = null;
      if (List1 != null)
        merge = List1.NewList();
      else if (List2 != null)
        merge = List2.NewList();
      else if (List3 != null)
        merge = List3.NewList();

      var item1 = List1?.NextItem(null);
      var item2 = List2?.NextItem(null);

      while ((item1 != null) || (item2 != null))
      {
        if (item1 == null)
        {
          item2 = merge.Merge_AddItem(List2, item2);
        }
        else if (item2 == null)
        {
          item1 = merge.Merge_AddItem(List1, item1);
        }
        else if (item1.Value.LogTime <= item2.Value.LogTime)
        {
          item1 = merge.Merge_AddItem(List1, item1);
        }
        else
        {
          item2 = merge.Merge_AddItem(List2, item2);
        }
      }

      return merge;
    }

    private static ListItem<ILogItem>[] Merge_NextItem(ILogList[] listArray)
    {
      var itemArray = new ListItem<ILogItem>[listArray.Length];
      for( int ix = 0; ix < listArray.Length; ++ix)
      {
        itemArray[ix] = listArray[ix]?.NextItem(null);
      }
      return itemArray;
    }

    public static ILogList Merge(
      this ILogList List1, 
      ILogList List2, ILogList List3 = null, ILogList List4 = null,
      ILogList List5 = null, ILogList List6 = null)
    {
      ILogList merge = null;

      ILogList[] listArray = new ILogList[] { List1, List2, List3, List4, List5, List6 };

      // create the merge list from the first non null input list.
      {
        var list = listArray.FirstOrDefault(c => c != null);
        if (list != null)
          merge = list.NewList();
      }

      // load itemArray with the first item from each of the listArray lists.
      var itemArray = new ListItem<ILogItem>[listArray.Length];
      {
        for (int ix = 0; ix < listArray.Length; ++ix)
        {
          itemArray[ix] = listArray[ix]?.NextItem(null);
        }
      }

      // find the itemArray item which is lowest in LogTime order.
      while (itemArray.FirstNonNull() != null)
      {
        int curIx = 0;
        ListItem<ILogItem> curItem = null;

        for (int ix = 0; ix < itemArray.Length; ++ix)
        {
          if (curItem == null)
          {
            curItem = itemArray[ix];
            curIx = ix;
          }
          else
          {
            var item5 = itemArray[ix];
            if ((item5 != null) && (item5.Value.LogTime <= curItem.Value.LogTime))
            {
              curItem = item5;
              curIx = ix;
            }
          }
        }

        if (curItem == null)
          break;

        {
          var logItem = curItem.Value.NewCopy();
          logItem.ApppendMergeName(listArray[curIx]);
          merge.AddItem(logItem);
          itemArray[curIx] = listArray[curIx].NextItem(curItem);
        }
      }

      return merge;
    }
    private static ListItem<ILogItem> Merge_AddItem(
      this ILogList MergeList, ILogList FromList, ListItem<ILogItem> Item)
    {
      var addItem = Item.Value.NewCopy();
      addItem.ApppendMergeName(FromList);

      MergeList.AddItem(addItem);

      var nextItem = FromList.NextItem(Item);
      return nextItem;
    }

    public static ListItem<ILogItem> NextItem(this ILogList List, ListItem<ILogItem> Current)
    {
      ListItem<ILogItem> item = Current;

      int ix = 0;
      if (item == null)
      {
        ix = 0;
        item = new ListItem<ILogItem>();
      }
      else if (item.Index == null)
        ix = 0;
      else
        ix = item.Index.Value + 1;

      if (ix < List.Count)
      {
        item.Index = ix;
        item.Value = List.GetItem(ix);
      }
      else
      {
        item = null;
      }

      return item;
    }

    public static IEnumerable<string> ToColumnReport(
      this ILogList LogList, string Title = null)
    {
      var report = new ColumnReport();
      if (Title != null)
        report.WriteTextLine(Title);
      else
      {
        var title = LogList.GetListTitle();
        if (title.IsNullOrEmpty() == false)
          report.WriteTextLine(title);
      } 

      report.AddColDefn("LogTime", 12, WhichSide.Left);
      report.AddColDefn("Source");
      report.AddColDefn("Message text", -1);

      report.WriteColumnHeading();

      foreach (var item in LogList.LogItems())
      {
        var valueList = new string[]
        {
          item.LogTime.ToString("HH:mm:ss.fff"),
          item.MergeName,
          item.LogText
        };

        if ( item.NewGroup == true )
        {
          Array.Clear(valueList, 0, valueList.Length);
        }

        report.WriteDetail(valueList);
      }

      return report;
    }
  }
}
