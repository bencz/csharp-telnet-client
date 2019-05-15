using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.File
{
  /// <summary>
  /// static Path methods
  /// </summary>
  public static class Pather
  {

    public static string AppendTail(string InPath, string InAppend)
    {
      string s1 = Path.Combine(InPath, InAppend);
      return s1;
    }

    public static string AppendHead( string InPath, string InAppend )
    {
      string s1 = Path.Combine( InAppend, InPath ) ;
      return s1 ;
    }

    /// <summary>
    /// Make sure each directory in the directory path exists.
    /// ( input path should not contain a file name. All segments of the
    ///   path are processed as directory names. )
    /// </summary>
    /// <param name="InPath"></param>
    public static void AssureDirectoryExists(string InPath)
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

      string pathRoot = Path.GetPathRoot(InPath);

      // crack the path into a list of directory name parts.
      while (curPath != null)
      {
        string dirName = System.IO.Path.GetDirectoryName(curPath);
        string fileName = System.IO.Path.GetFileName(curPath);

        // directory is null and file name is empty. Input path was a drive name.
        if ((dirName == null) && (fileName.Length == 0))
        {
          pathStub = curPath;
        }

        else if (( isNetworkPath == true ) && ( dirName == "c:\\" ))
        {
          pathStub = "\\\\" + fileName ;
          break;
        }

          // the network name in the path.
        else if ((dirName == null) && (fileName.Length > 0) &&
          ( fileName == Path.GetPathRoot(InPath)))
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
        curPath = Path.Combine(curPath, dirName);
        if (Directory.Exists(curPath) == false)
        {
          Directory.CreateDirectory(curPath);
        }
      }
    }

    /// <summary>
    /// Append the directory slash char to front of path string if it does
    /// not have one.
    /// </summary>
    /// <param name="InPath"></param>
    /// <param name="InSlashChar"></param>
    /// <returns></returns>
    public static string AssureRootSlash(string InPath, char InSlashChar)
    {
      string rootPath = InPath;
      if (InPath.Length == 0)
        rootPath = new string(InSlashChar, 1);
      else
      {
        char ch1 = InPath[0];
        if ((ch1 != '/') || (ch1 != '\\'))
          rootPath = InSlashChar + InPath;
      }
      return rootPath;
    }

    /// <summary>
    /// get the path which is common between two paths.
    /// </summary>
    /// <param name="OutCommonPath"></param>
    /// <param name="OutCommonDepth"></param>
    /// <param name="InPath1"></param>
    /// <param name="InPath1Depth"></param>
    /// <param name="InPath2"></param>
    public static void GetCommonPath(
      out string OutCommonPath, out int OutCommonDepth,
      string InPath1, string InPath2)
    {
      string commonPath = "";
      int commonDepth = 0;

      string path1 = InPath1;
      string path2 = InPath2;
      while (path1.Length > 0)
      {
        string[] rv1 = Pather.Head(path1);
        string[] rv2 = Pather.Head(path2);

        string dirPath1 = rv1[0];
        string dirPath2 = rv2[0];
        if (dirPath1 != dirPath2)
          break;

        commonPath = Path.Combine(commonPath, dirPath1);
        commonDepth += 1;
        path1 = rv1[1];
        path2 = rv2[1];
      }

      OutCommonPath = commonPath;
      OutCommonDepth = commonDepth;
    }

    /// <summary>
    /// Get the file name.ext from the end of a full path. 
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string GetFileName(string InPath)
    {
      string s1 = Path.GetFileName(InPath);
      if (s1 == null)
        s1 = "";
      return s1;
    }

    /// <summary>
    /// calc the directory path depth. 
    /// The root path "/" counts for one level of depth. /dir/file.ext returns a
    /// depth of 3. dir/file.ext returns depth = 2.
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static int GetDepth(string InPath)
    {
      string path = InPath;
      int depth = 0;
      while (path != null && path.Length > 0)
      {
        ++depth;
        path = Path.GetDirectoryName(path);
      }
      return depth;
    }

    /// <summary>
    /// Get the directory path portion of a full path.
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string GetDirectoryName(string InPath)
    {
      string s1 = null ;
      if (InPath.Length == 0)
        s1 = "";
      else
      {
        s1 = Path.GetDirectoryName(InPath);
        if (s1 == null)
          s1 = "";
      }
      return s1;
    }

    /// <summary>
    /// Get the file path part of a path which starts after end of RootPath. 
    /// </summary>
    /// <param name="InFullPath"></param>
    /// <param name="InRootPath"></param>
    /// <returns></returns>
    public static string GetFilePath(string InFullPath, string InRootPath)
    { 
      // todo: extract the first slash char used in the paths.  Then make sure all
      //       the slash chars are that char.

      // simplify the comparison by making sure both paths use the same slash char.
      string fullPath = InFullPath.ReplaceAll( '/', '\\' ) ;
      string rootPath = InRootPath.ReplaceAll('/', '\\');

      // if one of the paths starts with root slash, make sure the other does also.
      if ((fullPath.Length > 0) && (rootPath.Length > 0))
      {
        int fx1 = Scanner.ScanNotEqual(rootPath, 0, '\\').ResultPos;
        int fx2 = Scanner.ScanNotEqual(fullPath, 0, '\\').ResultPos;
        if ((fx1 == -1) && (fx2 != -1))
          rootPath = '\\' + rootPath;
        else if ((fx1 != -1) && (fx2 == -1))
          fullPath = '\\' + fullPath;
      }

      string filePath = "";
      while(true)
      {
        // leave loop when the remaining rootPath is the RootPath to crack on.
        if (fullPath == rootPath)
          break;

        // crack dirPath on the last "/".
        string s1 = Pather.GetDirectoryName(fullPath);
        string s2 = Pather.GetFileName(fullPath);

        // accumulate to the cracked right side path.
        if (filePath.Length == 0)
          filePath = s2;
        else
          filePath = Path.Combine(s2, filePath);

        fullPath = s1;
      }

      return filePath;
    }

    public static string GetRootPath(string InFullPath, string InScopePath)
    {
      string fullPath = InFullPath;
      string rootPath = "";
      while (true)
      {
        // the head has been split off from the front of the fullPath to
        // the point that fullPath and InScopePath are the same.
        if (fullPath == InScopePath)
          break;

          // the split from fullPath is now shorter than ScopePath. this means
          // ScopePath is not a subset of FullPath.
        else if (fullPath.Length < InScopePath.Length)
        {
          rootPath = null;
          break;
        }

        // crack the root directory from the front of fullPath.
        string[] rv = Head(fullPath);

        // accumulate to the rootPath which is cracked from the front of fullPath.
        rootPath = Path.Combine(rootPath, rv[0]);

        fullPath = rv[1];
      }
      return rootPath;
    }
    /// <summary>
    /// Create a temporary file with the extension specified. Use the GetTempFileName
    /// method to assign the name and location of the file, but dont use the file
    /// that method creates. If the assigned name, with extension, exists, use a loop
    /// to try again. Use the callback method after trying 100 times. The callback
    /// method then returns true to contine the try loop.
    /// </summary>
    /// <param name="InExt"></param>
    /// <param name="InContinueLoop"></param>
    /// <returns></returns>
    public static string GetTempFileName(string InExt, delContinueLoop InContinueLoop)
    {
      string extFullPath = null;
      IOException lastExcp = null;

      int loopCx = 0;
      while (extFullPath == null)
      {
        // dont loop forever.
        loopCx += 1;
        if (loopCx > 100)
        {
          if (InContinueLoop == null)
            throw new Exception("GetTempFileName method error", lastExcp);
          else
          {
            string s1 =
              "Temp file name clash." + Environment.NewLine +
              "Continue trying to assign temp file name?";
            bool rc = InContinueLoop(s1, loopCx);
            if (rc == false)
            {
              extFullPath = null;
              break;
            }
            loopCx = 0;
          }
        }

        // use framework method to create a temp file.
        string tempPath = Path.GetTempFileName();

        // remove the ext part of the temp file and replace with desired ext.
        string tempDirPath = Path.GetDirectoryName(tempPath);
        string extFileName = Path.GetFileNameWithoutExtension(tempPath) + "." + InExt;
        extFullPath = Path.Combine(tempDirPath, extFileName);

        // create the temp file with correct ext.
        try
        {
          FileStream fs = System.IO.File.Open(extFullPath, FileMode.CreateNew);
          fs.Close();
        }
        catch (IOException excp)
        {
          lastExcp = excp;
          if (excp.Message.Contains("already exists") == false)
            throw new Exception("PathExt.GetTempFileName method failed.", lastExcp);

          extFullPath = null;
        }
      }

      return extFullPath;
    }

    /// <summary>
    /// Crack path on its head, the root dir in the path.
    /// ( a path w/o a slash returns rootDir as an empty string )
    /// This is effectively the opposite of GetFileName and GetDirectoryName.
    /// Another name for this method would be "GetRootDirectory" and 
    /// "GetScopePath".
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string[] Head(string InPath)
    {
      string path = InPath;
      string rootDir = "";
      string remPath = "";

      // loop cracking names off the right side of the path until
      // a simple name remains.
      while (true)
      {
        string s1 = null ;
        string s2 = null ;

        if (path.Length == 0)
        {
          s1 = "";
          s2 = "";
        }
        else
        {
          s1 = Pather.GetDirectoryName(path);
          s2 = Pather.GetFileName(path);
        }

        // simple name from left side of path remains. This is the root
        // dir of the path.
        if (s1.Length == 0)
        {
          rootDir = path;
          break;
        }

        else
        {
          remPath = Path.Combine(s2, remPath);
          path = s1;
        }
      }

      return new string[] { rootDir, remPath };
    }

    /// <summary>
    /// name is in the form of a directory name. ( no "." in the name )
    /// </summary>
    /// <param name="InName"></param>
    /// <returns></returns>
    public static bool IsDirectoryName(string InName)
    {
      if (InName.IndexOf('.') >= 0)
        return false;
      else
        return true;
    }

    /// <summary>
    /// Split InPath into the input RootPath and the path that remains
    /// after the RootPath.
    /// </summary>
    /// <param name="InPath"></param>
    /// <param name="InRootPath"></param>
    /// <returns></returns>
    public static string[] Split(string InPath, string InRootPath)
    {
      string remPath = InPath;
      string rootPath = InRootPath;
      string rootRv = "";

      while (rootPath.Length > 0)
      {
        string[] rv1 = Head(rootPath);
        string[] rv2 = Head(remPath);

        string s1 = rv1[0];
        string s2 = rv2[0];
        if (s1 != s2)
          break;

        rootRv = Path.Combine(rootRv, s1);

        rootPath = rv1[1];
        remPath = rv2[1];
      }

      return new string[2] { rootRv, remPath };
    }


    /// <summary>
    /// Crack path on its tail, the file name or right side dir name.
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public static string[] Tail(string InPath)
    {
      var s1 = Path.GetDirectoryName(InPath);
      var s2 = Path.GetFileName(InPath);

      return new string[] { s1, s2 };
    }

    public delegate bool delContinueLoop(string InMessage, int InCurrentLoopCount);


  }
}
