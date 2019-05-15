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
using System.Windows.Shapes;
using AutoCoder.Ext;
using AutoCoder.Ehllapi;
using Microsoft.Win32;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.Ehllapi.Settings
{
  /// <summary>
  /// Interaction logic for EhllapiSettingsEntry.xaml
  /// </summary>
  public partial class EhllapiSettingsEntry : Window
  {
    public bool CancelMode
    { get; set; }

    public EhllapiSettings EhllapiSettings
    {
      get;
      set;
    }

    public EhllapiSettingsEntry()
    {
      this.CancelMode = false;

      InitializeComponent();
      this.Loaded += new RoutedEventHandler(EhlappiSettingsEntry_Loaded);
      this.Closed += new EventHandler(EhllapiSettingsEntry_Closed);
    }

    void EhllapiSettingsEntry_Closed(object sender, EventArgs e)
    {
      if (this.CancelMode == false)
      {
        var settings = EhllapiSettings.RecallSettings();
        settings.SystemName = this.EhllapiSettings.SystemName;
        settings.UserName = this.EhllapiSettings.UserName;
        settings.Password = this.EhllapiSettings.Password;
        settings.SessId = this.EhllapiSettings.SessId;
        settings.Path_WrkstnProfile = this.EhllapiSettings.Path_WrkstnProfile;
        settings.Path_cwblogon = this.EhllapiSettings.Path_cwblogon;
        settings.Path_pcsws = this.EhllapiSettings.Path_pcsws;
        settings.DirPath_Emulator = this.EhllapiSettings.DirPath_Emulator;
        settings.StoreSettings();
      }
    }

    void EhlappiSettingsEntry_Loaded(object sender, RoutedEventArgs e)
    {
      this.EhllapiSettings = EhllapiSettings.RecallSettings();
      grdSettingsPrompt.DataContext = this.EhllapiSettings;
    }

    private void butWrkstnProfile_Click(object sender, RoutedEventArgs e)
    {
      const string strFilter = "Wrkstn profile files (*.ws)|" +
                               "*.WS";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;

      dlg.InitialDirectory = OpenFileDialog_SetInitialDirectory(
        this.EhllapiSettings.Path_WrkstnProfile,
        EhllapiSettings.DefaultPath_WrkstnProfile);

      var rc = dlg.ShowDialog();
      if ((rc != null) && (rc.Value == true))
        this.EhllapiSettings.Path_WrkstnProfile = dlg.FileName;
    }

    private void butCwblogon_Click(object sender, RoutedEventArgs e)
    {
      const string strFilter = "cwblogon.exe file|" +
                               "cwblogon.exe";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;

      dlg.InitialDirectory = OpenFileDialog_SetInitialDirectory(
        this.EhllapiSettings.Path_cwblogon,
        EhllapiSettings.DefaultPath_cwblogon);

      var rc = dlg.ShowDialog();
      if ((rc != null) && (rc.Value == true))
        this.EhllapiSettings.Path_cwblogon = dlg.FileName;
    }

    private string OpenFileDialog_SetInitialDirectory(string Path, string Default)
    {
      string ext = System.IO.Path.GetExtension(Path);
      if (ext.IsNullOrEmpty())
      {
        if (System.IO.Directory.Exists(Path) == false)
          Path = "";
        return Path;
      }
      else
      {
        if (System.IO.File.Exists(Path) == false)
          return "";
        else
          return System.IO.Path.GetDirectoryName(Path);
      }
    }

    private void butPcsws_Click(object sender, RoutedEventArgs e)
    {
      const string strFilter = "pcsws.exe file|" +
                               "pcsws.exe";
      OpenFileDialog dlg = new OpenFileDialog();
      dlg.Filter = strFilter;
      
      dlg.InitialDirectory = OpenFileDialog_SetInitialDirectory(
        this.EhllapiSettings.Path_pcsws,
        EhllapiSettings.DefaultPath_pcsws);

      var rc = dlg.ShowDialog();
      if ((rc != null) && (rc.Value == true))
        this.EhllapiSettings.Path_pcsws = dlg.FileName;
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem mi = null;
      if (sender is MenuItem)
        mi = sender as MenuItem;

      if (mi != null)
      {
        if ((string)mi.Header == "Test")
        {
        }

        else if ((string)mi.Header == "Exit")
        {
          this.Close();
        }
      }
    }

    private void butOK_Click(object sender, RoutedEventArgs e)
    {
      this.CancelMode = false;
      this.Close();
    }

    private void butCancel_Click(object sender, RoutedEventArgs e)
    {
      this.CancelMode = true;
      this.Close();
    }


    void Menu_Session_Start(object InObj, RoutedEventArgs InArgs)
    {
      EhllapiSettings settings = EhllapiSettings.RecallSettings();
      SessionScript.Launch_LoadLogonCache(settings);
      SessionScript.Launch_ClientAccessSession(settings);
    }

    void Menu_Session_End(object InObj, RoutedEventArgs InArgs)
    {
      EhllapiSettings settings = EhllapiSettings.RecallSettings();
      SessionScript.End_ClientAccessSession(settings);
    }

    void Menu_Script_Signon(object InObj, RoutedEventArgs InArgs)
    {
      SessionScript script = new SessionScript();
      EhllapiSettings settings = EhllapiSettings.RecallSettings();

      SessionScript.Assure_ClientAccessSession(settings);
      script.Play_Signon(settings);

#if skip
      var lines = SessionScript.ReadPresentationSpace(settings).Lines;
      StringBuilder sb = new StringBuilder();
      foreach (var line in lines)
      {
        sb.Append(line + Environment.NewLine);
      }
      MessageBox.Show(sb.ToString());
#endif
    }

    void Menu_Script_Signoff(object InObj, RoutedEventArgs InArgs)
    {
      SessionScript script = new SessionScript();
      EhllapiSettings settings = EhllapiSettings.RecallSettings();

      SessionScript.Assure_ClientAccessSession(settings);
      script.Play_Signon(settings);

#if skip
      var lines = SessionScript.ReadPresentationSpace(settings).Lines;
      StringBuilder sb = new StringBuilder();
      foreach (var line in lines)
      {
        sb.Append(line + Environment.NewLine);
      }
      MessageBox.Show(sb.ToString());
#endif
    }

    private void butEmulatorDirPath_Click(object sender, RoutedEventArgs e)
    {
      var dlg = new System.Windows.Forms.FolderBrowserDialog();
      dlg.ShowNewFolderButton = false;
      dlg.Description = "Emulator director";

        dlg.SelectedPath = this.EhllapiSettings.DirPath_Emulator;

      var rv = dlg.ShowDialog();
      if (rv == System.Windows.Forms.DialogResult.OK)
      {
        this.EhllapiSettings.DirPath_Emulator = dlg.SelectedPath;
      }
    }
  }
}
