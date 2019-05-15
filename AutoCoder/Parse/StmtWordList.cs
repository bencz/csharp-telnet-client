using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;
using AutoCoder.Core.Enums;

namespace AutoCoder.Parse
{

  public class StmtWordList : LinkedList<StmtWord>
  {
    public StmtWordListCursor PositionBegin()
    {
      StmtWordListCursor csr = 
        new StmtWordListCursor(this, null, WhichEdge.None, RelativePosition.Begin);
      return csr;
    }


    public static bool IsComposite(LinkedListNode<StmtWord> InNode)
    {
      if (InNode.Value == null)
        return false;
      else
        return InNode.Value.IsComposite;
    }

    public static bool IsTopWord(LinkedListNode<StmtWord> InNode)
    {
      if (InNode.Value == null)
        return false;
      else
        return InNode.Value.IsTopWord;
    }

  }
}
