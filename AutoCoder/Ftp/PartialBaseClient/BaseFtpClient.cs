using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using AutoCoder.Ext;
using AutoCoder.Ext.System;
using AutoCoder.File;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Ftp.Enums;
using AutoCoder.Ext.System.IO;

namespace AutoCoder.Ftp
{

  public partial class BaseFtpClient
  {
    string mUrl ;
    string mUserName;
    string mPassword;
    int m_iRemotePort, m_iBytes;
    Socket m_objClientSocket;
    Encoding ASCII = Encoding.ASCII;
    bool mIsConnected;
    string m_sMes;

    //Set the size of the packet that is used to read and to write data to the FTP server
    // to the following specified size.
    const int BLOCK_SIZE = 512;

    public bool flag_bool;

    // General variable declaration
    string m_sMessageString;

    TransmitBuffer mTransmitBuffer = new TransmitBuffer();
    byte[] m_aBuffer = new byte[BLOCK_SIZE];

    FtpCommandTable mFtpCommands = null;

    //  Main class constructor
    public BaseFtpClient()
    {
      m_sMessageString = "";
      m_iRemotePort = 21;
      mIsConnected = false;
      int dummy = 22;

      if (dummy == 27)
      {
        Encoding enc = Encoding.UTF8; 
        Stream cmdsStream =
          Assembly.GetExecutingAssembly().GetManifestResourceStream(
          "AutoCoder.Ftp.FtpCommand.xml");
        mFtpCommands = FtpCommandTable.LoadFromXml( cmdsStream, enc ) ;
      }
    }

    public bool IsConnected
    {
      get { return mIsConnected; }
      set { mIsConnected = value; }
    }

    // Set the class MessageString.
    public string MessageString
    {
      get
      {
        return m_sMessageString;
      }
      set
      {
        m_sMessageString = value;
      }
    }

    public string Password
    {
      get { return mPassword; }
      set { mPassword = Stringer.TrimEndingBlanks(value); }
    }

    // Set or get the FTP Port Number of the FTP server that you want to connect.
    public Int32 RemotePort
    {
      get { return m_iRemotePort; }
      set { m_iRemotePort = value; }
    }

    /// <summary>
    /// the byte level transmit buffer used when reading and writing to the 
    /// FTP sockets.
    /// </summary>
    public TransmitBuffer TransmitBuffer
    {
      get { return mTransmitBuffer; }
      set { mTransmitBuffer = value; }
    }

    public string Url
    {
      get { return mUrl; }
      set { mUrl = Stringer.TrimEndingBlanks( value ) ; }
    }

    public string UserName
    {
      get { return mUserName; }
      set { mUserName = Stringer.TrimEndingBlanks( value ) ; }
    }

    /// <summary>
    /// change working directory
    /// </summary>
    /// <param name="InDirName"></param>
    /// <returns></returns>
    public virtual FtpResponse ChangeDirectory(string InDirName)
    {
      FtpResponse resp = null;
      resp = RunCommand( null,  "CWD " + InDirName);
      return resp;
    }

    //  Clean up some variables.
    private void Cleanup()
    {
      if (m_objClientSocket != null)
      {
        m_objClientSocket.Close();
        m_objClientSocket = null;
      }

      IsConnected = false;
    }

    //  Close the FTP connection of the remote server.
    public void CloseConnection()
    {
      if (m_objClientSocket != null)
      {
        // Send an FTP command to end an FTP server system.
        this.RunCommand(null, "QUIT");
      }

      Cleanup();
    }

    //  Close the FTP connection of the remote server.
    public void CloseConnection(CommLog CommLog)
    {
      if (m_objClientSocket != null)
      {
        // Send an FTP command to end an FTP server system.
        this.RunCommand(null, "QUIT", CommLog, null ) ;
      }

      Cleanup();
    }

    /// <summary>
    /// Connect to the FTP server using the supplied Url, UserName and 
    /// Password properties.
    /// </summary>
    /// <returns></returns>
    public virtual FtpResponse Connect()
    {
      ReplyLines replyLines = null;
      ReplyLines loginReplies = new ReplyLines();
      IPEndPoint ep = null;
      CommLog commLog = new CommLog();
      IPAddress ipAddr = null;

      m_objClientSocket =
        new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      if ((Url.Length >= 7) && (Url.Substring(0, 3) == "192"))
      {
        ipAddr = IPAddress.Parse(Url);
      }
      else
      {
        IPHostEntry hostInfo = Dns.GetHostEntry(Url);
        ipAddr = hostInfo.AddressList[0];
      }
      ep = new IPEndPoint(ipAddr, m_iRemotePort);

      // connect to the server
      try
      {
        m_objClientSocket.Connect(ep);
      }
      catch (Exception excp)
      {
        throw new FtpException(
          "Cannot connect to remote FTP server. " + excp.Message, excp);
      }

      // should get a 220 reply code back.
      replyLines = ReadReply( );
      if (replyLines.TopReplyCode != 220)
      {
        CloseConnection(commLog);
        throw new FtpException("Connect to server failed. Reply code not 220.");
      }
      loginReplies.AddLines(replyLines);

      // Send an FTP command to send a user logon ID to the server.
      {
        var resp = this.RunCommand(null, "USER " + UserName, commLog, null);
        replyLines = resp.ReplyLines;
        if (
          AcCommon.IndexOfAny(
          replyLines.ReplyCodes, new int[] { 230, 331 })
          == -1)
        {
          Cleanup();
          throw new FtpException("server did not accept USER command");
        }
        loginReplies.AddLines(replyLines);
      }

      // server wants a password ( did not reply to the USER command with 
      // 230 = user logged on )
      if (AcCommon.IndexOf(replyLines.ReplyCodes, 230) == -1)
      {
        // Send an FTP command to send a user logon password to the server.

        var resp = this.RunCommand(null, "PASS " + mPassword, commLog, null);
        replyLines = resp.ReplyLines;


        if ((replyLines.TopReplyCode != 230) && (replyLines.TopReplyCode != 202))
        {
          Cleanup();
          throw new FtpException("Server did not accept PASS command");
        }
      }
      loginReplies.AddLines(replyLines);

      IsConnected = true;

      return new FtpResponse(
        FtpCommandId.connect, commLog, null, loginReplies, null);
    }

