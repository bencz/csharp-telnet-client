using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Text.Enums;

namespace AutoCoder.Parse
{

  public class StmtWordListCursor
  {
    RelativePosition mRltv = RelativePosition.None;
    LinkedListNode<StmtWord> mNode;
    StmtWordList mList;

    // which edge of a composite word. Cursor returns words which contains sub words
    // twice. First time with WhichEdge set to LeadEdge. Then after returning all the
    // sub words, return the parent word again with TrailEdge.
    WhichEdge mEdge;
    
    // when true, Next( ) and Prev( ) will stay at same location and
    // clear the stay flag.
    bool mStayAtFlag;

    public StmtWordListCursor(StmtWordList List)
    {
      mRltv = RelativePosition.Begin;
      mNode = null;
      mList = List;
      mStayAtFlag = false;
      mEdge = WhichEdge.None;
    }

    public StmtWordListCursor(
      StmtWordList List, LinkedListNode<StmtWord> Node,
      WhichEdge Edge, RelativePosition Rltv)
    {
      mList = List;
      mRltv = Rltv;
      mNode = Node;
      mEdge = Edge;
      mStayAtFlag = false;
    }

    public StmtWordListCursor(StmtWord Word)
      : this(Word, WhichEdge.LeadEdge, RelativePosition.Before)
    {
    }

    /// <summary>
    /// construct this cursor as a copy of the input cursor.
    /// </summary>
    /// <param name="Cursor"></param>
    public StmtWordListCursor(StmtWordListCursor Cursor)
    {
      this.mRltv = Cursor.mRltv;
      this.mNode = Cursor.mNode;
      this.mList = Cursor.mList;
      this.mStayAtFlag = Cursor.mStayAtFlag;
      this.mEdge = Cursor.mEdge;
    }

    public StmtWordListCursor(
      StmtWord Word, WhichEdge Edge, RelativePosition Rltv)
    {
      if (Word.IsTopWord == true)
      {
        mList = null;
        mRltv = Rltv;
        mNode = new LinkedListNode<StmtWord>(Word);
        mEdge = Edge;
      }
      else
      {
        mList = Word.Parent.SubWords;
        mRltv = Rltv;
        mEdge = Edge;
        mNode = Word.SubWordNode;
      }
    }

    public WhichEdge Edge
    {
      get { return mEdge; }
    }

    public bool IsAtEnd
    {
      get { return (mRltv == RelativePosition.End); }
    }

    public bool IsCorrTrailEdge(StmtWordListCursor InLeadEdge)
    {
      bool isTrailEdge = false;

      if (InLeadEdge.IsLeadEdge == false)
        throw new ApplicationException("argument is not a lead edge cursor");

      if (IsTrailEdge == true)
      {
        StmtWord teWord = this.StmtWord;
        StmtWord leWord = InLeadEdge.StmtWord;

        // the leading and trailing edge cursor contains the same StmtWord.
        if (teWord == leWord)
          isTrailEdge = true;
      }

      return isTrailEdge;
    }

    public bool IsLeadEdge
    {
      get
      {
        if (mRltv != RelativePosition.At)
          return false;
        else if (StmtWord.IsComposite == false)
          return false;
        else
          return (mEdge == WhichEdge.LeadEdge);
      }
    }

    /// <summary>
    /// cursor is at the leading edge of a braced word.
    /// </summary>
    /// <param name="InOpenBracePattern"></param>
    /// <returns></returns>
    public bool IsLeadEdgeBraced(string InOpenBracePattern)
    {
      if (IsLeadEdge == true)
      {
        if (StmtWord.IsBraced(InOpenBracePattern) == true)
        {
          return true;
        }
      }
      return false;
    }

    public bool IsTrailEdge
    {
      get
      {
        if (mRltv != RelativePosition.At)
          return false;
        else if (StmtWord.IsComposite == false)
          return false;
        else
          return (mEdge == WhichEdge.TrailEdge);
      }
    }

