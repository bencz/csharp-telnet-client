using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.File
{
  public class TextLinesCursor
  {
    TextLines mLines;
    LinkedListNode<TextLine> mLinesNode;
    int mLineOx;
    RelativePosition mRltv;

    public TextLinesCursor()
    {
      mLines = null;
      mLinesNode = null;
      mLineOx = -1;
      mRltv = RelativePosition.None;
    }

    public TextLinesCursor(TextLinesCursor InCsr)
    {
      mLines = InCsr.mLines;
      mLinesNode = InCsr.mLinesNode;
      mLineOx = InCsr.mLineOx;
      mRltv = InCsr.mRltv;
    }

    public TextLinesCursor(
      LinkedListNode<TextLine> InLinesNode, int InLineOx, RelativePosition InRltv)
    {
      mLines = null;
      mLinesNode = InLinesNode;
      mLineOx = InLineOx;
      mRltv = InRltv;
    }

    public TextLinesCursor(
      TextLines InLines, RelativePosition InRltv)
    {
      mLines = InLines;
      mLinesNode = null;
      mLineOx = -1;
      mRltv = InRltv;

      if ((InRltv != RelativePosition.Begin) &&
        (InRltv != RelativePosition.End) &&
        ( InRltv != RelativePosition.None ))
        throw new ApplicationException(
          "only pos bgn and pos end allowed thru this constructor");
    }

    public char CursorChar
    {
      get
      {
        if (mRltv != RelativePosition.At)
          throw new ApplicationException("cursor not positioned at a character");
        char ch1 = mLinesNode.Value.LineData[mLineOx];
        return ch1;
      }
    }

    public bool EndOfLine
    {
      get
      {
        if ((mRltv == RelativePosition.At) &&
          (mLineOx == -1))
          return true;
        else
          return false;
      }
    }

    public string LineData
    {
      get
      {
        if (mRltv != RelativePosition.At)
          throw new ApplicationException("cursor not positioned at a character");
        return mLinesNode.Value.LineData ;
      }
    }

    public int LineOx
    {
      get { return mLineOx; }
      set { mLineOx = value; }
    }

    public LinkedListNode<TextLine> LinesNode
    {
      get { return mLinesNode; }
    }

    // -------------------- Postion -------------------------
    // use when calc the start position when scanning to next word.
    public RelativePosition Position
    {
      get { return mRltv; }
      set { mRltv = value; }
    }

    public void LineAdvance(TextLines InLines)
    {
      switch (mRltv)
      {
        case RelativePosition.Begin:
          mLinesNode = InLines.FirstLine;
          break;
        case RelativePosition.Before:
          break;
        case RelativePosition.At:
          mLinesNode = mLinesNode.Next;
          break;
        case RelativePosition.After:
          mLinesNode = mLinesNode.Next;
          break;
        default:
          mLinesNode = null;
          break;
      }

      if (mLinesNode == null)
        mRltv = RelativePosition.None;
      else
      {
        mRltv = RelativePosition.At;
        mLineOx = 0;
      }
    }
  }
}
