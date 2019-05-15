using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoCoder.Scan
{
  public class KeywordPatterns : ScanPatterns
  {

    public new ScanPatterns WhitespacePatterns
    { get; set; }

    public void AddPatterns(params string[] Patterns)
    {
      if (this.WhitespacePatterns == null)
        throw new ApplicationException("WhitespacePatterns is not assigned");

      foreach (var pattern in Patterns)
      {
        KeywordPattern pat = new KeywordPattern(pattern);
        pat.WhitespacePatterns = this.WhitespacePatterns;
        this.Add(pat);
      }
    }

    public void AddPattern(string Pattern, string UserCode)
    {
      KeywordPattern pat = new KeywordPattern(Pattern);
      pat.WhitespacePatterns = this.WhitespacePatterns;
      pat.UserCode = UserCode;
      this.Add(pat);
    }
  }
}
