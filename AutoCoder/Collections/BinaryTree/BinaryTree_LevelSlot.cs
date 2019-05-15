using global::System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AutoCoder.Collections.BinaryTree
{
  public partial class BinaryTree
  {

    public class LevelSlotSortedList<K, V> :
      IEnumerable<BinaryTreeNode<K, V>>
    {
      SortedList<LevelSlot<K, V>, BinaryTreeNode<K, V>> mSortedList = null;

      public LevelSlotSortedList()
      {
        mSortedList = new SortedList<LevelSlot<K, V>, BinaryTreeNode<K, V>>();
      }

      public void Add(BinaryTreeNode<K, V> InNode)
      {
        mSortedList.Add(new LevelSlot<K, V>(InNode), InNode);
      }

      #region IEnumerable<BinaryTreeNode<K,V>> Members

      IEnumerator<BinaryTreeNode<K, V>> IEnumerable<BinaryTreeNode<K, V>>.GetEnumerator()
      {
        foreach (KeyValuePair<LevelSlot<K, V>, BinaryTreeNode<K, V>> pair in mSortedList)
        {
          yield return pair.Value;
        }
      }

      #endregion

      #region IEnumerable Members

      IEnumerator IEnumerable.GetEnumerator()
      {
        throw new Exception("The method or operation is not implemented.");
      }

      #endregion
    }


    public class LevelSlot<K, V> : IComparable<LevelSlot<K, V>>
    {
      public int LevelNx;
      public int SlotNx;
      //    BinaryTreeNode<K, V> mNode = null;

      public LevelSlot(BinaryTreeNode<K, V> InNode)
      {
        LevelNx = InNode.LevelNx;
        SlotNx = InNode.SlotNx;
        //      mNode = InNode;
      }

      int IComparable<LevelSlot<K, V>>.CompareTo(LevelSlot<K, V> InFac2)
      {
        if (LevelNx != InFac2.LevelNx)
          return LevelNx.CompareTo(InFac2.LevelNx);
        else
          return SlotNx.CompareTo(InFac2.SlotNx);
      }
    }

  }


}
