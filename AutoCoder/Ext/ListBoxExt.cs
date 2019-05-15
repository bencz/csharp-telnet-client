using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using AutoCoder.Core.Enums;

namespace AutoCoder.Ext
{
  public static class ListBoxExt
  {
    // increment or decrement the SelectedIndex of the ListBox.
    // When the resulting SelectedIndex exceeds the bounds of the ListBox, wrap
    // around to the first or last item.
    public static void AdvanceSelectedIndex(this ListBox ListBox, AdvanceDirection Dir)
    {
      int nbrItems = ListBox.Items.Count;
      if (nbrItems > 0)
      {
        int itemNx = ListBox.SelectedIndex;
        if (Dir == AdvanceDirection.Forward)
        {
          itemNx += 1;
          if (itemNx >= nbrItems)
            itemNx = 0;
        }
        else
        {
          itemNx -= 1;
          if (itemNx <= 0)
            itemNx = nbrItems - 1;
        }

        ListBox.SelectedIndex = itemNx;
      }
    }
  }
}
