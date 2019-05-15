using AutoCoder.Ext.System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace AutoCoder.Composite
{
  /// <summary>
  /// composite object is an object with built in methods that use reflection to
  /// work with the properties of the object.
  /// </summary>
  public class CompositeObject
  {
    public UIElement ToUIElement()
    {
      // the outer element is a Grid.
      var ogrid = new Grid();
      Grid nameValueGrid = null;


      // this composite object has simple properties. Show the simple properties
      // as name=value pairs, where the prompt text is in a left side column and
      // the value in a right side column.
      if ( this.HasPrimitiveOrStringProperties( ) == true )
      {
        nameValueGrid = this.ToNameValueGrid();
      }

      // create a grid control for each property which is not primitive or string
      var compositeGridList = new List<Grid>();
      foreach (var pi in this.GetType().GetProperties())
      {
        if (pi.PropertyType.IsPrimitiveOrString() == true)
        {
        }

        else if (pi.PropertyType.IsEnumerableAndNotString() == true)
        {
          var propertyObject = this.GetPropertyValue(pi.Name);
          var uiElem = propertyObject.ToUIElement();
        }

        // the property is an IEnumerable. create a grid or listview control.
      }
      
      // an ItemsControl is created for each IEnumerable property.



      return ogrid;
    }
  }
}
