using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ftp.Enums;

namespace AutoCoder.Ftp
{
  public class FtpResponse_DirList : FtpResponse
  {

    public FtpResponse_DirList(
      FtpCommandId CommandId,
      CommLog InCommLog, string InCmdText, ReplyLines InReplyLines,
      List<FtpDirEntry> InRcvdData)
      : base( CommandId, InCommLog, InCmdText, InReplyLines, InRcvdData )
    {
    }

    public List<FtpDirEntry> RcvdDirList
    {
      get
      {
        return (List<FtpDirEntry>)RcvdData;
      }
    }
  }
}
