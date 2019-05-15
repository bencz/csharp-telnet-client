using AutoCoder.Composite;
using AutoCoder.Enums;
using AutoCoder.Ext.Controls;
using AutoCoder.Ext.System;
using AutoCoder.Ext.Windows.Controls;
using AutoCoder.Code.Ext;
using AutoCoder.Serialize;
using AutoCoder.Systm.Data;
using AutoCoder.Telnet.Common.RowCol;
using AutoCoder.Telnet.Common.ScreenLoc;
using AutoCoder.Telnet.IBM5250.Content;
using AutoCoder.Telnet.Settings;
using ScreenDefnLib.Defn;
using ScreenDefnLib.Scripts;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Diagnostics;
using AutoCoder.Code;
using AutoCoder.Ext;

namespace TextCanvasLib.Hover
{
  /// <summary>
  /// the control drawn on the canvas when mouse hover is detected.
  /// </summary>
  public class HoverBox : IDisposable
  {

    private HoverWindow HoverWindow
    { get; set; }

    public bool HoverAsWindow
    { get; set; }

    public System.Windows.Controls.Canvas Parent
    { get; set; }

    public Point? Position
    { get; set; }
    private System.Windows.Controls.Panel HoverFoundation
    { get; set;  }

    // events signaled when mouse enters and leaves the hover window. Subscriber
    // will suspend and resume hover detection on the item canvas.
    public event Action<HoverBox,EnterLeaveCode> HoverWindowMouseEnterLeave;

    // event signaled when the hover window changes. Either the location changes
    // or its size changes. While the window is being changed the hover timer 
    // should be delayed.
    public event Action<HoverBox, WindowChangedCode> HoverWindowChanged;
    
    public HoverBox( System.Windows.Controls.Canvas Parent )
    {
      this.Parent = Parent;
      this.HoverAsWindow = false;
    }

    public HoverBox( bool HoverAsWindow )
    {
      this.Parent = null;
      this.HoverAsWindow = HoverAsWindow;
    }

    /// <summary>
    /// fill and make visible the hover window.
    /// Called by the DrawHoverBox method of ItemCanvas. 
    /// </summary>
    /// <param name="Position"></param>
    /// <param name="CanvasRowCol"></param>
    /// <param name="MatchScreenDefn"></param>
    /// <param name="Content"></param>
    public void DrawHoverBox( 
      Point Position, IScreenLoc CanvasRowCol, IScreenDefn MatchScreenDefn,
      ScreenContent Content)
    {
      // first remove any existing popup hover box.
      RemoveHoverBox();
        
      // hovering on a screen with a screen defn. Find the item on the screen
      // which is being hovered over.
      string itemName = "";
      string itemValue = "";
      int itemRowNum = 0;
      ScreenItemInstance hoverItem = null;
      if (( MatchScreenDefn != null) && (CanvasRowCol != null))
      {
        var foundItem = MatchScreenDefn.FindItem(CanvasRowCol, Content);
        if (foundItem != null)
        {
          hoverItem = foundItem;
          itemName = foundItem.GetItemName().EmptyIfNull();
          itemValue = foundItem.GetValue(Content);
          itemRowNum = foundItem.RepeatNum;
        }
      }

      // capture the contents of the screen to a DataTable.
      EnhancedDataTable itemTable = null;
      Grid srcmbrGrid = null;
      object hoverData = null;
      string hoverXaml = null;
      string hoverCode = null;
      if (( MatchScreenDefn != null) && ( hoverItem != null))
      {
        itemTable = MatchScreenDefn.Capture(Content, hoverItem);
        {
          hoverXaml = FindHoverXaml(hoverItem.Item);
          hoverCode = FindHoverCode(hoverItem.Item);
        }

        if ( hoverCode.IsNullOrEmpty( ) == false)
        {
          hoverData = CompileAndRunHoverCode( hoverCode, itemTable );
        }

        if ((MatchScreenDefn.ScreenName == "wrkmbrpdm") && (hoverData == null))
        {
          // BgnTemp
          {
            if (itemTable.Rows.Count == 0)
            {
              var rep = Content.ToColumnReport("Content report");
              rep.DebugPrint();
              itemTable = MatchScreenDefn.Capture(Content, hoverItem);
            }
          }
          // EndTemp

          var srcmbr = itemTable.SelectedRow["MbrName"].ToString();
          var srcfName = itemTable.SelectedRow["SrcfName"].ToString();
          var srcfLib = itemTable.SelectedRow["SrcfLib"].ToString();
          var sourceLines = SrcmbrScripts.GetSrcmbrLines(srcfName, srcfLib, srcmbr);
          hoverData = new { srcmbr, srcfName, srcfLib, sourceLines };
        }
      }

      Grid grid = null;
      if (hoverData != null)
      {
        if ( hoverXaml.IsNullOrEmpty( ) == false)
        {
          var sr = new StringReader(hoverXaml);
          var xr = XmlReader.Create(sr);
          var uiElem = XamlReader.Load(xr);
          grid = uiElem as Grid;
          grid.DataContext = hoverData;
        }
        else
        {
          var uiElem = hoverData.ToUIElement();
          grid = uiElem as Grid;
        }
      }
      else
      {

        // create the controls that make up the hover control.
        ListBox lb = null;
        System.Windows.Controls.Canvas canvas = null;

        if (srcmbrGrid != null)
        {
          var rv = BuildSrcmbrHoverGrid(srcmbrGrid);
          grid = rv.Item1;
        }
        else
        {
          var rv = BuildFoundation();
          grid = rv.Item1;
          lb = rv.Item2;
          canvas = rv.Item3;

          lb.Items.Add("field name:" + itemName);
          lb.Items.Add("RowCol:" + CanvasRowCol.ToText());
          lb.Items.Add("Value:" + itemValue);
          lb.Items.Add("Row number:" + itemRowNum);
        }
      }

      ShowHoverBox(grid, Position);
    }

