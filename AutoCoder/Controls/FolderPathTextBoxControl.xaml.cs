using AutoCoder.Ext.System;
using Microsoft.Win32;
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
  /// Interaction logic for FolderPathTextBoxControl.xaml
  /// </summary>
  public partial class FolderPathTextBoxControl : UserControl
  {
    public string FolderPathText
    {
      get { return (string)GetValue(FolderPathTextProperty); }
      set { SetValue(FolderPathTextProperty, value); }
    }

    public static readonly DependencyProperty FolderPathTextProperty =
    DependencyProperty.Register(
      "FolderPathText", typeof(string),
      typeof(FolderPathTextBoxControl),
      new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public FolderPathTextBoxControl()
    {
      InitializeComponent();
      LayoutRoot.DataContext = this;
    }

    public string GetFolderPathText()
    {
      return this.FolderPathText;
    }

    private void butOpenFolderDialog_Click(object sender, RoutedEventArgs e)
    {
      var dlg = new System.Windows.Forms.FolderBrowserDialog();
      dlg.ShowNewFolderButton = true;
      dlg.Description = 
        "Select the folder in which to store captured screen data";
      dlg.SelectedPath = this.GetFolderPathText();
      var rv = dlg.ShowDialog();
      if (rv == System.Windows.Forms.DialogResult.OK)
      {
        this.FolderPathText = dlg.SelectedPath.TrimWhitespace();
      }
    }
  }
}
