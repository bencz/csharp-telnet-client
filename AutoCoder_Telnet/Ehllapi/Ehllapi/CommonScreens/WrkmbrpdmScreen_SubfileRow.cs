using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ehllapi.CommonScreens
{
  public partial class WrkmbrpdmScreen : DisplayScreen
  {
    public class SubfileRow
    {
      string _Opt;
      public string Opt
      {
        get { return _Opt; } 
        set
        {
          _Opt = SubfileRow.TrimEnd(value) ;
        }
      }

      string _Member ;
      public string Member
      {
        get { return _Member; }
        set
        {
          _Member = SubfileRow.TrimEnd(value);
        }
      }

      string _Type;
      public string Type
      {
        get { return _Type; }
        set { _Type = SubfileRow.TrimEnd(value); }
      }

      string _Text;
      public string Text
      {
        get { return _Text; }
        set { _Text = SubfileRow.TrimEnd(value); } 
      }

      private static string TrimEnd(string Text)
      {
        if ( Text == null )
          return "" ;
        else
        {
          string s1 = Text.TrimEnd(new char[]{' '}) ;
          return s1 ;
        }
      }
    }
  }
}
