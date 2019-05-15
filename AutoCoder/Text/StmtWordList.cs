using System;
using System.Collections.Generic;
using System.Text;
using AutoCoder.Core;

namespace AutoCoder.Text
{

  public class StmtWordList : LinkedList<StmtWord>
  {
    public StmtWordListCursor PositionBegin()
    {
      StmtWordListCursor csr = new StmtWordListCursor(this, null, AcRelativePosition.Begin);
      return csr;
    }
  }

}
