using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System.Reflection
{
  /// <summary>
  /// object extension methods that use reflection.
  /// </summary>
  public static class ObjectReflectionExt
  {
    /// <summary>
    /// return a list of properties of the object which are type IEnumerable or 
    /// have an IEnumerable interface. ( but are not strings. )
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IEnumerable<PropertyInfo> EnumerableProperties(this object obj)
    {
      var piArray = obj.GetType().GetProperties();
      foreach (var pi in piArray)
      {
        var propType = pi.PropertyType;
        if (propType.IsEnumerableAndNotString() == true)
          yield return pi;
      }
      yield break;
    }

    /// <summary>
    /// get the value of the property of the specified object.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="PropertyName"></param>
    /// <returns></returns>
    public static object GetPropertyValue(this Object obj, string PropertyName)
    {
      var vlu = obj.GetType().InvokeMember(PropertyName, BindingFlags.GetProperty,
                                  null, obj, new object[] { });
      return vlu;
    }

    public static bool HasPrimitiveOrStringProperties(this object obj)
    {
      bool hasPrimitiveOrString = false;
      var piArray = obj.GetType().GetProperties();
      foreach (var pi in obj.PrimitiveOrStringProperties())
      {
        hasPrimitiveOrString = true;
        break;
      }

      return hasPrimitiveOrString;
    }

    public static bool IsEnumerableAndNotString(this object obj)
    {
      bool isEnumerable = false;

      if (obj != null)
      {
        isEnumerable = obj.GetType().IsEnumerableAndNotString();
      }

      return isEnumerable;
    }

    /// <summary>
    /// this object is an IEnumerable of something. Check that the something is 
    /// either a primitive type or typeof(string).
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsEnumerableOfPrimitiveOrString(this object obj)
    {
      bool isEnumerable = false;
      if ( obj != null)
      {
        isEnumerable = obj.GetType().IsEnumerableOfPrimitiveOrString();
      }

      return isEnumerable;
    }

    /// <summary>
    /// return an IEnumerable containing the PropertyInfo of all the simple
    /// properties of this object. Simple being a property with a single value.
    /// Such as string, int, bool, ...
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static IEnumerable<PropertyInfo> PrimitiveOrStringProperties(this object obj)
    {
      var piArray = obj.GetType().GetProperties();
      foreach(var pi in piArray)
      {
        bool isSimple = false;
        if (pi.PropertyType.IsPrimitiveOrString())
          isSimple = true;

        if (isSimple == true)
          yield return pi;
      }
      yield break;
    }
  }
}
