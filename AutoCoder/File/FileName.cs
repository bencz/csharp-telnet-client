using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  /// <summary>
  /// Name of a file, including the extension.
  /// </summary>
  public class FileName : IComparable<FileName>
  {
    string mFileName;

    public FileName(FileBaseName InBaseName, FileExtension InExtension)
    {
      mFileName = InBaseName.ToString() + "." + InExtension.ToString();
    }

    public FileName(FileName InFileName)
    {
      mFileName = InFileName.ToString();
    }

    public FileName(string InFileName)
    {
      mFileName = InFileName;
    }

    /// <summary>
    /// the extension part of the file name.
    /// </summary>
    public FileExtension Extension
    {
      get
      {
        string ext = Path.GetExtension(mFileName);
        if (Stringer.Head(ext, 1) == ".")
          ext = ext.SubstringLenient(1);
        return new FileExtension(ext);
      }

      set
      {
        FileBaseName bs = BaseName;
        FileName nm = bs + value;
        mFileName = nm.mFileName;
      }
    }

    /// <summary>
    /// the base file name ( w/o extension ) part of the file name.
    /// </summary>
    public FileBaseName BaseName
    {
      get
      {
        string baseName = null ;
        string ext = Path.GetExtension( mFileName ) ;
        if (ext.Length == 0)
          baseName = mFileName;
        else
        {
          int fx = mFileName.IndexOf(ext);
          if (fx < 1)
            baseName = "";
          else
            baseName = mFileName.Substring(0, fx);
        }
        return new FileBaseName(baseName);
      }

      set
      {
        FileExtension ext = Extension;
        FileName nm = value + ext;
        mFileName = nm.mFileName;
      }
    }

    public override string ToString()
    {
      return mFileName;
    }

    public int CompareTo(FileName other)
    {
      return( mFileName.CompareTo( other.ToString( ))) ;
    }
 }
}
