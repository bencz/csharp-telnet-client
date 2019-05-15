                                       using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext.System;
using System.Collections;

namespace AutoCoder.Ftp
{

  public class ReplyPair
  {
    bool mReplyFlag = false;
    ReplyLines mReplyLines = null;

    public ReplyPair(bool InReplyFlag, ReplyLines InReplyLines)
    {
      mReplyFlag = InReplyFlag;
      mReplyLines = InReplyLines;
    }

    public bool ReplyFlag
    {
      get { return mReplyFlag; }
      set { mReplyFlag = value; }
    }

    public ReplyLines ReplyLines
    {
      get { return mReplyLines; }
      set { mReplyLines = value; }
    }
  }


  public class ReplyLines : IEnumerable<ReplyLine>
  {
    List<ReplyLine> mLines = new List<ReplyLine>();

    public ReplyLines()
    {
    }

    public ReplyLines( List<string> InLines)
    {
      foreach (var line in InLines)
      {
        AddLine(line);
      }
    }

    /// <summary>
    /// the replyCode from the first reply line. 
    /// </summary>
    public int TopReplyCode
    {
      get { return GetReplyCode(); }
    }

    /// <summary>
    /// the reply codes from all of the reply lines.
    /// </summary>
    public int[] ReplyCodes
    {
      get { return GetReplyCodes(); }
    }

    public List<ReplyLine> Lines
    {
      get { return mLines; }
      set { mLines = value; }
    }

    /// <summary>
    /// enable the lines of the class to be enumerated
    /// </summary>
    /// <returns></returns>
    IEnumerator<ReplyLine> IEnumerable<ReplyLine>.GetEnumerator()
    {
      if (mLines == null)
        yield break;
      else
      {
        foreach (ReplyLine line in mLines)
        {
          yield return line;
        }
      }
    }

    #region IEnumerable Members
    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }
    #endregion

    public void AddLine(string InLine)
    {
      // skip the first lines which are blank.
      if ((mLines.Count == 0) && (StringExt.IsBlank(InLine)))
      {
      }
      else
      {
        ReplyLine rpln = new ReplyLine(InLine);
        mLines.Add(rpln);
      }
    }
    public void AddPlainText( string Text)
    {
      var line = new ReplyLine();
      line.AssignPlainText(Text);
      mLines.Add(line);
    }

    public void AddLines(string[] InLines)
    {
      foreach (string line in InLines)
      {
        AddLine(line);
      }
    }

    public void AddLines( ReplyLines InLines)
    {
      foreach (var line in InLines)
      {
        mLines.Add(line);
      }
    }

    /// <summary>
    /// Find the first reply line that matches any of the reply codes.
    /// </summary>
    /// <param name="InReplyCodes"></param>
    /// <returns></returns>
    public ReplyLine FindAnyLine(int[] InReplyCodes)
    {
      foreach (ReplyLine line in mLines)
      {
        if (line.ContainsReplyCode == false)
          continue;
        if (Array.IndexOf<int>(InReplyCodes, line.ReplyCode) >= 0)
          return line;
      }
      return null;
    }

    /// <summary>
    /// Find the reply line by its reply code
    /// </summary>
    /// <param name="InReplyCode"></param>
    /// <returns></returns>
    public ReplyLine FindLine(int InReplyCode)
    {
      foreach (ReplyLine line in mLines)
      {
        if (line.ContainsReplyCode == false)
          continue;
        if ( InReplyCode == line.ReplyCode)
          return line;
      }
      return null;
    }

    /// <summary>
    /// extract the 3 digit reply code from the start of the first reply line.
    /// </summary>
    /// <returns></returns>
    private int GetReplyCode( )
    {
      int replyCode = 0;
      bool gotReplyCode = false;
      if ((mLines == null) || (mLines.Count == 0))
        throw new FtpException(
          "No reply lines. Cannot extract reply code." ) ;

      // get the reply code from the first non blank reply line.
      foreach (ReplyLine line in mLines)
      {
        if (line.ContainsReplyCode == true)
        {
          gotReplyCode = true;
          replyCode = line.ReplyCode;
          break;
        }
      }

      if (gotReplyCode == false)
      {

        throw new FtpException(
          "Reply lines do not contain 3 digit reply code. " +
          ToLineText( )) ;
      }

      return replyCode;
    }

    private int[] GetReplyCodes()
    {
      List<int> replyCodes = new List<int>();

      if (mLines != null)
      {

        // accumulate the reply codes.
        foreach (ReplyLine line in mLines)
        {
          if (line.ContainsReplyCode == true)
          {
            replyCodes.Add(line.ReplyCode);
          }
        }
      }
      return replyCodes.ToArray();
    }

    /// <summary>
    /// test if any of the reply lines contain a specified reply code.
    /// </summary>
    /// <param name="InReplyCode"></param>
    /// <returns></returns>
    public bool IncludesReplyCode(int InReplyCode)
    {
      ReplyLine line = FindLine(InReplyCode);
      if (line == null)
        return false;
      else
        return true;
    }

    /// <summary>
    /// Test if any of the reply lines contains any of the specified reply codes.
    /// </summary>
    /// <param name="InReplyCodes"></param>
    /// <returns></returns>
    public bool IncludesAnyReplyCode(int[] InReplyCodes)
    {
      ReplyLine line = FindAnyLine(InReplyCodes);
      if (line == null)
        return false;
      else
        return true;
    }

    /// <summary>
    ///  return a string containing a concat of all the reply line text.
    /// </summary>
    /// <returns></returns>
    public string ToLineText()
    {
      StringBuilder sb = new StringBuilder();
      foreach (var line in mLines)
      {
        sb.Append(line.LineText);
      }
      return sb.ToString();
    }
  }
}
