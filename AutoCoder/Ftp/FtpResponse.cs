using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Ftp.Enums;


namespace AutoCoder.Ftp
{

  /// <summary>
  /// FtpResponse is the object returned by the RunCommand method of FtpClient.
  /// It encapsulates all the values returned by the method.
  /// </summary>
  public class FtpResponse
  {
//    CommLog mCommLog = null;

    public FtpResponse(
      FtpCommandId CommandId,
      CommLog InCommLog, string InCmdText, ReplyLines InReplyLines, object InRcvdData)
    {
      this.CommandId = CommandId;
      CommLog = InCommLog;
      CmdText = InCmdText;
      ReplyLines = InReplyLines;
      RcvdData = InRcvdData;
      this.RunDateTime = DateTime.Now;
    }

    public FtpResponse( FtpInstruction Instruction, string CmdText)
    {
      this.CommandId = null;
      this.Instruction = Instruction;
      this.CmdText = CmdText;
      this.ReplyLines = new ReplyLines();
      this.RunDateTime = DateTime.Now;
    }

    public ReplyLines ReplyLines { get; set;}
    
    public CommLog CommLog { get; set; }

    public string CmdText { get; set; }
    
    public object RcvdData { get; set; }

    public FtpCommandId? CommandId
    { get; set; }

    public FtpInstruction? Instruction
    { get; set; }

    private DateTime RunDateTime;
  }
}
