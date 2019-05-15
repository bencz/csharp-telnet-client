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
using System.Windows.Navigation;
using System.Windows.Shapes;
using tnClient.Models;

namespace tnClient.Controls
{
  /// <summary>
  /// Interaction logic for ClientSettingsControl.xaml
  /// </summary>
  public partial class ClientSettingsControl : UserControl
  {
    public event Action<ClientModel> ApplySettings;
    public ClientSettingsControl()
    {
      InitializeComponent();
      this.Loaded += ClientSettingsControl_Loaded;
    }

    private void ClientSettingsControl_Loaded(object sender, RoutedEventArgs e)
    {
      this.LayoutRoot.DataContext = this;
    }

    public ClientModel Model
    {
      get { return (ClientModel)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
    DependencyProperty.Register("Model", typeof(ClientModel),
    typeof(ClientSettingsControl), new PropertyMetadata(null));

    private void butApply_Click(object sender, RoutedEventArgs e)
    {
      if (this.ApplySettings != null)
        this.ApplySettings(this.Model);
    }

    private void EditSupportCode_Click(object sender, RoutedEventArgs e)
    {
        string exePath =
          Environment.ExpandEnvironmentVariables(@"%windir%\system32\notepad.exe");
        Process.Start(exePath, this.Model.SupportCodeFilePath);
    }
  }
}
