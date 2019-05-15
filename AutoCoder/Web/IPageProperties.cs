using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Web
{
  public interface IPageProperties
  {
    /// <summary>
    /// the directory path of the page.
    /// Starting from the "~/..." web application root.
    /// </summary>
    /// <returns></returns>
    string GetPageDirPath();

    /// <summary>
    /// The file.ext file name of the page.
    /// </summary>
    /// <returns></returns>
    string GetPageFileName();
  }
}
