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

    public FtpResponse RunStorCommand(
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
        // create the socket to send on.
          cSocket = CreatePasvDataConnect(CommLog);

        // Send the FTP command,
        replyLines = SendCommand(CommandId, CmdText);

        // send the file bytes on the PASV channel.
        if (CommandId == FtpCommandId.STOR)
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
      }

      finally
      {
        if (cSocket != null)
          cSocket.Close();
      }

      // receive back the response on the command connection after file bytes 
      // sent on the data connection.
      if (CommandId == FtpCommandId.STOR)
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
