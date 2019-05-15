using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using AutoCoder.File;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Ext.System;

namespace AutoCoder.Ftp
{
  public class FtpClient : BaseFtpClient, IDisposable
  {
    string mLocalxDir;
    FullPath mCurrentPath = new FullPath("");
    RootPath mSitePath = null;
    RootPath mHomePath = null;

    public FtpClient()
    {
    }

    /// <summary>
    /// HomePath combined with SitePath. Also makes sure the path starts
    /// with "/".
    /// </summary>
    public RootPath CompleteHomePath
    {
      get
      {
        if ((mSitePath == null) && (mHomePath == null))
          return null;
        string absPath = "";

        // first add the SitePath.
        if (mSitePath != null)
          absPath = Path.Combine(absPath, mSitePath.ToString());

        // next add the HomeRootPath.
        if (mHomePath != null)
          absPath = Path.Combine(absPath, mHomePath.ToString());

        // make sure the resulting path starts with "/".
        if (absPath.SubstringLenient(0, 1) != "/")
          absPath = "/" + absPath;
        
        return new RootPath(absPath);
      }
    }

    public FullPath CurrentPath
    {
      get { return mCurrentPath; }
      set { mCurrentPath = value; }
    }

    public string LocalxDir
    {
      get
      {
        if (mLocalxDir == null)
          return "c:\\";
        else
          return mLocalxDir;
      }
      set { mLocalxDir = value; }
    }

    /// <summary>
    /// HomePath is the root path to all the FTP directories exposed
    /// thru this FtpClient instance. Combines with SitePath to form
    /// the CompleteHomePath.
    /// </summary>
    public RootPath HomePath
    {
      get { return mHomePath ; }
      set { mHomePath = value ; }
    }

    /// <summary>
    /// SitePath is the root path to access FTP directories at the
    /// URL of the site.
    /// </summary>
    public RootPath SitePath
    {
      get { return mSitePath; }
      set { mSitePath = value; }
    }


    public FtpResponse AssureConnected( )
    {
      FtpResponse resp = null;
      if (IsConnected  == false)
      {
        resp = Connect();
      }
      return resp;
    }

    public void AssureCloseConnection()
    {
      if (IsConnected == true)
      {
        base.CloseConnection();
      }
    }

    /// <summary>
    /// make sure the current working directory is the ScopePath from
    /// the CompleteHomePath.
    /// This method assumes that the CurrentPath is accurate with respect
    /// to the working directory of the FTP client.
    /// </summary>
    /// <param name="InPath"></param>
    /// <returns></returns>
    public FtpResponse AssureCurrentDirectory(ScopePath InPath)
    {
      FtpResponse resp = null;
      string comPath = null;
      int comDepth = 0;

      // build the desired full path.
      FullPath rqsFull = CompleteHomePath + InPath;
      int rqsDepth = rqsFull.Depth;

      // step back the CurrentPath to a subset of requested path.
      while (true)
      {
        Pather.GetCommonPath(
          out comPath, out comDepth,
          rqsFull.ToString(), CurrentPath.ToString());
        if (comDepth < CurrentPath.Depth)
        {
          resp = ChangeDirectory("..");
        }
        else
          break;
      }

      string filePath = Pather.GetFilePath(rqsFull.ToString(), CurrentPath.ToString());
      if (filePath.Length > 0)
      {
        if (mCurrentPath.IsEmpty == true)
          filePath = Pather.AssureRootSlash(filePath, '\\');
        ChangeDirectory(filePath);
      }

      return resp;
    }

    public override FtpResponse ChangeDirectory(string InDirName)
    {
      FtpResponse resp = null;
      resp = base.ChangeDirectory( InDirName ) ;

      // update the current working directory.
      if (InDirName == "..")
        mCurrentPath.RemoveTail();
      else
        mCurrentPath.AppendTail(InDirName);

      return resp;
    }