    private string FindHoverXaml(IScreenItem StartItem)
    {
      string hoverXaml = null;
      IScreenItem item = StartItem;
      while(true)
      {
        if (item == null)
          break;
        hoverXaml = item.HoverXaml.ToAllText();
        if (hoverXaml.IsNullOrEmpty() == false)
          break;
        var section = item.SectionHeader;
        item = section as IScreenItem;
      }
      return hoverXaml;
    }
    private string FindHoverCode(IScreenItem StartItem)
    {
      string hoverCode = null;
      IScreenItem item = StartItem;
      while (true)
      {
        if (item == null)
          break;
        hoverCode = item.HoverCode.ToAllText();
        if (hoverCode.IsNullOrEmpty() == false)
          break;
        var section = item.SectionHeader;
        item = section as IScreenItem;
      }
      return hoverCode;
    }

    private object CompileAndRunHoverCode(
      string hoverCode, EnhancedDataTable screenDefnTable)
    {
      object hoverData = null;

      IEnumerable<string> errmsgs = null;
      var supportCode = ClientSettings.GetCachedSupportCode();
      Assembly assem = null;
      {
        string[] sourceInput = new string[] { hoverCode, supportCode };
        var autocoderTelnet = AutoCoder.Code.Ext.TypeExt.GetReference(typeof(ClientSettings));
        var rv = CSharpCompilationExt.CompileSource(sourceInput, autocoderTelnet);
        assem = rv.Item1;
        errmsgs = rv.Item2;
      }

      if (assem == null)
      {
        foreach (var errmsg in errmsgs)
        {
          Debug.WriteLine(errmsg);
        }
        throw new Exception("compile of hover code failed");
      }

      // run the first static method of the hoverCode.
      else 
      {
        var rv = CodeAnalyzer.GetFirstMethod(hoverCode);
        var namespaceName = rv.Item1;
        var className = rv.Item2;
        var methodName = rv.Item3;
        if (methodName.IsNullOrEmpty() == false)
        {
          var qualClassName = namespaceName + "." + className;
          var classType = assem.GetType(qualClassName);
          var mi = classType.GetMethod(methodName);
          if (mi != null)
          {
            var parametersArray = new object[] { screenDefnTable };
            hoverData = mi.Invoke(null, new object[] { screenDefnTable });
          }
        }
      }

      return hoverData;
    }

    public object GetHoverData( EnhancedDataTable itemTable)
    {
      var srcmbr = itemTable.SelectedRow["MbrName"].ToString();
      var srcfName = itemTable.SelectedRow["SrcfName"].ToString();
      var srcfLib = itemTable.SelectedRow["SrcfLib"].ToString();
      var hoverData = SrcmbrScripts.GetSrcmbrLines(srcfName, srcfLib, srcmbr);
      return hoverData;
    }

