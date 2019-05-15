using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ftp.Enums
{
  public enum FtpCommandId
  {
    BINARY, CWD, DELE, 
    NLST, 
    LIST, 
    MDTM, 
    MKD, 
    NOOP,
    PASS,
    PASV, PWD, QUIT,
    QUOTE, 
    RCMD,
    RETR, 
    RMD, 
    RNFR, 
    RNTO, 
    SITE, 
    STOR, 
    TYPE,
    USER,
    connect
  }

  public static class FtpCommandIdExt
  {

    public static FtpCommandId? TryParse(string Text)
    {
      var lcText = Text.ToLower();

      var cmdName = new string[] {"binary",
        "cwd", "dele", "nlst",
        "list", "mdtm", "mkd", "noop",
        "pass", "pasv", "pwd", "quit",
        "quote", "rcmd", "retr", "rmd",
        "rnfr",
        "rnto", "site", "stor", "type",
        "user", "connect" };

      var cmdCode = new FtpCommandId[] {FtpCommandId.BINARY,
        FtpCommandId.CWD, FtpCommandId.DELE, FtpCommandId.NLST,
        FtpCommandId.LIST, FtpCommandId.MDTM, FtpCommandId.MKD, FtpCommandId.NOOP,
        FtpCommandId.PASS, FtpCommandId.PASV, FtpCommandId.PWD, FtpCommandId.QUIT,
        FtpCommandId.QUOTE, FtpCommandId.RCMD, FtpCommandId.RETR, FtpCommandId.RMD,
        FtpCommandId.RNFR,
        FtpCommandId.RNTO, FtpCommandId.SITE, FtpCommandId.STOR, FtpCommandId.TYPE,
        FtpCommandId.USER, FtpCommandId.connect };

      int ix = Array.IndexOf<string>(cmdName, lcText);

      if (ix == -1)
        return null;
      else
        return cmdCode[ix];
    }

#if skip
    public static FtpCommandId? TryParse(string Text)
    {
      if (Text == "BINARY")
        return FtpCommandId.BINARY;
      else if (Text == "CWD")
        return FtpCommandId.CWD;
      else if (Text == "DELE")
        return FtpCommandId.DELE;
      else if (Text == "LIST")
        return FtpCommandId.LIST;
      else if (Text == "NLST")
        return FtpCommandId.NLST;
      else if (Text == "MDTM")
        return FtpCommandId.MDTM;
      else if (Text == "MKD")
        return FtpCommandId.MKD;
      else if (Text == "NOOP")
        return FtpCommandId.NOOP;
      else if (Text == "PASS")
        return FtpCommandId.PASS;
      else if (Text == "PASV")
        return FtpCommandId.PASV;
      else if (Text == "PWD")
        return FtpCommandId.PWD;
      else if (Text == "QUOTE")
        return FtpCommandId.QUOTE;
      else if (Text == "QUIT")
        return FtpCommandId.QUIT;
      else if (Text == "RETR")
        return FtpCommandId.RETR;
      else if (Text == "RMD")
        return FtpCommandId.RMD;
      else if (Text == "RNFR")
        return FtpCommandId.RNFR;
      else if (Text == "RNTO")
        return FtpCommandId.RNTO;
      else if (Text == "SITE")
        return FtpCommandId.SITE;
      else if (Text == "STOR")
        return FtpCommandId.STOR;
      else if (Text == "TYPE")
        return FtpCommandId.TYPE;
      else if (Text == "USER")
        return FtpCommandId.USER;
      else
        return null;
    }
#endif

  }
}
