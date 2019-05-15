using AutoCoder.Telnet.Common.ScreenDm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AutoCoder.Telnet.Controls
{
  /// <summary>
  /// Interaction logic for ScreenDimControl.xaml
  /// </summary>
  public partial class ScreenDimControl : UserControl, INotifyPropertyChanged
  {
    public ScreenDimControl()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public IScreenDim ScreenDim
    {
      get { return (IScreenDim)GetValue(ScreenDimProperty); }
      set { SetValue(ScreenDimProperty, value); }
    }

    public static readonly DependencyProperty ScreenDimProperty =
    DependencyProperty.Register(
      "ScreenDim", typeof(IScreenDim),
      typeof(ScreenDimControl),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
        OnScreenDimPropertyChanged));

    private static void OnScreenDimPropertyChanged(
       DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
      var thisControl = sender as ScreenDimControl;
      if (e.NewValue != null)
      {
        var newValue = e.NewValue as IScreenDim;
        if (newValue.CompareEquals(thisControl.wasScreenDim) == false)
        {
          thisControl.wasScreenDim = newValue;
          var isWide = newValue.GetIsWideScreen();
          thisControl.WideScreenChecked = isWide;
        }
      }
    }
    private IScreenDim wasScreenDim
    { get; set;  }


    public bool WideScreenChecked
    {
      get
      {
        _WideScreenChecked = this.ScreenDim.GetIsWideScreen();
        return _WideScreenChecked;
      }
      set
      {
        if (_WideScreenChecked != value)
        {
          _WideScreenChecked = value;
          RaisePropertyChanged("WideScreenChecked");

          if ((_WideScreenChecked == true) && (this.ScreenDim.GetIsWideScreen() == false))
            this.ScreenDim = new ScreenDimModel(new ScreenDim(27, 132));
          else if ((_WideScreenChecked == false) && (this.ScreenDim.GetIsWideScreen() == true))
            this.ScreenDim = new ScreenDimModel(new ScreenDim(24, 80));
        }
      }
    }
    private bool _WideScreenChecked;

    public event PropertyChangedEventHandler PropertyChanged;
    protected void RaisePropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null)
      {
        handler(this, new PropertyChangedEventArgs(name));
      }
    }
  }
}
