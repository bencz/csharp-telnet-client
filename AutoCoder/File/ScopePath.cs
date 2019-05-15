using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.Text;
using System.Xml.Serialization;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  /// <summary>
  /// The ScopePath is the path of a file less the HomePath.
  /// </summary>
  public class ScopePath
  {
    string mScopePath;

    public ScopePath()
    {
      mScopePath = "";
    }

    public ScopePath(string InScopePath)
    {
      mScopePath = InScopePath;
    }

    public ScopePath(ScopePath InDirPath, FileName InFileName)
    {
      mScopePath = Path.Combine(InDirPath.ToString(), InFileName.ToString( ));
    }

    [XmlIgnore]
    public string DirectoryName
    {
      get
      {
        string dirName ;
        if (mScopePath.IsBlank( ) == true)
          dirName = "";
        else
          dirName = Path.GetDirectoryName(mScopePath);
        return dirName;
      }
    }

    [XmlIgnore]
    public string Extension
    {
      get
      {
        string s1 = Path.GetExtension(mScopePath);
        return s1;
      }
    }

    [XmlIgnore]
    public string FileName
    {
      get
      {
        string fileName = Path.GetFileName(mScopePath);
        return fileName;
      }
    }

    [XmlIgnore]
    public bool IsEmpty
    {
      get { return StringExt.IsBlank(mScopePath); }
    }

    [XmlIgnore]
    public bool IsNotEmpty
    {
      get { return !IsEmpty; }
    }

    public string StringValue
    {
      get { return ToString(); }
      set { mScopePath = value; }
    }

    public static FullPath operator +(RootPath InRootPath, ScopePath InScopePath)
    {
      string fullPath = Path.Combine(InRootPath.ToString(), InScopePath.ToString());
      return new FullPath(fullPath);
    }

    public static ScopePath operator +(ScopePath InDirPath, FileName InFileName)
    {
      return new ScopePath( InDirPath, InFileName );
    }

    public override string ToString()
    {
      return mScopePath;
    }

  }
}
