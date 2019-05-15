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
using AutoCoder.Ehllapi.Settings;
using AutoCoder.Ehllapi;
using AutoCoder.Ehllapi.CommonScreens;
using Ehllapi;

namespace Tester
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      string itemText = null;
      if (sender is MenuItem)
        itemText = (sender as MenuItem).Header as string;

      if (itemText == "Test")
      {
        SignoffTester();
        return;
        SessionTester();
      }

      else if (itemText == "Exit")
      {
        this.Close();
      }

      else if (itemText == "Settings")
      {
        EhllapiSettings settings = EhllapiSettings.RecallSettings();
        var wdw = new EhllapiSettingsEntry();
        wdw.ShowDialog();
      }
    }

    private void SignoffTester()
    {
      AutoCoder.Ehllapi.CommonScreens.SignonScreen.AssureSignedOff("A");
    }

    private void SessionTester( )
    {
      var ehSettings = EhllapiSettings.RecallSettings();

      // make sure the session is active.
      SessionScript.Assure_ClientAccessSession(ehSettings);

      // bring the 5250 window to the foreground.
      Ehllapier.SetForegroundWindow(ehSettings.SessId);

      // make sure signed on.
      if (SignonScreen.IsScreen(ehSettings))
      {
        var script = new SessionScript();
        script.Play_Signon(ehSettings);
      }

      // in the strseu screen. Exit back to presumably wrkmbrpdm.
      if (StrseuScreen.EditScreen.IsScreen(ehSettings))
      {
        StrseuScreen.EditScreen.F3_Exit(ehSettings);

        if (StrseuScreen.ExitScreen.IsScreen(ehSettings))
        {
          StrseuScreen.ExitScreen.Enter(ehSettings);
        }
      }

      // in the seu browse screen. press enter to exit.
      if (StrseuScreen.BrowseScreen.IsScreen(ehSettings))
      {
        StrseuScreen.BrowseScreen.Enter_Exit(ehSettings);
      }

      using (DisplaySession sess = new DisplaySession())
      {
        bool isScreen = false;
        sess.Connect(ehSettings.SessId);

        // display messages. press enter.
        if (DisplayMessagesScreen.IsScreen(sess))
        {
          sess.SendKeys(KeyboardKey.Enter);
        }

      }
    }

  }
}
