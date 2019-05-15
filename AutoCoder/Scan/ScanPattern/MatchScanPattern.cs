using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;
using AutoCoder.Ext;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Scan
{
  /// <summary>
  /// Info about the match of a ScanPattern against the text within a string.
  /// Info stored are the ScanPattern and the AtomText of the matched text.
  /// AtomText contains the text and location of the text in the string. 
  /// </summary>
  public class MatchScanPattern
  {
    public ScanPattern MatchPattern
    { get; set; }

    public int Pos
    { get; set; }

    public int MatchLength
    { get; set; }

    // the 
    public AtomText AtomText
    { get; set; }

    public MatchScanPattern(ScanPattern MatchPattern, int Pos, int MatchLength)
    {
      this.MatchPattern = MatchPattern;
      this.Pos = Pos;
      this.MatchLength = MatchLength;
    }

    public void AssignAtomText(ScanStream ScanStream)
    {
      TextLocation wordBx = new StreamLocation(this.Pos).ToTextLocation(ScanStream);
      int lx = this.MatchLength;
      TextLocation wordEx =
        new StreamLocation(this.Pos + lx - 1).ToTextLocation(ScanStream);
      string scanText = ScanStream.Stream.Substring(this.Pos, lx);

      this.AtomText = new AtomText(
        this.MatchPattern.DelimClassification.ToScanAtomCode().Value,
        scanText, this.MatchPattern.ReplacementValue,
        wordBx, wordEx,
        this.MatchPattern.UserCode);
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      if (this.MatchPattern != null)
        sb.SentenceAppend(this.MatchPattern.ToString());
      sb.SentenceAppend(this.MatchLength.ToString());
      return sb.ToString();
    }
  }
}
