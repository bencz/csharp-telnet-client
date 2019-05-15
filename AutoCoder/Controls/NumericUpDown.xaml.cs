using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
  /// Interaction logic for NumericUpDown.xaml
  /// </summary>
  public partial class NumericUpDown : UserControl
  {
    public bool IsReadOnly {  get { return false; } }
    public int NumericValue
    {
      get { return (int)GetValue(NumericValueProperty); }
      set { SetValue(NumericValueProperty, value); }
    }

    public static readonly DependencyProperty NumericValueProperty =
    DependencyProperty.Register("NumericValue", typeof(int),
    typeof(NumericUpDown), new PropertyMetadata(0));

    public event Action<int> ValueChanged;

    public NumericUpDown()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    private void Repeat1_Click(object sender, RoutedEventArgs e)
    {
      RepeatButton but = sender as RepeatButton;
      if (but.Tag as string == "Up")
        this.NumericValue += 1;
      else if (but.Tag as string == "Down")
        this.NumericValue -= 1;

      // signal the value changed event.
      if (this.ValueChanged != null )
      {
        ValueChanged(this.NumericValue);
      }
    }

    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Down)
        this.NumericValue -= 1;
      else if (e.Key == Key.Up)
        this.NumericValue += 1;
    }
  }
}
