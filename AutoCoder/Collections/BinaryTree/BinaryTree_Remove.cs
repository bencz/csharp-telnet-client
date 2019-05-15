using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections.BinaryTree
{

  public partial class BinaryTree<K, V>
  {
    private void RemoveNode(BinaryTreeNode<K, V> InNode)
    {
      BinaryTreeNode<K, V> removedNode = InNode;
      BinaryTreeNode<K, V> parent = InNode.Up;
      TreeNodeDir dirFromParent = InNode.DirectionFromParent;
      BinaryTreeNode<K, V> bottom = null;

      // node to remove is an end node.
      if (InNode.IsEndNode == true)
      {
        bottom = parent;
        InNode.DetachFromParent(this);
      }

      // node to remove is heavy to the left. 
      // replace the node with the prior node.
      else if (InNode.IsDeeperLeft)
      {
        BinaryTreeNode<K, V> prev = InNode.PrevActual;
        bottom = RemoveNode_DetachAndAttach(
          parent, dirFromParent, InNode, prev);
      }

      // replace with the next node.
      else
      {
        BinaryTreeNode<K, V> next = InNode.NextActual;
        bottom = RemoveNode_DetachAndAttach(
          parent, dirFromParent, InNode, next);
      }

      // rebalance the tree starting from the bottom up.
      if (bottom != null)
      {
        Balance_UpdateDepthFromNode(bottom);
        this.Balance(bottom);
        Balance_UpdateDepthFromNode(bottom);
      }

      // free up the removed node.
      InNode.Empty();
    }

    private BinaryTreeNode<K, V> RemoveNode_DetachAndAttach(
      BinaryTreeNode<K, V> InParent, TreeNodeDir InDirFromParent,
      BinaryTreeNode<K, V> InDetachNode, BinaryTreeNode<K, V> InAttachNode)
    {
      BinaryTreeNode<K, V> bottom = null;
      BinaryTreeNode<K, V> node = null;

      // the bottom node is the lowest node adjacent to the attach node.
      if (InAttachNode.Left != null)
        bottom = InAttachNode.Left;
      else if (InAttachNode.Right != null)
        bottom = InAttachNode.Right;
      else if (InAttachNode.Up == InDetachNode)
        bottom = InAttachNode;
      else
        bottom = InAttachNode.Up;

      // the replacement node, the one being attached, must first be
      // detached and its child node should be slid up into its place.
      // The rule is, this replacement node should only have max one
      // branch ( otherwise, it would not have been chosen as a 
      // replacement node. )
      if (InAttachNode.Left != null)
      {
        node = InAttachNode.Left;
        node.DetachFromParent(this);
        RemoveNode_DetachAndReplace(InAttachNode, node);
      }
      else if (InAttachNode.Right != null)
      {
        node = InAttachNode.Right;
        node.DetachFromParent(this);
        RemoveNode_DetachAndReplace(InAttachNode, node);
      }
      else
      {
        InAttachNode.DetachFromParent(this);
      }

      // Detach the removed node and replace with its now detached replacement.
      RemoveNode_DetachAndReplace(InDetachNode, InAttachNode);

      return bottom;
    }

    private void RemoveNode_DetachAndReplace(BinaryTreeNode<K, V> InDetachNode, BinaryTreeNode<K, V> InReplaceNode)
    {

      // save the attachments of the this node ( the detach node )
      BinaryTreeNode<K, V> parent = InDetachNode.Up;
      BinaryTreeNode<K, V> left = InDetachNode.Left;
      BinaryTreeNode<K, V> right = InDetachNode.Right;
      TreeNodeDir dirFromParent = InDetachNode.DirectionFromParent;

      // detach from the parent node.
      InDetachNode.DetachFromParent(this);

      // detach from left node.
      if (InDetachNode.Left != null)
      {
        InDetachNode.Left.Up = null;
        InDetachNode.Left = null;
      }

      // detach from right node.
      if (InDetachNode.Right != null)
      {
        InDetachNode.Right.Up = null;
        InDetachNode.Right = null;
      }

      // hook the replacement node to the parent.
      InReplaceNode.AttachToParent(this, parent, dirFromParent);

      // connect replacement to the left node.
      if ((left != null) && (left != InReplaceNode))
      {
        left.Up = InReplaceNode;
        InReplaceNode.Left = left;
      }

      // connect replacement to the right node.
      if ((right != null) && (right != InReplaceNode))
      {
        right.Up = InReplaceNode;
        InReplaceNode.Right = right;
      }
    }
  }


}