    public FtpResponse ChangeDirectoryToFullPath(FullPath InPath)
    {
      FtpResponse resp = null ;

      // if this FtpClient object contains a SitePath and/or HomePath, then
      // can only change the absolute dir path to a ScopePath. 
      if (CompleteHomePath != null)
        throw new FtpException(
          "Cannot change dir to absolute full path." +
          " FtpClient has a Site and/or Home RootPath." +
          " Use the ChangeDirectoryToScopePath method instead.");

      // make sure path starts with "/".
      string s1 = InPath.ToString( ) ;
      string absPath = null ;
      if ( s1.SubstringLenient(0,1) != "/" )
        absPath = "/" + s1 ;
      else
        absPath = s1 ;
      
      // change the directory to the absolute path.
      CurrentPath.Empty();
      resp = ChangeDirectory(absPath);
      return resp ;
    }

    public FtpResponse ChangeCurrentDirectoryToHome()
    {
      FtpResponse resp = null;
      string comPath = null ;
      int comDepth = 0 ;

      // compare the current path to the home path.
      Pather.GetCommonPath(
        out comPath, out comDepth,
        CompleteHomePath.ToString(), CurrentPath.ToString());

      // start of current directory must match the CompleteHomePath
      int homeDepth = CompleteHomePath.Depth;
      if (comDepth != homeDepth)
        throw new FtpException(
          "AssureHomeDirectory failed. " +
          "Current directory does not start with HomePath. ");

      while (true)
      {
        int curDepth = mCurrentPath.Depth;
        if (curDepth == homeDepth)
          break;

        resp = ChangeDirectory("..");
      }

      return resp;
    }

    /// <summary>
    /// change the working directory to a ScopePath off of the site
    /// RootPath.
    /// </summary>
    /// <param name="InPath"></param>
    public void ChangeDirectoryToScopePath(ScopePath InPath)
    {
      RootPath absHomePath = CompleteHomePath;

      // if this FtpClient object contains a SitePath and/or HomePath, then
      // can only change the absolute dir path to a ScopePath. 
      if (absHomePath == null)
        throw new FtpException(
          "Cannot change dir to absolute scope path." +
          " FtpClient does not have a SitePath and HomeRootPath." +
          " Use the ChangeDirectoryToFullPath method instead.");

      FullPath absPath = absHomePath + InPath;

      CurrentPath.Empty();
      ChangeDirectory(absPath.ToString( ));
    }

    public void ClearDirectory( )
    {
      var list = base.GetDirList( ) ;
      foreach( FtpDirEntry de in list.RcvdDirList )
      {
        if ( de.EntryType == AcFileType.Folder )
        {
          ClearDirectory(de.EntryName);
          base.RemoveDirectory( de.EntryName ) ;
        }
        else
        {
          DeleteFile( de.EntryName ) ;
        }
      }
    }

    public void ClearDirectory( string InDirName )
    {
      ChangeDirectory( InDirName ) ;
      ClearDirectory( ) ;
      ChangeDirectory("..");
    }

    /// <summary>
    /// Connect to the FTP server using the supplied Url, UserName and Password.
    /// Then change directory to the supplied Site and Home root paths.
    /// </summary>
    /// <returns></returns>
    public override FtpResponse Connect()
    {
      FtpResponse resp = base.Connect();

      RootPath absRoot = CompleteHomePath;
      if (absRoot != null)
        ChangeDirectory(absRoot.ToString());

      return resp;
    }

    /// <summary>
    /// Get all the text lines of the FTP file. 
    /// </summary>
    /// <param name="InFtpFileName"></param>
    /// <returns></returns>
    public string[] GetAllLines(string InFtpFileName)
    {
      string[] lines = null;
      string tempPath = null;
      try
      {
        tempPath = Path.GetTempFileName();
        this.GetFile(InFtpFileName, tempPath);
        lines = System.IO.File.ReadAllLines(tempPath);
      }
      finally
      {
        if (tempPath != null)
          System.IO.File.Delete(tempPath);
      }

      return lines;
    }

    /// <summary>
    /// returns the last change date of file or folder.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public DateTime GetChangeDateTime(string InFileName)
    {
      DateTime chgDttm = new DateTime(1, 1, 1);
      
      string fileName = InFileName;
      if (Pather.IsDirectoryName(fileName) == true)
        fileName = fileName + "*";

      FtpResponse_DirList dl = this.GetDirList( fileName );
      foreach (FtpDirEntry de in dl.RcvdDirList)
      {
        if (de.EntryName.ToLower( ) == InFileName.ToLower( ))
        {
          chgDttm = de.ChgDateTime;
          break;
        }
      }

      return chgDttm;
    }

