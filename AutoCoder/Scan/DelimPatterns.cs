using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Text.Enums;

namespace AutoCoder.Scan
{

#if skip

  public class DelimPatterns : ScanPatterns
  {
    // an array of delimeters that correspond to the scan patterns
    // stored in the base class.
    DelimClassification[] mDelims;

    List<DelimClassification> mWip = new List<DelimClassification>();

    public DelimPatterns()
      : base()
    {
      ScanPatternsChanged += new delScanPatternsChanged(DelimPatterns_ScanPatternsChanged);
    }

    public DelimClassification[] Delims
    {
      get
      {
        if (mDelims == null)
        {
          mDelims = mWip.ToArray( ) ;
        }
        return mDelims ;
      }
    }

    public void Add(ScanPatterns InPatterns, DelimClassification InDelimClass)
    {
      foreach (ScanPattern pat in InPatterns)
      {
        base.Add(pat.PatternValue);
        mWip.Add(InDelimClass);
      }
    }

    public void AddDistinct(ScanPatterns Patterns, DelimClassification DelimClass)
    {
      foreach (ScanPattern pat in Patterns)
      {
        int px = base.AddDistinct(pat.PatternValue);
        if (px != -1)
          mWip.Add(DelimClass);
      }
    }

    public void AddDistinct(ScanPattern Pattern, DelimClassification DelimClass)
    {
      int px = base.AddDistinct(Pattern.PatternValue);
      if (px != -1)
        mWip.Add(DelimClass);
    }

    public void AddDistinct(char[] InPatterns, DelimClassification InDelimClass)
    {
      foreach (char pat in InPatterns)
      {
        int px = base.AddDistinct(pat.ToString( ));
        if (px != -1)
          mWip.Add(InDelimClass);
      }
    }

    void DelimPatterns_ScanPatternsChanged(ScanPatterns InPatterns)
    {
      mDelims = null;
    }

    public DelimClassification GetDelimClass(ScanPattern InPattern)
    {
      DelimClassification dc = Delims[InPattern.ArrayPosition];
      return dc;
    }

    public bool IsOpenBraced(ScanPattern InPattern)
    {
      DelimClassification dc = GetDelimClass(InPattern);
      if ((dc == DelimClassification.OpenContentBraced)
        || (dc == DelimClassification.OpenNamedBraced))
        return true;
      else
        return false;
    }
  }

#endif

}
