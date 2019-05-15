using AutoCoder.Systm.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoCoder.Ext.System.Reflection
{
  public static class AssemblyExt
  {

   public static Type FindFirstTypeName(this Assembly assem, string typeName)
    {
      Type found = null;

      Type[] arrayOfTypes = null;
      try
      {
        arrayOfTypes = assem.GetTypes();
      }
      catch (Exception ex)
      {
        if (ex is ReflectionTypeLoadException)
        {
          var typeLoadException = ex as ReflectionTypeLoadException;
          DebugMore.WriteExceptions(typeLoadException.LoaderExceptions);
          throw typeLoadException;
        }
        throw ex;
      }

      foreach (var typeItem in arrayOfTypes)
      {
        var simpleName = typeItem.Name;
        if (simpleName == typeName)
        {
          found = typeItem;
          break;
        }
      }

      return found;
    }


    /// <summary>
    /// resolve to the class and the static method within the assembly.
    /// Then call the static method with the array of arguments.
    /// </summary>
    /// <param name="assem"></param>
    /// <param name="ClassName"></param>
    /// <param name="MethodName"></param>
    /// <param name="inputArgs"></param>
    /// <returns></returns>
    public static object InvokeStaticMethod(
      this Assembly assem, string ClassName, string MethodName,
      object[] inputArgs)
    {
      object rv = null;
      var pgm = assem.CreateInstance(ClassName);
      var tt = pgm.GetType();
      MethodInfo methodInfo = tt.GetMethod(MethodName);

      if (methodInfo != null)
      {
        rv = methodInfo.Invoke(null, inputArgs);
      }
      return rv;
    }
  }
}


