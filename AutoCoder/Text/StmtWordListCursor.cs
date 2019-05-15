using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Text
{

  public class StmtWordListCursor
  {
    AcRelativePosition mRltv = AcRelativePosition.None;
    LinkedListNode<StmtWord> mNode;
    StmtWordList mList;
    
    // when true, Next( ) and Prev( ) will stay at same location and
    // clear the stay flag.
    bool mStayAtFlag;

    public StmtWordListCursor(StmtWordList InList)
    {
      mRltv = AcRelativePosition.Begin;
      mNode = null;
      mList = InList;
      mStayAtFlag = false;
    }

    public StmtWordListCursor(
      StmtWordList InList, LinkedListNode<StmtWord> InNode, AcRelativePosition InRltv)
    {
      mList = InList;
      mRltv = InRltv;
      mNode = InNode;
      mStayAtFlag = false;
    }

    public AcRelativePosition Position
    {
      get { return mRltv; }
      set { mRltv = value; }
    }

    /// <summary>
    /// Stay at current location on next advance of cursor.
    /// </summary>
    public bool StayAtFlag
    {
      get { return mStayAtFlag; }
      set { mStayAtFlag = value; }
    }

    public StmtWord StmtWord
    {
      get
      {
        if (mRltv != AcRelativePosition.At)
          throw new ApplicationException("not positioned at StmtWord");
        else
          return mNode.Value;
      }
    }

    public WordCursor WordCursor
    {
      get
      {
        if (mRltv != AcRelativePosition.At)
          throw new ApplicationException("not positioned at StmtWord");
        else
          return mNode.Value.WordCursor;
      }
    }

    public StmtWordListCursor Next()
    {
      LinkedListNode<StmtWord> node;
      
      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (mRltv != AcRelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        node = mNode;
      }
      
      else
      {
        switch (mRltv)
        {
          case AcRelativePosition.Begin:
            node = mList.First;
            break;
          case AcRelativePosition.Before:
            node = mNode;
            break;
          case AcRelativePosition.At:
            node = mNode.Next;
            break;
          case AcRelativePosition.After:
            node = mNode.Next;
            break;
          case AcRelativePosition.End:
            node = null;
            break;
          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if (node == null)
        return null;
      else
        return new StmtWordListCursor(mList, node, AcRelativePosition.At);
    }

    public StmtWordListCursor PositionAfter()
    {
      if (mRltv == AcRelativePosition.Begin)
        return PositionBegin();
      else if (mRltv == AcRelativePosition.End)
        return PositionEnd();
      else if (mRltv == AcRelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
        return new StmtWordListCursor(mList, mNode, AcRelativePosition.After);
    }

    public StmtWordListCursor PositionBefore()
    {
      if (mRltv == AcRelativePosition.Begin)
        return PositionBegin();
      else if (mRltv == AcRelativePosition.End)
        return PositionEnd();
      else if (mRltv == AcRelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
        return new StmtWordListCursor(mList, mNode, AcRelativePosition.Before);
    }

    public StmtWordListCursor PositionBegin()
    {
      return new StmtWordListCursor(mList, null, AcRelativePosition.Begin);
    }

    public StmtWordListCursor PositionEnd()
    {
      return new StmtWordListCursor(mList, null, AcRelativePosition.End);
    }

    public StmtWordListCursor PositionNone()
    {
      return new StmtWordListCursor(mList, null, AcRelativePosition.None);
    }
  }

}
