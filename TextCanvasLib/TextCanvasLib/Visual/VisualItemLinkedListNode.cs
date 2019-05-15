using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCanvasLib.Visual
{
  public static class VisualItemLinkedListNode
  {
    public static LinkedListNode<IVisualItem> NextShowItem(
      this LinkedListNode<IVisualItem> Node )
    {
      var node = Node;
      while (node != null)
      {
        node = node.Next;
        if (node == null)
          break;

        var item = node.Value;
        if ((item is VisualSpanner) == false)
          break;
      }
      return node;
    }
  }
}
