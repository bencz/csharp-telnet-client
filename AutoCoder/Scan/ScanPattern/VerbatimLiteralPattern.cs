using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{
  public class VerbatimLiteralPattern : ScanPattern
  {

    public VerbatimLiteralPattern(string PatternValue)
      : base(PatternValue, DelimClassification.Quote)
    {
    }

    public char QuoteChar
    {
      get
      {
        char ch1 = this.PatternValue.Last();
        return ch1;
      }
    }
  }
}
