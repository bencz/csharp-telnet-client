using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TextCanvasLib.Hover
{
  public class HoverWindow : Window
  {
    public HoverWindow( )
      : base( )
    {
      this.Loaded += HoverWindow_Loaded;
    }

    private void HoverWindow_Loaded(object sender, RoutedEventArgs e)
    {
    }
  }
}
