using AutoCoder.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.IO
{
  public static class DirectoryExt
  {
    /// <summary>
    /// not needed. use System.IO.Directory.Delete with the 2nd parm set to true instead.
    /// </summary>
    /// <param name="DirectoryPath"></param>
    public static void ClearAndDelete(this string DirectoryPath )
    {
      // delete all the files in the directory
      foreach( var filePath in global::System.IO.Directory.GetFiles( DirectoryPath ))
      {
        global::System.IO.File.Delete(filePath);
      }

      // clear and delete all the folders.
      foreach( var subFolderpath in global::System.IO.Directory.GetDirectories(DirectoryPath))
      {
        subFolderpath.ClearAndDelete();
      }

      // delete this directory itself.
      global::System.IO.Directory.Delete(DirectoryPath);
    }

    /// <summary>
    /// return a sequence containing the directories and files contained in a directory.
    /// </summary>
    /// <param name="DirPath"></param>
    /// <returns></returns>
    public static IEnumerable<Tuple<string,AcFileType>> NextDirectoryItem(string DirPath)
    {
      var dirItems = global::System.IO.Directory.GetDirectories(DirPath);
      foreach (var dirItem in dirItems)
      {
        yield return new Tuple<string, AcFileType>(dirItem, AcFileType.Directory);
      }

      var fileItems = global::System.IO.Directory.GetFiles(DirPath);
      foreach (var fileItem in fileItems)
      {
        yield return new Tuple<string, AcFileType>(fileItem, AcFileType.File);
      }

      yield break;
    }

    /// <summary>
    /// return a sequence containing the files and directories of a directory with names which
    /// generically match the generic match path.
    /// </summary>
    /// <param name="DirPath"></param>
    /// <param name="GenericMatchPath"></param>
    /// <returns></returns>
    public static IEnumerable<Tuple<string,AcFileType>> NextGenericDirectoryItem( 
      string DirPath, string GenericMatchPath )
    {
      foreach( var item in NextDirectoryItem(DirPath))
      {
        if (PathExt.IsGenericMatch(item.Item1, GenericMatchPath) == true)
          yield return item;
      }
      yield break;
    }

    public static string SetCurrentDirectory( string Path)
    {
      string path = Path.Trim();

      var pathRoot = global::System.IO.Path.GetPathRoot(path);

      if ( path == "..")
      {
        var curDir = Directory.GetCurrentDirectory();
        var dirPath = global::System.IO.Path.GetDirectoryName(curDir);
        Directory.SetCurrentDirectory(dirPath);
      }
      else if ( pathRoot.IsNullOrEmpty() == false)
      {
        Directory.SetCurrentDirectory(path);
      }
      else
      {
        var setDir = global::System.IO.Path.Combine(
          Directory.GetCurrentDirectory(), path);
        Directory.SetCurrentDirectory(setDir);
      }
      return Directory.GetCurrentDirectory();
    }
  }
}
