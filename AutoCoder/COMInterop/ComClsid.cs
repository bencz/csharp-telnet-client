using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.COMInterop
{
  /// <summary>
  /// A class wrapper around a text or guid that contains the value of a CLSID.
  /// </summary>
  public class ComClsid : IEquatable<ComClsid>
  {
    string _TextClsid;
    public string TextClsid
    { 
      get { return _TextClsid ; }
      set
      {
        _TextClsid = value.ToLower();
      }
    }

    public ComClsid(string TextClsid)
    {
      this.TextClsid = TextClsid;
    }

    public override string ToString()
    {
      return this.TextClsid;
    }

    public bool Equals(ComClsid other)
    {
      if (other == null)
        return false;
      else if (this.TextClsid == other.TextClsid)
        return true;
      else
        return false;
    }
  }
}
