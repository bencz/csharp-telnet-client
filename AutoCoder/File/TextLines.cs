using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Collections;
using AutoCoder.Text;
using AutoCoder.Core;
using AutoCoder.Scan;
using AutoCoder.Core.Enums;
using System.Collections;

namespace AutoCoder.File
{

  public class TextLinesScanResults
  {
    public ScannerWhatFound WhatFound = ScannerWhatFound.NotFound;
    public TextLinesCursor Pos = null;

    public char FoundPatternChar
    {
      get
      {
        if (WhatFound != ScannerWhatFound.PatternChar)
          return '\0';
        else
          return Pos.CursorChar;
      }
    }
  }

public class TextLines : IEnumerable<TextLine>
  {
    KeyedLinkedList<long, TextLine> mLines;
    long mNextLineId = 1;

    public TextLines()
    {
      mLines = new KeyedLinkedList<long, TextLine>();
    }

    public LinkedListNode<TextLine> this[long InKey]
    {
      get
      {
        LinkedListNode<TextLine> node = mLines[InKey];
        return node;
      }
    }

    public TextLine AddLast( string InLineData )
    {
      // next lineid.
      long lineId = mNextLineId;
      ++mNextLineId;

      TextLine line = new TextLine( InLineData, lineId ) ;
      mLines.AddLast(line.LineId, line);

      return line ;
    }

    public LinkedListNode<TextLine> FirstLine
    {
      get
      {
        if (mLines.Count == 0)
          return null;
        else
          return this[0];
      }
    }

    public TextLinesCursor PosBgn()
    {
      return new TextLinesCursor(this, RelativePosition.Begin);
    }

    public TextLinesCursor PosEnd()
    {
      return new TextLinesCursor(this, RelativePosition.End);
    }

    public TextLinesCursor PosNone()
    {
      return new TextLinesCursor(this, RelativePosition.None);
    }

    /// <summary>
    /// scan from the current cursor position, from line to line, until
    /// a char not equal any of the pattern chars is found.
    /// </summary>
    /// <param name="InCsr"></param>
    /// <param name="InNotEqualChars"></param>
    /// <returns></returns>
    public TextLinesScanResults ScanNotEqual(
      TextLinesCursor InCsr, 
      ScanPatterns InNotEqualPatterns,
      ScannerEndOfLineConsider InEolConsider)
    {
      ScanPatternResults res ;
      TextLinesScanResults rv = new TextLinesScanResults();

      TextLinesCursor csr = Scanner_InitialCursorSetup(this, InCsr);
      while (true)
      {

        if ((csr.Position == RelativePosition.End) ||
          (csr.Position == RelativePosition.None))
        {
          csr.Position = RelativePosition.None;
          rv.Pos = csr;
          rv.WhatFound = ScannerWhatFound.NotFound;
          break;
        }

        int ex = csr.LinesNode.Value.LineData.Length - 1;
        res = Scanner.ScanNotEqual(
          csr.LinesNode.Value.LineData, csr.LineOx, ex, 
          InNotEqualPatterns ) ;

        // a char not equal to the pattern chars is found.
        if (res.FoundPos >= 0)
        {
          csr.LineOx = res.FoundPos;
          rv.Pos = csr;
          rv.WhatFound = ScannerWhatFound.PatternChar;
          break;
        }
        else if (InEolConsider == ScannerEndOfLineConsider.Found)
        {
          csr.LineOx = -1;
          rv.Pos = csr;
          rv.WhatFound = ScannerWhatFound.HardLineBreak;
          break;
        }
        else
        {
          csr.LineAdvance(this);
        }
      }

      return rv;
    }

  private static TextLinesCursor Scanner_InitialCursorSetup(
    TextLines InLines, TextLinesCursor InCsr)
  {
    TextLinesCursor csr = new TextLinesCursor(InCsr);

    if (csr.Position == RelativePosition.Begin)
      csr.LineAdvance(InLines);
    else if (csr.EndOfLine == true)
      csr.LineAdvance(InLines);

    return csr;
  }

  /// <summary>
  /// Scan forward in TextLines until char equal any of the pattern chars.
  /// </summary>
  /// <param name="InCsr"></param>
  /// <param name="InPatternChars"></param>
  /// <param name="InEolConsider"></param>
  /// <returns></returns>
  public TextLinesScanResults ScanEqualAny(
    TextLinesCursor InCsr,
    char[] InPatternChars,
    ScannerEndOfLineConsider InEolConsider)
  {
    TextLinesScanResults rv = new TextLinesScanResults();
    Scanner.ScanCharResults res;

    TextLinesCursor csr = Scanner_InitialCursorSetup(this, InCsr);
    while (true)
    {

      if ((csr.Position == RelativePosition.End) ||
        (csr.Position == RelativePosition.None))
      {
        csr.Position = RelativePosition.None;
        rv.Pos = csr;
        rv.WhatFound = ScannerWhatFound.NotFound;
        break;
      }

      res = Scanner.ScanEqualAny(
        csr.LineData, csr.LineOx, InPatternChars);

      // char is found.
      if (res.ResultPos >= 0)
      {
        csr.LineOx = res.ResultPos;
        rv.Pos = csr;
        rv.WhatFound = ScannerWhatFound.PatternChar;
        break;
      }

      else if (InEolConsider == ScannerEndOfLineConsider.Found)
      {
        csr.LineOx = -1;
        rv.Pos = csr;
        rv.WhatFound = ScannerWhatFound.HardLineBreak;
        break;
      }

      csr.LineAdvance(this);
    }

    return rv;
  }

    public string[] ToArray()
    {
      string[] array = new string[mLines.Count];
      int ix = 0;
      foreach (TextLine line in this)
      {
        array[ix] = line.LineData;
        ++ix;
      }
      return array;
    }

  public override string ToString()
  {
    StringBuilder sb = new StringBuilder();
    foreach (TextLine line in this)
    {
      sb.Append(line.LineData + Environment.NewLine);
    }
    return sb.ToString();
  }

  /// <summary>
  /// Build both a newline delimited string and a locator map which
  /// locates lines and offset locations within the string.
  /// </summary>
  /// <param name="xx"></param>
  /// <returns></returns>
  public AcPair<string, TextLineToStringLocatorMap> ToStringAndLocatorMap( int xx )
  {
    TextLineToStringLocatorMap map = new TextLineToStringLocatorMap();
    StringBuilder sb = new StringBuilder();
    
    foreach (TextLine line in this)
    {
      int bx = sb.Length;
      sb.Append(line.LineData + Environment.NewLine);
      int ex = sb.Length - 1;

      TextLineToStringLocator tlLoc = 
        new TextLineToStringLocator(line.LineId, bx, ex);
    }

    return new AcPair<string, TextLineToStringLocatorMap>(sb.ToString(), map);
  }

    #region IEnumerable<TextLine> Members

    IEnumerator<TextLine> IEnumerable<TextLine>.GetEnumerator()
    {
      if (mLines == null)
        yield break;
      else
      {
        foreach (TextLine elem in mLines)
        {
          yield return elem;
        }
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion
  }
}
