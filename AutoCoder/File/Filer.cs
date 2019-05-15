using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using AutoCoder.Core;

namespace AutoCoder.File
{
  /// <summary>
  /// static file related functions
  /// </summary>
  public class Filer
  {

    /// <summary>
    /// write text line to the end of a text file.
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <param name="InTextLine"></param>
    public static void AppendText(string InFilePath, string InTextLine)
    {
      using (StreamWriter sw = new StreamWriter(InFilePath, true))
      {
        sw.WriteLine(InTextLine) ;
      }
    }

    /// <summary>
    /// write text lines to the end of a text file.
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <param name="InTextLine"></param>
    public static void AppendText(string InFilePath, string[] InTextLines)
    {
      using (StreamWriter sw = new StreamWriter(InFilePath, true))
      {
        foreach (string s1 in InTextLines)
        {
          sw.WriteLine(s1);
        }
      }
    }

    /// <summary>
    /// delete the directory entry. Either a file or folder.
    /// </summary>
    /// <param name="InFullPath"></param>
    public static void DeleteDirectoryEntry(string InFullPath)
    {
      if (IsDirectory(InFullPath) == true)
      {
        System.IO.Directory.Delete(InFullPath, true);
      }
      else
      {
        System.IO.File.Delete(InFullPath);
      }
    }

    /// <summary>
    /// Ensure file exists on the system. If it does not exist,
    /// create the file.
    /// </summary>
    /// <param name="InFullPath"></param>
    public static void EnsureExists(string InFullPath)
    {
      if (System.IO.File.Exists(InFullPath) != true)
      {
        using (Stream st = System.IO.File.Create(InFullPath))
        {
        }
      }
    }

    /// <summary>
    /// Calc a file name which does not exist within the DirPath.
    /// </summary>
    /// <param name="DirPath"></param>
    /// <param name="FileName"></param>
    /// <param name="Ext"></param>
    /// <param name="VersionSuffix"></param>
    /// <param name="MaxVersions"></param>
    /// <returns></returns>
    public static string CalcUniqueName(
      string DirPath, string FileName, string Ext,
      string VersionSuffix = "_v", int MaxVersions = 99 )
    {
      int versionCx = 0;
      string fullPath = null;
      while (true)
      {
        fullPath = Path.Combine(DirPath, FileName);
        if (versionCx > 0)
          fullPath = fullPath + VersionSuffix + versionCx;
        fullPath = fullPath + "." + Ext;

        if (System.IO.File.Exists(fullPath) == false)
          break;

        versionCx += 1;
        if (versionCx > MaxVersions)
          throw new ApplicationException(
            "unique file name versions exceeded. " + fullPath);
      }
      return fullPath;
    }

    /// <summary>
    /// return enum that specifies file is Folder or File.
    /// </summary>
    /// <param name="InFullPath"></param>
    /// <returns></returns>
    public static AcFileType GetFileType( string InFullPath )
    {
      if ( IsDirectory( InFullPath ) == true )
        return AcFileType.Folder ;
      else
        return AcFileType.File ;
    }

    /// <summary>
    /// file is a Hidden file
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <returns></returns>
    public static bool IsHidden(string InFilePath)
    {
      FileAttributes attr = System.IO.File.GetAttributes(InFilePath);
      return ((attr & FileAttributes.Hidden) == FileAttributes.Hidden);
    }

    /// <summary>
    /// file is a Temporary file
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <returns></returns>
    public static bool IsTemporary(string InFilePath)
    {
      FileAttributes attr = System.IO.File.GetAttributes(InFilePath);
      return ((attr & FileAttributes.Temporary) == FileAttributes.Temporary);
    }

    /// <summary>
    /// file is a directory
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <returns></returns>
    public static bool IsDirectory(string InFilePath)
    {
      FileAttributes attr = System.IO.File.GetAttributes(InFilePath);
      return ((attr & FileAttributes.Directory) == FileAttributes.Directory);
    }

  }
}
