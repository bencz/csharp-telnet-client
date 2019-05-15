using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using System.Collections;

namespace AutoCoder.Collections.BinaryTree
{

  public enum TreeFindRule { eq, lt, le, ge, gt, none }

  // direction of the tree node from its parent. 
  public enum TreeNodeDir { Left, Right, None }

  public class BinaryTree<V>
  {
  }


  public partial class BinaryTree<K, V> :
    IEnumerable<BinaryTreeNode<K, V>>
  {
    BinaryTreeNode<K, V> mTop;
    Int64 mNodeAddCx;
    Int64 mNodeDeleteCx;
    string mTreeName;

    public BinaryTree()
    {
    }

    /// <summary>
    /// the left most node in the tree. ( this is the node with the lowest key value. )
    /// </summary>
    public BinaryTreeNode<K, V> LeftMostNode
    {
      get
      {
        BinaryTreeNode<K, V> node = this.Top;
        while (node != null)
        {
          if (node.Left == null)
            break;
          node = node.Left;
        }
        return node;
      }
    }

    /// <summary>
    /// returns a list of nodes sorted in "Level of tree, Slot of level" order.
    /// </summary>
    public BinaryTree.LevelSlotSortedList<K, V> LevelSlotSortedList
    {
      get
      {
        BinaryTree.LevelSlotSortedList<K, V> sortedList =
          new BinaryTree.LevelSlotSortedList<K, V>();
        foreach (BinaryTreeNode<K, V> node in this)
        {
          sortedList.Add(node);
        }

        return sortedList;
      }
    }

    /// <summary>
    /// top TreeNode of the tree.
    /// </summary>
    public BinaryTreeNode<K, V> Top
    {
      get { return mTop; }
      set { mTop = value; }
    }

    public Int64 NodeAddCx
    {
      get { return mNodeAddCx; }
      set { mNodeAddCx = value; }
    }

    public Int64 NodeDeleteCx
    {
      get { return mNodeDeleteCx; }
      set { mNodeDeleteCx = value; }
    }

    /// <summary>
    /// the right most node in the tree. ( this is the node with the highest key value. )
    /// </summary>
    public BinaryTreeNode<K, V> RightMostNode
    {
      get
      {
        BinaryTreeNode<K, V> node = this.Top;
        while (node != null)
        {
          if (node.Right == null)
            break;
          node = node.Right;
        }
        return node;
      }
    }

    public string TreeName
    {
      get { return mTreeName; }
      set { mTreeName = value; }
    }

    public BinaryTreeNode<K, V> FindAndDelete(K InFindKey, TreeFindRule InFindRule)
    {
      BinaryTreeNode<K, V> dltNode = FindNode(InFindKey, InFindRule);
      if (dltNode != null)
      {
        dltNode.IsDeleted = true;
        ++NodeDeleteCx;

        this.RemoveNode(dltNode);
      }

      return dltNode;
    }

    public BinaryTreeNode<K, V> FindNode(K InFindKey, TreeFindRule InFindRule)
    {
      BinaryTreeNode<K, V> foundNode = null;
      BinaryTreeNode<K, V> node = null;
      int rc = 0;

      node = this.Top;
      while (node != null)
      {
        rc = node.CompareKey(InFindKey);

        // key compares equal
        if (rc == 0)
        {
          // update the found node.
          if ((InFindRule == TreeFindRule.le)
            || (InFindRule == TreeFindRule.eq)
            || (InFindRule == TreeFindRule.ge))
            foundNode = node;

          // drill down further.
          if ((InFindRule == TreeFindRule.gt)
            || (InFindRule == TreeFindRule.ge))
            node = node.Right;
          else
            node = node.Left;
        }

        // node key is greater than search key.
        else if (rc == 1)
        {
          // update the found node.
          if ((InFindRule == TreeFindRule.lt)
            || (InFindRule == TreeFindRule.le))
            foundNode = node;

          // drill down further.
          node = node.Left;
        }

        // node key is less than search key.
        else
        {
          // update the found node.
          if ((InFindRule == TreeFindRule.gt)
            || (InFindRule == TreeFindRule.ge))
            foundNode = node;

          // drill down further.
          node = node.Right;
        }
      }

      // the found node.
      node = foundNode;

      // found a node but it is deleted. 
      // Depending on find rule, read next or prev to the first active node.
      if ((node != null) && (node.IsDeleted))
      {
      }

      return node;
    }

    public BinaryTreeNode<K, V> Insert(K InKey)
    {
      BinaryTreeNode<K, V> node = new BinaryTreeNode<K, V>(InKey);
      this.AddNode(node);
      return node;
    }

    public BinaryTreeNode<K, V> Insert(K InKey, V InValue)
    {
      BinaryTreeNode<K, V> node = new BinaryTreeNode<K, V>(InKey, InValue);
      this.AddNode(node);
      return node;
    }

    public BinaryTreeCursor<K, V> PosAfter(K InKey)
    {
      BinaryTreeCursor<K, V> csr = null;
      BinaryTreeNode<K, V> node = FindNode(InKey, TreeFindRule.ge);
      if (node != null)
        csr = new BinaryTreeCursor<K, V>(this, node, AcRelativePosition.After);
      else
        csr = new BinaryTreeCursor<K, V>(this, null, AcRelativePosition.None);

      return csr;
    }

    public BinaryTreeCursor<K, V> PosBefore(K InKey)
    {
      BinaryTreeCursor<K, V> csr = null;
      BinaryTreeNode<K, V> node = FindNode(InKey, TreeFindRule.le);

      if (node != null)
        csr = new BinaryTreeCursor<K, V>(this, node, AcRelativePosition.Before);
      else
        csr = new BinaryTreeCursor<K, V>(this, null, AcRelativePosition.None);

      return csr;
    }

    public BinaryTreeCursor<K, V> PosBegin()
    {
      BinaryTreeCursor<K, V> csr = new BinaryTreeCursor<K, V>(this);
      csr.PosBegin();
      return csr;
    }

    public BinaryTreeCursor<K, V> PosEnd()
    {
      BinaryTreeCursor<K, V> csr = new BinaryTreeCursor<K, V>(this);
      csr.PosEnd();
      return csr;
    }

    #region IEnumerable<TreeNode> Members

    IEnumerator<BinaryTreeNode<K, V>> IEnumerable<BinaryTreeNode<K, V>>.GetEnumerator()
    {
      BinaryTreeNode<K, V> node = LeftMostNode;
      while (node != null)
      {
        yield return node;
        node = node.Next;
      }
    }

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new Exception("The method or operation is not implemented.");
    }

    #endregion


    //    public IEnumerator<TreeNodeLevelSlot<K, V>> GetEnumerator()
    //    {
    //      throw new Exception("The method or operation is not implemented.");
    //    }

  }


}
