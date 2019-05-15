using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace AutoCoder.Ext.Windows.Controls
{
  /// <summary>
  /// methods that extend TreeViewItem
  /// </summary>
  public static class TreeViewItemExt
  {
    /// <summary>
    /// Add a dummy, stub item to this TreeViewItem to make the item expandable.
    /// In the Expanded event handler, check for the stub sub item. If it exists,
    /// remove it and replace with the actual sub items.
    /// </summary>
    /// <param name="InItem"></param>
    public static void AddExpandableStub(this TreeViewItem Item)
    {
      TreeViewItem subItem = new TreeViewItem();
      string s1 = "stub";
      Item.Items.Add(s1);
    }

    /// <summary>
    /// collapse all the child items of the TreeViewItem.
    /// </summary>
    /// <param name="Item"></param>
    public static void CollapseAll(this TreeViewItem Item)
    {
      foreach (var item in Item.Items)
      {
        var tvItem = item as TreeViewItem;
        if (tvItem != null)
        {
          if (tvItem.IsExpanded)
            tvItem.IsExpanded = false;
          tvItem.CollapseAll();
        }
      }
    }

    /// <summary>
    /// Test if this TreeViewItem contains a single sub item added by the 
    /// AddExpandableStub method.
    /// </summary>
    /// <param name="InItem"></param>
    /// <returns></returns>
    public static bool HasExpandableStub(this TreeViewItem Item)
    {
      if ((Item.Items.Count == 1) && (Item.Items[0] is string) &&
        ((Item.Items[0] as string) == "stub"))
        return true;
      else
        return false;
    }
  }
}
