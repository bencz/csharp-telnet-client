using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text;

namespace AutoCoder.Scan
{
  /// <summary>
  /// A keyword is an identifier with special meaning.
  /// The pattern string can contain the special "[ws]" pattern. That enables a keyword
  /// pattern to span multiple words with varying whitespace between the words.
  /// Use the CompareCase property to specifiy that the matching to a keyword is case
  /// sensitive or not.
  /// </summary>
  public class KeywordPattern : ScanPattern
  {

    public TextCase CompareCase
    {
      get
      {
        return _CompareCase;
      }
      set
      {
        _CompareCase = value;
        if (_CompareCase == TextCase.MonoCase)
        {
          this.PatternValue = this.PatternValue.ToLower();
        }
      }
    }
    TextCase _CompareCase;

    public override string PatternValue
    {
      get
      {
        return base.PatternValue;
      }
      set
      {
        base.PatternValue = value;

        // examine the pattern value for "[ws]" special value. 
        // When the 
        string patValue = this.PatternValue;
        if (patValue.Contains("[ws]"))
        {

          // split on "[ws]" into parts. Use those parts in the virtual EqualsText
          // method when comparing this pattern for a match in the search string.
          _PatternValueParts = patValue.Split(
            new string[] { "[ws]" }, StringSplitOptions.RemoveEmptyEntries);

          // build the ReplacementValue property from the parts with a space between each part.
          {
            StringBuilder sb = new StringBuilder();
            foreach (var part in _PatternValueParts)
            {
              if (sb.Length > 0)
                sb.Append(" ");
              sb.Append(part);
            }
            this.ReplacementValue = sb.ToString();
          }
        }
        else
        {
          _PatternValueParts = null;
          this.ReplacementValue = null;
        }
      }
    }
    string[] _PatternValueParts;

    public override int MinPatternLength
    {
      get
      {
        if (_PatternValueParts == null)
          return base.MinPatternLength;
        else
        {
          int minLx = 0;
          foreach (string patPart in _PatternValueParts)
          {
            if (minLx > 0)
              minLx += 1;
            minLx += patPart.Length;
          }
          return minLx;
        }
      }
    }

    public ScanPatterns WhitespacePatterns
    { get; set; }

    public KeywordPattern(
      string PatternValue, 
      TextCase CompareCase = TextCase.SameCase)
      : base(PatternValue, DelimClassification.Keyword)
    {
      this.CompareCase = CompareCase;
    }

    public KeywordPattern(KeywordPattern Pattern)
      : base(Pattern)
    {
      this.CompareCase = Pattern.CompareCase;
      this.ReplacementValue = Pattern.ReplacementValue ;
      this.WhitespacePatterns = Pattern.WhitespacePatterns;
      this.UserCode = Pattern.UserCode;
    }

    public override ScanPattern Duplicate()
    {
      KeywordPattern dup = new KeywordPattern(this);
      return dup;
    }

    public override Tuple<bool, int> EqualsText(string Text, int Ix)
    {
      int matchLx = 0;
      bool doesMatch = false;

      if (this._PatternValueParts != null)
      {
        var rv = EqualsWhitespaceSeparatedParts(Text, Ix, this.WhitespacePatterns);
        doesMatch = rv.Item1;
        matchLx = rv.Item2;
      }

      else
      {
        var rv = EqualsText(this.PatternValue, this.CompareCase, Text, Ix);
        doesMatch = rv.Item1;
        matchLx = rv.Item2;
      }

      return new Tuple<bool, int>(doesMatch, matchLx);
    }

    private static Tuple<bool, int> EqualsText(
      string Pattern, TextCase CompareCase, string Text, int Ix)
    {
      int matchLx = 0;
      bool doesMatch = false;
      int remLx = Text.Length - Ix;
      int patLx = Pattern.Length;

      if (remLx < patLx)
        doesMatch = false;

      else if ((CompareCase == TextCase.SameCase) &&
        (Pattern == Text.Substring(Ix, patLx)))
      {
        doesMatch = true;
        matchLx = patLx;
      }

      else if ((CompareCase == TextCase.MonoCase) &&
        (Pattern == Text.Substring(Ix, patLx).ToLower()))
      {
        doesMatch = true;
        matchLx = patLx;
      }

      else
        doesMatch = false;

      return new Tuple<bool, int>(doesMatch, matchLx);
    }

    /// <summary>
    /// Match a location in the input text string to a whitespace separated keyword 
    /// pattern.
    /// </summary>
    /// <param name="Text"></param>
    /// <param name="Ix"></param>
    /// <param name="WhitespacePatterns"></param>
    /// <returns></returns>
    private Tuple<bool,int> EqualsWhitespaceSeparatedParts(
      string Text, int Ix, 
      ScanPatterns WhitespacePatterns )
    {
      bool doesMatch = false;
      int ix = Ix;
      int nbrParts = this._PatternValueParts.Length;
      int partIx = 0;
      int cummMatchLx = 0;

      // the start of the match.
      int bx = Ix;

      while(true)
      {
        string patPart = this._PatternValueParts[partIx];

        // match the current part of the keyword pattern to the input text string.
        var rv = EqualsText(patPart, this.CompareCase, Text, ix);
        bool isMatch = rv.Item1;
        int matchLx = rv.Item2;

        // not a match. break out.
        if (isMatch == false)
        {
          doesMatch = false;
          break;
        }
        cummMatchLx += matchLx;

        // this is the last part. the full pattern is a match.
        if ((partIx + 1) == nbrParts)
        {
          doesMatch = true;
          break;
        }

        // advance past whitespace.
        ix = Ix + cummMatchLx;
        int fx = Scanner.ScanNotEqual(Text, WhitespacePatterns, ix);
        
        // no whitespace. not a match.
        if (fx == ix)
        {
          doesMatch = false;
          break;
        }

        // total up the match length up to the last whitespace char.
        ix = fx;
        cummMatchLx = ix - Ix;

        partIx += 1;
      }

      return new Tuple<bool,int>(doesMatch, cummMatchLx) ;
    }
  }
}
