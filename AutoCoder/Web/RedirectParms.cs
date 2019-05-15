using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Web
{
  // ------------------------ RedirectParms ------------------------------
  // simple class used to call the overloaded "RedirectTo" method with a prepared
  // sequence of parms in QueryString form.
  public class RedirectParms
  {
    StringBuilder mQueryString = null;

    /// <summary>
    /// The redirect parameters in QueryString form.
    /// </summary>
    public string QueryString
    {
      get
      {
        if ( mQueryString == null )
          return null;
        else
          return mQueryString.ToString();
      }
      set
      {
        if (mQueryString == null)
          mQueryString = new StringBuilder(512);
        mQueryString.Length = 0 ;
        mQueryString.Append( value ) ;
      }
    }

    /// <summary>
    /// Add to the end of the QueryString.
    /// </summary>
    /// <param name="InValue"></param>
    /// <returns></returns>
    public RedirectParms AppendQueryString(string InValue)
    {
      if (mQueryString == null)
        mQueryString = new StringBuilder( 512 ) ;
      if ( mQueryString.Length > 0 )
        mQueryString.Append( "&" ) ;
      mQueryString.Append( InValue ) ;
      return this;
    }

    /// <summary>
    /// Add to the end of the QueryString.
    /// </summary>
    /// <param name="InVarName"></param>
    /// <param name="InVarValue"></param>
    /// <returns></returns>
    public RedirectParms AppendQueryString(string InParmName, string InParmValue)
    {
      string fullValue = InParmName + "=" + InParmValue;
      return (AppendQueryString(fullValue));
    }

    public bool IsEmpty
    {
      get
      {
        if (mQueryString == null)
          return true;
        else
          return false;
      }
    }

    public bool IsNotEmpty
    {
      get { return !IsEmpty; }
    }
  } // end class RedirectParms
}