    //  Create a data socket.
    public Socket CreatePasvDataConnect( CommLog InCommLog )
    {
      Int32 index1, index2, len;
      Int32 partCount, i, port;
      string ipData, buf, ipAddress;
      Int32[] parts = new Int32[6];
      char ch;
      Socket s;
      IPEndPoint ep;
      ReplyLines replyLines;

      // Send an FTP command to use a passive data connection.
      try
      {
        replyLines = SendCommand(FtpCommandId.PASV, "PASV");
      }
      catch (SocketException excp)
      {
        throw new FtpException(
          "PASV command failed. SocketException occured. " + excp.ToString());
      }

      var replyLine = replyLines.FindLine(227);    
      if ( replyLine == null )
      {
        throw new FtpException("Server did not send 227 reply to PASV command" ) ;
      }

      index1 = replyLine.ReplyData.IndexOf("(");
      index2 = replyLine.ReplyData.IndexOf(")");
      ipData = replyLine.ReplyData.Substring(index1 + 1, index2 - index1 - 1);

      len = ipData.Length;
      partCount = 0;
      buf = "";

      for (i = 0; i < len; ++i)
      {
        if (partCount > 6)
          break;
        ch = Char.Parse(ipData.Substring(i, 1));

        // data character must be either digit or ","
        if (Char.IsDigit(ch))
          buf += ch;
        else if (ch != ',')
        {
          MessageString = replyLine.LineText ;
          throw new FtpException("Malformed PASV reply: " + replyLine );
        }

        // load the accumulated digit characters into the parts array.
        if ((ch == ',') || (i + 1 == len))
        {
          try
          {
            parts[partCount] = Int32.Parse(buf);
            partCount += 1;
            buf = "";
          }
          catch (Exception)
          {
            MessageString = replyLine.LineText;
            throw new FtpException("Malformed PASV reply: " + replyLine );
          }
        }
      }

      ipAddress = parts[0].ToString() + "." + parts[1].ToString() +
        "." + parts[2].ToString() + "." + parts[3].ToString();

      //  calc the data port number.
      port = parts[4] << 8;
      int dummy1 = parts[4] * 256;
      int dummy2 = dummy1 + parts[5];
      port = port + parts[5];

      s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      IPAddress ipAddr = IPAddress.Parse(ipAddress);
      //      ep = new IPEndPoint(Dns.Resolve(ipAddress).AddressList[0], port);
      //      ep = new IPEndPoint(Dns.GetHostByAddress(ipAddr).AddressList[0], port);

      ep = new IPEndPoint( ipAddr, port);
      // ep = new IPEndPoint(Dns.GetHostEntry(ipAddr).AddressList[0], port);

      try
      {
        s.Connect(ep);
      }
      catch (Exception excp)
      {
        // if you cannot connect to the FTP
        // server that is specified, make the boolean variable false.
        flag_bool = false;
        MessageString = replyLine.LineText ;
        throw new FtpException("CreatePasvDataConnect failed.", excp );
      }

      // if you can connect to the FTP server that is specified, make the 
      // boolean variable true.
      flag_bool = true;
      return s;
    }

    //  Delete a file from the remote FTP server.
    public FtpResponse DeleteFile(string sFileName)
    {
      FtpResponse resp = null;

      if (IsConnected == false)
      {
        Connect();
      }

      // Send an FTP command to delete a file.
      resp = RunCommand( null,  "DELE " + sFileName);

      return resp;
    }

    /// <summary>
    /// Check if directory exists in the current working directory.
    /// </summary>
    /// <param name="InDirName"></param>
    /// <returns></returns>
    public bool DirectoryExists(string InDirName)
    {
      bool dirExists = false;

      // search in the current directory for a dir with name starting
      // with the directory to check for.
      // ( do this because if only the dir name is spcfd, FTP will return
      //   a dir listing of that directory. )
      FtpResponse_DirList dl = this.GetDirList( InDirName + "*" );

      if (dl.RcvdDirList.Count == 0)
        dirExists = false;
      else
      {
        foreach (FtpDirEntry de in dl.RcvdDirList)
        {
          if ((de.EntryName == InDirName) &&
            (de.EntryType == AcFileType.Folder))
          {
            dirExists = true;
            break;
          }
        }
      }
      return dirExists;
    }

