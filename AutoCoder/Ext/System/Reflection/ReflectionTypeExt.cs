using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System.Reflection
{
  public static class ReflectionTypeExt
  {

    /// <summary>
    /// search for the method name among the methods of the type.
    /// </summary>
    /// <param name="tp"></param>
    /// <param name="methodName"></param>
    /// <returns></returns>
    public static MethodInfo FindMethodByName(this Type tp, string methodName)
    {
      MethodInfo found = null;
      var miArray = tp.GetMethods();
      foreach (var mi in miArray)
      {
        if (mi.Name == methodName)
        {
          found = mi;
          break;
        }
      }
      return found;
    }

    /// <summary>
    /// return the actual underlying type of the type. If is not a nullable type,
    /// return the type. If it is nullable, return the underlying type.
    /// </summary>
    /// <param name="tp"></param>
    /// <returns></returns>
    public static Tuple<Type,bool> GetActualType(this Type tp)
    {
      bool isNullable = false;
      var actualType = tp;

      if ( tp.IsNullableType( ))
      {
        isNullable = true;
        actualType = Nullable.GetUnderlyingType(tp);
      }

      return new Tuple<Type, bool>(actualType, isNullable);
    }

    /// <summary>
    /// return PropertyInfo array containing all the properties of the type which
    /// have both a public getter and public setter.
    /// </summary>
    /// <param name="tp"></param>
    /// <returns></returns>
    public static PropertyInfo[] GetGetSetProperties(this Type tp)
    {
      var piList = new List<PropertyInfo>();
      var piArray = tp.GetProperties();
      foreach (var pi in piArray)
      {
        if ((pi.GetGetMethod() != null) && (pi.GetSetMethod() != null))
        {
          piList.Add(pi);
        }
      }
      return piList.ToArray();
    }

    public static bool IsDataTable(this Type type)
    {
      bool isDataTable = false;

      if (type.IsPrimitiveOrString() == true)
        isDataTable = false;

      else if (type.IsArray == true)
        isDataTable = false;

      else if (type.IsEnum == true)
        isDataTable = false;

      else if (type.Name.Contains("DataTable") == true)
        isDataTable = true;

      return isDataTable;
    }

    /// <summary>
    /// determine if this type is an IEnumerable. ( but is not a string which
    /// also implements IEnumberable to return a sequence of characters. )
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnumerableAndNotString(this Type type)
    {
      bool isEnumerable = false;

      if (type.IsPrimitiveOrString() == true)
      {
        isEnumerable = false;
      }

      // an array. this is an IEnumerable type.
      else if (type.IsArray == true)
      {
        isEnumerable = true;
      }

      // check that the property type itself is IEnumerable.
      else if (type.Name.Contains("IEnumerable") == true)
      {
        isEnumerable = true;
      }

      // check that the property type implements IEnumerable.
      else
      {
        var interfaces = type.GetInterfaces();
        if (interfaces != null)
        {
          foreach (var interfac in interfaces)
          {
            var s1 = interfac.Name;
            if (s1.Contains("IEnumerable"))
            {
              isEnumerable = true;
              break;
            }
          }
        }
      }
      return isEnumerable;
    }

    /// <summary>
    /// this type is an IEnumerable of something. Check that the something is 
    /// either a primitive type or typeof(string).
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsEnumerableOfPrimitiveOrString(this Type type)
    {
      bool isEnumerable = false;

      if ( type.IsArray == true )
      {
        var elemType = type.GetElementType();
        if (( elemType != null) && ( elemType.IsPrimitiveOrString( ) == true ))
        {
          isEnumerable = true;
        }
      }
      else if ( type.IsGenericType == true )
      {
        var genericTypeArray = type.GetGenericArguments();
        if (genericTypeArray.Length == 1)
        {
          var genType = genericTypeArray[0];
          if (genType.IsPrimitiveOrString() == true)
            isEnumerable = true;
        }
      }

      return isEnumerable;
    }

    public static bool IsNullableType(this Type tp)
    {
      if ((tp.IsGenericType == true) && tp.GetGenericTypeDefinition() == typeof(Nullable<>))
        return true;
      else
        return false;
    }

    public static bool IsPrimitiveOrString(this Type type)
    {
      if (type.IsPrimitive == true)
        return true;
      else if (type.FullName == "System.String")
        return true;
      else
        return false;
    }
  }
}
