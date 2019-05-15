using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Ext.Windows.Controls
{
  public static class GridExt
  {
    public static void AddUIElement(this Grid grid, UIElement element, int RowNum, int ColNum)
    {
      grid.Children.Add(element);
      Grid.SetRow(element, RowNum);
      Grid.SetColumn(element, ColNum);
    }

    public static Button AddButton(
      this Grid grid, string Name, string Text, int RowNum, int ColNum)
    {
      var but = new Button();
      if (Name != null)
        but.Name = Name;
      if (Text != null)
        but.Content = Text;
      grid.Children.Add(but);
      Grid.SetRow(but, RowNum);
      Grid.SetColumn(but, ColNum);
      return but;
    }
    public static ListBox AddListBox( 
      this Grid grid, string Name, int RowNum, int ColNum)
    {
      var lb = new ListBox();
      if (Name != null)
        lb.Name = Name;
      grid.Children.Add(lb);
      Grid.SetRow(lb, RowNum);
      Grid.SetColumn(lb, ColNum);
      return lb;
    }

    public static TextBlock AddTextBlock(this Grid grid, string Text, int RowNum, int ColNum )
    {
      var tb = new TextBlock();
      tb.Text = Text;
      grid.Children.Add(tb);
      Grid.SetRow(tb, RowNum);
      Grid.SetColumn(tb, ColNum);
      return tb;
    }

    public static TextBox AddTextBox( this Grid grid, string Name, int RowNum, int ColNum )
    {
      var tb = new TextBox();
      if ( Name.IsNullOrEmpty( ) == false)
      {
        tb.Name = Name;
      }
      grid.Children.Add(tb);
      Grid.SetRow(tb, RowNum);
      Grid.SetColumn(tb, ColNum);
      return tb;
    }
  }
}
