using ScreenDefnLib.Models;
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

namespace ScreenDefnLib.Controls
{
  /// <summary>
  /// Interaction logic for AddScreenDefnControl.xaml
  /// </summary>
  public partial class AddScreenDefnWindow : Window
  {

    public WorkScreenDefnModel WorkScreenDefnModel
    {
      get { return (WorkScreenDefnModel)GetValue(WorkScreenDefnModelProperty); }
      set { SetValue(WorkScreenDefnModelProperty, value); }
    }

    public static readonly DependencyProperty WorkScreenDefnModelProperty =
    DependencyProperty.Register(
      "WorkScreenDefnModel", typeof(WorkScreenDefnModel),
      typeof(AddScreenDefnWindow),
      new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public WorkScreenDefnModel Model { get; set; }

    public AddScreenDefnWindow()
    {
      InitializeComponent();
      this.Loaded += AddScreenDefnWindow_Loaded;
      this.PreviewKeyDown += AddScreenDefnWindow_PreviewKeyDown;
    }

    private void AddScreenDefnWindow_Loaded(object sender, RoutedEventArgs e)
    {
      this.Model = new WorkScreenDefnModel(this.WorkScreenDefnModel);
      this.DataContext = this.Model;
    }

    private void AddScreenDefnWindow_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Escape)
        DialogResult = false;
      else if (e.Key == Key.Enter)
        AcceptEntry();
    }

    private void AcceptEntry()
    {
      this.WorkScreenDefnModel.Apply(this.Model);
      DialogResult = true;
    }

    private void butAdd_Click(object sender, RoutedEventArgs e)
    {
      AcceptEntry();
    }
  }
}
