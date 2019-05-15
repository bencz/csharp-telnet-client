using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Text.Enums;
using System.Collections;

namespace AutoCoder.Scan
{

#if skip
  public class FoundNonWord
  {
    public FoundNonWord()
    {
    }

    public FoundNonWord(ScanPattern Pattern, int Pos, int Lgth)
    {
      this.AddFound(Pattern, Pos, Lgth);
    }

    public ScanPattern FoundPattern
    { get; set; }

    public int FoundPos
    { get; set; }

    public int MatchLgth
    { get; set; }

    public List<MatchScanPattern> FoundPatterns
    {
      get;
      set;
    }

    public void AddFound(ScanPattern Pattern, int Pos, int Lgth)
    {
      if (FoundPattern == null)
      {
        this.FoundPattern = Pattern;
        this.FoundPos = Pos;
        this.MatchLgth = Lgth;
      }
      else
      {
        if (FoundPatterns == null)
        {
          FoundPatterns = new List<MatchScanPattern>();
          MatchScanPattern matPat = new MatchScanPattern(FoundPattern, MatchLgth);
          FoundPatterns.Add(matPat);
        }

        {
          MatchScanPattern matPat = new MatchScanPattern(Pattern, Lgth);
          FoundPatterns.Add(matPat);
        }
      }
    }
  }

#endif


  // FoundAtLocationResults

  public class PatternScanResults : IEnumerable<MatchScanPattern>
  {
    public PatternScanResults()
    {
    }

    public PatternScanResults(ScanPattern Pattern, int Pos, int Lgth)
    {
      this.AddFound(Pattern, Pos, Lgth);
    }

    public MatchScanPattern FirstFoundPattern
    {
      get
      {
        if (this.FoundPatterns == null)
          return this.FoundPattern;
        else if (this.FoundPatterns.Count == 0)
          return null;
        else
          return this.FoundPatterns.First.Value;
      }
    }

    public int FoundCount
    {
      get
      {
        if (this.FoundPatterns != null)
          return this.FoundPatterns.Count;
        else if (this.FoundPattern != null)
          return 1;
        else
          return 0;
      }
    }

    public MatchScanPattern FoundPattern
    { get; set; }

    public MatchScanPatternList FoundPatterns
    {
      get;
      set;
    }

    IEnumerator<MatchScanPattern> IEnumerable<MatchScanPattern>.GetEnumerator()
    {
      if (this.FoundPatterns != null)
      {
        foreach (var pat in this.FoundPatterns)
        {
          yield return pat;
        }
        yield break;
      }
      else if (this.FoundPattern != null)
      {
        yield return this.FoundPattern;
        yield break;
      }
      else
      {
        yield break;
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public bool IsEmpty
    {
      get
      {
        if (this.FoundPattern != null)
          return false;
        else if ((this.FoundPatterns != null) && (this.FoundPatterns.Count > 0))
          return false;
        else
          return true;
      }
    }

    public int Position
    { get; private set; }

    public void AddFound(ScanPattern Pattern, int Pos, int Lgth)
    {
      this.Position = Pos;

      if (FoundPattern == null)
      {
        this.FoundPattern = new MatchScanPattern(Pattern, Pos, Lgth);
      }
      else
      {
        if (FoundPatterns == null)
        {
          FoundPatterns = new MatchScanPatternList();
          FoundPatterns.Add(this.FoundPattern);
        }

        {
          MatchScanPattern matPat = new MatchScanPattern(Pattern, Pos, Lgth);
          FoundPatterns.Add(matPat);
        }
      }
    }
  }

  public static class PatternScanResultsExt
  {

    public static MatchScanPattern FindPattern(
      this PatternScanResults Results, DelimClassification DelimClass)
    {
      MatchScanPattern found = null;
      if (Results == null)
        found = null;
      else if (Results.FoundPatterns != null)
        found = Results.FoundPatterns.FindPattern(DelimClass);
      else if ((Results.FoundPattern != null)
        && (Results.FoundPattern.MatchPattern.DelimClassification == DelimClass))
        found = Results.FoundPattern;
      else
        found = null;

      return found;
    }

    public static bool FoundAtPosition(
      this PatternScanResults Results, DelimClassification DelimClass,
      int Pos)
    {
      bool rc = false;
//      MatchScanPattern found = null;
      if (Results == null)
        rc = false;
      else if (Results.Position != Pos)
        rc = false;
      else
      {
        var pat = Results.FindPattern(DelimClass) ;
        if ( pat == null )
          rc = false ;
        else
          rc = true ;
      }
      return rc ;
    }

    public static bool IsDelimClass(
      this PatternScanResults Results, DelimClassification DelimClass)
    {
      MatchScanPattern found = FindPattern(Results, DelimClass);
      if (found == null)
        return false;
      else
        return true;
    }
  }
}
