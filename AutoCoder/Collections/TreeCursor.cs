using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoCoder.Core.Enums;
using AutoCoder.Core;
using AutoCoder.Text;

namespace AutoCoder.Collections
{
  public class TreeCursor<T> where T: class, ITreeTraverse<T>
  {

    #region constructors
    public TreeCursor()
    {
      this.Position = RelativePosition.None;
      this.Node = null;
      this.Top = null;
      this.StayAtFlag = false;
    }

    public TreeCursor(RelativePosition Rltv, T Node)
      : this( )
    {
      this.Position = Rltv;
      this.Node = Node;
    }

    public TreeCursor(T Top)
      : this( )
    {
      this.Top = Top;
      this.Position = RelativePosition.Begin;
    }

    public TreeCursor(RelativePosition Rltv, T Node, T Top)
      : this()
    {
      this.Position = Rltv;
      this.Node = Node;
      this.Top = Top;
    }

    public TreeCursor(TreeCursor<T> Cursor)
    {
      this.Position = Cursor.Position;
      this.Node = Cursor.Node;
      this.Top = Cursor.Top;
      this.StayAtFlag = Cursor.StayAtFlag;
      this.NodeDepthCeiling = Cursor.NodeDepthCeiling;
    }

    #endregion

    #region debug properties

    public TreeCursor<T> _DebugNext
    {
      get
      {
        var csr = this.Next();
        return csr;
      }
    }

    public TreeCursor<T> _DebugNextDeep
    {
      get
      {
        var csr = this.NextDeep();
        return csr;
      }
    }

    #endregion

    #region public get/set properties

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

    T _Node ;
    public T Node
    {
      get { return _Node; }
      set { _Node = value; }
    }

    int? _NodeDepth;
    public int NodeDepth
    {
      get
      {
        if (_NodeDepth == null)
        {
          int depth = 0;
          if (this.Node.Parent == null)
            depth = 0;
          else
          {
            T node = this.Node.Parent;
            while (true)
            {
              if (node == null)
                break;
              depth += 1;
              node = node.Parent;
            }
          }
          _NodeDepth = depth;
        }
        return _NodeDepth.Value;
      }
    }

    /// <summary>
    /// the upper level node depth at which NextDeep does not step above.
    /// NextDeep returns null when stepping up to the parent node which is above
    /// the NodeDepthCeiling.
    /// </summary>
    int? _NodeDepthCeiling;
    public int? NodeDepthCeiling
    {
      get { return _NodeDepthCeiling; }
      set { _NodeDepthCeiling = value; }
    }

    T _Top;
    public T Top
    {
      get { return _Top; }
      set { _Top = value; }
    }

    #endregion

		#region public get properties

		public WhichEdge CompositeNodeEdge
		{
			get
			{
				if (this.Node == null)
					return WhichEdge.None;
				
				else if (this.Node.HasChildren() == true)
				{
					if (this.Position == RelativePosition.After)
						return WhichEdge.TrailEdge;
					else
						return WhichEdge.LeadEdge;
				}
				
				else
					return WhichEdge.None;
			}
		}

    /// <summary>
    /// the depth of this cursor is above the 
    /// </summary>
    public bool IsAboveCeiling
    {
      get
      {
        if (this.NodeDepthCeiling == null)
          return false;
        else if (this.NodeDepth < this.NodeDepthCeiling.Value)
          return true;
        else
          return false;
      }
    }

		#endregion

		#region public methods

