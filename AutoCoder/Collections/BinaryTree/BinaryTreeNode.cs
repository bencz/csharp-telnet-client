using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections.BinaryTree
{

  public class BinaryTreeNode<K, V>
  {
    public BinaryTreeNode<K, V> Up;
    public BinaryTreeNode<K, V> Left;
    public BinaryTreeNode<K, V> Right;
    bool mIsDeleted;
    public int LeftDepth;
    public int RightDepth;
    public Int64 NodeNx;

    public bool KeyIsInherentType;
    public K Key;
    private V mValue;

    public BinaryTreeNode(K InKey)
    {
      KeyIsInherentType = true;
      Key = InKey;
    }

    public BinaryTreeNode(K InKey, V InValue)
    {
      KeyIsInherentType = true;
      Key = InKey;
      mValue = InValue;
    }

    public V Value
    {
      get { return mValue; }
      set { mValue = value; }
    }

    public TreeNodeDir DirectionFromParent
    {
      get
      {
        if (this.Up == null)
          return TreeNodeDir.None;
        else if (this.Up.Left == this)
          return TreeNodeDir.Left;
        else
          return TreeNodeDir.Right;
      }
    }

    public bool IsDeleted
    {
      get { return mIsDeleted; }
      set
      {
        mIsDeleted = value;
        if (mIsDeleted == true)
        {
          mValue = default(V);
        }
      }
    }

    public bool IsDeeperLeft
    {
      get
      {
        if (LeftDepth > RightDepth)
          return true;
        else
          return false;
      }
    }

    public bool IsDeeperRight
    {
      get
      {
        if (RightDepth > LeftDepth)
          return true;
        else
          return false;
      }
    }

    public bool IsEndNode
    {
      get
      {
        if ((Left == null) && (Right == null))
          return true;
        else
          return false;
      }
    }

    public BinaryTreeNode<K, V> LeftMostChild
    {
      get
      {
        BinaryTreeNode<K, V> node = this;
        while (node.Left != null)
        {
          node = node.Left;
        }
        return node;
      }
    }

    /// <summary>
    /// the calculated level the node occupies in the tree.
    /// </summary>
    public int LevelNx
    {
      get
      {
        int lvl = 0;
        BinaryTreeNode<K, V> parent = Up;
        while (parent != null)
        {
          ++lvl;
          parent = parent.Up;
        }
        return lvl;
      }
    }

    public int MaxDepth
    {
      get
      {
        if (LeftDepth > RightDepth)
          return LeftDepth;
        else
          return RightDepth;
      }
    }

    /// <summary>
    /// Get the next node ( that is not deleted ) in the tree.  
    /// </summary>
    public BinaryTreeNode<K, V> Next
    {
      get
      {
        BinaryTreeNode<K, V> node = NextActual;
        while ((node != null) && (node.IsDeleted == true))
        {
          node = node.NextActual;
        }
        return node;
      }
    }

    /// <summary>
    /// get the next node in keyed order, whether it is deleted or not.
    /// </summary>
    public BinaryTreeNode<K, V> NextActual
    {
      get
      {
        BinaryTreeNode<K, V> next = null;
        BinaryTreeNode<K, V> node = null;

        // node has a right branch. next is the left most node of the right branch.
        if (this.Right != null)
        {
          next = this.Right.LeftMostChild;
        }

        // node does not have a parent. end of the line.
        else if (this.Up == null)
        {
          next = null;
        }

        // parent left branches to this node. Next is the parent.
        else if (this.DirectionFromParent == TreeNodeDir.Left)
        {
          next = this.Up;
        }

        // parent right branches to this node. Next is the first parent which left
        // branches to the branch containing this node.
        else
        {
          node = this.Up;
          while (true)
          {
            if (node.Up == null)
            {
              next = null;
              break;
            }
            else if (node.DirectionFromParent == TreeNodeDir.Left)
            {
              next = node.Up;
              break;
            }
            else
            {
              node = node.Up;
            }
          }
        }

        return next;
      }
    }

    /// <summary>
    /// Get the prev node ( that is not deleted ) in the tree.  
    /// </summary>
    public BinaryTreeNode<K, V> Prev
    {
      get
      {
        BinaryTreeNode<K, V> node = PrevActual;
        while ((node != null) && (node.IsDeleted == true))
        {
          node = node.PrevActual;
        }
        return node;
      }
    }

    /// <summary>
    /// get the prev node in keyed order, whether it is deleted or not.
    /// </summary>
    public BinaryTreeNode<K, V> PrevActual
    {
      get
      {
        BinaryTreeNode<K, V> prev = null;
        BinaryTreeNode<K, V> node = null;

        // node has a right branch. next is the left most node of the right branch.
        if (this.Left != null)
        {
          prev = this.Left.RightMostChild;
        }

        // node does not have a parent. end of the line.
        else if (this.Up == null)
        {
          prev = null;
        }

        // parent right branches to this node. Prev is the parent.
        else if (this.DirectionFromParent == TreeNodeDir.Right)
        {
          prev = this.Up;
        }

        // parent left branches to this node. Prev is the first parent which right
        // branches to the branch containing this node.
        else
        {
          node = this.Up;
          while (true)
          {
            if (node.Up == null)
            {
              prev = null;
              break;
            }
            else if (node.DirectionFromParent == TreeNodeDir.Right)
            {
              prev = node.Up;
              break;
            }
            else
            {
              node = node.Up;
            }
          }
        }

        return prev;
      }
    }

    public BinaryTreeNode<K, V> RightMostChild
    {
      get
      {
        BinaryTreeNode<K, V> node = this;
        while (node.Right != null)
        {
          node = node.Right;
        }
        return node;
      }
    }

    /// <summary>
    /// the calculated slot the node occupies at its level of the tree.
    /// </summary>
    public int SlotNx
    {
      get
      {
        int slotNx = 0;

        if (this.Up == null)
          slotNx = 0;
        else
        {

          // calc the slotnx of the parent.
          int parentSlotNx = this.Up.SlotNx;

          slotNx = parentSlotNx * 2;
          if (DirectionFromParent == TreeNodeDir.Right)
            slotNx = slotNx + 1;
        }
        return Math.Min(999999, slotNx);
      }
    }

    public void AttachToParent(
      BinaryTree<K, V> InTree, BinaryTreeNode<K, V> InParent, TreeNodeDir InDirFromParent)
    {
      if (InParent == null)
      {
        InTree.Top = this;
        this.Up = null;
      }
      else if (InDirFromParent == TreeNodeDir.Left)
      {
        InParent.Left = this;
        this.Up = InParent;
      }
      else
      {
        InParent.Right = this;
        this.Up = InParent;
      }
    }

    public int CalcDepthDown()
    {
      int depth = 1; // the depth of this node itself.
      int leftDepth = 0;
      int rightDepth = 0;

      if (this.Left != null)
      {
        leftDepth = this.Left.CalcDepthDown();
      }

      if (this.Right != null)
      {
        rightDepth = this.Right.CalcDepthDown();
      }

      depth = depth + Math.Max(leftDepth, rightDepth);
      return depth;
    }

    public int CalcLeftDepthDown()
    {
      if (Left == null)
        return 0;
      else
        return Left.CalcDepthDown();
    }

    public int CalcRightDepthDown()
    {
      if (Right == null)
        return 0;
      else
        return Right.CalcDepthDown();
    }

    /// <summary>
    /// Compare the node key to the argument key. Return -1 if node key is
    /// less than argument key, +1 if greater than, 0 if equal.
    /// </summary>
    /// <param name="InFac2"></param>
    /// <returns></returns>
    public int CompareKey(BinaryTreeNode<K, V> InFac2)
    {
      int rv = CompareKey(InFac2.Key);
      return rv;
    }

    public int CompareKey(K InKey)
    {
      IComparable ic = (IComparable)this.Key;
      int rv = ic.CompareTo(InKey);
      return rv;
    }

    public void DetachFromParent(BinaryTree<K, V> InTree)
    {
      if (this.Up == null)
      {
        InTree.Top = null;
      }
      else if (this.DirectionFromParent == TreeNodeDir.Left)
      {
        this.Up.Left = null;
        this.Up = null;
      }
      else
      {
        this.Up.Right = null;
        this.Up = null;
      }
    }

    public void Empty()
    {
      mValue = default(V);
      Up = null;
      Left = null;
      Right = null;
      Key = default(K);
    }

    /// <summary>
    /// set the left node. 
    /// </summary>
    /// <param name="InLeft"></param>
    public void SetLeft(BinaryTreeNode<K, V> InLeft)
    {
      // node currently has a left branch. 
      // top of this left node should be cleared.
      // ( presumably, the caller has a plan for this now disconnected left branch ) 
      if (this.Left != null)
        this.Left.Up = null;

      // actually setting the left node to null.
      if (InLeft == null)
      {
        this.LeftDepth = 1;
        this.Left = null;
      }

      else
      {

        // handle the disconnecting of this left node from its parent.

        // the left node does not have a parent.
        if (InLeft.Up == null)
        {
        }

          // disconnect from the left branch of the parent node.
        else if (InLeft == InLeft.Up.Left)
        {
          InLeft.Up.Left = null;
          InLeft.Up.LeftDepth = 1;
        }

          // disconnect from the right branch of the parent.
        else
        {
          InLeft.Up.Right = null;
          InLeft.Up.RightDepth = 1;
        }

        // the left node points up to this node.
        this.Left = InLeft;
        this.LeftDepth = InLeft.MaxDepth + 1;
        InLeft.Up = this;
      }
    }

    /// <summary>
    /// set the right node. 
    /// </summary>
    /// <param name="InRight"></param>
    public void SetRight(BinaryTreeNode<K, V> InRight)
    {
      // node currently has a right branch. 
      // top of this right node should be cleared.
      // ( presumably, the caller has a plan for this now disconnected right branch ) 
      if (this.Right != null)
        this.Right.Up = null;

      // actually setting the left node to null.
      if (InRight == null)
      {
        this.RightDepth = 1;
        this.Right = null;
      }

      else
      {

        // handle the disconnecting of this right node from its parent.

        // the right node does not have a parent.
        if (InRight.Up == null)
        {
        }

          // disconnect from the left branch of the parent node.
        else if (InRight == InRight.Up.Left)
        {
          InRight.Up.Left = null;
          InRight.Up.LeftDepth = 1;
        }

          // disconnect from the right branch of the parent.
        else
        {
          InRight.Up.Right = null;
          InRight.Up.RightDepth = 1;
        }

        // the right node points up to this node.
        this.Right = InRight;
        this.RightDepth = InRight.MaxDepth + 1;
        InRight.Up = this;
      }
    }

    public string ToLevelSlotString()
    {
      string s1 =
        "LevelNx: " + this.LevelNx.ToString() +
        " SlotNx: " + this.SlotNx.ToString() +
        " Key: " + this.Key.ToString();
      return s1;
    }

    /// <summary>
    /// calc and update the left depth of a node.
    /// </summary>
    public void UpdateLeftDepth()
    {
      BinaryTreeNode<K, V> left = this.Left;
      if (left == null)
        this.LeftDepth = 1;
      else
      {
        this.LeftDepth = left.MaxDepth + 1;
      }
    }

    /// <summary>
    /// calc and update the right depth of a node.
    /// </summary>
    public void UpdateRightDepth()
    {
      BinaryTreeNode<K, V> right = this.Right;
      if (right == null)
        this.RightDepth = 1;
      else
      {
        this.RightDepth = right.MaxDepth + 1;
      }
    }

  }

}
