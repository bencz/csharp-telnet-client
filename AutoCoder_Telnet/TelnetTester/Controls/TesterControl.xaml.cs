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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TelnetTester.Controls
{
  /// <summary>
  /// Interaction logic for TesterControl.xaml
  /// </summary>
  public partial class TesterControl : UserControl
  {
    public TesterControl()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public string CustName
    {
      get { return (string)GetValue(CustNameProperty); }
      set { SetValue(CustNameProperty, value); }
    }

    public static readonly DependencyProperty CustNameProperty =
    DependencyProperty.Register("CustName", typeof(string),
    typeof(TesterControl), new PropertyMetadata(""));
  }
}