    public RelativePosition Position
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
        if (mRltv != RelativePosition.At)
          throw new ApplicationException("not positioned at StmtWord");
        else
          return mNode.Value;
      }
    }

    public WordCursor WordCursor
    {
      get
      {
        if (mRltv != RelativePosition.At)
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
        if (mRltv != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        node = mNode;
      }
      
      else
      {
        switch (mRltv)
        {
          case RelativePosition.Begin:
            node = mList.First;
            break;
          case RelativePosition.Before:
            node = mNode;
            break;
          case RelativePosition.At:
            node = mNode.Next;
            break;
          case RelativePosition.After:
            node = mNode.Next;
            break;
          case RelativePosition.End:
            node = null;
            break;
          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if (node == null)
        return new StmtWordListCursor(null, null, WhichEdge.None, RelativePosition.End);
      else
        return new StmtWordListCursor(
          mList, node, WhichEdge.None, RelativePosition.At);
    }

    /// <summary>
    /// Get next stmt word deep. Assert that word gotten is either a sibling or child. 
    /// Also assert that gotten word composite code is any of array of values.
    /// </summary>
    /// <param name="InAssertNodeRltv"></param>
    /// <param name="InAssertEqualAnyCompositeCode"></param>
    /// <param name="InExceptionText"></param>
    /// <returns></returns>
    public StmtWordListCursor NextDeep(
      AssertNextNodeRelative InAssertNodeRltv, 
      WordCompositeCode[] InAssertEqualAnyCompositeCode,
      string InExceptionText)
    {
      StmtWord startWord = null;
      StmtWord nextWord = null;

      // Setup startWord. Current cursor must be at a word.
      if ( this.Position != RelativePosition.At )
        throw new ApplicationException(
          InExceptionText + " Not positioned at start word.") ;
      startWord = this.Node.Value;

      // do the actual get next.
      StmtWordListCursor c1 = NextDeep();

      // isolate next word.
      if (c1.Position == RelativePosition.At)
      {
        nextWord = c1.Node.Value;
      }

      if (InAssertNodeRltv == AssertNextNodeRelative.IsChild)
      {
        if ((nextWord == null ) || (nextWord.IsChildOf(startWord) == false))
          throw new ApplicationException(InExceptionText + " No child words.");
      }

      if (InAssertNodeRltv == AssertNextNodeRelative.IsSibling)
      {
        if (( nextWord == null ) || (nextWord.IsSiblingOf(startWord) == false))
          throw new ApplicationException(InExceptionText + " No next sibling word.");
      }

      if (InAssertEqualAnyCompositeCode != null) 
      {
        if (nextWord == null)
        {
          throw new ApplicationException(InExceptionText + " Next word not found." ) ;
        }

        else if (Array.IndexOf<WordCompositeCode>(
          InAssertEqualAnyCompositeCode, nextWord.CompositeCode) == -1)
        {
          throw new ApplicationException(InExceptionText + " Unexpected composite code " +
            nextWord.CompositeCode.ToString());
        }
      }

      return c1;
    }

    public StmtWordListCursor NextDeep()
    {
      StmtWordListCursor rv = null;
      LinkedListNode<StmtWord> node;
      WhichEdge edge = WhichEdge.None;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (mRltv != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        rv = new StmtWordListCursor(mList, mNode, mEdge, mRltv);
        node = mNode;
      }

      else
      {
        switch (mRltv)
        {
          case RelativePosition.Begin:
            
          // at begin of top word.
            if (( mNode != null ) && ( StmtWordList.IsTopWord(mNode) == true ))
            {
              rv = new StmtWordListCursor(null, mNode, WhichEdge.LeadEdge, RelativePosition.At ) ;
            }

            // stmt list is null.
            else if ( mList == null )
              throw new ApplicationException("List of sub words is null" ) ;

            else if ( mList.Count == 0 )
            {
              rv = new StmtWordListCursor(mList, null, WhichEdge.None, RelativePosition.End ) ;
            }
            else
            {
              StmtWordListCursor c1 = new StmtWordListCursor(
                mList, mList.First, WhichEdge.LeadEdge, RelativePosition.Before ) ;
              rv = c1.NextDeep();
            }
            break;
          
          case RelativePosition.Before:
            node = mNode;

            if (StmtWordList.IsComposite(node))
            {
              edge = mEdge;
              if (edge == WhichEdge.None)
                edge = WhichEdge.LeadEdge;
            }
            else
              edge = WhichEdge.None;

            rv = new StmtWordListCursor(mList, mNode, edge, RelativePosition.At);
            break;
          
          case RelativePosition.At:

            // step down a level and read the first sub word of this word.
            if ((mEdge == WhichEdge.LeadEdge) &&  (mNode.Value.HasSubWords == true))
            {
              StmtWordListCursor c1 =
                new StmtWordListCursor(
                  mNode.Value.SubWords, null, WhichEdge.None, RelativePosition.Begin);
              rv = c1.NextDeep();
              node = rv.Node;
            }

              // node has no child nodes. read the next sibling.
            else if (mNode.Next != null)
            {
              rv = new StmtWordListCursor(
                mList, mNode.Next, WhichEdge.LeadEdge, RelativePosition.At);
              node = mNode.Next;
            }

              // read next from the parent of this node.
            else
            {
              rv = NextDeepFromParent( mNode.Value ) ;
              node = rv.Node ;
            }

            break;

          case RelativePosition.After:

            // Positioned after the lead edge of a composite word. 
            // Step down a level and read the first sub word of this word.
            // ( the cursor will only be positioned like this if explicity set by user
            //   code. )
            if ((mEdge == WhichEdge.LeadEdge) && (mNode.Value.HasSubWords == true))
            {
              StmtWordListCursor c1 =
                new StmtWordListCursor(
                  mNode.Value.SubWords, null, WhichEdge.None, RelativePosition.Begin);
              rv = c1.NextDeep();
              node = rv.Node;
            }

            // read next sibling.
            else if (mNode.Next != null)
            {
              node = mNode.Next;
              rv = new StmtWordListCursor(
                mList, mNode.Next, WhichEdge.LeadEdge, RelativePosition.At);
            }

              // read next from the parent of this node.
            else
            {
              rv = NextDeepFromParent(mNode.Value);
              node = rv.Node;
            }

            break;

          case RelativePosition.End:
            rv = new StmtWordListCursor(
              null, null, WhichEdge.None, RelativePosition.End);
            break;

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      return rv;
    }

    public StmtWordListCursor NextDeepFromParent(StmtWord InChildWord)
    {
      StmtWordListCursor c1 = null;

      // end of the line if this node does not have a parent.
      if ( InChildWord.Parent == null )
      {
        c1 = new StmtWordListCursor(
          null, null, WhichEdge.None, RelativePosition.End);
      }

      // parent is the TopWord. Top word is special in that it is not a node in
      // a word list.
      else if (InChildWord.Parent.IsTopWord == true)
      {
        LinkedListNode<StmtWord> node = new LinkedListNode<StmtWord>(InChildWord.Parent);
        c1 = new StmtWordListCursor(
          null, node, WhichEdge.TrailEdge, RelativePosition.At);
      }

      // position at the trailing edge of the parent word.
      else
      {
        StmtWord par = InChildWord.Parent;
        c1 = new StmtWordListCursor(
          par.SubWords, par.SubWordNode, WhichEdge.TrailEdge, RelativePosition.At);
      }

      return c1;
    }

    public LinkedListNode<StmtWord> Node
    {
      get { return mNode; }
    }

    public StmtWordListCursor PositionAfter( WhichEdge Edge )
    {
      if (mRltv == RelativePosition.Begin)
        return PositionBegin();
      else if (mRltv == RelativePosition.End)
        return PositionEnd();
      else if (mRltv == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
        return new StmtWordListCursor(mList, mNode, Edge, RelativePosition.After);
    }

    public StmtWordListCursor PositionBefore( WhichEdge InEdge )
    {
      if (mRltv == RelativePosition.Begin)
        return PositionBegin();
      else if (mRltv == RelativePosition.End)
        return PositionEnd();
      else if (mRltv == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
        return new StmtWordListCursor(mList, mNode, InEdge, RelativePosition.Before);
    }

    public StmtWordListCursor PositionBegin()
    {
      return new StmtWordListCursor(
        mList, null, WhichEdge.None, RelativePosition.Begin);
    }

    public StmtWordListCursor PositionEnd()
    {
      return new StmtWordListCursor(mList, null, WhichEdge.None, RelativePosition.End);
    }
  }

}
