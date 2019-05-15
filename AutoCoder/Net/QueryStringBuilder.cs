using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

#if skip

namespace AutoCoder.Net
{
  public class QueryStringBuilder
  {
    NameValueCollection _qs = null;

    public QueryStringBuilder()
    {
      // ParseQueryString returns an instance of HttpValueCollection.
      // see the ToString method. The ToString method of the HttpValueCollection
      // object will format the collection as a URL encoded query string.
      _qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
    }

    public void Append(string Key, string Value)
    {
      _qs[Key] = Value;
    }

    public bool IsEmpty( )
    {
      if ( _qs == null )
        return true ;
      else if ( _qs.Count == 0 )
        return true ;
      else
        return false ;
    }

    public override string ToString()
    {
      return _qs.ToString();
    }
  }

  public static class QueryStringBuilderExt
  {
    public static bool IsNullOrEmpty(this QueryStringBuilder Builder)
    {
      if (Builder == null)
        return true;
      else if (Builder.IsEmpty() == true)
        return true;
      else
        return false;
    }
  }
}

#endif
