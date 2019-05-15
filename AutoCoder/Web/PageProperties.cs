using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Web
{

  public class PageProperties
  {
    string mPageDirPath = null;
    string mPageFileName = null;

    public PageProperties()
    {
    }

    /// <summary>
    /// the directory path to the web page
    /// </summary>
    public string PageDirPath
    {
      get
      { 
        if ( mPageDirPath == null )
          return "";
        else
          return mPageDirPath;
      }
      set { mPageDirPath = value; }
    }

    /// <summary>
    /// the name.aspx file name of the web page.
    /// </summary>
    public string PageFileName
    {
      get
      {
        if ( mPageFileName == null )
          return "";
        else
          return mPageFileName;
      }
      set { mPageFileName = value; }
    }
  }

}
