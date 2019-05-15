using AutoCoder.Ext.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenDefnLib.Defn
{
  public interface IScreenLiteral : IScreenAtomic
  {
    string Value { get; }
    IList<string> ListValues { get; }
  }

  public static class IScreenLiteralExt
  {

    /// <summary>
    /// match the Text string against all of the possible values of the screen
    /// literal. Return true if any match.
    /// </summary>
    /// <param name="screenLit"></param>
    /// <param name="Text"></param>
    /// <returns></returns>
    public static bool MatchValue( this IScreenLiteral screenLit, string Text)
    {
      bool isMatch = false;
      if (screenLit.ListValues.Count == 1)
      {
        if (screenLit.Value == Text)
          isMatch = true;
      }
      else if (screenLit.ListValues.Count == 0)
      {
        if (Text.IsNullOrEmpty() == true)
          isMatch = true;
      }
      else
      {
        foreach( var litValue in screenLit.ListValues)
        {
          if( litValue == Text )
          {
            isMatch = true;
            break;
          }
        }
      }
      return isMatch;
    }

  }
}
