using AutoCoder.Telnet.Common.ScreenDm;
using AutoCoder.Telnet.Enums;
using System.Diagnostics;
using System.Windows;

namespace TelnetTester.Windows
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {

    public string FirstName
    { get; set; }
    public DsplyAttr dsplyAttr
    { get; set; }

    public ScreenDimModel Dim
    { get; set; }
    public Window1()
    {
      InitializeComponent();

      this.dsplyAttr = DsplyAttr.HI;
      this.FirstName = "Steve";
      this.Dim = new ScreenDimModel(new ScreenDim(24, 80));
      this.LayoutRoot.DataContext = this;
    }

    private void butOK_Click(object sender, RoutedEventArgs e)
    {
      DsplyAttr v1;
      v1 = (DsplyAttr)radioButton1.EnumValue;
      Debug.Print( "dsply attr:" + v1.ToString( ));
    }
  }
}
