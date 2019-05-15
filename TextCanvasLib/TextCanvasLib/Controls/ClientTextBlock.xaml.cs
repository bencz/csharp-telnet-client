using AutoCoder.Ext.Windows.Media;
using AutoCoder.Telnet.Common;
using AutoCoder.Telnet.Common.RowCol;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TextCanvasLib.Common;

namespace TextCanvasLib.Controls
{
  /// <summary>
  /// Interaction logic for ClientTextBlock.xaml
  /// ClientTextBlock is a TextBlock with a DrawUnderline method.
  /// </summary>
  public partial class ClientTextBlock : UserControl
  {
    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set
      {
        SetValue(TextProperty, value);
      }
    }

    public static readonly DependencyProperty TextProperty =
    DependencyProperty.Register("Text", typeof(string),
    typeof(ClientTextBlock), new PropertyMetadata(""));

    /// <summary>
    /// the dot dim of each char drawn on the canvas. To increase the spacing
    /// between characters on the canvas, keep the KernDim fixed and increase the
    /// box dim.
    /// </summary>
    private Size CharBoxDim
    { get; set; }

    /// <summary>
    /// the actual dim of the char. Does not include the spacing after and below
    /// the character. That is what CharBoxDim is for.
    /// </summary>
    private Size KernDim
    { get; set; }

    private Line Underline
    { get; set; }

    public ClientTextBlock( Size CharBoxDim, Size KernDim)
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
      this.CharBoxDim = CharBoxDim;
      this.KernDim = KernDim;
    }

    public void ChangeSizeBasis(Size CharBoxDim, Size KernDim)
    {
      this.CharBoxDim = CharBoxDim;
      this.KernDim = KernDim;
      if (this.Underline != null)
      {
        RemoveUnderline();
        DrawUnderline();
      }
    }

    /// <summary>
    /// the from and to point coordinates of the underline line. Points being
    /// relative to the canvas of this clientTextBlock control.
    /// </summary>
    public Tuple<Point, Point> UnderlineCoordinates
    {
      get
      {
        double vertPos = this.KernDim.Height - 2;

        // from point starts at zero. and then down by the height of the character
        var fromPoint = new Point(0, vertPos);

        // to point is the length of the text * the box width of each char.
        // ( actually the length of the text - 1. want to calc the end pos of the
        //   2nd to last char in the textblock. )
        var endRowCol = new ZeroRowCol(0, this.Text.Length - 1);
        var horzPos = endRowCol.ToCanvasPos(this.CharBoxDim).X;

        // then add the to point by the actual draw width of each character. 
        var toPoint = new Point(horzPos + this.KernDim.Width, vertPos);

        return new Tuple<Point, Point>(fromPoint, toPoint);
      }
    }
    public Line DrawUnderline( )
    {
      this.RemoveUnderline();

      var color = Colors.DarkGreen;
      color = color.AdjustColor(0.4f);
      var brush = new SolidColorBrush(color);

      var line = new Line();
      line.Stroke = brush;
      line.Fill = brush;
      line.StrokeLineJoin = PenLineJoin.Bevel;

      var coor = UnderlineCoordinates;
      line.X1 = coor.Item1.X;
      line.Y1 = coor.Item1.Y;
      line.X2 = coor.Item2.X;
      line.Y2 = coor.Item2.Y;

      line.StrokeThickness = 1.3;
      line.SnapsToDevicePixels = true;
      this.Underline = line;
      LayoutRoot.Children.Add(line);

      return line;
    }

    public void RemoveUnderline( )
    {
      if ( this.Underline != null)
      {
        LayoutRoot.Children.Remove(this.Underline);
      }
    }
  }

  public static class ClientTextBlockExt
  {
    public static void SetFontDefn(this ClientTextBlock TextBlock, FontDefn FontDefn)
    {
      TextBlock.FontFamily = FontDefn.Family;
      TextBlock.FontSize = FontDefn.PointSize;
      TextBlock.FontWeight = FontDefn.Weight;
      TextBlock.Foreground = FontDefn.Foreground;
    }
  }

}
