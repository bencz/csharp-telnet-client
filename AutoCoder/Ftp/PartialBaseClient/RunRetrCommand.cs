using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Ext;
using AutoCoder.Ftp.Enums;
using System.Net.Sockets;
using System.IO;

namespace AutoCoder.Ftp
{
  public partial class BaseFtpClient
  {

    public FtpResponse RunRetrCommand(
      FtpCommandId CommandId,
      Nullable<bool> OutNotFound, string CmdText,
      CommLog CommLog, FileStream InPcFileStream)
    {
      ReplyLines replyLines = null;
      object rcvdData = null;
      Socket cSocket = null;

      // return flag. file or dir is not found.
      if (OutNotFound != null)
        OutNotFound = false;

      try
      {
        // create the socket to receive on.
        cSocket = CreatePasvDataConnect(CommLog);

        // Send the FTP command,
        replyLines = SendCommand(CommandId, CmdText);

        if (replyLines.IncludesAnyReplyCode(new int[] { 125, 150 }) == false)
        {
          throw new FtpException(CmdText, CommandId, replyLines);
        }

        // receive the file bytes on the PASV connection.
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

      finally
      {
        if (cSocket != null)
          cSocket.Close();
      }

      // receive back the response on the command connection after all file
      // bytes received on the PASV connection.
      {
        ReplyLines postReplyLines = ReadReply();
        if (postReplyLines.IncludesReplyCode(550) == true)
        {
          if (OutNotFound == null)
            throw new FtpException(CmdText, CommandId, postReplyLines);
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
            throw new FtpException(CmdText, CommandId, replyLines, moreInfo);
          }
        }
      }

      return new FtpResponse(CommandId, CommLog, CmdText, replyLines, rcvdData);
    }
  }
}