		public TreeCursor<T> Next()
    {
      T nx = null;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (this.Position != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        nx = this.Node;
      }

      else
      {
        switch (this.Position)
        {
          case RelativePosition.Begin:
            if (this.Top == null)
              nx = null;
            else if (this.Top.HasChildren() == false)
              nx = null;
            else
              nx = this.Top.FirstChild;
            break;

          case RelativePosition.Before:
            nx = this.Node;
            break;

          case RelativePosition.At:
            nx = this.Node.NextSibling();
            break;

          case RelativePosition.After:
            nx = this.Node.NextSibling();
            break;

          case RelativePosition.End:
            nx = null;
            break;

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      if (nx == null)
        return null;
      else
      {
        var csr = new TreeCursor<T>(RelativePosition.At, nx, this.Top);

        // store the ceiling of this cursor in the return cursor.
        csr.NodeDepthCeiling = this.NodeDepthCeiling;
        
        return csr;
      }
    }

    public TreeCursor<T> NextDeep()
    {
      TreeCursor<T> csr = null;

      // stay at the current location.
      if (StayAtFlag == true)
      {
        if (this.Position != RelativePosition.At)
          throw new ApplicationException("cursor not position at location to stay at");
        StayAtFlag = false;
        var nx = this.Node;
        csr = new TreeCursor<T>(RelativePosition.At, this.Node, this.Top);
      }

      else
      {
        switch (this.Position)
        {
          case RelativePosition.Begin:
            csr = this.Next();
            break;

          case RelativePosition.Before:
            csr = this.Next( );
            break;

          case RelativePosition.At:
            if (this.Node.HasChildren() == true)
            {
              var nx = this.Node.FirstChild;
              csr = new TreeCursor<T>(RelativePosition.At, nx, this.Top);
            }
            else if (this.Node.NextSibling() != null)
            {
              var nx = this.Node.NextSibling();
              csr = new TreeCursor<T>(RelativePosition.At, nx, this.Top);
            }

              // return from NextDeep positioned after the parent node.
              // caller should test for position after to know that all the child nodes
              // of a node have been iterated and position in the tree has moved up
              // to the parents level.
            else if (this.Node.Parent != null)
            {
              csr = 
                new TreeCursor<T>(RelativePosition.After, this.Node.Parent, this.Top);
//              csr = csr.NextDeep();
            }

            else
            {
              csr = null;
            }
            break;

          case RelativePosition.After:
            if (this.Node.NextSibling() != null)
            {
              var nx = this.Node.NextSibling();
              csr = new TreeCursor<T>(RelativePosition.At, nx, this.Top);
            }
            else if (this.Node.Parent != null)
            {
              csr = new TreeCursor<T>(
                RelativePosition.After, this.Node.Parent, this.Top);
//              csr = csr.NextDeep();
            }
            else
            {
              csr = null;
            }
            break;

          case RelativePosition.End:
            csr = null;
            break;

          default:
            throw new ApplicationException("Next failed. Relative position is not set");
        }
      }

      // store the ceiling of this cursor in the return cursor.
      if (csr != null)
      {
        csr.NodeDepthCeiling = this.NodeDepthCeiling;

        // above the ceiling. null the return cursor to indicator eof.
        if (csr.IsAboveCeiling == true)
        {
          csr = null;
        }
      }

      return csr;
    }

    public TreeCursor<T> PositionAfter( )
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");

        // position after a cursor that is positioned before or after. no change 
        // in position.
      else if (( _Position == RelativePosition.Before ) 
        || ( _Position == RelativePosition.After ))
      {
        var csr = new TreeCursor<T>(this) ;
        return csr ;
      }

      else
      {
        var csr = new TreeCursor<T>(RelativePosition.After, this.Node, this.Top);
        csr.NodeDepthCeiling = this.NodeDepthCeiling;
        return csr;
      }
    }

    public TreeCursor<T> PositionBefore()
    {
      if (_Position == RelativePosition.Begin)
        return PositionBegin();
      else if (_Position == RelativePosition.End)
        return PositionEnd();
      else if (_Position == RelativePosition.None)
        throw new ApplicationException("PositionBefore cursor is set to None");

        // position before a cursor that is positioned before or after. no change 
      // in position.
      else if ((_Position == RelativePosition.Before)
        || (_Position == RelativePosition.After))
      {
        var csr = new TreeCursor<T>(this);
        return csr;
      }

      else
      {
        var csr = new TreeCursor<T>(RelativePosition.Before, this.Node, this.Top);
        csr.NodeDepthCeiling = this.NodeDepthCeiling;
        return csr;
      }
    }

    public TreeCursor<T> PositionBeforeFirstSubNode( )
    {
      TreeCursor<T> csr = null ;
      if (this.Node.HasChildren())
      {
        var fs = this.Node.FirstChild;
        csr = new TreeCursor<T>(RelativePosition.Before, fs, this.Top);
      }

        // no child nodes. positioning before child nodes means that NextDeep will
        // return all the child nodes, then step up and return the next peer node.
        // so, if there are not child nodes, position after so that NextDeep will
        // return the next peer.
      else
      {
        csr = new TreeCursor<T>(RelativePosition.After, this.Node, this.Top);
      }

      csr.NodeDepthCeiling = this.NodeDepthCeiling;
      return csr;
    }

    public TreeCursor<T> PositionBegin()
    {
      return new TreeCursor<T>(RelativePosition.Begin, null, this.Top);
    }

    public TreeCursor<T> PositionEnd()
    {
      return new TreeCursor<T>(RelativePosition.End, null, this.Top);
		}
		
		#endregion

    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(this.Position.ToString());
      if (this.Node != null)
      {
        sb.Append(" ");
        sb.Append(this.Node.ToString());
      }
      return sb.ToString();
    }
	}
}

