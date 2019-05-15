using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace tnClient.Windows
{
  /// <summary>
  /// Interaction logic for CanvasDemo.xaml
  /// </summary>
  public partial class CanvasDemo : Window
  {
    public CanvasDemo()
    {
      InitializeComponent();
      this.Loaded += CanvasDemo_Loaded;
    }

    private void CanvasDemo_Loaded(object sender, RoutedEventArgs e)
    {
      this.Canvas1.Focusable = true;
      this.Canvas1.IsEnabled = true;
      this.Canvas1.Focus();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Test")
      {
        return;
      }

      else if (itemText == "Focus")
      {
        this.TabControl1.SelectedItem = TabItem1;
        this.TabControl1.SelectedIndex = 1;
        UpdateLayout();
        TabItem1.Focusable = true;
        TabItem1.IsEnabled = true;
        TabItem1.Focus();

        this.Canvas1.Focusable = true;
        this.Canvas1.IsEnabled = true;
        var bb = Keyboard.Focus(this.Canvas1);
        var xx = this.Canvas1.Focus();
      }

      else if (itemText == "Exit")
      {
        this.Close();
      }
    }

    private void Canvas1_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      char? keyChar = null;
      if (e.Key == Key.A)
        keyChar = 'A';
      else if (e.Key == Key.B)
        keyChar = 'B';
      else if (e.Key == Key.C)
        keyChar = 'C';

      if ( keyChar != null )
        this.TextBlock1.Text += keyChar.Value;
    }
  }
}
