using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;

namespace AutoCoder.Ext.System.Management
{
  public static class ManagementBaseObjectExt
  {
    public static string Caption(this ManagementBaseObject BaseObject)
    {
      if (BaseObject == null)
        return "";
      else
      {
        var prop = BaseObject.Properties["Caption"];
        if (prop == null)
          return "";
        else
          return prop.Value.ToString();
      }
    }

    public static string Description(this ManagementBaseObject BaseObject)
    {
      return GetProperty(BaseObject, "Description");
    }

    public static string ExecutablePath(this ManagementBaseObject BaseObject)
    {
      return GetProperty(BaseObject, "ExecutablePath");
    }

    public static string Name(this ManagementBaseObject BaseObject)
    {
      return GetProperty(BaseObject, "Name");
    }

    public static ManagementBaseObject TargetInstance(this ManagementBaseObject BaseObject)
    {
      var targetInstance = BaseObject["TargetInstance"] as ManagementBaseObject;
      return targetInstance;
    }

    private static string GetProperty(ManagementBaseObject BaseObject, string PropertyName)
    {
      if (BaseObject == null)
        return "";
      else
      {
        var prop = BaseObject.Properties[PropertyName];
        if ((prop == null) || ( prop.Value == null ))
          return "";
        else
          return prop.Value.ToString();
      }
    }

    public static IEnumerable<string> ListProperties(this ManagementBaseObject BaseObject)
    {
      {
        var msg = "Properties:";
        yield return msg;
      }

      foreach (var item in BaseObject.Properties)
      {
        string itemValue = "";
        if (item.Value != null)
          itemValue = item.Value.ToString();
        var msg = item.Name + ":" + itemValue;
        yield return msg;
      }

      {
        var msg = "System properties:";
        yield return msg;
      }

      foreach (var item in BaseObject.SystemProperties)
      {
        string itemValue = "";
        if (item.Value != null)
          itemValue = item.Value.ToString();
        var msg = item.Name + ":" + itemValue;
        yield return msg;
      }

      yield break;
    }
  }
}
