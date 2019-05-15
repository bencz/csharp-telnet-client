using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ftp.Enums;
using System.Net.Sockets;

namespace AutoCoder.Ftp
{
  public partial class BaseFtpClient
  {

    public FtpResponse RunListCommand(FtpCommandId CommandId, string CmdText)
    {
      ReplyLines replyLines = null;
      CommLog commLog = new CommLog();
      Socket cSocket = null;
      List<FtpDirEntry> listDire = null;

      try
      {
        cSocket = CreatePasvDataConnect(commLog);

        // Send the FTP command,
        replyLines = SendCommand(CommandId, CmdText);

        // 550 = directory not found
        if (replyLines.IncludesReplyCode(550))
        {
          throw new FtpException(
            "Directory to list is not found.", CommandId, replyLines);
        }

        // the reply from the list command on the command connection should say
        // to look on the passive connection for the directory listing.
        var replyLine = replyLines.FindAnyLine(new int[] { 125, 150, 250 });
        if (replyLine == null)
        {
          throw new FtpException(
            "not expected response to LIST command", CommandId, replyLines);
        }

        // receive the directory listing on the passive connection.
        string[] rcvdLines = null;
        {
          Int32 bytes;
          char[] seperator = new char[] { '\n' };

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
        }

        // parse the directory listing
        if ((CommandId == FtpCommandId.LIST) || ( CommandId == FtpCommandId.NLST))
        {
          listDire = new List<FtpDirEntry>();
          foreach (string line in rcvdLines)
          {
            if (line.Length > 0)
            {
              var dire = new FtpDirEntry(line);
              listDire.Add(dire);
            }
          }
        }
      }

      finally
      {
        if (cSocket != null)
          cSocket.Close();
      }

      // receive back the response on the command connection after dir listing
      // sent on the data connection.
      {
        ReplyLines postReplyLines = ReadReply( );
        if (postReplyLines.IncludesReplyCode(550) == true)
        {
          throw new FtpException(
            "Directory to list is not found", CommandId, postReplyLines);
        }
        else
        {
          if (postReplyLines.IncludesAnyReplyCode(new int[] { 226, 250 }) == false)
          {
            string moreInfo =
              "Unexpected FTP server reply following " +
              "data channel disconnect. " +
              postReplyLines.Lines.ToArray();
            throw new FtpException(moreInfo, CommandId, replyLines);
          }
        }
      }

      return new FtpResponse_DirList(
        CommandId,
        commLog, CommandId.ToString(), replyLines, listDire);
    }
  }
}
