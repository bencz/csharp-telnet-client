using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ftp.Enums
{
  public enum FtpInstruction
  {
    bin,
    cd,   // change current directory of remote
    lcd,  // local change directory
    lls,  // local list contents of current directory
    lpwd, // local print current directory
    ls,   // list files of current remote directory
    rcmd,
    put,
    get,
    md,     // make dir
    mkdir,  // same as md
    open,
    pwd,
    close
  }
  public static class FtpInstructionExt
  {

    public static FtpCommandId? ToCommandId(this FtpInstruction Instruction )
    {
      if ( Instruction == FtpInstruction.bin )
      {

      }

      return null;
    }
    public static FtpInstruction? TryParse(string Text)
    {
      var lcText = Text.ToLower();

      var instName = new string[] {"bin", "cd",
        "lcd", "lls", "lpwd",
        "ls",
        "rcmd",
        "put", "get", "md", "mkdir",
        "open",
        "pwd", "close" };

      var instCode = new FtpInstruction[] {FtpInstruction.bin, FtpInstruction.cd,
        FtpInstruction.lcd, FtpInstruction.lls, FtpInstruction.lpwd,
        FtpInstruction.ls,
        FtpInstruction.rcmd,
        FtpInstruction.put, FtpInstruction.get, FtpInstruction.md, FtpInstruction.mkdir,
        FtpInstruction.open,
        FtpInstruction.pwd, FtpInstruction.close };

      int ix = Array.IndexOf<string>(instName, lcText);

      if (ix == -1)
        return null;
      else
        return instCode[ix];
    }
  }

}
