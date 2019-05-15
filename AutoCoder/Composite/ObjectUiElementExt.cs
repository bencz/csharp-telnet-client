using AutoCoder.Collections;
using AutoCoder.Ext.System.Reflection;
using AutoCoder.Ext.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoCoder.Composite
{
  /// <summary>
  /// object extension methods which use reflection to create UIElement from the
  /// properties of an object.
  /// </summary>
  public static class ObjectUiElementExt
  {
    public static Grid ToNameValueGrid(
      this object obj, string bindingPropertyName = null, bool primitiveOnly = true)
    {
      Grid grid = new Grid();

      // set binding of the DataContext of the grid.
      if ( bindingPropertyName != null)
      {
        var binding = new Binding(bindingPropertyName);
        grid.SetBinding(Grid.DataContextProperty, binding);
      }
      else
      {
        grid.DataContext = obj;
      }

      // add two columns.
      grid.ColumnDefinitions.AddAutoColumn();
      grid.ColumnDefinitions.AddStarColumn();

      // add row for each property
      foreach (var pi in obj.GetType( ).GetProperties( ))
      {
        if ((primitiveOnly == false) || 
          ((primitiveOnly == true) && (pi.PropertyType.IsPrimitiveOrString( ) == true)))
        {
          grid.RowDefinitions.AddAutoRow();
        }
      }

      // populate each row with the property name and its value.
      int rownum = -1;
      foreach (var pi in obj.GetType().GetProperties())
      {
        if ((primitiveOnly == false) ||
          ((primitiveOnly == true) && (pi.PropertyType.IsPrimitiveOrString() == true)))
        {
          rownum += 1;
          var tb0 = grid.AddTextBlock(pi.Name, rownum, 0);
          tb0.FontWeight = FontWeights.Bold;
          tb0.VerticalAlignment = VerticalAlignment.Center;
          tb0.Margin = new Thickness(0, 0, 3, 0);

          var getObj = obj.GetPropertyValue(pi.Name);
          var uiElem = getObj.ToUIElement( pi.Name );

          if (uiElem != null)
          {
            if (uiElem is TextBlock)
            {
              var tb = uiElem as TextBlock;
              tb.VerticalAlignment = VerticalAlignment.Center;
            }

            grid.Children.Add(uiElem);
            Grid.SetRow(uiElem, rownum);
            Grid.SetColumn(uiElem, 1);
          }
        }
      }

      return grid;
    }

    /// <summary>
    /// create a ListBox and bind the IEnumerable of primitive type to it.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="bindingPropertyName"></param>
    /// <returns></returns>
    public static UIElement ToListBoxOfPrimitive(
      this object obj, string bindingPropertyName = null)
    {
      ListBox lb = new ListBox();
      if ( bindingPropertyName != null)
      {
        var binding = new Binding(bindingPropertyName);
        lb.SetBinding(ListBox.ItemsSourceProperty, binding);
      }
      else
      {
        var enumerableObj = (IEnumerable<object>)obj;
        lb.ItemsSource = enumerableObj;
      }

      return lb;
    }

    public static ListView ToListView(
      this IEnumerable<object> objList, string bindingPropertyName = null)
    {
      var lv = new ListView();
      var gv = new GridView();

      // add column to the gridView for each property of the objectList.
      var olObject = objList.FirstOrDefault();
      if (olObject != null)
      {
        var piArray = olObject.GetType().GetProperties();
        foreach (var pi in piArray)
        {
          var gvCol = new GridViewColumn();
          gvCol.Header = pi.Name;
          gvCol.DisplayMemberBinding = new Binding(pi.Name);
          gvCol.Width = 80;
          gv.Columns.Add(gvCol);
        }
      }

      // set the View of the ListView to the GridView.
      lv.View = gv;

      // bind the ItemsSource to this list of objects.
      if (bindingPropertyName != null)
      {
        var binding = new Binding(bindingPropertyName);
        lv.SetBinding(ListView.ItemsSourceProperty, binding);
      }
      else
      {
        lv.ItemsSource = objList;
      }

      return lv;
    }

    public static UIElement ToUIElement( this object obj, string bindingPropertyName = null)
    {
      UIElement uiElem = null;
      var objType = obj.GetType();

      // object is a DataTable.  Implement as a DataGrid control.
      if ( obj is DataTable )
      {
        var dataGrid = new DataGrid();
        dataGrid.AutoGenerateColumns = true;
        dataGrid.CanUserAddRows = false;
        if (bindingPropertyName != null)
        {
          var binding = new Binding(bindingPropertyName);
          dataGrid.SetBinding(DataGrid.ItemsSourceProperty, binding);
        }
        else
        {
          dataGrid.ItemsSource = (obj as DataTable).DefaultView;
        }
        uiElem = dataGrid;
      }

      // object is an IEnumerable. Implement as a ListView or ListBox.
      else if (obj.IsEnumerableAndNotString( ) == true )
      {
        if (obj.IsEnumerableOfPrimitiveOrString() == true)
        {
          uiElem = obj.ToListBoxOfPrimitive(bindingPropertyName);
        }
        else
        {
          var enumerableObj = (IEnumerable<object>)obj;
          var lv = enumerableObj.ToListView(bindingPropertyName);
          uiElem = lv;
        }
      }

      // object is a primitive or string. Implement as a TextBlock.
      else if (obj.GetType( ).IsPrimitiveOrString( ) == true )
      {
        var tb = new TextBlock();

        // either set the binding of the TextBlock or set its actual text value.
        if (bindingPropertyName != null)
        {
          var binding = new Binding(bindingPropertyName);
          tb.SetBinding(TextBlock.TextProperty, binding);
        }
        else
        {
          tb.Text = obj.ToString();
        }

        uiElem = tb;
      }

      // object has properties. Display as a name/value grid.
      else
      {
        uiElem = obj.ToNameValueGrid(bindingPropertyName, false);
      }

      return uiElem;
    }
  }
}
