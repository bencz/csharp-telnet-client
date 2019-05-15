using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class ListBoxExt
  {
    public static void FindFocusItem(this ListBox lv, object Item)
    {
      // find the item in the ListBox
      var ix = lv.Items.IndexOf(Item);
      if (ix != -1)
      {
        // set as the selected item.
        lv.SelectedIndex = ix;

        // set focus to the selected item. ( need focus on the selected item in
        // order for the up and down arrow to navigate the list. )
        var uiElement = lv.GetContainerFromItem(Item);
        if (uiElement != null)
          uiElement.Focus();
      }
    }

    public static UIElement GetContainerFromItem(this ListBox lv, object Item)
    {
      var container = lv.ItemContainerGenerator.ContainerFromItem(Item);
      if (container == null)
      {
        lv.UpdateLayout();
        container = lv.ItemContainerGenerator.ContainerFromItem(Item);
      }
      return container as UIElement;
    }
  }
}
