using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoCoder.Ext
{
  public static class TextBoxExt
  {
    /// <summary>
    /// update the binding source of the text property of a TextBox.
    /// </summary>
    /// <param name="TextBox"></param>
    public static void UpdateTextBindingSource(this TextBox TextBox)
    {
      BindingExpression be = TextBox.GetBindingExpression(TextBox.TextProperty);
      if (be != null)
      {
        be.UpdateSource();
      }
    }
  }
}