    /// <summary>
    /// return the directory info of a file or folder.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public FtpDirEntry GetFileInfo(string InFileName)
    {
      FtpDirEntry info = null;
      AcFileType fileType = AcFileType.None;

      if (Pather.IsDirectoryName(InFileName) == true)
        fileType = AcFileType.Folder;
      else
        fileType = AcFileType.File;

      info = GetFileInfo(InFileName, fileType);

      return info;
    }

    /// <summary>
    /// return the directory info of a file or folder.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public FtpDirEntry GetFileInfo( string InFileName, AcFileType InFileType )
    {
      FtpDirEntry info = null ;

      string fileName = InFileName;
      if ( InFileType == AcFileType.Folder )
        fileName = fileName + "*";

      FtpResponse_DirList dl = this.GetDirList(fileName);
      foreach (FtpDirEntry de in dl.RcvdDirList)
      {
        if (de.EntryName.ToLower() == InFileName.ToLower())
        {
          info = de;
          break;
        }
      }

      return info;
    }

    /// <summary>
    /// Get the text string contained in the specified file on the FTP site.
    /// </summary>
    /// <param name="InFtpFileName"></param>
    /// <returns></returns>
    public string GetStringContentsOfFile(string InFtpFileName)
    {
      string rv = null;
      string tempPath = null;
      try
      {
        tempPath = Path.GetTempFileName();
        this.GetFile( InFtpFileName, tempPath);
        BufferBuilder bb = new BufferBuilder();
        
        using (FileStream fs = new FileStream(tempPath, FileMode.Open))
        {
          byte[] buf = bb.AppendChunk(512);
          int cx = fs.Read(buf, 0, buf.Length);
          bb.LastChunkLength = cx;
        }

        byte[] bigbuf = bb.GetBuffer();
        Encoding enc = Encoding.UTF8;
        rv = enc.GetString(bigbuf, 0, bigbuf.Length);
      }
      finally
      {
        if (tempPath != null)
          System.IO.File.Delete(tempPath);
      }

      return rv;
    }

    /// <summary>
    /// Write all the text lines to a temporary file, then put that
    /// temporary file to the FTP site.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <param name="InLines"></param>
    /// <returns></returns>
    public FtpResponse PutAllLines(
      string InFileName, string[] InLines)
    {
      FtpResponse resp = null;
      string tempPath = null;

      try
      {
        tempPath = Path.GetTempFileName();
        System.IO.File.WriteAllLines(tempPath, InLines);

        // put the temporary file to the working dir of the ftp site.
        resp = PutFile(InFileName, tempPath);
      }

      finally
      {
        if (tempPath != null)
          System.IO.File.Delete(tempPath);
      }

      return resp;
    }

    /// <summary>
    /// Put a string of text into a file on the FTP site.
    /// Method writes the string to a temporary file on the PC, then puts that
    /// temp file to the working directory of the FTP site.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <param name="InString"></param>
    /// <returns></returns>
    public FtpResponse PutString(
      string InFileName, string InString)
    {
      FtpResponse resp = null;
      string tempPath = null;
      
      try
      {
        tempPath = Path.GetTempFileName();

        // write the text string to the temporary file.
        using (FileStream fs = System.IO.File.OpenWrite(tempPath))
        {
          Encoding enc = Encoding.UTF8;
          byte[] encBytes = enc.GetBytes(InString);

          fs.Write(encBytes, 0, encBytes.Length);
        }

        // put the temporary file to the working dir of the ftp site.
        resp = PutFile(InFileName, tempPath);
      }
      
      finally
      {
        if (tempPath != null)
          System.IO.File.Delete(tempPath);
      }

      return resp;
    }


    #region IDisposable Members

    public void Dispose()
    {
      AssureCloseConnection();
    }

    #endregion
  } // end class FtpClient
}