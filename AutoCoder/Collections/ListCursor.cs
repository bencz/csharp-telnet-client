using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Core;
using AutoCoder.Text;
using AutoCoder.Ext.System.Text;

namespace AutoCoder.Collections
{

  public class ListCursor<T> where T: class
  {

    #region constructors
    public ListCursor( List<T> List)
    {
      this.List = List;
      this.Position = RelativePosition.None;
      this.StayAtFlag = false;
    }

    public ListCursor(List<T> List, int Index, RelativePosition Rltv)
      : this(List)
    {
      this.Index = Index;
      this.Position = Rltv;
    }

    public ListCursor(ListCursor<T> Cursor)
    {
      this.List = Cursor.List;
      this.Position = Cursor.Position;
      this.Index = Cursor.Index;
      this.StayAtFlag = Cursor.StayAtFlag;
    }

    #endregion

    #region public get/set properties

    public List<T> List
    {
      get { return _List; }
      set { _List = value; }
    }
    List<T> _List;

    public int Index
    {
      get { return _Index; }
      set { _Index = value; }
    }
    int _Index;

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
          return this.List[this.Index];
        }
        else
          throw new ApplicationException("cursor is not positioned at an item");
      }
    }

    #endregion


    #region public methods

    public ListCursor<T> Next( )
    {
      int nxIndex = -1;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (this.Position != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        nxIndex = this.Index;
      }

      else
      {
        switch (this.Position)
        {
          case RelativePosition.Begin:
            nxIndex = 0;
            break;

          case RelativePosition.Before:
            nxIndex = this.Index;
            break;

          case RelativePosition.At:
            nxIndex = this.Index + 1 ;
            break;

          case RelativePosition.After:
            nxIndex = this.Index + 1 ;
            break;

          case RelativePosition.End:
            nxIndex = -1 ;
            break;

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if ((nxIndex == -1) || ( nxIndex >= this.List.Count ))
        return null;
      else
      {
        var csr = new ListCursor<T>(this.List, nxIndex, RelativePosition.At);
        return csr;
      }
    }

    public ListCursor<T> PositionAfter( )
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
      {
        var csr = new ListCursor<T>(this.List, this.Index, RelativePosition.After) ;
        return csr ;
      }
    }

    public ListCursor<T> PositionBefore()
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");
      else
      {
        var csr = new ListCursor<T>(this.List, this.Index, RelativePosition.Before);
        return csr;
      }
    }

    public ListCursor<T> PositionBegin()
    {
      return new ListCursor<T>(this.List, -1, RelativePosition.Begin);
    }

    public ListCursor<T> PositionEnd()
    {
      return new ListCursor<T>(this.List, -1, RelativePosition.End);
		}
		
		#endregion

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.Position.ToString());
      sb.SentenceAppend(this.Index.ToString());
      return sb.ToString();
    }
	}
}

