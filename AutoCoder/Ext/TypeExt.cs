using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AutoCoder.Ext
{
  public static class TypeExt
  {

    /// <summary>
    /// find the base type of the StartType which matches on type name.
    /// </summary>
    /// <param name="StartType"></param>
    /// <param name="BaseTypeName"></param>
    /// <returns></returns>
    public static Type FindBaseTypeDeep(this Type StartType, string BaseTypeName)
    {
      Type foundType = null;

      // drill down the base type trail until System.Object.
      Type bt = StartType.BaseType;
      while (true)
      {
        if (bt.Name == null)
        {
          foundType = null;
          break;
        }

        else if (bt.Name.Length == 0)
        {
          foundType = null;
          break;
        }
        else if (bt.Name == "Object")
        {
          foundType = null;
          break;
        }
        else if (bt.Name == BaseTypeName)
        {
          foundType = bt;
          break;
        }

        bt = bt.BaseType;
      }

      return foundType;
    }

    /// <summary>
    /// return a list of class type types which are derived from the specified base
    /// class.
    /// </summary>
    /// <param name="BaseClass"></param>
    /// <returns></returns>
    public static Type[] GetDerivedClasses(this Type BaseClass)
    {
      string baseClassName = BaseClass.Name;
      List<Type> derivedTypes = new List<Type>();

      // get the assembly of one of the statement types.
      Assembly stmtAssembly = null;
      stmtAssembly = Assembly.GetAssembly(BaseClass);

      // get all the types in the assembly. 
      Type[] types = stmtAssembly.GetTypes();
      foreach (var tp in types)
      {

        if (tp.IsClass == false)
          continue;

        // drill down the base type trail until System.Object.
        Type bt = tp.FindBaseTypeDeep(baseClassName);

        // this type is derived from ComponentWord.
        if (bt != null)
        {
          derivedTypes.Add(tp);
        }
      }

      return derivedTypes.ToArray();
    }

    public static T GetCustomAttribute<T>( this Type Type ) where T: class
    {
      T customAttr = null ;
      var attrs = Type.GetCustomAttributes(typeof(T), false);
      if ((attrs != null) && (attrs.Length >= 1))
        customAttr = attrs[0] as T;
      return customAttr;
    }

    public static Type[] GetImmedDerivedClasses(this Type BaseClass)
    {
      string baseClassName = BaseClass.Name;
      string baseClassFullName = BaseClass.FullName;
      List<Type> derivedTypes = new List<Type>();

      // get the assembly of the BaseClass.
      Assembly stmtAssembly = null;
      stmtAssembly = Assembly.GetAssembly(BaseClass);

      // get all the types in the assembly. 
      Type[] types = stmtAssembly.GetTypes();
      foreach (var tp in types)
      {
        if (tp.IsClass == false)
          continue;

        // the type is a class and its base class is the input parm type.
        if ((tp.BaseType.Name == baseClassName) 
          && ( tp.BaseType.FullName == baseClassFullName ))
        {
          derivedTypes.Add(tp);
        }
      }

      return derivedTypes.ToArray();
    }

    /// <summary>
    /// determine if a type has a custom attribute or not.
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="CustomAttributeType"></param>
    /// <returns></returns>
    public static bool HasCustomAttribute(this Type Type, Type CustomAttributeType)
    {
      bool hasCustomAttr = false;
      var customAttrs = Type.GetCustomAttributes(CustomAttributeType, false);
      if ((customAttrs != null) && (customAttrs.Length > 0))
      {
        hasCustomAttr = true;
      }
      return hasCustomAttr;
    }
  }
}
