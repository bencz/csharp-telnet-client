using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutoCoder.File
{
  public class DirPath
  {
    string mDirPath = null;

    public DirPath(string InDirPath)
    {
      mDirPath = InDirPath;
    }

    public DirPath(DirPath InDirPath)
    {
      mDirPath = InDirPath.ToString();
    }

    public DirPath(RootPath InRootPath, ScopePath InScopePath)
    {
      string dirPath = Path.Combine(InRootPath.ToString(), InScopePath.ToString());
      mDirPath = dirPath;
    }

    public ScopePath GetScopePath(RootPath InHomePath)
    {
      string scopePath =
        Pather.GetFilePath(mDirPath, InHomePath.ToString());
      return new ScopePath(scopePath);
    }

    public override string ToString()
    {
      return mDirPath;
    }

    /// <summary>
    /// a FileName joined to a DirPath forms a FullPath
    /// </summary>
    /// <param name="InDirPath"></param>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public static FullPath operator +(DirPath InDirPath, FileName InFileName)
    {
      string fullPath = Path.Combine(InDirPath.ToString(), InFileName.ToString());
      return new FullPath(fullPath);
    }

    /// <summary>
    /// a DirName joined to a DirPath forms a longer DirPath.
    /// </summary>
    /// <param name="InDirPath"></param>
    /// <param name="InDirName"></param>
    /// <returns></returns>
    public static DirPath operator +(DirPath InDirPath, DirName InDirName)
    {
      string dirPath = Path.Combine(InDirPath.ToString( ), InDirName.ToString());
      return new DirPath(dirPath);
    }

    /// <summary>
    /// a DirPath joined to another DirPath forms a longer DirPath.
    /// </summary>
    /// <param name="InDirPath"></param>
    /// <param name="InDirName"></param>
    /// <returns></returns>
    public static DirPath operator +(DirPath InDirPath1, DirPath InDirPath2)
    {
      string dirPath = Path.Combine(InDirPath1.ToString(), InDirPath2.ToString());
      return new DirPath(dirPath);
    }

  }
}
