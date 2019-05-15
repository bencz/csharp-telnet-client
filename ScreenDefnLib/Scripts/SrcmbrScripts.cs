using AutoCoder.Ext.Windows.Controls;
using AutoCoder.Telnet.Settings;
using ScreenDefnLib.Defn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ScreenDefnLib.Scripts
{
  public static class SrcmbrScripts
  {
    public static Grid SrcmbrHoverHandler(string SrcfName, string SrcfLib, string Srcmbr)
    {
      ObservableCollection<string> Lines = new ObservableCollection<string>();

      SrcmbrModel model = new SrcmbrModel()
      {
        Lines = Lines,
        SrcfName = SrcfName,
        Srcmbr = Srcmbr
      };

      var lines = GetSrcmbrLines(SrcfName, SrcfLib, Srcmbr);
      foreach (var line in lines)
      {
        Lines.Add(line);
      }

      var grid = BuildControls(model);

      return grid;
    }

#if skip
    public static Grid SrcmbrHoverHandler(DataTable ContentTable, ScreenItemInstance Instance)
    {
      string srcmbr, srcfName, srcfLib;
      ObservableCollection<string> Lines = new ObservableCollection<string>();

      {
        var rv = GetSrcmbrName(ContentTable, Instance);
        srcfName = rv.Item1;
        srcfLib = rv.Item2;
        srcmbr = rv.Item3;
      }

      SrcmbrModel model = new SrcmbrModel()
      {
        Lines = Lines,
        SrcfName = srcfName,
        Srcmbr = srcmbr
      };

      var lines = GetSrcmbrLines(srcfName, srcfLib, srcmbr);
      foreach (var line in lines)
      {
        Lines.Add(line);
      }

      var grid = BuildControls(model);

      return grid;
    }
#endif

#if skip
    private static Tuple<string, string, string> GetSrcmbrName(
      DataTable contentTable, ScreenItemInstance instance)
    {
      // get the row of the table.
      int rownum = instance.RepeatNum;
      if (rownum == 0)
        rownum = 1;

      var row = contentTable.Rows[rownum - 1];
      var srcmbr = row["MbrName"] as string;
      var srcfName = row["SrcfName"] as string;
      var srcfLib = row["SrcfLib"] as string;

      return new Tuple<string, string, string>(srcfName, srcfLib, srcmbr);
    }
#endif

    private static Grid BuildControls(SrcmbrModel model)
    {
      Grid gridMain = new Grid();
      gridMain.ColumnDefinitions.AddStarColumn();
      gridMain.RowDefinitions.AddAutoRow();
      gridMain.RowDefinitions.AddAutoRow();
      gridMain.RowDefinitions.AddStarRow();
      gridMain.RowDefinitions.AddAutoRow();

      var sp = gridMain_AddSrcmbrNameRow(gridMain, model);
      gridMain.AddUIElement(sp, 0, 0);

      var but = gridMain.AddButton("butOk", "OK", 3, 0);
      but.Click += But_Click;
      but.HorizontalAlignment = HorizontalAlignment.Left;
      but.Margin = new Thickness(5);
      but.Padding = new Thickness(5, 0, 5, 0);

      var listBox1 = gridMain.AddListBox(null, 2, 0);
      listBox1.FontFamily = new System.Windows.Media.FontFamily("Lucida console");
      listBox1.FontSize = 12;
      listBox1.Padding = new Thickness(3);

      var binding = new Binding();
      binding.Source = model;
      binding.Path = new PropertyPath("Lines");
      binding.Mode = BindingMode.OneWay;
      BindingOperations.SetBinding(listBox1, ListBox.ItemsSourceProperty, binding);

      return gridMain;
    }

    private static StackPanel gridMain_AddSrcmbrNameRow(Grid gridMain, SrcmbrModel model)
    {
      var sp = new StackPanel();
      sp.Orientation = Orientation.Horizontal;
      sp.Background = Brushes.LightBlue;

      {
        var lab1 = new Label();
        lab1.Content = "Source file:";
        lab1.FontWeight = FontWeights.Bold;
        lab1.VerticalAlignment = VerticalAlignment.Center;
        lab1.Margin = new Thickness(0, 0, 3, 0);
        sp.Children.Add(lab1);
      }

      {
        var tb = new TextBlock();
        tb.VerticalAlignment = VerticalAlignment.Center;

        var binding = new Binding();
        binding.Source = model;
        binding.Path = new PropertyPath("SrcfName");
        binding.Mode = BindingMode.OneWay;
        BindingOperations.SetBinding(tb, TextBlock.TextProperty, binding);
        sp.Children.Add(tb);
      }

      {
        var lab1 = new Label();
        lab1.Content = "Source member:";
        lab1.FontWeight = FontWeights.Bold;
        lab1.Margin = new Thickness(3, 0, 3, 0);
        lab1.VerticalAlignment = VerticalAlignment.Center;
        sp.Children.Add(lab1);
      }

      {
        var tb = new TextBlock();
        tb.VerticalAlignment = VerticalAlignment.Center;

        var binding = new Binding();
        binding.Source = model;
        binding.Path = new PropertyPath("Srcmbr");
        binding.Mode = BindingMode.OneWay;
        BindingOperations.SetBinding(tb, TextBlock.TextProperty, binding);
        sp.Children.Add(tb);
      }

      return sp;
    }

    private static void But_Click(object sender, RoutedEventArgs e)
    {
    }

    public static IEnumerable<string> GetSrcmbrLines(
      string SrcfName, string SrcfLib, string SrcmbrName)
    {
      var userDsn = ClientSettings.GetCachedOdbcDsn();
      var userName = "srichter";
      var password = "denville";
      var lines = new List<string>();

      using (var conn = OpenConnection(userDsn, userName, password))
      {
        var aliasCmd = conn.CreateCommand();
        aliasCmd.CommandText = "create or replace alias qtemp.demo3r for " + SrcfLib +
          "." + SrcfName + "(" + SrcmbrName + ")";
        aliasCmd.ExecuteNonQuery();

        var cmd = conn.CreateCommand();
        cmd.CommandText = "select srcseq, srcdat, srcdta " +
          "from qtemp.demo3r " +
          " order by srcseq ";

        using (OdbcDataReader reader = cmd.ExecuteReader())
        {
          while (reader.Read())
          {
            string srcseq = reader.GetString(0);
            string srcdat = reader.GetString(1);
            string srcdta = reader.GetString(2);

            lines.Add(srcdta);
          }
        }
      }
      return lines;
    }

    private static OdbcConnection OpenConnection(string Dsn, string User, string Pwd)
    {
      string connString = "DSN=" + Dsn + "; UID=SRICHTER; PWD=DENVILLE;";
      var conn = new OdbcConnection(connString);
      conn.Open();
      return conn;
    }
  }

  public class SrcmbrModel
  {
    public IList<string> Lines { get; set; }
    public string SrcfName { get; set; }
    public string Srcmbr { get; set; }
  }
}

