using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text;
using AutoCoder.Ext.System;

namespace AutoCoder.Scan
{

  // -------------------------- ScanPatternResults ----------------------------
  public class ScanPatternResults
  {
    int mScanStartIx;
    int mScanBoundsEx ;  // the end pos within the string being scanned.
    string mScannedString;
    int mFoundPos;
    char? mFoundChar;
    string mFoundPattern;
    ScanPattern mFoundPat;

    int mAnyPatternIx;  // location of the found pattern in the array of any pattern
    // to scan for.

    public ScanPatternResults(int InNotFoundIx)
    {
      mScanStartIx = -1;
      mScannedString = null;
      mScanBoundsEx = -1 ;
      mFoundPos = InNotFoundIx;
      mFoundChar = null;
      mFoundPattern = null;
      mAnyPatternIx = -1;
      mFoundPat = null;
    }

    public ScanPatternResults(int InFoundPos, string InFoundPattern)
    {
      mScanStartIx = -1;
      mScannedString = null;
      mScanBoundsEx = -1 ;
      mFoundPos = InFoundPos;
      mFoundChar = InFoundPattern[0];
      mFoundPattern = InFoundPattern;
      mAnyPatternIx = -1;
      mFoundPat = null;
    }

    public ScanPatternResults(int InFoundPos, char InFoundChar)
    {
      mScanStartIx = -1;
      mScannedString = null;
      mScanBoundsEx = -1 ;
      mFoundPos = InFoundPos;
      mFoundChar = InFoundChar;
      mFoundPattern = null;
      mAnyPatternIx = -1;
      mFoundPat = null;
    }

    public ScanPatternResults(int InFoundPos, string InFoundPattern, int InAnyPatternIx)
    {
      mScanStartIx = -1;
      mScannedString = null;
      mScanBoundsEx = -1 ;
      mFoundPos = InFoundPos;
      mFoundChar = InFoundPattern[0];
      mFoundPattern = InFoundPattern;
      mAnyPatternIx = InAnyPatternIx;
      mFoundPat = null;
    }

    public ScanPatternResults(int InFoundPos, ScanPattern InFoundPattern)
    {
      mScanStartIx = -1;
      mScannedString = null;
      mScanBoundsEx = -1 ;
      mFoundPos = InFoundPos;
      mFoundChar = InFoundPattern.LeadChar;
      mFoundPattern = InFoundPattern.PatternValue;
      mAnyPatternIx = InFoundPattern.ArrayPosition;
      mFoundPat = InFoundPattern;
    }

    public int AnyPatternIx
    {
      get { return mAnyPatternIx; }
    }

    public char? FoundChar
    {
      get { return mFoundChar; }
    }

    public string FoundPattern
    {
      get { return mFoundPattern; }
    }

    public ScanPattern FoundPat
    {
      get { return mFoundPat; }
    }

    public int FoundPos
    {
      get { return mFoundPos; }
    }

    public bool IsFound
    {
      get
      {
        if (mFoundPos != -1)
          return true;
        else
          return false;
      }
    }

    public bool IsNotFound
    {
      get
      {
        if (mFoundPos == -1)
          return true;
        else
          return false;
      }
    }

    public int ScanBoundsEx
    {
      get
      {
        if ( mScanBoundsEx == -1 )
          throw new ApplicationException("ScanBounds end pos is not assigned" ) ;
        return mScanBoundsEx ; 
      }
      set { mScanBoundsEx = value ; }
    }

    public int ScannedOverLength
    {
      get
      {
        if ( mScannedString == null )
          throw new ApplicationException("Scanned string is not assigned" ) ;
        else if ( FoundPos == -1 )
          return ScanBoundsEx - ScanStartIx + 1 ;
        else
          return FoundPos - ScanStartIx ;
      }
    }

    public string ScannedOverString
    {
      get
      {
        if (mScannedString == null)
          throw new ApplicationException("Scanned string is not assigned");
        else
          return ScannedString.SubstringLenient(ScanStartIx, ScannedOverLength);
      }
    }

    public string ScannedString
    {
      get
      {
        if (mScannedString == null)
          throw new ApplicationException("Scanned string is not assigned");
        return mScannedString;
      }
      set { mScannedString = value; }
    }

    public int ScanStartIx
    {
      get
      {
        if (mScanStartIx == -1)
          throw new ApplicationException("Scan start location is not assignedt");
        return mScanStartIx;
      }
      set { mScanStartIx = value; }
    }
  }
}
