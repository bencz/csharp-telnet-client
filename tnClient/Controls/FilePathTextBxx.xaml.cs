using AutoCoder.Ext.System;
using Microsoft.Win32;
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

namespace tnClient.Controls
{
  /// <summary>
  /// Interaction logic for FilePathTextBxx.xaml
  /// </summary>
  public partial class FilePathTextBxx : UserControl
  {
    public string FilePathText
    {
      get { return (string)GetValue(FilePathTextProperty); }
      set { SetValue(FilePathTextProperty, value); }
    }

    public static readonly DependencyProperty FilePathTextProperty =
    DependencyProperty.Register(
      "FilePathText", typeof(string),
      typeof(FilePathTextBxx),
      new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


    public string Filter
    {
      get { return (string)GetValue(FilterProperty); }
      set { SetValue(FilterProperty, value); }
    }

    public static readonly DependencyProperty FilterProperty =
    DependencyProperty.Register("Filter", typeof(string),
    typeof(FilePathTextBxx), new PropertyMetadata(""));

    public FilePathTextBxx()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    private void butOpenFileDialog_Click(object sender, RoutedEventArgs e)
    {
      var openDialog = new OpenFileDialog();

      if (this.Filter.IsNullOrEmpty())
        openDialog.Filter = "txt Files |*.txt|All Files |*.*";
      else
        openDialog.Filter = this.Filter;

      openDialog.CheckFileExists = false;
      var rc = openDialog.ShowDialog();
      if ((rc != null) && (rc.Value == true))
      {
        this.FilePathText = openDialog.FileName;
      }
    }

    public string GetFilePathText( )
    {
      return this.FilePathText;
    }
  }
}
