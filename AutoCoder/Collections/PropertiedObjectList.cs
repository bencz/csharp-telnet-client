using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace AutoCoder.Collections
{

  // could place this object a Propertied or Composite namespace.
  // maybe call this class a CompositeObjectList.

  /// <summary>
  /// list of objects. Methods of the class use reflection to convert the list
  /// to a datatable, create UIElement controls that use binding to display the
  /// properties of each object.
  /// </summary>
  public class PropertiedObjectList : IEnumerable<object>
  {
    public IEnumerable<object> ObjectList
    {
      get; set; 
    }

    public PropertiedObjectList(IEnumerable<object> objectList)
    {
      this.ObjectList = objectList;
    }

    public IEnumerator<object> GetEnumerator()
    {
      return this.ObjectList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.ObjectList.GetEnumerator();
    }

#if skip
    public ListView ToListView( string bindingPropertyName = null)
    {
      var lv = new ListView();
      var gv = new GridView();

      // add column to the gridView for each property of the objectList.
      var olObject = this.ObjectList.FirstOrDefault();
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
        lv.ItemsSource = this.ObjectList;
      }

      return lv;
    }
#endif

  }
}
