using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Parse;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.Parse
{
  /// <summary>
  /// words in a stmt which span multiple atomic words
  /// Examples being "create table", "results sets", ...
  /// </summary>
  public class StmtWordSpan : List<StmtWord>
  {

    /// <summary>
    /// the combined WordText of the component StmtWords, with a
    /// single space between each word.
    /// </summary>
    public string WordText
    {
      get
      {
        StringBuilder sb = new StringBuilder();
        foreach (StmtWord w1 in this)
        {
          if (sb.Length > 0)
            sb.Append(" ");
          sb.Append(w1.WordText);
        }
        return sb.ToString();
      }
    }

    public string UpperWordText
    {
      get
      {
        string s1 = WordText ;
        return s1.ToUpper( ) ;
      }
    }

    public StmtWordSpan AddWord(StmtWord InWord)
    {
      this.Add(InWord);
      return this;
    }

    /// <summary>
    /// Add word to the span.  Throw an error if the word value is not the expected
    /// value.
    /// </summary>
    /// <param name="InWord"></param>
    /// <param name="InExpectedWordText"></param>
    /// <returns></returns>
    public StmtWordSpan AddWord(StmtWord InWord, MonoCaseString InExpectedWordText )
    {
      if (InExpectedWordText != InWord.WordText)
      {
        throw new ApplicationException(
          "Expecting value " + InExpectedWordText.ToString() +
          ". Actual value is " + InWord.WordText);
      }

      this.Add(InWord);
      return this;
    }

    public StmtWordSpan AddWord(StmtWordListCursor InCursor)
    {
      this.Add(InCursor.Node.Value);
      return this;
    }

    public StmtWordSpan AddWord(
      StmtWordListCursor InCursor, MonoCaseString InExpectedWordText )
    {
      if (InCursor.Position != RelativePosition.At)
        throw new ApplicationException("StmtWord cursor is not positioned at a word");
      AddWord(InCursor.Node.Value, InExpectedWordText);
      return this;
    }

    /// <summary>
    /// advance the StmtWord cursor to the next word.  Then test that it contains the
    /// expected word value. Finally, add that StmtWord to the WordSpan string.
    /// </summary>
    /// <param name="InCursor"></param>
    /// <param name="InExpectedWordText"></param>
    /// <returns></returns>
    public StmtWordListCursor AddNextWord(
      StmtWordListCursor InCursor, MonoCaseString InExpectedWordText)
    {
      StmtWordListCursor c1 = InCursor.Next();
      if (c1.Position != RelativePosition.At)
        throw new ApplicationException("Next StmtWord cursor is not positioned at a word");
      AddWord(c1.Node.Value, InExpectedWordText);
      return c1;
    }

  }
}
