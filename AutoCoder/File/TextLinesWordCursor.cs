using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Text; 

namespace AutoCoder.File
{
  public class TextLinesWordCursor : WordCursor
  {
    // the TextLine locations where the word starts and ends.
    // ( most classification of words will end on the same line as then
    //   begin. braced and quoted words can span many lines. ) 
    TextLinesCursor mBeginLineCursor;
    TextLinesCursor mEndLineCursor;

    public TextLinesWordCursor()
    {
      mBeginLineCursor = null;
      mEndLineCursor = null;
    }

    public TextLinesWordCursor(WordCursor InWordCursor)
      : base(InWordCursor)
    {
      mBeginLineCursor = null;
      mEndLineCursor = null;
    }

    public TextLinesWordCursor(
      WordCursor InWordCursor, 
      TextLinesCursor InBeginLineCursor, TextLinesCursor InEndLineCursor)
      : base(InWordCursor)
    {
      mBeginLineCursor = InBeginLineCursor;
      mEndLineCursor = InEndLineCursor;
    }

    public TextLinesCursor BeginLineCursor
    {
      get { return mBeginLineCursor; }
      set { mBeginLineCursor = value; }
    }

    public TextLinesCursor EndLineCursor
    {
      get { return mEndLineCursor; }
      set { mEndLineCursor = value; }
    }

    public WordCursor WordCursor
    {
      get { return this; }
    }
  }
}
