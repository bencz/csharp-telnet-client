using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.File
{
  /// <summary>
  ///  the extension part of a file name
  /// </summary>
  public class FileExtension
  {
    string mExtension;

    public FileExtension(string InExtension)
    {
      mExtension = InExtension;
    }

    public override string ToString()
    {
      return mExtension;
    }
  }
}
