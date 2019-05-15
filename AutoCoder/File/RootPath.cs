using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  public class RootPath
  {
    string mRootPath;

    public RootPath()
    {
      mRootPath = "";
    }

    public RootPath(string InRootPath)
    {
      mRootPath = InRootPath;
    }

    public RootPath(RootPath InRootPath)
    {
      mRootPath = InRootPath.ToString();
    }

    /// <summary>
    /// combine two root paths into a single root. 
    /// </summary>
    /// <param name="InRootPath1"></param>
    /// <param name="InRootPath2"></param>
    public RootPath(RootPath InRootPath1, RootPath InRootPath2 )
    {
      mRootPath = Path.Combine(InRootPath1.ToString(), InRootPath2.ToString());
    }

    public int Depth
    {
      get { return Pather.GetDepth(mRootPath); }
    }

    public string DirectoryName
    {
      get
      {
        string dirName;
        if (StringExt.IsBlank(mRootPath) == true)
          dirName = "";
        else
          dirName = Path.GetDirectoryName(mRootPath);
        return dirName;
      }
    }

    public string FileName
    {
      get
      {
        string fileName = Path.GetFileName(mRootPath);
        return fileName;
      }
    }

    public bool IsEmpty
    {
      get { return StringExt.IsBlank(mRootPath); }
    }

    public bool IsNotEmpty
    {
      get { return !IsEmpty; }
    }

    public string StringValue
    {
      get { return mRootPath; }
      set { mRootPath = value; }
    }

    public DirPath ToDirPath()
    {
      return new DirPath(this.ToString());
    }

    public override string ToString()
    {
      return mRootPath;
    }

    public static RootPath operator +(RootPath InRootPath1, RootPath InRootPath2)
    {
      return new RootPath(InRootPath1, InRootPath2);
    }

  }
}

