using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace AutoCoder.Reflection
{
  public static class Reflectioner
  {

    /// <summary>
    /// return an array of parameter names of a method.
    /// </summary>
    /// <param name="InMethodInfo"></param>
    /// <returns></returns>
    public static string[] GetParameterNames( MethodInfo InMethodInfo )
    {
      ParameterInfo[] parms = InMethodInfo.GetParameters( ) ;
      string[] names = new string[parms.Length] ;
      for( int ix = 0 ; ix < parms.Length ; ++ix )
      {
        names[ix] = parms[ix].Name ;
      }
      return names ;
    }

  }
}