    /// <summary>
    /// Check if file exists in the current working directory.
    /// </summary>
    /// <param name="InFileName"></param>
    /// <returns></returns>
    public bool FileExists(string InFileName)
    {
      bool fileExists = false;
      bool notFound = false ;
      FtpResponse_DirList dl = null ;

      // search in the current directory for a dir with name starting
      // with the directory to check for.
      // ( do this because if only the dir name is spcfd, FTP will return
      //   a dir listing of that directory. )
      dl = (FtpResponse_DirList)RunCommand( notFound, "LIST " + InFileName );
      if (notFound == true)
      {
        fileExists = false;
      }
      else
      {

        if (dl.RcvdDirList.Count == 0)
          fileExists = false;
        else
        {
          foreach (FtpDirEntry de in dl.RcvdDirList)
          {
            if ((de.EntryName == InFileName) &&
              (de.EntryType == AcFileType.File))
            {
              fileExists = true;
              break;
            }
          }
        }
      }
      return fileExists;
    }

    /// <summary>
    /// Get full contents of the current FTP directory
    /// </summary>
    /// <returns></returns>
    public FtpResponse_DirList GetDirList()
    {
      var resp = GetDirList("");
      return resp;
    }

    public FtpResponse_DirList GetDirList( string InFilterFileName )
    {
      FtpResponse_DirList resp = null ;
      resp = (FtpResponse_DirList)RunCommand( null, "LIST " + InFilterFileName );
      return resp;
    }

    //  Download a remote file to a local file name and include a path. Then, set the
    //  resume flag. The local file name will be created or will be overwritten,
    //  but the path must exist.
    public FtpResponse GetFile( string FtpFileName, string PcFullPath)
    {
      FtpResponse resp = null;

      SetBinaryMode(true);

      string toPath = System.IO.Path.Combine(PcFullPath, FtpFileName);

      using (FileStream output = new FileStream(toPath, FileMode.Create))
      {
        resp = RunCommand(null, "RETR " + FtpFileName, output);
      }

      return resp;
    }

#if skip
    public string[] GetFileList(CommLog InCommLog)
    {
      string mask = null;
      string[] fileList = GetFileList(InCommLog, mask, "NLST");
      return fileList;
    }
#endif

#if skip
    public string[] GetFileList(CommLog InCommLog, string sMask)
    {
      string[] fileList = GetFileList(InCommLog, sMask, "NLST");
      return fileList;
    }
#endif

#if skip
    // return a list of files in a string() array from the file system.
    public string[] GetFileList( CommLog InCommLog, string InMask, string InListCommand )
    {
      Socket cSocket;
      Int32 bytes;
      char[] seperator = new char[] { '\n' } ;
      ReplyLines replyLines = null;
      ReplyLine replyLine = null;
      string mask = null;
      string[] mess;

      m_sMes = "";

      if (InMask == null)
        mask = "";
      else
        mask = InMask;

      // Check if you are logged on to the FTP server.
      if (IsConnected == false)
      {
        Connect();
      }

      // create the socket to receive on.
      cSocket = CreatePasvDataConnect( InCommLog );

      // Send the GetFileList FTP command,
      replyLines = SendCommand(InListCommand + " " + mask);

      // 550 = directory not found
      if (replyLines.IncludesReplyCode(550))
      {
        mess = null;
      }
      else
      {

        replyLine = replyLines.FindAnyLine(new int[] { 125, 150, 250 });
        if (replyLine == null)
        {
          throw new FtpException(
            "Unexpected FTP server reply to " + InListCommand + " command");
        }

        StringBuilder sb = new StringBuilder();
        while (true)
        {
          Array.Clear(m_aBuffer, 0, m_aBuffer.Length);
          bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0);
          sb.Append(ASCII.GetString(m_aBuffer, 0, bytes));
          if (bytes == 0)
            break;
        }

        mess = sb.ToString( ).Split(seperator);
        cSocket.Close();

        // receive back the response on the command connection after dir listing
        // sent on the data connection.
        replyLines = ReadReply(InCommLog);
        if (replyLines.IncludesAnyReplyCode(new int[] { 226, 250 }) == false)
        {
          throw new FtpException(
            "Unexpected FTP server reply following " +
            InListCommand + " response data");
        }
      }

      return mess;
    }
#endif

#if skip
    //  get the size of the file on the FTP server.
    public Int64 GetFileSize(string sFileName)
    {
      Int64 size;

      if (IsConnected == false)
      {
        Connect();
      }

      // Send an FTP command.
      SendCommand("SIZE " + sFileName);
      size = 0;

      if (m_iRetValue == 213)
      {
        size = Int64.Parse(m_sReply.Substring(4));
      }
      else
      {
        MessageString = m_sReply;
        throw new IOException(m_sReply.Substring(4));
      }

      return size;
    }
#endif

