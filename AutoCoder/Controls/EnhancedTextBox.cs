using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace AutoCoder.Controls
{
  public class EnhancedTextBox : TextBox
  {
    public EnhancedTextBox( )
      : base( )
    {
      this.Loaded += EnhancedTextBox_Loaded;
    }

    private void EnhancedTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      this.PreviewKeyDown += EnhancedTextBox_PreviewKeyDown;
    }

    private void EnhancedTextBox_PreviewKeyDown(
      object sender, System.Windows.Input.KeyEventArgs e)
    {

    }
  }
}
