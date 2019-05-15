using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.WebControls
{

  /// <summary>
  /// a css class name.
  /// </summary>
  public class CssClass
  {
    string mClassName = null;

    public CssClass(string InClassName)
    {
      mClassName = InClassName;
    }

    public string ClassName
    {
      get { return mClassName; }
      set { mClassName = value; }
    }
  }
}