    public string GetWorkingDirectory()
    {
      FtpResponse resp = null;
      resp = RunCommand( null, "PWD");
      string wkDir = (string)resp.RcvdData;
      return wkDir;
    }

    /// <summary>
    /// Make a directory on the remote FTP site.
    /// </summary>
    /// <param name="InDirName"></param>
    /// <returns></returns>
    public FtpResponse MakeDirectory(string InDirName)
    {
      var resp = RunCommand( null, "MKD " + InDirName);
      return resp;
    }

    public FtpResponse PutFile(string InFtpFileName, FileStream InPcStream)
    {
      FtpResponse resp = null;
      resp = RunCommand( null, "STOR " + InFtpFileName, InPcStream);
      return resp;
    }

    public FtpResponse PutFile(string InFtpFileName, string InPcFilePath)
    {
      FtpResponse resp = null;

      // build a complete pc path.
      string pcFilePath = InPcFilePath;
      if (Path.GetPathRoot(InPcFilePath).IsNullOrEmpty())
      {
        var curDir = Directory.GetCurrentDirectory();
        pcFilePath = Path.Combine(curDir, InPcFilePath);
      }

      // file name to put is generic. Loop for each generic and directory in the PC 
      // directory.
      if (PathExt.IsGenericName(pcFilePath) == true)
      {
        resp = new FtpResponse(FtpInstruction.put, "");
        var dirPath = Path.GetDirectoryName(pcFilePath);
        foreach (var item in DirectoryExt.NextGenericDirectoryItem(dirPath, pcFilePath))
        {
          var filePath = item.Item1;
          var fileType = item.Item2;
          if (fileType == AcFileType.File)
          {
            var ftpFileName = Path.GetFileName(filePath);
            var r1 = PutFile(ftpFileName, filePath);
            resp.ReplyLines.AddLines(r1.ReplyLines);
          }
        }
      }

      // put the single file to the FTP site.
      else
      {
        using (FileStream fs = new FileStream(InPcFilePath, FileMode.Open))
        {
          resp = PutFile(InFtpFileName, fs);
        }
      }

      return resp;
    }

    /// <summary>
    /// Remove an empty directory.
    /// </summary>
    /// <param name="InFilePath"></param>
    /// <returns></returns>
    public FtpResponse RemoveDirectory(string InFilePath)
    {
      FtpResponse resp = null;
      resp = RunCommand( null, "RMD " + InFilePath);
      return resp;
    }

    private string ReadLine()
    {
      bool bClearMes = false;
      string line = ReadLine(bClearMes);
      return line;
    }

    //  Read a line from the FTP server.
    private string ReadLine(bool bClearMes)
    {
      char[] seperator = new char[] { '\n' } ;
      string[] mess;
      TransmitBufferItem buf = null;

      if (bClearMes == true)
      {
        m_sMes = "";
      }

      while (true)
      {
        buf = mTransmitBuffer.AddItem(CommDirection.Receive, BLOCK_SIZE);
        buf.UsedSx = m_objClientSocket.Receive( buf.Bytes, buf.Bytes.Length, 0);
        m_sMes += ASCII.GetString( buf.Bytes, 0, buf.UsedSx);
        if (buf.UsedSx < buf.Bytes.Length)
        {
          break;
        }
      }

      mess = m_sMes.Split(seperator);
      if (m_sMes.Length > 2)
      {
        m_sMes = mess[mess.Length - 2];
      }
      else
      {
        m_sMes = mess[0];
      }

      if (m_sMes.Substring(3, 1).Equals(" ") == false)
      {
        return ReadLine(true);
      }

      return m_sMes;
    }

#if skip
    //  Read the reply from the FTP server.
    private ReplyLines ReadReply()
    {
      ReplyLines lines = new ReplyLines( ) ;
      m_sMes = "";
      var line = ReadLine( ) ;
      m_sReply = line ;
      m_iRetValue = Int32.Parse(m_sReply.Substring(0, 3));
      lines.AddLine(line);
      return lines;
    }
#endif

    /// <summary>
    /// receive reply text lines from the FTP remote site.
    /// </summary>
    /// <param name="InCommLog"></param>
    /// <returns></returns>
    private ReplyLines ReadReply( )
    {
      ReplyLines lines = ReadReplyAsciiLines();
      if (lines.ReplyCodes.Length == 0)
      {
        lines = ReadReplyAsciiLines();
      }
      return lines;
    }

    //  Read a line from the FTP server.
    private ReplyLines ReadReplyAsciiLines( )
    {
      StringBuilder sbMsg = new StringBuilder(2000);
      TransmitBufferItem buf = null;
      ReplyLines replyLines = new ReplyLines();

      char[] seperator = new char[] { '\n' };

      // receive all the remote has to send. convert from ascii text to unicode and
      // store in the sbMsg string buffer.
      while (true)
      {
        buf = mTransmitBuffer.AddItem(CommDirection.Receive, BLOCK_SIZE);
        buf.UsedSx = m_objClientSocket.Receive(buf.Bytes, buf.Bytes.Length, 0);
        sbMsg.Append(ASCII.GetString(buf.Bytes, 0, buf.UsedSx));
        if (buf.UsedSx < buf.Bytes.Length)
        {
          break;
        }
      }

      // split the received text into lines on the new line character.
      replyLines.AddLines( sbMsg.ToString( ).Split(seperator));

      // write to the comm log
#if skip
      if (InCommLog != null)
      {
        foreach (var msgLine in replyLines)
        {
          InCommLog.AddItem(CommDirection.Receive, msgLine.LineText );
        }
      }
#endif

      return replyLines;
    }

