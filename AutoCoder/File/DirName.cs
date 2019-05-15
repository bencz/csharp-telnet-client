using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.File
{
  public class DirName
  {
    string mDirName;

    public DirName(string InDirName)
    {
      mDirName = InDirName;
    }

    public DirName(DirName InDirName)
    {
      mDirName = InDirName.ToString( ) ;
    }

    public override string ToString()
    {
      return mDirName;
    }
  }
}
