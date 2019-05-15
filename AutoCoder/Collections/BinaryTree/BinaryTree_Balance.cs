using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections.BinaryTree
{
  public partial class BinaryTree<K, V>
  {
    void Balance(BinaryTreeNode<K, V> InFromNode)
    {
      int balance;

      BinaryTreeNode<K, V> node = InFromNode;
      while (node != null)
      {
        balance = node.LeftDepth - node.RightDepth;

        // left is deeper than the right
        if (balance > 1)
        {
          Balance_RotateLeftToRight(node);
        }

        // right is deeper than the left.
        else if (balance < -1)
        {
          Balance_RotateRightToLeft(node);
        }

        // move up the tree.
        node = node.Up;
      }

      // move up the tree to the new top node.
      Balance_CalcAndSetTop();
    }

    void Balance_CalcAndSetTop()
    {
      BinaryTreeNode<K, V> up = null;
      BinaryTreeNode<K, V> node = this.Top;

      // from the current top node, step up to the actual top node.
      while (node.Up != null)
      {
        up = node.Up;
        node = up;
      }

      // update the new top node of the tree.
      this.Top = node;
    }

    void Balance_RotateLeftLeft(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> parentParent = null;
      BinaryTreeNode<K, V> parentParentLeft = null;
      BinaryTreeNode<K, V> child = null;

      parentParent = InParent.Up;
      child = InParent.Left;

      if (parentParent == null)
        parentParentLeft = null;
      else
        parentParentLeft = parentParent.Left;

      InParent.SetLeft(child.Right);
      child.SetRight(InParent);

      if (parentParent == null)
      { }
      else if (parentParentLeft == InParent)
        parentParent.SetLeft(child);
      else
        parentParent.SetRight(child);
    }

    void Balance_RotateLeftRight(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> parentParent = null;
      BinaryTreeNode<K, V> child = null;
      BinaryTreeNode<K, V> subChild = null;
      BinaryTreeNode<K, V> parentParentLeft = null;

      parentParent = InParent.Up;
      if (parentParent == null)
        parentParentLeft = null;
      else
        parentParentLeft = parentParent.Left;

      child = InParent.Left;
      subChild = child.Right;

      child.SetRight(subChild.Left);
      subChild.SetLeft(child);
      InParent.SetLeft(subChild.Right);
      subChild.SetRight(InParent);

      if (parentParent != null)
      {
        if (parentParentLeft == InParent)
          parentParent.SetLeft(subChild);
        else
          parentParent.SetRight(subChild);
      }
    }

    void Balance_RotateLeftToRight(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> left = InParent.Left;
      if (left.LeftDepth > left.RightDepth)
        Balance_RotateLeftLeft(InParent);
      else
        Balance_RotateLeftRight(InParent);
    }

    void Balance_RotateRightRight(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> parentParent = null;
      BinaryTreeNode<K, V> parentParentLeft = null;
      BinaryTreeNode<K, V> child = null;

      parentParent = InParent.Up;
      child = InParent.Right;

      if (parentParent == null)
        parentParentLeft = null;
      else
        parentParentLeft = parentParent.Left;

      InParent.SetRight(child.Left);
      child.SetLeft(InParent);

      if (parentParent == null)
      { }
      else if (parentParentLeft == InParent)
        parentParent.SetLeft(child);
      else
        parentParent.SetRight(child);
    }

    void Balance_RotateRightLeft(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> parentParent = null;
      BinaryTreeNode<K, V> child = null;
      BinaryTreeNode<K, V> subChild = null;
      BinaryTreeNode<K, V> parentParentLeft = null;

      parentParent = InParent.Up;
      child = InParent.Right;
      subChild = child.Left;

      if (parentParent == null)
        parentParentLeft = null;
      else
        parentParentLeft = parentParent.Left;


      child.SetLeft(subChild.Right);
      subChild.SetRight(child);
      InParent.SetRight(subChild.Left);
      subChild.SetLeft(InParent);

      if (parentParent != null)
      {
        if (parentParentLeft == InParent)
          parentParent.SetLeft(subChild);
        else
          parentParent.SetRight(subChild);
      }
    }

    void Balance_RotateRightToLeft(BinaryTreeNode<K, V> InParent)
    {
      BinaryTreeNode<K, V> right = InParent.Right;
      if (right.RightDepth > right.LeftDepth)
        Balance_RotateRightRight(InParent);
      else
        Balance_RotateRightLeft(InParent);
    }

    void Balance_UpdateDepthFromNode(BinaryTreeNode<K, V> InNode)
    {
      BinaryTreeNode<K, V> node = InNode;

      while (node != null)
      {
        node.UpdateLeftDepth();
        node.UpdateRightDepth();
        node = node.Up;
      }
    }


  }

}
