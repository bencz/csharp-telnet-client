using System;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections.BinaryTree
{
  public partial class BinaryTree<K, V>
  {
    /// <summary>
    /// add the node, loaded with key and cargo, to the tree.
    /// </summary>
    /// <param name="InNode"></param>
    private void AddNode(BinaryTreeNode<K, V> InAddNode)
    {
      ++this.NodeAddCx;
      InAddNode.NodeNx = this.NodeAddCx;

      if (mTop == null)
      {
        AddNode_AddToEmptyTree(InAddNode);
      }
      else
      {
        AddNode_AddInPlace(InAddNode);
        Balance_UpdateDepthFromNode(InAddNode);
        Balance(InAddNode);
        Balance_UpdateDepthFromNode(InAddNode);
      }
    }

    private void AddNode_AddToEmptyTree(BinaryTreeNode<K, V> InNode)
    {

      this.Top = InNode;
    }

    /// <summary>
    /// add new node to tree, place it in its key sequence location.
    /// </summary>
    /// <param name="InNode"></param>
    private void AddNode_AddInPlace(BinaryTreeNode<K, V> InAddNode)
    {
      bool keepLooking = true;
      BinaryTreeNode<K, V> parentNode = null;
      int rv;

      parentNode = this.Top;
      keepLooking = true;
      while (keepLooking)
      {
        rv = InAddNode.CompareKey(parentNode);

        // add key is less than current node.
        if (rv < 0)
        {
          if (parentNode.Left != null)
            parentNode = parentNode.Left;
          else
          {
            keepLooking = false;
            parentNode.Left = InAddNode;
            InAddNode.Up = parentNode;
          }
        }

        // add key is greater than or equal. 
        else
        {
          if (parentNode.Right != null)
            parentNode = parentNode.Right;
          else
          {
            keepLooking = false;
            parentNode.Right = InAddNode;
            InAddNode.Up = parentNode;
          }
        }
      }
    }
  }



}
