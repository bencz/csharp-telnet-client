using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace AutoCoder.Controls
{
  public class EnhancedListBox : ListBox
  {
    /// <summary>
    /// ListBox with some enhanced features. CTRL-INSERT copies selected items to the 
    /// clipboard.
    /// </summary>
    public EnhancedListBox()
      : base()
    {
      this.PreviewKeyDown += NewListBox_PreviewKeyDown;
    }
    private void NewListBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
      // copy selected items to clipboard.
      if (e.Key == Key.Insert)
      {
        if (Keyboard.IsKeyDown(Key.LeftCtrl))
        {
          StringBuilder sb = new StringBuilder();
          foreach (object item in this.SelectedItems)
          {
            sb.Append(item as string + Environment.NewLine);
          }
          Clipboard.SetText(sb.ToString());
        }
      }
    }
  }
}
