using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AutoCoder.Reflection
{
  /// <summary>
  /// static methods which apply to the Type class.
  /// see also ReflectionTypeExt static class in autocode.Ext.System.Reflection
  /// </summary>
  public static class Typer
  {
    // consider NamespacePath class. A formalized encapsulation of the dot delimited
    // namespace string.  Other possible name is "NamespaceString".

    /// <summary>
    /// Find the type by name amoung all the assemblies of AppDomain.CurrentDomain.
    /// An example of a full name is "System.Drawing.Size".
    /// </summary>
    /// <param name="InFullName"></param>
    /// <returns></returns>
    public static Type FindType(string InFullName)
    {
      Type foundType = null;

      Assembly[] assems = AppDomain.CurrentDomain.GetAssemblies();
      foreach (Assembly assem in assems)
      {
        foreach (Type tp in assem.GetTypes())
        {
          if (tp.FullName == InFullName)
          {
            foundType = tp;
            break;
          }
        }

        if (foundType != null)
          break;
      }
      return foundType;
    }

    /// <summary>
    /// split the namespace string into its elemental '.' delimited namespace names.
    /// </summary>
    /// <param name="InNamespaceString"></param>
    /// <returns></returns>
    public static string[] GetNamespaceArray(string InNamespaceString)
    {
      string[] items = InNamespaceString.Split(new char[] { '.' });
      return items;
    }

    /// <summary>
    /// Find the property of the object, then Invoke that property to get
    /// its value.
    /// </summary>
    /// <param name="InAnonymous"></param>
    /// <param name="InPropertyName"></param>
    /// <returns></returns>
    public static object 
      GetValueOfPropertyOfAnonymousType(object InAnonymous, string InPropertyName)
    {
      object propValue = null;
      Type ot = InAnonymous.GetType();
      PropertyInfo[] props = ot.GetProperties();
      foreach (PropertyInfo pi in props)
      {
        string s1 = pi.Name;
        if (s1 == InPropertyName)
        {
          propValue = pi.GetValue(InAnonymous, null);
          break;
        }
      }
      return propValue;
    }

    public static bool IsWithinNamespace(Type InType, string InNamespace)
    {
      bool isWithin = false;

      int Lx = InNamespace.Length;
      if (InType.Namespace.Length > Lx)
      {
        if (InType.Namespace.Substring(0, Lx) == InNamespace)
        {
          if (InType.Namespace[Lx] == '.')
            isWithin = true;
        }
      }
      return isWithin;
    }

    public static string[] SplitNamespace(string InNamespace, string InRootNamespace)
    {
      string[] rv = new string[2];

      string[] nsParts = GetNamespaceArray(InNamespace);
      string[] rootParts = GetNamespaceArray(InRootNamespace);

      // make sure the root matches the start of the namespace.
      for( int ix = 0 ; ix < rootParts.Length ; ++ix )
      {
        if (rootParts[ix] != nsParts[ix])
          throw new ApplicationException(
            "Namespace " + InNamespace + " does not contain root namespace " + InRootNamespace);
      }

      // build the namespace string that follows the root.
      StringBuilder sb = new StringBuilder( ) ;
      int jx = rootParts.Length ;
      while( jx < nsParts.Length )
      {
        if ( sb.Length > 0 )
          sb.Append('.') ;
        sb.Append( nsParts[jx] ) ;
        jx += 1;
      }

      rv[0] = InRootNamespace;
      rv[1] = sb.ToString();

      return rv;
    }
  }
}
