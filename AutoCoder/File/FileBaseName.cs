using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.File
{
  /// <summary>
  /// the file name with the extension removed.
  /// </summary>
  public class FileBaseName
  {
    string mBaseName;

    public FileBaseName(string InBaseName)
    {
      mBaseName = InBaseName;
    }

    public FileBaseName(FileBaseName InBaseName)
    {
      mBaseName = InBaseName.ToString();
    }

    public override string ToString()
    {
      return mBaseName;
    }

    public static FileName operator +(FileBaseName InBaseName, FileExtension InExtension)
    {
      return new FileName(InBaseName, InExtension);
    }
  }
}
