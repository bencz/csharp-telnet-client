using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Parse
{
  /// <summary>
  /// collection of CursorMatchPattern(s) used for comparing if a cursor
  /// location in a StmtWord tree contains a sequence of StmtWord which match
  /// this collection of patterns. 
  /// </summary>
  public class CursorMatchPatterns : List<CursorMatchPattern>
  {

    int mLastUsedCursorIx = -1;

    public CursorMatchPatterns()
    {
    }

    public CursorMatchPatterns(string InFreeFormPattern)
    {
      Add(InFreeFormPattern);
    }

    public CursorMatchPatterns(string[] InFreeFormPatterns)
    {
      foreach (string pat in InFreeFormPatterns)
      {
        Add(pat);
      }
    }

    public new CursorMatchPatterns Add(CursorMatchPattern InMatchPat)
    {
      base.Add(InMatchPat);
      return this;
    }

    public void Add(string InFreeFormPattern)
    {
      CursorMatchPattern pat = new CursorMatchPattern(InFreeFormPattern);
      base.Add(pat);
    }

    /// <summary>
    ///  Check that the sequence of patterns matches the StmtWord stream starting
    ///  at the specified cursor.  Returns list of cursors which match the pattern.
    /// </summary>
    /// <param name="InCursor"></param>
    /// <returns></returns>
    public List<StmtWordListCursor> DoMatch(StmtWordListCursor InCursor)
    {
      bool mr = true;
      StmtWordListCursor c1 = InCursor;
      List<StmtWordListCursor> matchedCursors = new List<StmtWordListCursor>();

      foreach (CursorMatchPattern pat in this)
      {
        if (pat.DoesMatch(c1) == false)
        {
          mr = false;
          break;
        }

        // add to list of matching cursors.  This list corresponds to the list of
        // match patterns. 
        matchedCursors.Add(c1);
        
        c1 = c1.NextDeep();
      }

      if (mr == false)
        matchedCursors = null;

      return matchedCursors;
    }

    /// <summary>
    /// index to last pattern in the pattern list which identifies a useable
    /// cursor value. ( there may be a pattern at the end of the sequence which
    /// identifies the start of a new sentence. )
    /// </summary>
    public int LastUsedCursorIx
    {
      get 
      {
        if (mLastUsedCursorIx == -1)
          throw new ApplicationException("Last used cursor index is not assigned");
        return mLastUsedCursorIx;
      }
      set { mLastUsedCursorIx = value; }
    }

  }
}
