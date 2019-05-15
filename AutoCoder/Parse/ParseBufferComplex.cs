using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Parse
{
  /// <summary>
  /// An array of strings that is prepared for parsing.
  /// The strings are concatenated as a single NewLine delimited
  /// string. A list of ParseBufferLine objects provides a cross reference between
  /// positions in the concatenated string and the lines which
  /// formed that string.
  /// </summary>
  public class ParseBufferComplex
  {
    string[] mLines = null;
    string mBuffer = null;

    List<ParseBufferLine> mBufferLineList;
    int mLookupIx = -1 ;

    public ParseBufferComplex( string[] InLines )
    {
      Lines = InLines;
    }

    public string Buffer
    {
      get { return mBuffer; }
    }

    void BuildBuffer( )
    {
      StringBuilder buf = new StringBuilder();
      mBufferLineList = new List<ParseBufferLine>();
      int lineNx = 0 ;

      foreach (string lineText in mLines)
      {
        // buffer line text includes the newline pattern at the end.
        string s1 = lineText + Environment.NewLine;

        // add to the buffer xref list.
        ParseBufferLine bl = new ParseBufferLine(s1, lineNx, buf.Length);
        mBufferLineList.Add(bl);

        // add to the buffer itself.
        buf.Append(s1);

        // inc to next line nbr.
        lineNx += 1;
      }

      // store the concatenated string buffer.
      mBuffer = buf.ToString();
    }

    /// <summary>
    /// Find the BufferLine which contains the searched for buffer pos.
    /// </summary>
    /// <param name="InPos"></param>
    /// <returns></returns>
    public ParseBufferLine FindBufferLine(int BufferPos)
    {
      int ix = mLookupIx;

      ParseBufferLine bufLine = null;
      if ( ix == -1 )
        ix = 0 ;

      while( true )
      {
        if (( ix >= mBufferLineList.Count ) || ( ix < 0 ))
        {
          bufLine = null ;
          break ;
        }
        bufLine = mBufferLineList[ix];

        // pos to search for is before current buffer line.
        if ( BufferPos < bufLine.BufferPos )
          ix = ix - 1 ;

          // the pos to search for is after current buffer line.
        else if (BufferPos > bufLine.BufferEndPos)
          ix = ix + 1;

          // line found. save the index for next search. then exit loop.
        else
        {
          mLookupIx = ix;
          break;
        }
      }

      return bufLine;
    }

    public string[] Lines
    {
      get { return mLines; }
      set
      {
        mLines = value;
        BuildBuffer();
      }
    }
  }
}
