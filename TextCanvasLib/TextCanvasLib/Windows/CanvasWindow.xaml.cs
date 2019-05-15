using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenDm;
using System;
using System.Collections.Generic;
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
using TextCanvasLib.Canvas;

namespace TextCanvasLib.Windows
{
  /// <summary>
  /// Interaction logic for WindowWindow.xaml
  /// </summary>
  public partial class CanvasWindow : Window
  {

    public ScreenDim WindowDim
    { get; set; }

    public double PointSize
    { get; set; }

    /// <summary>
    /// row/col location of this window within the parent window.
    /// </summary>
    public ZeroRowCol RowCol
    { get; set; }
    public CanvasDefn CanvasDefn
    {
      get; set;
    }

    /// <summary>
    /// the ItemCanvas of the parent canvas of this CanvasWindow.
    /// </summary>
    public ItemCanvas ParentItemCanvas
    { get; set; }
    public ItemCanvas WindowItemCanvas
    { get; set; }

    private Size DotDim
    { get; set;  }
    public CanvasWindow(ScreenDim WindowDim)
    {
      this.WindowDim = WindowDim;

      InitializeComponent();
      this.Closed += CanvasWindow_Closed;
      this.Loaded += CanvasWindow_Loaded;
      this.PreviewKeyDown += CanvasWindow_PreviewKeyDown;
    }

    private void CanvasWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        this.Close();
    }

    private void CanvasWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.CanvasDefn = new CanvasDefn(this.WindowDim, this.PointSize);
      this.DotDim = WindowDim.ToCanvasDim(this.CanvasDefn.CharBoxDim);
      this.WindowStyle = WindowStyle.None;

      var canvasChild = this.WindowItemCanvas.Canvas;
      canvasChild.Height = this.DotDim.Height;
      canvasChild.Width = this.DotDim.Width;
      this.SizeToContent = SizeToContent.WidthAndHeight;

      SetStartupLocation();
    }

    private void SetStartupLocation( )
    {
      var parentCanvas = this.ParentItemCanvas.Canvas;
      var feParent = parentCanvas as FrameworkElement;
      while(feParent != null)
      {
        feParent = feParent.Parent as FrameworkElement;
        if ((feParent != null) && (feParent is Window))
          break;
      }

      if ( feParent != null)
      {
        var parentWindow = feParent as Window;
        this.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;

        var winDotPos = this.RowCol.ToCanvasPos(this.CanvasDefn.CharBoxDim);

        this.Left = parentWindow.Left + winDotPos.X;
        this.Top = parentWindow.Top + winDotPos.Y;
      }
    }

    private void CanvasWindow_Closed(object sender, EventArgs e)
    {
    }

    public void SetItemCanvas(ItemCanvas ItemCanvas)
    {
      this.WindowItemCanvas = ItemCanvas;

      System.Windows.Controls.Canvas canvasChild = null;
      foreach( var item in this.LayoutRoot.Children)
      {
        if ( item is System.Windows.Controls.Canvas )
        {
          canvasChild = item as System.Windows.Controls.Canvas;
          break;
        }
      }

      this.LayoutRoot.Children.Remove(canvasChild);

      canvasChild = ItemCanvas.Canvas;
      this.LayoutRoot.Children.Add(canvasChild);
      Grid.SetRow(canvasChild, 0);
      Grid.SetColumn(canvasChild, 0);
      canvasChild.Background = Brushes.Black;

      canvasChild.Focusable = true;
      canvasChild.IsEnabled = true;
      canvasChild.Focus();

    }
  }
}
