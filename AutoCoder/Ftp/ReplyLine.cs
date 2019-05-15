using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Ftp
{
  public class ReplyLine
  {
    string mLine = null;
    int mReplyCode = 0;
    bool mContainsReplyCode = false;
    string mReplyData = null;

    public ReplyLine(string InLine)
    {
      mLine = InLine;
      this.IsPlainText = false;
      Parse();
    }

    public ReplyLine( )
    {
    }

    public void AssignPlainText(string Text)
    {
      this.IsPlainText = true;
      this.mContainsReplyCode = false;
      this.mLine = Text;
    }
    public bool ContainsReplyCode
    {
      get { return mContainsReplyCode; }
    }

    private bool IsPlainText { get; set; }
    public string LineText
    {
      get { return mLine; }
    }

    public int ReplyCode
    {
      get
      {
        if (mContainsReplyCode == false)
          throw new FtpException("Reply line does not contain reply code. " + mLine, null );
        return mReplyCode;
      }
    }

    public string ReplyData
    {
      get { return mReplyData; }
    }

    private void Parse()
    {
      mContainsReplyCode = false;
      if (mLine.Length > 3)
      {
        try
        {
          mReplyCode = Int32.Parse(mLine.Substring(0, 3));
          mContainsReplyCode = true;
        }
        catch (Exception)
        {
        }
      }

      // space after the reply code.
      if ((mContainsReplyCode == true) && (mLine.Length >= 4))
      {
        if (mLine[3] != ' ')
          mContainsReplyCode = false;
      }

      // remainder of the line contains reply data.
      if (mContainsReplyCode == true)
      {
        if (mLine.Length <= 4)
          mReplyData = "";
        else
          mReplyData = mLine.Substring(4).Trim(new char[] { ' ', '\r' });
      }
      else
      {
        mReplyData = mLine.TrimEnd(new char[] { ' ', '\r' });
      }
    }

    public override string ToString()
    {
      if (this.IsPlainText == true)
        return mLine;
      else
      {
        StringBuilder sb = new StringBuilder();
        if (this.ContainsReplyCode == true)
        {
          sb.Append(this.ReplyCode.ToString());
          sb.Append(" ");
        }
        sb.Append(this.ReplyData.Trim());
        return sb.ToString();
      }
    }
  }
}
