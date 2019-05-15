using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;
using AutoCoder.Text.Location;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Text
{
  /// <summary>
  /// Text in a string found when scanning for ScanPatterns.
  /// Info stored in this class include the text itself, the location in the string of the text,
  /// and the classification of the text.
  /// </summary>
  public class AtomText
  {
    public ScanAtomCode AtomCode
    { get; set; }

    public TextLocation EndLoc
    { get; set; }

    public LiteralType? LiteralType
    { get; set; }

    public string Text
    {
      get
      {
        if (this.ReplacementText != null)
          return this.ReplacementText;
        else
          return this.ScanText;
      }
    }

    /// <summary>
    /// Text that replaces the ScanText when retrieving the text contents of the Atom.
    /// ( the scanned text might be a keyword with a variable amount of whitespace between
    ///   the words of the keyword string. )
    /// </summary>
    public string ReplacementText
    {
      get { return _ReplacementText; }
      set { _ReplacementText = value; }
    }
    string _ReplacementText;

    /// <summary>
    /// the actual scanned text, from startLoc to EndLoc in the scan stream.
    /// </summary>
    public string ScanText
    {
      get { return _ScanText; }
      set { _ScanText = value; }
    }
    string _ScanText;

    public TextLocation StartLoc
    { get; set; }

    public string UserCode
    { get; set; }

    public AtomText(ScanAtomCode AtomCode)
    {
      if (AtomCode != ScanAtomCode.EndOfString)
        throw new ApplicationException("only special EndOfString AtomCode allowed.");
      this.AtomCode = AtomCode;
    }

    public AtomText( 
      ScanAtomCode AtomCode, 
      string ScanText, string ReplacementText,
      TextLocation StartLoc, TextLocation EndLoc,
      string UserCode)
    {
      this.AtomCode = AtomCode;
      this.ScanText = ScanText;
      this.ReplacementText = ReplacementText;
      this.LiteralType = null;
      this.StartLoc = StartLoc;
      this.EndLoc = EndLoc;
      this.UserCode = UserCode;
    }

    public AtomText(
      ScanAtomCode AtomCode, 
      string LitText, LiteralType LiteralType,
      TextLocation StartLoc, TextLocation EndLoc)
    {
      this.AtomCode = AtomCode;
      this.ScanText = LitText;
      this.ReplacementText = null;
      this.LiteralType = LiteralType;
      this.StartLoc = StartLoc;
      this.EndLoc = EndLoc;
      this.UserCode = null;
    }

    /// <summary>
    /// combine the text of two adjacent tokens.
    /// </summary>
    /// <param name="Value1"></param>
    /// <param name="Value2"></param>
    /// <param name="AtomCode"></param>
    /// <returns></returns>
    public static AtomText Combine(AtomText Value1, AtomText Value2, ScanAtomCode AtomCode)
    {
      // cant combine literal values.
      if ((Value1.LiteralType != null) || (Value2.LiteralType != null))
        throw new ApplicationException("cannot combine literal tokens");
      string userCode = null;
      TextLocation startLoc = Value1.StartLoc;
      TextLocation endLoc = Value2.EndLoc;
      string scanText = Value1.ScanText + Value2.ScanText;
      AtomText combo = new AtomText(AtomCode, scanText, null, startLoc, endLoc, userCode);
      return combo;
    }

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.SentenceAppend(this.AtomCode.ToString());
      if (this.LiteralType != null)
      {
        sb.SentenceAppend(this.LiteralType.Value.ToString());
      }
      sb.SentenceAppend(this.Text);
      sb.SentenceAppend("Start:" + this.StartLoc.ToString());
      return sb.ToString();
    }

  }
}
