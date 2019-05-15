using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Core;
using AutoCoder.Text;

namespace AutoCoder.Collections
{

  public class LinkedListCursor<T> where T: class
  {

    #region constructors
    public LinkedListCursor( )
    {
      this.Position = RelativePosition.None;
      this.StayAtFlag = false;
    }

    public LinkedListCursor(LinkedListNode<T> Node, RelativePosition Rltv)
      : this()
    {
      this.Node = Node;
      this.Position = Rltv;
    }

    public LinkedListCursor(LinkedListCursor<T> Cursor)
      : this( )
    {
      this.Position = Cursor.Position;
      this.Node = Cursor.Node;
      this.StayAtFlag = Cursor.StayAtFlag;
    }

    #endregion

    #region public get/set properties

    public LinkedListNode<T> Node
    {
      get { return _Node; }
      set { _Node = value; }
    }
    LinkedListNode<T> _Node;

    public RelativePosition Position
    {
      get { return _Position; }
      set { _Position = value; }
    }
    RelativePosition _Position;

    bool _StayAtFlag;
    public bool StayAtFlag
    {
      get { return _StayAtFlag; }
      set { _StayAtFlag = value; }
    }

    #endregion

    #region public get properties

    public T Item
    {
      get
      {
        if ((this.Position == RelativePosition.At)
          || (this.Position == RelativePosition.Before)
          || (this.Position == RelativePosition.After))
        {
          return this.Node.Value;
        }
        else
          throw new ApplicationException("cursor is not positioned at an item");
      }
    }

    #endregion


    #region public methods

    public LinkedListCursor<T> Next( )
    {
      LinkedListNode<T> nxNode = null ;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (this.Position != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        nxNode = this.Node;
      }

      else
      {
        switch (this.Position)
        {
          case RelativePosition.Begin:
              throw new ApplicationException("begin position not supported.");

          case RelativePosition.Before:
            nxNode = this.Node;
            break;

          case RelativePosition.At:
            nxNode = this.Node.Next ;
            break;

          case RelativePosition.After:
            nxNode = this.Node.Next ;
            break;

          case RelativePosition.End:
            nxNode = null ;
            break;

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if (nxNode == null )
        return null;
      else
      {
        var csr = new LinkedListCursor<T>(nxNode, RelativePosition.At);
        return csr;
      }
    }

    public LinkedListCursor<T> Previous()
    {
      LinkedListNode<T> pvNode = null;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (this.Position != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        pvNode = this.Node;
      }

      else
      {
        switch (this.Position)
        {
          case RelativePosition.Begin:
            pvNode = null;
            break;

          case RelativePosition.Before:
            pvNode = this.Node.Previous;
            break;

          case RelativePosition.At:
            pvNode = this.Node.Previous;
            break;

          case RelativePosition.After:
            pvNode = this.Node;
            break;

          case RelativePosition.End:
            throw new ApplicationException("end position not supported.");

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if (pvNode == null)
        return null;
      else
      {
        var csr = new LinkedListCursor<T>(pvNode, RelativePosition.At);
        return csr;
      }
    }

    public LinkedListCursor<T> PositionAfter( )
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
      {
        var csr = new LinkedListCursor<T>(this.Node, RelativePosition.After) ;
        return csr ;
      }
    }

    public LinkedListCursor<T> PositionBefore()
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
      {
        var csr = new LinkedListCursor<T>(this.Node, RelativePosition.Before);
        return csr;
      }
    }

    public LinkedListCursor<T> PositionBegin()
    {
      return new LinkedListCursor<T>(null, RelativePosition.Begin);
    }

    public LinkedListCursor<T> PositionEnd()
    {
      return new LinkedListCursor<T>(null, RelativePosition.End);
		}
		
		#endregion

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.Position.ToString());
      if (this.Node != null)
        sb.SentenceAppend(this.Node.Value.ToString());
      return sb.ToString();
    }
	}
}

