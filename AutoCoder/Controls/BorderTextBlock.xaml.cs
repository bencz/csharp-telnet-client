using AutoCoder.Ext.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoCoder.Controls
{
    /// <summary>
  /// BorderTextBlock is a textblock with a border.
  /// </summary>
  public partial class BorderTextBlock : UserControl
  {
    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string), typeof(BorderTextBlock), new PropertyMetadata(""));

    /// <summary>
    /// do not draw the vertical left border line. Default is false. Use when
    /// there is another BorderTextBlock immed to the left of this control. Set
    /// to not draw the left border line.
    /// </summary>
    public bool? NoLeftBorder
    {
      get { return (bool?)GetValue(NoLeftBorderProperty); }
      set { SetValue(NoLeftBorderProperty, value); }
    }

    // Using a DependencyProperty as the backing store for NoLeftBorder.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty NoLeftBorderProperty =
        DependencyProperty.Register("NoLeftBorder", typeof(bool?), typeof(BorderTextBlock), 
          new PropertyMetadata(null, OnNoLeftBorderPropertyChanged));

    private static void OnNoLeftBorderPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = sender as BorderTextBlock;
      if (e.NewValue != null)
      {
        bool? newValue = e.NewValue as bool?;
        thisControl.SetBorder();
      }
    }


    public BorderTextBlock()
    {
      InitializeComponent();

      // set the DataContent to this.  Enables child controls to bind to 
      // properties of this control.
      border1.DataContext = this;
    }

    private void SetBorder( )
    {
      if ( this.NoLeftBorder == true )
      {
        // get current border thickness.
        var th = border1.BorderThickness;

        // set thickness of each side of the border. Only left side is zero.
        border1.BorderThickness = new Thickness(0, th.Top, th.Right, th.Bottom);
      }
    }
  }
}