    private void ShowHoverBox(Grid grid, Point position)
    {
      this.HoverFoundation = grid;
      this.Position = position;

      if ( this.HoverAsWindow == false )
      {
        this.Parent.Children.Add(this.HoverFoundation);
        CanvasExt.SetUpperLeft(this.HoverFoundation, this.Position.Value);
      }
      else
      {
        EnsureHoverWindow();
        this.HoverWindow.Content = this.HoverFoundation;
        this.HoverWindow.ShowActivated = false;
        this.HoverWindow.Show();
      }
    }

    public void RemoveHoverBox( )
    {
      if (this.HoverAsWindow == false)
      {
        if (this.HoverFoundation != null)
        {
          this.Parent.Children.Remove(this.HoverFoundation);
          this.HoverFoundation = null;
          this.Position = null;
        }
      }
      else
      {
        if ( this.HoverWindow != null)
        {
          this.HoverWindow.Content = null;
          this.HoverFoundation = null;
          this.Position = null;
          this.HoverWindow.Hide();
        }
      }
    }

    private void EnsureHoverWindow( )
    {
      if (this.HoverWindow == null)
      {
        this.HoverWindow = new HoverWindow();
        this.HoverWindow.Width = 300;
        this.HoverWindow.Height = 300;
        this.HoverWindow.Closed += HoverWindow_Closed;
        this.HoverWindow.MouseEnter += HoverWindow_MouseEnter;
        this.HoverWindow.MouseLeave += HoverWindow_MouseLeave;
        this.HoverWindow.LocationChanged += HoverWindow_LocationChanged;
        this.HoverWindow.SizeChanged += HoverWindow_SizeChanged;
      }
    }

    private void HoverWindow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
      if (this.HoverWindowChanged != null)
        this.HoverWindowChanged(this, WindowChangedCode.SizeChanged);
    }

    private void HoverWindow_LocationChanged(object sender, EventArgs e)
    {
      if (this.HoverWindowChanged != null)
        this.HoverWindowChanged(this, WindowChangedCode.LocationChanged);
    }

    private void HoverWindow_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (this.HoverWindowMouseEnterLeave != null)
        this.HoverWindowMouseEnterLeave(this, EnterLeaveCode.Leave);
    }

    /// <summary>
    ///  mouse has entered the hover window. Suspend mouse hover detection on the
    ///  item canvas.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HoverWindow_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
      if (this.HoverWindowMouseEnterLeave != null)
        this.HoverWindowMouseEnterLeave(this, EnterLeaveCode.Enter);
    }

    private void HoverWindow_Closed(object sender, EventArgs e)
    {
      this.HoverWindow = null;
    }

    Tuple<Grid, ListBox, System.Windows.Controls.Canvas> BuildFoundation()
    {
      var grid = new Grid();

      grid.RowDefinitions.AddAutoRow();
      grid.RowDefinitions.AddStarRow();
      grid.ColumnDefinitions.AddStarColumn();

      // ListBox on row 0.
      var lb = new ListBox();
      lb.Background = Brushes.LightCyan;
      lb.Height = 90;
      grid.Children.Add(lb);
      Grid.SetColumn(lb, 0);
      Grid.SetRow(lb, 0);

      // canvas on the 2nd row. 
      var canvas = new System.Windows.Controls.Canvas();
      canvas.Background = Brushes.White;
      grid.Children.Add(canvas);
      Grid.SetColumn(canvas, 0);
      Grid.SetRow(canvas, 1);

      return new Tuple<Grid, ListBox, System.Windows.Controls.Canvas>(
        grid, lb, canvas);
    }

    Tuple<Grid> BuildSrcmbrHoverGrid( Grid SrcmbrGrid)
    {
      var grid = new Grid();
      grid.RowDefinitions.AddStarRow();
      grid.ColumnDefinitions.AddStarColumn();

      grid.AddUIElement(SrcmbrGrid, 0, 0);

      return new Tuple<Grid>(grid);
    }

    Tuple<Grid> BuildSrcmbrHoverGrid(ItemsControl itemsControl)
    {
      var grid = new Grid();
      grid.RowDefinitions.AddStarRow();
      grid.ColumnDefinitions.AddStarColumn();

      grid.AddUIElement(itemsControl, 0, 0);

      return new Tuple<Grid>(grid);
    }

    public void Dispose()
    {
      if ( this.HoverWindow != null)
      {
        this.HoverWindow.Close();
        this.HoverWindow = null;
      }
    }
  }
}
