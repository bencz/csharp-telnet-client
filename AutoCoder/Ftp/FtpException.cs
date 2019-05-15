using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text ;
using AutoCoder.Ftp.Enums;

namespace AutoCoder.Ftp
{

  public class FtpException : ApplicationException
  {
    string mCmdText = null;
    string mMoreInfo = null;
    ReplyLines mReplyLines = null;

    public string CmdText
    {
      get { return mCmdText; }
    }

    FtpCommandId? _CommandId = null ;
    public FtpCommandId? CommandId
    {
      get 
      { 
        return _CommandId; 
      }
      set { _CommandId = value; }
    }

    public string MoreInfo
    {
      get { return mMoreInfo; }
    }

    public ReplyLines ReplyLines
    {
      get { return mReplyLines; }
    }

    public FtpException(
      string InCmdText, FtpCommandId InCommandId, ReplyLines InReplyLines )
      : base( "FTP command failed. " + InCmdText )
    {
      mCmdText = InCmdText;
      _CommandId = InCommandId;
      mReplyLines = InReplyLines;
    }

    public FtpException(
      string InCmdText, FtpCommandId InCommandId, ReplyLines InReplyLines, Exception InInnerExcp)
      : base("FTP command failed. " + InCmdText, InInnerExcp)
    {
      mCmdText = InCmdText;
      _CommandId = InCommandId;
      mReplyLines = InReplyLines;
    }

    public FtpException(
      string InCmdText, FtpCommandId InCommandId, ReplyLines InReplyLines,
      string InMoreInfo )
      : base("FTP command failed. " + InCmdText + " More info: " + InMoreInfo )
    {
      mCmdText = InCmdText;
      this.CommandId = InCommandId;
      mMoreInfo = InMoreInfo;
      mReplyLines = InReplyLines;
    }

    public FtpException(string InText, Exception InInnerExcp)
      : base(InText, InInnerExcp)
    {
    }

    public FtpException(string InText )
      : base(InText )
    {
    }
  }
}
