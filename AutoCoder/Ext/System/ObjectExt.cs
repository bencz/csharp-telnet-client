using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class ObjectExt
  {
    public static string NullOrToString( this object Value, string VariableName )
    {
      if (Value == null)
        return VariableName + " is null";
      else
        return VariableName + " " + Value.ToString();
    }

    public static string GetPropertyValue_String( this object Object, string PropertyName)
    {
      string text = null;
      if (Object != null)
      {
        var itemType = Object.GetType();
        var prop = itemType.GetProperty(PropertyName);
        text =
          (string)itemType.InvokeMember(
          prop.Name, BindingFlags.GetProperty, null, Object, null);
      }
      return text;
    }
  }
}