    public string[] ReceiveData(CommLog InCommLog, Socket InPasvDataSock)
    {
      Int32 bytes;
      char[] seperator = new char[] { '\n' };
      string[] mess;

      m_sMes = "";

      m_sMes = "";
      while (true)
      {
        Array.Clear(m_aBuffer, 0, m_aBuffer.Length);
        bytes = InPasvDataSock.Receive(m_aBuffer, m_aBuffer.Length, 0);
        m_sMes += ASCII.GetString(m_aBuffer, 0, bytes);
        if (bytes == 0)
          break;
      }
      mess = m_sMes.Split(seperator);

      return mess;
    }

    //  Rename a file on the remote FTP server.
    public bool Rename(string InFileName, string InToFileName)
    {
      bool bResult = true;
      FtpResponse resp = null;

      bResult = true;

      // Send an FTP command to rename a file.
      resp = RunCommand( null, "RNFR " + InFileName);

      // Send an FTP command to rename a file to a file name.
      // It will overwrite if newFileName exists.
      resp = RunCommand( null, "RNTO " + InToFileName);

      //  return the final result.
      return bResult;
    }

    public FtpResponse RunCommand(
      Nullable<bool> OutNotFound, string InCmdText)
    {
      FileStream fs = null;
      FtpResponse resp = RunCommand( OutNotFound, InCmdText, fs);
      return resp;
    }

