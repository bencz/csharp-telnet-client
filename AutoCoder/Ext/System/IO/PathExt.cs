using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System.IO
{
  public static class PathExt
  {
    /// <summary>
    /// Make sure each directory in the directory path exists.
    /// ( input path should not contain a file name. All segments of the
    ///   path are processed as directory names. )
    /// </summary>
    /// <param name="InPath"></param>
    public static void AssureDirectoryExists(this string InPath)
    {
      string pathStub = "";
      List<string> dirParts = new List<string>();
      string curPath = InPath;
      bool isNetworkPath = false;

      if ((InPath.Length > 2) && (InPath.Substring(0, 2) == "\\\\"))
      {
        curPath = "c:\\" + InPath.Substring(2);
        isNetworkPath = true;
      }

      string pathRoot = global::System.IO.Path.GetPathRoot(InPath);

      // crack the path into a list of directory name parts.
      while (curPath != null)
      {
        string dirName = global::System.IO.Path.GetDirectoryName(curPath);
        string fileName = global::System.IO.Path.GetFileName(curPath);

        // directory is null and file name is empty. Input path was a drive name.
        if ((dirName == null) && (fileName.Length == 0))
        {
          pathStub = curPath;
        }

        else if ((isNetworkPath == true) && (dirName == "c:\\"))
        {
          pathStub = "\\\\" + fileName;
          break;
        }

          // the network name in the path.
        else if ((dirName == null) && (fileName.Length > 0) &&
          (fileName == global::System.IO.Path.GetPathRoot(InPath)))
        {
          pathStub = fileName;
        }

        else if (fileName.Length > 0)
        {
          dirParts.Add(fileName);
        }

        curPath = dirName;
      }

      // process the directory parts
      curPath = pathStub;
      for (int ix = dirParts.Count - 1; ix >= 0; ix -= 1)
      {
        string dirName = dirParts[ix];
        curPath = global::System.IO.Path.Combine(curPath, dirName);
        if ( global::System.IO.Directory.Exists(curPath) == false)
        {
          global::System.IO.Directory.CreateDirectory(curPath);
        }
      }
    }

    /// <summary>
    /// compose the full file path name with " - Copy" at the end of the file name part.
    /// </summary>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    public static string ComposeCopyPath(this string FilePath)
    {
      // split name into its components.
      var dirName = global::System.IO.Path.GetDirectoryName(FilePath);
      var fileName = global::System.IO.Path.GetFileNameWithoutExtension(FilePath);
      var extName = global::System.IO.Path.GetExtension(FilePath);

      var copyName = fileName + " - Copy" + extName;
      var copyPath = global::System.IO.Path.Combine(dirName, copyName);

      return copyPath;
    }

    /// <summary>
    /// test if the 2nd path is a generic match to the first path.
    /// </summary>
    /// <param name="FilePath"></param>
    /// <param name="GenericPath"></param>
    /// <returns></returns>
    public static bool IsGenericMatch(string FilePath, string GenericPath )
    {
      bool isMatch = true;

      // check that the directory parts of the path match.
      string dir1 = global::System.IO.Path.GetDirectoryName(FilePath).ToLower();
      string dir2 = global::System.IO.Path.GetDirectoryName(GenericPath).ToLower();
      if (dir1 != dir2)
        isMatch = false;

      // match file name without extension.
      if ( isMatch == true )
      {
        var fileName1 = global::System.IO.Path.GetFileNameWithoutExtension(FilePath).ToLower();
        var fileName2 = global::System.IO.Path.GetFileNameWithoutExtension(GenericPath).ToLower();
        isMatch = IsTextGenericMatch(fileName1, fileName2);
      }

      // match extension parts of the file name.
      if (isMatch == true)
      {
        var ext1 = global::System.IO.Path.GetExtension(FilePath).ToLower();
        var ext2 = global::System.IO.Path.GetExtension(GenericPath).ToLower();
        isMatch = IsTextGenericMatch(ext1, ext2);
      }

      return isMatch;
    }

    private static bool IsTextGenericMatch(string Text, string Generic )
    {
      bool isMatch = true;
      if (Generic.Tail(1) == "*")
      {
        int lx = Generic.Length - 1;
        if (lx > 0)
        {
          if (Text.Length < lx)
            isMatch = false;
          else if (Text.Substring(0, lx) != Generic.Substring(0, lx))
            isMatch = false;
        }
      }
      else if (Text != Generic)
        isMatch = false;
      return isMatch;
    }

    /// <summary>
    /// the file name at the end of the path is a generic name. ( ends with "*" )
    /// </summary>
    /// <param name="FilePath"></param>
    /// <returns></returns>
    public static bool IsGenericName(string FilePath)
    {
      var fileName = global::System.IO.Path.GetFileNameWithoutExtension(FilePath);
      var fileExt = global::System.IO.Path.GetExtension(FilePath);
      if ((fileName.Tail(1) == "*") || (fileExt.Tail(1) == "*"))
        return true;
      else
        return false;
    }

    public static string SetExtension(string FilePath, string extension)
    {
      // get the name without the extension.
      var fileName = global::System.IO.Path.GetFileNameWithoutExtension(FilePath);
      var dirPath = global::System.IO.Path.GetDirectoryName(FilePath);
      var filePath = global::System.IO.Path.Combine(dirPath, fileName + "." + extension);
      return filePath;
    }
  }
}
