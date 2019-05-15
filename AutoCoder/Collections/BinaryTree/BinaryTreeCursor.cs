using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Collections.BinaryTree
{

  /// <summary>
  /// cursor used to traverse the nodes of the tree in key order
  /// </summary>
  public class BinaryTreeCursor<K, V>
  {
    BinaryTree<K, V> mBinaryTree;
    BinaryTreeNode<K, V> mNode = null;

    AcRelativePosition mRltv = AcRelativePosition.None;

    public BinaryTreeCursor(BinaryTree<K, V> InTree)
    {
      mRltv = AcRelativePosition.Begin;
      mBinaryTree = InTree;
      mNode = null;
    }

    public BinaryTreeCursor(
      BinaryTree<K, V> InTree, BinaryTreeNode<K, V> InNode, AcRelativePosition InRltv)
    {
      mRltv = InRltv;
      mBinaryTree = InTree;
      mNode = InNode;
    }

    public bool IsPosNone
    {
      get { return mRltv == AcRelativePosition.None; }
    }

    public BinaryTreeNode<K, V> Node
    {
      get { return mNode; }
    }

    public BinaryTree<K, V> Tree
    {
      get { return mBinaryTree; }
    }

    /// <summary>
    /// return a new cursor that is advanced to the next node.
    /// </summary>
    /// <returns></returns>
    public BinaryTreeCursor<K, V> Next()
    {
      BinaryTreeNode<K, V> node = null;

      switch (mRltv)
      {
        case AcRelativePosition.Begin:
          node = Tree.LeftMostNode;
          break;
        case AcRelativePosition.Before:
          node = Node;
          break;
        case AcRelativePosition.At:
          node = Node.Next;
          break;
        case AcRelativePosition.End:
          node = null;
          break;
        case AcRelativePosition.After:
          node = Node.Next;
          break;
        case AcRelativePosition.None:
          throw new
            ApplicationException("tree cursor is not positioned in the tree");
        default:
          throw new ApplicationException("unexpected relative position value");
      }

      // return a new cursor.
      if (node == null)
        return new BinaryTreeCursor<K, V>(mBinaryTree, null, AcRelativePosition.None);
      else
        return new BinaryTreeCursor<K, V>(mBinaryTree, node, AcRelativePosition.At);
    }

    public void PosBegin()
    {
      if (mBinaryTree == null)
        throw new ApplicationException("tree of the cursor is not set");
      mRltv = AcRelativePosition.Begin;
    }

    public void PosEnd()
    {
      if (mBinaryTree == null)
        throw new ApplicationException("tree of the cursor is not set");
      mRltv = AcRelativePosition.End;
    }

    public BinaryTreeCursor<K, V> Prev()
    {
      BinaryTreeNode<K, V> node = null;

      switch (mRltv)
      {
        case AcRelativePosition.Begin:
          node = null;
          break;
        case AcRelativePosition.Before:
          node = Node.Prev;
          break;
        case AcRelativePosition.At:
          node = Node.Prev;
          break;
        case AcRelativePosition.End:
          node = Tree.RightMostNode;
          break;
        case AcRelativePosition.After:
          node = Node;
          break;
        case AcRelativePosition.None:
          throw new
            ApplicationException("tree cursor is not positioned in the tree");
        default:
          throw new ApplicationException("unexpected relative position value");
      }

      // return a new cursor.
      if (node == null)
        return new BinaryTreeCursor<K, V>(mBinaryTree, null, AcRelativePosition.None);
      else
        return new BinaryTreeCursor<K, V>(mBinaryTree, node, AcRelativePosition.At);
    }
  }


}