    public FtpResponse RunInstruction(
      Nullable<bool> OutNotFound, FtpInstruction Instruction, string CmdText, string ParmText)
    {
      CommLog commLog = new CommLog();
      FtpResponse resp = null;
      FileStream fs = null;
      string cmdText = String.Empty;
      FtpCommandId cmdId;

      if ( Instruction == FtpInstruction.bin )
      {
        cmdText = "TYPE I";
        cmdId = FtpCommandId.TYPE;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      // cd - change current directory of remote 
      else if (Instruction == FtpInstruction.cd)
      {
        cmdText = "CWD " + ParmText;
        cmdId = FtpCommandId.CWD;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      else if ( Instruction == FtpInstruction.lcd )
      {
        AutoCoder.Ext.System.IO.DirectoryExt.SetCurrentDirectory(ParmText);
        resp = new FtpResponse(Instruction, CmdText);
        resp.ReplyLines.AddPlainText(
          "local current directory:" + Directory.GetCurrentDirectory());
      }

      // lls - local list of current directory contents
      else if ( Instruction == FtpInstruction.lls )
      {
        var curDir = Directory.GetCurrentDirectory();
        var report = localListDirectory( curDir );
        resp = new FtpResponse(Instruction, CmdText);
        foreach (var textLine in report)
        {
          resp.ReplyLines.AddPlainText(textLine);
        }
      }

      else if ( Instruction == FtpInstruction.lpwd )
      {
        var curDir = Directory.GetCurrentDirectory();
        resp = new FtpResponse(Instruction, CmdText);
        resp.ReplyLines.AddPlainText("local current directory:" + curDir);
      }

      // ls - list contents of remote directory
      else if ( Instruction == FtpInstruction.ls )
      {
        cmdText = "LIST";
        cmdId = FtpCommandId.LIST;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      else if ( Instruction == FtpInstruction.put )
      {

        var parmText = ParmText.Trim();
        var rv = parmText.Split(new char[] { ' ' });
        var fromPcFile = rv[0];

        string toFtpFile = String.Empty;
        if (rv.Length == 1)
          toFtpFile = fromPcFile;
        else
          toFtpFile = rv[1];

        resp = PutFile(toFtpFile, fromPcFile);
      }

      // pwd - print current directory of remote 
      else if (Instruction == FtpInstruction.pwd)
      {
        cmdText = "PWD";
        cmdId = FtpCommandId.PWD;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      // make directory on remote site.
      else if ((Instruction == FtpInstruction.mkdir) || (Instruction == FtpInstruction.md))
      {
        cmdText = "MKD" + " " + ParmText;
        cmdId = FtpCommandId.MKD;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      else if (Instruction == FtpInstruction.rcmd )
      {
        cmdText = "RCMD" + " " + ParmText;
        cmdId = FtpCommandId.RCMD;
        resp = RunCommand(OutNotFound, cmdId, cmdText, commLog, fs);
      }

      else
      {
        throw new Exception("Invalid instruction");
      }

      return resp;
    }


    public FtpResponse RunCommand(
      Nullable<bool> OutNotFound, FtpCommandId CmdId, string InCmdText)
    {
      CommLog commLog = new CommLog();
      FileStream fs = null;
      FtpResponse resp = RunCommand(OutNotFound, CmdId, InCmdText, commLog, fs);
      return resp;
    }

    public FtpResponse RunCommand(
      Nullable<bool> OutNotFound, string CmdText, FileStream PcFileStream)
    {
      CommLog commLog = new CommLog();
      var rv = RunCommand(OutNotFound, CmdText, commLog, PcFileStream ) ;
      return rv;
    }

    public FtpResponse RunCommand(
      Nullable<bool> OutNotFound, string CmdText, 
      CommLog CommLog, FileStream InPcFileStream)
    {
      FtpResponse resp = null;

      // return flag. file or dir is not found.
      if ( OutNotFound != null )
        OutNotFound = false;

      // the ftp command is the first word in the cmdText.
      string cmdName;
      {
        var rv = CmdText.firstWord(new char[] { ' ', '\t' });
        cmdName = rv.Item1;
        cmdName = cmdName.ToUpper();
      }

      FtpCommandId cmdId;
      {
        var rv = FtpCommandIdExt.TryParse(cmdName);
        if (rv == null)
          throw new Exception("ftp cmd " + cmdName + " is not found");
        cmdId = rv.Value;
      }

      resp = RunCommand(OutNotFound, cmdId, CmdText, CommLog, InPcFileStream);
      return resp;
    }

    public FtpResponse RunCommand(
      Nullable<bool> OutNotFound, FtpCommandId CmdId, string CmdText,
      CommLog CommLog, FileStream InPcFileStream)
    {
      ReplyLines replyLines = null;
      object rcvdData = null;
      Socket cSocket = null;
      string pwd_WkDir = null;

      // return flag. file or dir is not found.
      if (OutNotFound != null)
        OutNotFound = false;

      // not connected to server.
      if ((CmdId != FtpCommandId.USER) && (CmdId != FtpCommandId.PASS))
      {
        if (IsConnected == false)
        {
          throw new FtpException("Not connected to server");
        }
      }

      // run the list command.
      if ((CmdId == FtpCommandId.LIST) || (CmdId == FtpCommandId.NLST))
      {
        var resp = this.RunListCommand(CmdId, CmdText);
        return resp;
      }

      // some commands run as low level commands.
      if (CmdId == FtpCommandId.BINARY)
      {
        FtpResponse resp = RunCommand(OutNotFound, "TYPE I", InPcFileStream);
        return resp;
      }

      try
      {

        // create the socket to receive on.
        if ((CmdId == FtpCommandId.NLST) || (CmdId == FtpCommandId.LIST)
          || (CmdId == FtpCommandId.STOR) || (CmdId == FtpCommandId.RETR))
        {
          cSocket = CreatePasvDataConnect(CommLog);
        }

        // Send the FTP command,
        replyLines = SendCommand(CmdId, CmdText);

        // 550 = directory not found
        if ((CmdId == FtpCommandId.NLST) || (CmdId == FtpCommandId.LIST))
        {
          if (replyLines.IncludesReplyCode(550))
          {
            rcvdData = null;
            OutNotFound = true;
          }
        }

        // successful delete command gets back a 250.
        else if (CmdId == FtpCommandId.DELE)
        {
          if (replyLines.IncludesReplyCode(250) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // change working directory.
        else if (CmdId == FtpCommandId.CWD)
        {
          if (replyLines.IncludesReplyCode(250) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // MDTM - get file last change date/tim.
        // sample return value : 213 20090217005831
        else if (CmdId == FtpCommandId.MDTM)
        {
          ReplyLine ln = replyLines.FindLine(213);
          if (ln != null)
          {
            DateTime dttm = CrackReplyLine_mdtm(ln.ReplyData);
            rcvdData = dttm;
          }
          else if (replyLines.IncludesReplyCode(550) == true)
          {
            rcvdData = null;
          }
          else
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // make a directory
        else if (CmdId == FtpCommandId.MKD)
        {
          if (replyLines.IncludesAnyReplyCode( new int[] { 250, 257 }) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // remove a directory
        else if (CmdId == FtpCommandId.RMD)
        {
          if (replyLines.IncludesReplyCode(250) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // rename from - the file to be renamed
        else if (CmdId == FtpCommandId.RNFR)
        {
          if (replyLines.IncludesReplyCode(350) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // rename to - change rename from file to this name.
        else if (CmdId == FtpCommandId.RNTO)
        {
          if (replyLines.IncludesReplyCode(250) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
        }

        // successful PWD command gets back a 257.
        else if (CmdId == FtpCommandId.PWD)
        {
          var replyLine = replyLines.FindLine(257);
          if (replyLine == null)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }
          else
          {
            pwd_WkDir = CrackReplyLine_pwd(replyLine.LineText);
          }
        }

        if ((CmdId == FtpCommandId.NLST) || (CmdId == FtpCommandId.LIST))
        {
          string[] rcvdLines = null;
          Int32 bytes;
          char[] seperator = new char[] { '\n' };

          var replyLine = replyLines.FindAnyLine(new int[] { 125, 150, 250 });
          if (replyLine == null)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }

          StringBuilder sb = new StringBuilder();
          while (true)
          {
            Array.Clear(m_aBuffer, 0, m_aBuffer.Length);
            bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0);
            sb.Append(ASCII.GetString(m_aBuffer, 0, bytes));
            if (bytes == 0)
              break;
          }

          rcvdLines = sb.ToString().Split(seperator);
          rcvdData = rcvdLines;
        }

        else if (CmdId == FtpCommandId.RETR)
        {
          if (replyLines.IncludesAnyReplyCode(new int[] { 125, 150 }) == false)
          {
            throw new FtpException(CmdText, CmdId, replyLines);
          }

          while (true)
          {
            Array.Clear(m_aBuffer, 0, m_aBuffer.Length);
            m_iBytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0);
            InPcFileStream.Write(m_aBuffer, 0, m_iBytes);

            if (m_iBytes <= 0)
            {
              break;
            }
          }
        }

        // put the FileStream file bytes.
        if (CmdId == FtpCommandId.STOR)
        {
          byte[] buf = new byte[9999];
          while (true)
          {
            int byteCx = InPcFileStream.Read(buf, 0, buf.Length);
            if (byteCx == 0)
              break;
            cSocket.Send(buf, byteCx, 0);
          }
        }

        // parse the directory listing
        if (CmdId == FtpCommandId.LIST)
        {
          string[] rcvdLines = (string[])rcvdData;
          List<FtpDirEntry> listDire = new List<FtpDirEntry>();
          foreach (string line in rcvdLines)
          {
            if (line.Length > 0)
            {
              FtpDirEntry dire = new FtpDirEntry(line);
              listDire.Add(dire);
            }
          }
          rcvdData = listDire;
        }
      }

      finally
      {
        if (cSocket != null)
          cSocket.Close();
      }

      // receive back the response on the command connection after dir listing
      // sent on the data connection.
      if ((CmdId == FtpCommandId.NLST) || (CmdId == FtpCommandId.LIST)
        || (CmdId == FtpCommandId.STOR) || (CmdId == FtpCommandId.RETR))
      {
        ReplyLines postReplyLines = ReadReply();

        // accum to reply lines of the command.
        replyLines.AddLines(postReplyLines);

        if (postReplyLines.IncludesReplyCode(550) == true)
        {
          if (OutNotFound == null)
            throw new FtpException(CmdText, CmdId, postReplyLines);
          else
            OutNotFound = true;
        }
        else
        {
          if (postReplyLines.IncludesAnyReplyCode(new int[] { 226, 250 }) == false)
          {
            string moreInfo =
              "Unexpected FTP server reply following " +
              "data channel disconnect. " +
              postReplyLines.Lines.ToArray();
            throw new FtpException(CmdText, CmdId, replyLines, moreInfo);
          }
        }
      }

      if ((CmdId == FtpCommandId.NLST) || (CmdId == FtpCommandId.LIST))
        return new FtpResponse_DirList(
          CmdId,
          CommLog, CmdText, replyLines, (List<FtpDirEntry>)rcvdData);
      else if (CmdId == FtpCommandId.PWD)
      {
        return new FtpResponse(CmdId, CommLog, CmdText, replyLines, pwd_WkDir);
      }
      else
        return new FtpResponse(CmdId, CommLog, CmdText, replyLines, rcvdData);
    }

    // -------------------------- CrackReplyLine_pwd ------------------------------
    string CrackReplyLine_pwd(string InReplyLine)
    {
      string wrkdir = null;

      // parse the ftp command string.  
      TextTraits traits = new TextTraits();
      WordCursor csr = Scanner.ScanFirstWord(InReplyLine, traits);

      if (csr.IsAtWord && csr.Word.Value == "257")
      {
        csr = Scanner.ScanNextWord(InReplyLine, csr);
        wrkdir = csr.Word.NonQuotedSimpleValue;
      }
      return wrkdir ;
    }

    DateTime CrackReplyLine_mdtm(string InReplyData)
    {
      int yy = Int32.Parse(InReplyData.Substring(0, 4));
      int mm = Int32.Parse(InReplyData.Substring(4, 2));
      int dd = Int32.Parse(InReplyData.Substring(6, 2));
      int hh = Int32.Parse(InReplyData.Substring(8, 2));
      int min = Int32.Parse(InReplyData.Substring(10, 2));
      int ss = Int32.Parse(InReplyData.Substring(12, 2));
      return new DateTime(yy, mm, dd, hh, min, ss);
    }

#if skip
    // run an FTP command at the server that sends the command on the control channel
    // and receives text line replies on the data channel.
    private string[] RunCommand_DualxChannel_TextResponse(
      CommLog InCommLog, string InCommandString )
    {
      Socket cSocket;
      Int32 bytes;
      char[] seperator = new char[] { '\n' };
      ReplyLines replyLines = null;
      string[] mess;

      m_sMes = "";

      // Check if you are logged on to the FTP server.
      if (IsConnected == false)
      {
        Connect();
      }

      // create the socket to receive on.
      using (cSocket = CreatePasvDataConnect(InCommLog))
      {

        // Send the FTP command string on the control channel.
        replyLines = SendCommand(InCommLog, InCommandString ) ;
        if (replyLines.IncludesReplyCode(500))
          throw new FtpException("Server rejected FTP command");
        else
        {
          var replyLine = replyLines.FindAnyLine(new int[] { 125, 150, 250 });
          if (replyLine == null)
          {
            throw new FtpException(
              "Unexpected FTP server reply to command: " + InCommandString);
          }

          m_sMes = "";
          while (true)
          {
            Array.Clear(m_aBuffer, 0, m_aBuffer.Length);
            bytes = cSocket.Receive(m_aBuffer, m_aBuffer.Length, 0);
            m_sMes += ASCII.GetString(m_aBuffer, 0, bytes);
            if (bytes == 0)
              break;
          }
          mess = m_sMes.Split(seperator);
        }
      }

      replyLines = ReadReply( );
      if (replyLines.IncludesAnyReplyCode(new int[] { 226, 250 }) == false)
      {
        throw new FtpException(
          "Unexpected FTP server reply following response to command: " +
          InCommandString );
      }

      return mess;
    }
#endif
    private List<string> localListDirectory(string DirPath)
    {
      var report = new List<string>();

      var files = Directory.GetFiles(DirPath);
      foreach (var fileName in files)
      {
        report.Add("          " + Path.GetFileName(fileName));
      }

      var subDirList = Directory.GetDirectories(DirPath);
      foreach (var dirName in subDirList)
      {
        report.Add("<dir>     " + Path.GetFileName(dirName));
      }

      return report;
    }

    private ReplyLines SendCommand(
      FtpCommandId CommandId, string InCommand )
    {
      ReplyLines replyLines = null;
      byte[] cmdbytes = ASCII.GetBytes( InCommand + Environment.NewLine);
      m_objClientSocket.Send(cmdbytes, cmdbytes.Length, 0);
      replyLines = ReadReply( );
      return replyLines;
    }

    // ifthe value of mode is true, set the binary mode for downloads. Otherwise, set ASCII mode.
    public void SetBinaryMode(bool bMode)
    {
      FtpResponse resp = null;

      if (bMode == true)
      {
        // Send the FTP command to set the binary mode.
        // (TYPE is an FTP command that is used to specify representation type.)
        resp = RunCommand( null, "TYPE I");
      }
      else
      {
        // Send the FTP command to set ASCII mode.
        // (TYPE is a FTP command that is used to specify representation type.)
        resp = RunCommand( null, "TYPE A");
      }

      if ( resp.ReplyLines.IncludesReplyCode( 200 ) == false )
      {
        MessageString = null;
        throw new FtpException("Server rejected SetBinaryMode command");
      }
    }

#if skip
    //  This is a function that is used to upload a file from your local hard disk to your FTP site.
    public void UploadFile( CommLog InCommLog, string sFileName)
    {
      UploadFile( InCommLog, sFileName, false);
    }
#endif

#if skip
    //  This is a function that is used to upload a file from your local hard disk to your FTP site
    //  and then set the resume flag.
    public void UploadFile( CommLog InCommLog, string sFileName, bool bResume)
    {
      Socket cSocket;
      long offset;
      FileStream input;
      bool bFileNotFound;

      if (IsConnected != true)
        Connect();

      cSocket = CreatePasvDataConnect( InCommLog );
      offset = 0;

      if (bResume)
      {
        try
        {
          SetBinaryMode(true);
          offset = GetFileSize(sFileName);
        }
        catch (Exception)
        {
          offset = 0;
        }
      }

      if (offset > 0)
      {
        SendCommand("REST " + offset);
        if (m_iRetValue != 350)
        {
          // The remote server may not support resuming.
          offset = 0;
        }
      }

      // Send an FTP command to store a file.
      SendCommand("STOR " + Path.GetFileName(sFileName));
      if ((m_iRetValue != 125) && (m_iRetValue != 150))
      {
        MessageString = m_sReply;
        throw new IOException(m_sReply.Substring(4));
      }

      // Check to see if the file exists before the upload.
      bFileNotFound = false;
      if (System.IO.File.Exists(sFileName))
      {
        //  Open the input stream to read the source file.
        input = new FileStream(sFileName, FileMode.Open);
        if (offset != 0)
        {
          input.Seek(offset, SeekOrigin.Begin);
        }

        // Upload the file.
        m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length);
        while (m_iBytes > 0)
        {
          cSocket.Send(m_aBuffer, m_iBytes, 0);
          m_iBytes = input.Read(m_aBuffer, 0, m_aBuffer.Length);
        }

        input.Close();
      }
      else
      {
        bFileNotFound = true;
      }

      if (cSocket.Connected)
      {
        cSocket.Close();
      }

      // Check the return value if the file was not found.
      if (bFileNotFound)
      {
        MessageString = m_sReply;
        throw new IOException("The file: " + sFileName + " was not found." +
        " Cannot upload the file to the FTP site.");
      }

      ReadReply();
      if ((m_iRetValue != 226) && (m_iRetValue != 250))
      {
        MessageString = m_sReply;
        throw new IOException(m_sReply.Substring(4));
      }
    }
#endif

  }
}
