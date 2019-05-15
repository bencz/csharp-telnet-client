using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace AutoCoder.Forms
{
  public static class ListBoxer
  {
    /// <summary>
    /// set the SelectedIndex of the ListBox to the item closest to the
    /// specified index.
    /// </summary>
    /// <param name="InListBox"></param>
    /// <param name="InIndex"></param>
    /// <returns></returns>
    public static int SelectClosestItem(ListBox InListBox, int InIndex)
    {
      int ix = InIndex;
      if (ix < 0)
        ix = 0;
      if (ix > InListBox.Items.Count - 1)
        ix = InListBox.Items.Count - 1;
      InListBox.SelectedIndex = ix;
      return ix;
    }
  }
}
