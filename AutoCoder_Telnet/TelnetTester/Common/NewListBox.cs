using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TelnetTester.Common
{
  public class NewListBox : ListBox
  {
    public NewListBox( )
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
