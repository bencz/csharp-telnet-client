using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Collections;

namespace AutoCoder.File
{
  public class TextLine
  {
    string mLineData;
    long mLineId;

    public TextLine(string InLineData, long InLineId)
    {
      mLineData = InLineData;
      mLineId = InLineId;
    }

    public string LineData
    {
      get { return mLineData; }
    }

    public long LineId
    {
      get { return mLineId; }
    }

  }

  public class TextLineToStringLocator
  {
    long mLineId;
    int mStringBx;
    int mStringEx;

    public TextLineToStringLocator(
      long InLineId, int InStringBx, int InStringEx)
    {
      mLineId = InLineId;
      mStringBx = InStringBx;
      mStringEx = InStringEx;
    }
  }

  public class TextLineToStringLocatorMap : 
    KeyedLinkedList<long, TextLineToStringLocator>
  {

  }

}
