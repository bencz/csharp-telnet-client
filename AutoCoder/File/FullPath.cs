using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.Text;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  public class FullPath
  {
    string mFullPath = null;

    public FullPath()
    {
      mFullPath = "";
    }

    public FullPath(string InFullPath)
    {
      mFullPath = InFullPath;
    }

    public FullPath(RootPath InRootPath, ScopePath InScopePath)
    {
      mFullPath = 
        Path.Combine(InRootPath.ToString(), InScopePath.ToString());
    }

    public int Depth
    {
      get
      { 
        return Pather.GetDepth(mFullPath); 
      }
    }

    [XmlIgnore]
    public string DirectoryName
    {
      get
      {
        string dirName;
        if (StringExt.IsBlank(mFullPath) == true)
          dirName = "";
        else
          dirName = Path.GetDirectoryName(mFullPath);
        return dirName;
      }
    }

    [XmlIgnore]
    public DirPath DirPath
    {
      get
      {
        string dirName = this.DirectoryName;
        return new DirPath(dirName);
      }
    }

    [XmlIgnore]
    public FileName FileName
    {
      get
      {
        string fileName = Path.GetFileName(mFullPath);
        return new FileName(fileName);
      }
    }

    public string StringValue
    {
      get { return ToString(); }
      set { mFullPath = value; }
    }

    public static FullPath FromString(string InValue)
    {
      return new FullPath(InValue);
    }

    public bool IsEmpty
    {
      get { return StringExt.IsBlank(mFullPath); }
    }

    public bool IsNotEmpty
    {
      get { return !IsEmpty; }
    }

    /// <summary>
    /// Add a path to the tail end of the path.
    /// </summary>
    /// <param name="InAppend"></param>
    public void AppendTail(string InAppend)
    {
      mFullPath = Path.Combine(mFullPath, InAppend);
    }

    /// <summary>
    /// Set the path to an empty string.
    /// </summary>
    public void Empty()
    {
      mFullPath = "" ;
    }

    public ScopePath GetScopePath(RootPath InHomePath)
    {
      string scopePath = 
        Pather.GetFilePath(mFullPath, InHomePath.ToString());
      return new ScopePath(scopePath);
    }

    public RootPath GetRootPath(ScopePath InScopePath)
    {
      string rootPath = Pather.GetRootPath(mFullPath, InScopePath.ToString());
      return new RootPath(rootPath);
    }

    /// <summary>
    /// Remove the last segment of the FullPath. This leaves the path set to
    /// what had been the DirectoryName.
    /// </summary>
    public void RemoveTail()
    {
      string s1 = this.DirectoryName;
      mFullPath = s1;
    }

    public DirPath ToDirPath()
    {
      return new DirPath(this.ToString());
    }

    public override string ToString()
    {
      return mFullPath;
    }

    public static FullPath operator +(FullPath InFullPath, FileName InAppendPath)
    {
      string fullPath = Path.Combine(InFullPath.ToString(), InAppendPath.ToString( ));
      return new FullPath(fullPath);
    }
  }
}
