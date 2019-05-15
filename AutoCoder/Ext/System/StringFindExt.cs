using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Ext.System
{
  public static class StringFindExt
  {

    /// <summary>
    /// Find all the instances of text in a string.
    /// Return an array of FindResult. Where the successive FindResult array items
    /// represent all the text of the string. 
    /// Each FindResult item represents a substring in the text. Substrings which do
    /// not contain the find pattern. Followed by a FindResult substring which matches
    /// the find pattern.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Pattern"></param>
    /// <returns></returns>
    public static FindResult[] FindAll(this string Text, string Pattern)
    {
      int ix = 0;
      List<FindResult> resultList = null;

      // initial find.
      ix = Text.IndexOf(Pattern);
      if (ix == -1)
        return null;

      resultList = new List<FindResult>();

      ix = 0;
      while (true)
      {
        int remLx = Text.Length - ix;
        if (remLx <= 0)
          break;

        // not enough text remaining to match the pattern.
        if (Pattern.Length > remLx)
        {
          var res = new FindResult()
          {
            IsFind = false,
            Start = ix,
            Text = Text.Substring(ix)
          };
          resultList.Add(res);
          break;
        }

        else
        {
          int fx = Text.IndexOf(Pattern, ix);
          if (fx == -1)
          {
            var res = new FindResult()
            {
              IsFind = false,
              Start = ix,
              Text = Text.Substring(ix)
            };
            resultList.Add(res);
            break;
          }
          else
          {
            // pattern not found at start of search. Store this text as
            // "not found" text from the string.
            if (fx > ix)
            {
              int passOverLx = fx - ix;
              var res = new FindResult()
              {
                IsFind = false,
                Start = ix,
                Text = Text.Substring(ix, passOverLx)
              };
              resultList.Add(res);
            }

            // store the found text.
            {
              var res = new FindResult()
              {
                IsFind = true,
                Start = fx,
                Text = Pattern
              };
              resultList.Add(res);
            }

            // advance find index to after pattern find.
            ix = fx + Pattern.Length;
          }
        }
      }

      return resultList.ToArray();
    }

  }

  /// <summary>
  /// results of a find of text in a string.
  /// The start position, the text itself, and boolean indicates if the text
  /// matches the find pattern or not.
  /// 
  /// For usage, see the FindAll string extension method in class StringExt. That method
  /// returns an array of FindResult items. Each item containing either a substring in 
  /// the text which does not contain a pattern. Or a substring in the text that matches
  /// the find pattern. 
  /// </summary>
  public class FindResult
  {
    public int Start
    { get; set; }

    public string Text
    { get; set; }

    public bool IsFind
    { get; set; }

    public int Length
    {
      get
      {
        return this.Text.Length;
      }
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.IsFind + " " + this.Start);
      if (this.Text != null)
      {
        sb.Append(" " + this.Text);
      }
      return sb.ToString();
    }
  }

}
